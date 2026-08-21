using Bonsai;
using OpenEphys.Onix1;
using System;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;

namespace Aeon.Ephys;

[Combinator]
[Description("Converts YAML-loaded probe settings into an ONIX probe device configuration.")]
[WorkflowElementCategory(ElementCategory.Transform)]
public class ConvertToProbeConfiguration
{
    [Description("Which of the two probes on the headstage this configures.")]
    public ProbeName ProbeName { get; set; }

    [Description("The headstage port, which determines the device address.")]
    public PortName Port { get; set; }

    const string NeuropixelsV2eName = "HeadstageNeuropixelsV2e";
    const string NeuropixelsV2eBetaName = "HeadstageNeuropixelsV2eBeta";

    uint GetDeviceAddress()
    {
        int index = ProbeName == ProbeName.ProbeA ? 0 : 1;
        return (uint)(((int)Port << 8) + index);
    }

    string GetDeviceName(string headstageName)
    {
        return headstageName + (ProbeName == ProbeName.ProbeA ? "/NeuropixelsV2A" : "/NeuropixelsV2B");
    }

    public IObservable<ConfigureNeuropixelsV2PsbDecoder> Process(IObservable<NeuropixelsV2Probe> source)
    {
        return source.Select(value => new ConfigureNeuropixelsV2PsbDecoder
        {
            Enable = value.Enable,
            ProbeConfiguration = value.ProbeConfiguration,
            DeviceAddress = GetDeviceAddress(),
            DeviceName = GetDeviceName(NeuropixelsV2eName),
        });
    }

    public IObservable<ConfigureNeuropixelsV2BetaPsbDecoder> Process(IObservable<NeuropixelsV2BetaProbe> source)
    {
        return source.Select(value => new ConfigureNeuropixelsV2BetaPsbDecoder
        {
            Enable = value.Enable,
            EnableLed = value.EnableLed,
            ProbeConfiguration = value.ProbeConfiguration,
            DeviceAddress = GetDeviceAddress(),
            DeviceName = GetDeviceName(NeuropixelsV2eBetaName),
        });
    }
}

public enum ProbeName
{
    ProbeA,
    ProbeB
}
