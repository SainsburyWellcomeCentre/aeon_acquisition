using System;

namespace Aeon.Ephys;

static class ConfigurationConversion
{
    public static uint ToUInt32(int value, string name)
    {
        if (value < 0)
        {
            throw new ArgumentOutOfRangeException(name, value, $"The value of {name} cannot be negative.");
        }

        return (uint)value;
    }
}
