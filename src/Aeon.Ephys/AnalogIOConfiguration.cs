using System.Collections.Generic;
using OpenEphys.Onix1;

namespace Aeon.Ephys;

public partial class AnalogIOConfiguration
{
    /// <summary>
    /// Reads the setting of one channel, falling back to the ONIX default when the list is
    /// shorter than the board. The schema requires twelve entries, but the YAML deserializer
    /// does not enforce it.
    /// </summary>
    static T ChannelSetting<T>(List<T> values, int channel, T fallback)
    {
        return values != null && channel < values.Count ? values[channel] : fallback;
    }

    public static explicit operator ConfigureAnalogIO(AnalogIOConfiguration value)
    {
        var ranges = value.InputRange;
        var directions = value.Direction;
        return new ConfigureAnalogIO
        {
            Enable = value.Enable,
            InputRange0 = ChannelSetting(ranges, 0, AnalogIOVoltageRange.TenVolts),
            InputRange1 = ChannelSetting(ranges, 1, AnalogIOVoltageRange.TenVolts),
            InputRange2 = ChannelSetting(ranges, 2, AnalogIOVoltageRange.TenVolts),
            InputRange3 = ChannelSetting(ranges, 3, AnalogIOVoltageRange.TenVolts),
            InputRange4 = ChannelSetting(ranges, 4, AnalogIOVoltageRange.TenVolts),
            InputRange5 = ChannelSetting(ranges, 5, AnalogIOVoltageRange.TenVolts),
            InputRange6 = ChannelSetting(ranges, 6, AnalogIOVoltageRange.TenVolts),
            InputRange7 = ChannelSetting(ranges, 7, AnalogIOVoltageRange.TenVolts),
            InputRange8 = ChannelSetting(ranges, 8, AnalogIOVoltageRange.TenVolts),
            InputRange9 = ChannelSetting(ranges, 9, AnalogIOVoltageRange.TenVolts),
            InputRange10 = ChannelSetting(ranges, 10, AnalogIOVoltageRange.TenVolts),
            InputRange11 = ChannelSetting(ranges, 11, AnalogIOVoltageRange.TenVolts),
            Direction0 = ChannelSetting(directions, 0, AnalogIODirection.Input),
            Direction1 = ChannelSetting(directions, 1, AnalogIODirection.Input),
            Direction2 = ChannelSetting(directions, 2, AnalogIODirection.Input),
            Direction3 = ChannelSetting(directions, 3, AnalogIODirection.Input),
            Direction4 = ChannelSetting(directions, 4, AnalogIODirection.Input),
            Direction5 = ChannelSetting(directions, 5, AnalogIODirection.Input),
            Direction6 = ChannelSetting(directions, 6, AnalogIODirection.Input),
            Direction7 = ChannelSetting(directions, 7, AnalogIODirection.Input),
            Direction8 = ChannelSetting(directions, 8, AnalogIODirection.Input),
            Direction9 = ChannelSetting(directions, 9, AnalogIODirection.Input),
            Direction10 = ChannelSetting(directions, 10, AnalogIODirection.Input),
            Direction11 = ChannelSetting(directions, 11, AnalogIODirection.Input)
        };
    }
}
