using Bonsai;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reactive.Linq;

namespace Aeon.Ephys;

/// <summary>
/// Represents an operator that copies the files referenced by each enabled probe into
/// a destination folder. The recording is then self-contained.
/// </summary>
[Combinator]
[Description("Copies the gain calibration file resolved from the surgery record and the probe interface file of each enabled probe into the destination folder.")]
[WorkflowElementCategory(ElementCategory.Sink)]
public class CopyConfigurationFiles
{
    /// <summary>
    /// Gets or sets the folder into which the configuration files are copied.
    /// </summary>
    [Description("The folder into which the configuration files are copied.")]
    [Editor(DesignTypes.FolderNameEditor, DesignTypes.UITypeEditor)]
    public string Path { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether files already in the destination folder
    /// are overwritten.
    /// </summary>
    [Description("Indicates whether files already in the destination folder are overwritten.")]
    public bool Overwrite { get; set; }

    /// <summary>
    /// Copies the files referenced by the enabled probes of each pair of ephys configuration
    /// and surgery record in an observable sequence.
    /// </summary>
    /// <param name="source">
    /// The sequence of ephys configurations, each paired with the surgery record of its subject.
    /// </param>
    /// <returns>The sequence of destination paths, one for each copied file.</returns>
    public IObservable<string> Process(IObservable<Tuple<EphysConfiguration, ImplantedSubject>> source)
    {
        return source.SelectMany(value => CopyFiles(value.Item1, value.Item2));
    }

    IEnumerable<string> CopyFiles(EphysConfiguration configuration, ImplantedSubject subject)
    {
        var destinationFolder = Path;
        if (string.IsNullOrEmpty(destinationFolder))
        {
            throw new InvalidOperationException("A valid destination folder path must be specified.");
        }

        // Resolving first means a missing record entry or calibration file fails before
        // anything is copied, with the same message the acquisition reports.
        var headstage = ResolveGainCalibration.Resolve(configuration, subject);

        // Files keep their original name so analysis can locate them from the record and the
        // configuration. Two different sources sharing a name is an error.
        var copied = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var file in GetConfigurationFiles(headstage))
        {
            var sourcePath = System.IO.Path.GetFullPath(file.Path);
            if (!File.Exists(sourcePath))
            {
                throw new FileNotFoundException(string.Format(
                    "The {0} '{1}' was not found.", file.Description, sourcePath), sourcePath);
            }

            var fileName = System.IO.Path.GetFileName(sourcePath);
            if (copied.TryGetValue(fileName, out var previousSource))
            {
                if (string.Equals(previousSource, sourcePath, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                throw new InvalidOperationException(string.Format(
                    "The configuration files '{0}' and '{1}' share the name '{2}' and cannot both be copied to the dataset. Rename one of them.",
                    previousSource, sourcePath, fileName));
            }

            copied.Add(fileName, sourcePath);
            Directory.CreateDirectory(destinationFolder);
            var destinationPath = System.IO.Path.Combine(destinationFolder, fileName);
            File.Copy(sourcePath, destinationPath, Overwrite);
            yield return destinationPath;
        }
    }

    /// <summary>
    /// Enumerates the files referenced by the enabled probes of a resolved headstage.
    /// </summary>
    static IEnumerable<ConfigurationFile> GetConfigurationFiles(Headstage headstage)
    {
        switch (headstage)
        {
            case NeuropixelsV2eConfiguration neuropixelsV2e:
                foreach (var file in GetProbeFiles(ProbeName.ProbeA, neuropixelsV2e.ProbeA)) yield return file;
                foreach (var file in GetProbeFiles(ProbeName.ProbeB, neuropixelsV2e.ProbeB)) yield return file;
                break;
            case NeuropixelsV2eBetaConfiguration neuropixelsV2eBeta:
                foreach (var file in GetProbeFiles(ProbeName.ProbeA, neuropixelsV2eBeta.ProbeA)) yield return file;
                foreach (var file in GetProbeFiles(ProbeName.ProbeB, neuropixelsV2eBeta.ProbeB)) yield return file;
                break;
        }

        // The 64-channel headstage references no files.
    }

    static IEnumerable<ConfigurationFile> GetProbeFiles(ProbeName probeName, NeuropixelsV2eProbeConfiguration probe)
    {
        if (probe == null || !probe.Enable) return Array.Empty<ConfigurationFile>();
        return GetProbeFiles(probeName, probe.ProbeConfiguration);
    }

    static IEnumerable<ConfigurationFile> GetProbeFiles(ProbeName probeName, NeuropixelsV2BetaProbeConfiguration probe)
    {
        if (probe == null || !probe.Enable) return Array.Empty<ConfigurationFile>();
        return GetProbeFiles(probeName, probe.ProbeConfiguration);
    }

    static IEnumerable<ConfigurationFile> GetProbeFiles(
        ProbeName probeName,
        NeuropixelsV2QuadShankConfiguration probeConfiguration)
    {
        if (probeConfiguration == null) yield break;
        if (!string.IsNullOrWhiteSpace(probeConfiguration.GainCalibrationFileName))
        {
            yield return new ConfigurationFile(
                probeName + " gain calibration file",
                probeConfiguration.GainCalibrationFileName);
        }

        if (!string.IsNullOrWhiteSpace(probeConfiguration.ProbeInterfaceFileName))
        {
            yield return new ConfigurationFile(
                probeName + " probe interface file",
                probeConfiguration.ProbeInterfaceFileName);
        }
    }

    sealed class ConfigurationFile(string description, string path)
    {
        public string Description { get; } = description;

        public string Path { get; } = path;
    }
}
