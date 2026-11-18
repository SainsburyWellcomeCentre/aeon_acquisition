namespace Aeon.Ephys
{
    /// <summary>
    /// Identifies one of the two probes addressed by a Neuropixels 2.0 headstage.
    /// </summary>
    public enum ProbeName
    {
        /// <summary>
        /// The probe on the first headstage connector, streamed as ProbeA.
        /// </summary>
        ProbeA,

        /// <summary>
        /// The probe on the second headstage connector, streamed as ProbeB.
        /// </summary>
        ProbeB
    }
}
