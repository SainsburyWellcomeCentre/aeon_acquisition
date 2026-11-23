using System.Linq;

namespace Aeon.Ephys;

public partial class PolledBno055Configuration
{
    public static explicit operator OpenEphys.Onix1.ConfigurePolledBno055(
        PolledBno055Configuration value)
    {
        return new OpenEphys.Onix1.ConfigurePolledBno055
        {
            Enable = value.Enable,
            AxisMap = value.AxisMap,
            AxisSign = value.AxisSign.Aggregate(
                OpenEphys.Onix1.Bno055AxisSign.Default, (sign, axis) => sign | axis)
        };
    }
}
