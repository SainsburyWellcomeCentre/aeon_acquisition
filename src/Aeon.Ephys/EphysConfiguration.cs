namespace Aeon.Ephys;

public partial class EphysConfiguration
{
    /// <summary>
    /// Returns a copy of the acquisition settings using the specified headstage settings.
    /// </summary>
    internal EphysConfiguration WithHeadstage(Headstage headstage)
    {
        return new EphysConfiguration(this) { Headstage = headstage };
    }
}
