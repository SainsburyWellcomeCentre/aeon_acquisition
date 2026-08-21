using Bonsai;
using OpenEphys.Onix1;
using System;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;

namespace Aeon.Ephys;

[Combinator]
[Description("Configures the 64 channel headstage devices using the port settings driven from the YAML configuration input.")]
[WorkflowElementCategory(ElementCategory.Transform)]
public class Configure64ChannelHeadstageDevices
{
    uint GetDeviceAddress(PortName port, uint index)
    {
        return (uint)(((int)port << 8) + index);
    }

    public IObservable<Headstage64> Process(IObservable<Headstage64> source)
    {
        return source.Select(value => {

            var port = value.Port;

            var rhd2164 = value.Rhd2164;
            rhd2164.DeviceName = "Headstage64/Rhd2164";
            rhd2164.DeviceAddress = GetDeviceAddress(port, 0);
            rhd2164.Enable = true;

            var bno055 = value.Bno055;
            bno055.DeviceName = "Headstage64/Bno055";
            bno055.DeviceAddress = GetDeviceAddress(port, 1);
            bno055.Enable = true;

            var ts4231 = value.Ts4231;
            ts4231.DeviceName = "Headstage64/Ts4231";
            ts4231.DeviceAddress = GetDeviceAddress(port, 2);
            ts4231.Enable = false;

            var electricalStimulator = value.ElectricalStimulator;
            electricalStimulator.DeviceName = "Headstage64/ElectricalStimulator";
            electricalStimulator.DeviceAddress = GetDeviceAddress(port, 3);
            electricalStimulator.Arm = false;

            var opticalStimulator = value.OpticalStimulator;
            opticalStimulator.DeviceName = "Headstage64/OpticalStimulator";
            opticalStimulator.DeviceAddress = GetDeviceAddress(port, 4);
            opticalStimulator.Arm = false;

            var ephys = new Headstage64 {
                Port = port,
                PortVoltage = value.PortVoltage,
                BufferSize = value.BufferSize
            };

            ephys.Rhd2164 = rhd2164;
            ephys.Bno055 = bno055;
            ephys.Ts4231 = ts4231;
            ephys.ElectricalStimulator = electricalStimulator;
            ephys.OpticalStimulator = opticalStimulator;

            return ephys;
        });
    }
}
