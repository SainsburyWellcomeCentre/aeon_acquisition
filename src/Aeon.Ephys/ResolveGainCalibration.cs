using Bonsai;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reactive.Linq;

namespace Aeon.Ephys;

/// <summary>
/// Represents an operator that resolves the gain calibration file of each enabled probe
/// from the probe serial recorded in the surgery record.
/// </summary>
[Combinator]
[Description("Resolves the gain calibration file of each enabled probe from the probe serial in the surgery record. Emits the ephys configuration using those files.")]
[WorkflowElementCategory(ElementCategory.Transform)]
public class ResolveGainCalibration
{
    const string FileNameSuffix = "_gainCalValues.csv";

    /// <summary>
    /// Resolves the gain calibration files of each pair of ephys configuration and surgery
    /// record in an observable sequence.
    /// </summary>
    /// <param name="source">
    /// The sequence of ephys configurations, each paired with the surgery record of its subject.
    /// </param>
    /// <returns>
    /// A sequence of copies of each ephys configuration. In each copy every enabled probe uses
    /// the gain calibration file named by its serial number. All other settings are unchanged.
    /// </returns>
    public IObservable<EphysConfiguration> Process(
        IObservable<Tuple<EphysConfiguration, ImplantedSubject>> source)
    {
        return source.Select(value => value.Item1.WithHeadstage(Resolve(value.Item1, value.Item2)));
    }

    /// <summary>
    /// Returns a copy of the configured headstage in which every enabled probe uses the gain
    /// calibration file named by its serial number.
    /// </summary>
    internal static Headstage Resolve(EphysConfiguration configuration, ImplantedSubject subject)
    {
        if (configuration == null) throw new ArgumentNullException(nameof(configuration));
        if (subject == null) throw new ArgumentNullException(nameof(subject));
        if (!string.Equals(subject.Subject, configuration.Subject, StringComparison.Ordinal))
        {
            throw new InvalidOperationException(string.Format(
                "The surgery record is for subject '{0}' but the ephys configuration names subject '{1}'.",
                subject.Subject,
                configuration.Subject));
        }

        var root = configuration.CalibrationRoot;
        return configuration.Headstage switch
        {
            NeuropixelsV2eConfiguration headstage => headstage.WithProbes(
                Resolve(headstage.ProbeA, ProbeName.ProbeA, root, subject),
                Resolve(headstage.ProbeB, ProbeName.ProbeB, root, subject)),
            NeuropixelsV2eBetaConfiguration headstage => headstage.WithProbes(
                Resolve(headstage.ProbeA, ProbeName.ProbeA, root, subject),
                Resolve(headstage.ProbeB, ProbeName.ProbeB, root, subject)),
            _ => configuration.Headstage
        };
    }

    static NeuropixelsV2eProbeConfiguration Resolve(
        NeuropixelsV2eProbeConfiguration probe,
        ProbeName probeName,
        string calibrationRoot,
        ImplantedSubject subject)
    {
        if (probe == null || !probe.Enable) return probe!;
        return probe.WithGainCalibrationFileName(GetFileName(calibrationRoot, subject, probeName));
    }

    static NeuropixelsV2BetaProbeConfiguration Resolve(
        NeuropixelsV2BetaProbeConfiguration probe,
        ProbeName probeName,
        string calibrationRoot,
        ImplantedSubject subject)
    {
        if (probe == null || !probe.Enable) return probe!;
        return probe.WithGainCalibrationFileName(GetFileName(calibrationRoot, subject, probeName));
    }

    /// <summary>
    /// Returns the gain calibration file of the specified probe, from the calibration root
    /// and the serial number in the implant entry of that probe.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// The surgery record has no single implant entry with a serial number for the probe,
    /// or the calibration root is empty.
    /// </exception>
    /// <exception cref="FileNotFoundException">The gain calibration file does not exist.</exception>
    internal static string GetFileName(string calibrationRoot, ImplantedSubject subject, ProbeName probeName)
    {
        var probe = probeName.ToString();
        var root = string.IsNullOrWhiteSpace(calibrationRoot) ? "<calibrationRoot>" : calibrationRoot;
        var implants = (subject.Implants ?? Enumerable.Empty<ProbeImplant>())
            .Where(implant => implant != null && string.Equals(implant.Probe, probe, StringComparison.Ordinal))
            .ToList();
        if (implants.Count == 0)
        {
            throw new InvalidOperationException(string.Format(
                "{0} is enabled but the surgery record of subject '{1}' has no implant entry for it. " +
                "The probe serial is unknown, so the gain calibration file '{2}' cannot be looked for.",
                probe,
                subject.Subject,
                FormatFileName(root, "<serial>")));
        }

        if (implants.Count > 1)
        {
            throw new InvalidOperationException(string.Format(
                "The surgery record of subject '{0}' has {1} implant entries for {2}. " +
                "Each probe must have exactly one entry, giving the serial that names '{3}'.",
                subject.Subject,
                implants.Count,
                probe,
                FormatFileName(root, "<serial>")));
        }

        var serial = implants[0].SerialNumber;
        if (string.IsNullOrWhiteSpace(serial))
        {
            throw new InvalidOperationException(string.Format(
                "The implant entry for {0} in the surgery record of subject '{1}' has no serial number. " +
                "The probe serial is unknown, so the gain calibration file '{2}' cannot be looked for.",
                probe,
                subject.Subject,
                FormatFileName(root, "<serial>")));
        }

        if (string.IsNullOrWhiteSpace(calibrationRoot))
        {
            throw new InvalidOperationException(string.Format(
                "The gain calibration file of {0} with serial {1} cannot be looked for because " +
                "calibrationRoot is empty in the ephys configuration. The file is expected at '{2}'.",
                probe,
                serial,
                FormatFileName(root, serial)));
        }

        var fileName = GetFileName(calibrationRoot, serial);
        if (!File.Exists(fileName))
        {
            throw new FileNotFoundException(
                string.Format(
                    "The gain calibration file of {0} with serial {1} was not found at '{2}'.",
                    probe,
                    serial,
                    fileName),
                fileName);
        }

        return fileName;
    }

    static string GetFileName(string calibrationRoot, string serial)
    {
        return Path.Combine(calibrationRoot, serial, serial + FileNameSuffix);
    }

    // Placeholders such as <serial> are not legal path characters on .NET Framework.
    // The path shown in an error message is therefore joined by hand, not with Path.Combine.
    static string FormatFileName(string calibrationRoot, string serial)
    {
        var separator = Path.DirectorySeparatorChar.ToString();
        var folder = calibrationRoot.EndsWith(separator) || calibrationRoot.EndsWith("/")
            ? calibrationRoot
            : calibrationRoot + separator;
        return folder + serial + separator + serial + FileNameSuffix;
    }
}
