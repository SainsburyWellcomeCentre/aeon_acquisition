using Bonsai;
using System;
using System.ComponentModel;
using System.IO;
using System.Reactive.Linq;

namespace Aeon.Ephys;

/// <summary>
/// Represents an operator that reads the surgery record of the configured subject from the
/// subject records folder.
/// </summary>
[Combinator]
[Description("Reads the surgery record of the configured subject as text, from the file named <subject>.yaml in the subject records folder. The record is optional when no probe is enabled.")]
[WorkflowElementCategory(ElementCategory.Transform)]
public class ReadSubjectRecord
{
    /// <summary>
    /// Gets or sets the folder holding one surgery record file per subject.
    /// </summary>
    [Description("The folder holding one surgery record file per subject.")]
    [Editor(DesignTypes.FolderNameEditor, DesignTypes.UITypeEditor)]
    public string Path { get; set; } = string.Empty;

    /// <summary>
    /// Reads the surgery record of the subject named by each ephys configuration in an
    /// observable sequence.
    /// </summary>
    /// <remarks>
    /// A record is needed only to resolve the gain calibration file of an enabled probe.
    /// When the record file is missing and no probe is enabled, the operator emits an
    /// empty record for the subject instead. A headstage used only for its other devices
    /// then needs no surgery record.
    /// </remarks>
    /// <param name="source">The sequence of ephys configurations.</param>
    /// <returns>The sequence of surgery record texts, in YAML.</returns>
    /// <exception cref="FileNotFoundException">
    /// The subject has no surgery record file and a probe is enabled.
    /// </exception>
    public IObservable<string> Process(IObservable<EphysConfiguration> source)
    {
        return source.Select(configuration =>
        {
            var subject = configuration.Subject;
            var fileName = GetFileName(Path, subject);
            if (File.Exists(fileName))
            {
                return File.ReadAllText(fileName);
            }

            if (!HasEnabledProbe(configuration))
            {
                return GetEmptyRecord(subject);
            }

            throw new FileNotFoundException(
                string.Format(
                    "The surgery record of subject '{0}' was not found at '{1}'. " +
                    "A record named <subject>.yaml in the subject records folder is needed " +
                    "to resolve the gain calibration file of each enabled probe.",
                    subject,
                    fileName),
                fileName);
        });
    }

    internal static string GetFileName(string folder, string subject)
    {
        if (string.IsNullOrWhiteSpace(subject))
        {
            throw new InvalidOperationException(
                "The ephys configuration names no subject. No surgery record can be read.");
        }

        return System.IO.Path.Combine(folder ?? string.Empty, subject + ".yaml");
    }

    static bool HasEnabledProbe(EphysConfiguration configuration)
    {
        return GetProbeEnable.IsEnabled(configuration, ProbeName.ProbeA) ||
            GetProbeEnable.IsEnabled(configuration, ProbeName.ProbeB);
    }

    static string GetEmptyRecord(string subject)
    {
        // Single quotes keep any subject ID a plain string. A quote inside is doubled.
        return "subject: '" + subject.Replace("'", "''") + "'\nimplants: []\n";
    }
}
