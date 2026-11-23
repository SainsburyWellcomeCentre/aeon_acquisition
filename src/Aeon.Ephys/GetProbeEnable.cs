using Bonsai;
using System;
using System.ComponentModel;
using System.Reactive.Linq;

namespace Aeon.Ephys
{
    /// <summary>
    /// Represents an operator that extracts the enable flag of the selected probe
    /// from the ephys configuration.
    /// </summary>
    [Combinator]
    [Description("Extracts the enable flag of the selected probe from the ephys configuration. Emits false for headstages without probes.")]
    [WorkflowElementCategory(ElementCategory.Transform)]
    public class GetProbeEnable
    {
        /// <summary>
        /// Gets or sets which of the two probes on the headstage to look up.
        /// </summary>
        [Description("Which of the two probes on the headstage to look up.")]
        public ProbeName ProbeName { get; set; }

        /// <summary>
        /// Extracts the enable flag of the selected probe from each configuration
        /// in an observable sequence.
        /// </summary>
        /// <param name="source">The sequence of ephys configurations.</param>
        /// <returns>
        /// A sequence of boolean values indicating whether the selected probe is
        /// enabled in each configuration.
        /// </returns>
        public IObservable<bool> Process(IObservable<EphysConfiguration> source)
        {
            return source.Select(configuration => IsEnabled(configuration, ProbeName));
        }

        /// <summary>
        /// Returns whether the specified probe is enabled in the ephys configuration.
        /// </summary>
        internal static bool IsEnabled(EphysConfiguration configuration, ProbeName probeName)
        {
            return configuration.Headstage switch
            {
                NeuropixelsV2eConfiguration headstage => IsEnabled(
                    probeName == ProbeName.ProbeA ? headstage.ProbeA : headstage.ProbeB),
                NeuropixelsV2eBetaConfiguration headstage => IsEnabled(
                    probeName == ProbeName.ProbeA ? headstage.ProbeA : headstage.ProbeB),
                _ => false
            };
        }

        static bool IsEnabled(NeuropixelsV2eProbeConfiguration? probe)
        {
            return probe != null && probe.Enable;
        }

        static bool IsEnabled(NeuropixelsV2BetaProbeConfiguration? probe)
        {
            return probe != null && probe.Enable;
        }
    }
}
