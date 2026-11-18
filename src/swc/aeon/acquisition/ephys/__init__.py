"""Models for the Aeon.Ephys package."""

from enum import StrEnum
from typing import Annotated

from bonsai.sgen import DiscriminatedUnion, SchemaEnum, SchemaModel, SchemaUnion
from pydantic import Field

SGEN_NAMESPACE = "Aeon.Ephys"


class PortName(SchemaEnum, StrEnum, sgen_typename="OpenEphys.Onix1.PortName"):
    """Specifies the available headstage ports on the breakout board."""

    PORT_A = "PortA"
    PORT_B = "PortB"


class Rhd2000AnalogHighCutoff(SchemaEnum, StrEnum, sgen_typename="OpenEphys.Onix1.Rhd2000AnalogHighCutoff"):
    """Specifies the available upper cutoff frequencies of the analog bandwidth filter."""

    HIGH_20000_HZ = "High20000Hz"
    HIGH_15000_HZ = "High15000Hz"
    HIGH_10000_HZ = "High10000Hz"
    HIGH_7500_HZ = "High7500Hz"
    HIGH_5000_HZ = "High5000Hz"
    HIGH_3000_HZ = "High3000Hz"
    HIGH_2500_HZ = "High2500Hz"
    HIGH_2000_HZ = "High2000Hz"
    HIGH_1500_HZ = "High1500Hz"
    HIGH_1000_HZ = "High1000Hz"
    HIGH_750_HZ = "High750Hz"
    HIGH_500_HZ = "High500Hz"
    HIGH_300_HZ = "High300Hz"
    HIGH_250_HZ = "High250Hz"
    HIGH_200_HZ = "High200Hz"
    HIGH_150_HZ = "High150Hz"
    HIGH_100_HZ = "High100Hz"


class Rhd2000AnalogLowCutoff(SchemaEnum, StrEnum, sgen_typename="OpenEphys.Onix1.Rhd2000AnalogLowCutoff"):
    """Specifies the available lower cutoff frequencies of the analog bandwidth filter."""

    LOW_500_HZ = "Low500Hz"
    LOW_300_HZ = "Low300Hz"
    LOW_250_HZ = "Low250Hz"
    LOW_200_HZ = "Low200Hz"
    LOW_150_HZ = "Low150Hz"
    LOW_100_HZ = "Low100Hz"
    LOW_75_HZ = "Low75Hz"
    LOW_50_HZ = "Low50Hz"
    LOW_30_HZ = "Low30Hz"
    LOW_25_HZ = "Low25Hz"
    LOW_20_HZ = "Low20Hz"
    LOW_15_HZ = "Low15Hz"
    LOW_10_HZ = "Low10Hz"
    LOW_7500_MHZ = "Low7500mHz"
    LOW_5000_MHZ = "Low5000mHz"
    LOW_3000_MHZ = "Low3000mHz"
    LOW_2500_MHZ = "Low2500mHz"
    LOW_2000_MHZ = "Low2000mHz"
    LOW_1500_MHZ = "Low1500mHz"
    LOW_1000_MHZ = "Low1000mHz"
    LOW_750_MHZ = "Low750mHz"
    LOW_500_MHZ = "Low500mHz"
    LOW_300_MHZ = "Low300mHz"
    LOW_250_MHZ = "Low250mHz"
    LOW_100_MHZ = "Low100mHz"


class Rhd2000DspCutoff(SchemaEnum, StrEnum, sgen_typename="OpenEphys.Onix1.Rhd2000DspCutoff"):
    """Specifies the available cutoff frequencies of the digital high-pass filter."""

    DIFFERENTIAL = "Differential"
    DSP_3309_HZ = "Dsp3309Hz"
    DSP_1374_HZ = "Dsp1374Hz"
    DSP_638_HZ = "Dsp638Hz"
    DSP_308_HZ = "Dsp308Hz"
    DSP_152_HZ = "Dsp152Hz"
    DSP_75_HZ = "Dsp75Hz"
    DSP_37_HZ = "Dsp37Hz"
    DSP_19_HZ = "Dsp19Hz"
    DSP_9336_MHZ = "Dsp9336mHz"
    DSP_4665_MHZ = "Dsp4665mHz"
    DSP_2332_MHZ = "Dsp2332mHz"
    DSP_1166_MHZ = "Dsp1166mHz"
    DSP_583_MHZ = "Dsp583mHz"
    DSP_291_MHZ = "Dsp291mHz"
    DSP_146_MHZ = "Dsp146mHz"
    OFF = "Off"


class Rhd2164Configuration(SchemaModel):
    """Represents the acquisition settings of an Intan RHD2164 bioamplifier."""

    enable: bool = Field(default=True, description="Specifies whether the amplifier produces data.")
    analog_high_cutoff: Rhd2000AnalogHighCutoff = Field(
        default=Rhd2000AnalogHighCutoff.HIGH_10000_HZ,
        description="The upper cutoff frequency of the analog bandwidth filter.",
    )
    analog_low_cutoff: Rhd2000AnalogLowCutoff = Field(
        default=Rhd2000AnalogLowCutoff.LOW_100_MHZ,
        description="The lower cutoff frequency of the analog bandwidth filter.",
    )
    dsp_cutoff: Rhd2000DspCutoff = Field(
        default=Rhd2000DspCutoff.OFF,
        description="The cutoff frequency of the digital filter removing the amplifier offset.",
    )


class PortVoltageConfiguration(SchemaModel):
    """Represents the link voltage supplied to a headstage port."""

    requested: float | None = Field(
        default=None,
        description="The voltage to supply, in volts. Left unset, the voltage is negotiated.",
    )


class Bno055Configuration(SchemaModel):
    """Represents the acquisition settings of a BNO055 inertial measurement unit."""

    enable: bool = Field(default=True, description="Specifies whether the unit produces data.")


class Ts4231Configuration(SchemaModel):
    """Represents the acquisition settings of a TS4231 lighthouse receiver array."""

    enable: bool = Field(default=False, description="Specifies whether the receivers produce data.")


class ElectricalStimulatorConfiguration(SchemaModel):
    """Represents the settings of the headstage onboard electrical stimulator."""

    arm: bool = Field(default=False, description="Specifies whether the stimulator responds to triggers.")
    trigger_delay: int = Field(default=0, ge=0, description="The delay from trigger to stimulus, in us.")
    phase_one_duration: int = Field(default=0, ge=0, description="The duration of phase one, in us.")
    phase_one_current: float = Field(default=0, description="The current during phase one, in uA.")
    inter_phase_interval: int = Field(default=0, ge=0, description="The interval between phases, in us.")
    inter_phase_current: float = Field(default=0, description="The current between phases, in uA.")
    phase_two_duration: int = Field(default=0, ge=0, description="The duration of phase two, in us.")
    phase_two_current: float = Field(default=0, description="The current during phase two, in uA.")
    inter_pulse_interval: int = Field(default=0, ge=0, description="The interval between pulses, in us.")
    burst_pulse_count: int = Field(default=0, ge=0, description="The number of pulses in each burst.")
    inter_burst_interval: int = Field(default=0, ge=0, description="The interval between bursts, in us.")
    train_burst_count: int = Field(default=0, ge=0, description="The number of bursts in each train.")


class OpticalStimulatorConfiguration(SchemaModel):
    """Represents the settings of the headstage onboard optical stimulator."""

    arm: bool = Field(default=False, description="Specifies whether the stimulator responds to triggers.")
    max_current: float = Field(default=100, description="The maximum current per channel, in mA.")
    channel_one_current: float = Field(
        default=100, description="The percentage of the maximum current driving channel one."
    )
    channel_two_current: float = Field(
        default=0, description="The percentage of the maximum current driving channel two."
    )
    delay: float = Field(default=0, description="The delay from trigger to stimulus, in ms.")
    pulse_duration: float = Field(default=5, description="The duration of each pulse, in ms.")
    pulses_per_second: float = Field(default=50, description="The pulse frequency within a burst, in Hz.")
    pulses_per_burst: int = Field(default=20, ge=0, description="The number of pulses in each burst.")
    inter_burst_interval: float = Field(default=0, description="The interval between bursts, in ms.")
    bursts_per_train: int = Field(default=1, ge=0, description="The number of bursts in each train.")


class Headstage64Configuration(SchemaModel):
    """Represents the acquisition settings of an ONIX 64-channel headstage."""

    name: str = Field(
        default="Headstage64", description="The name identifying the devices in the headstage."
    )
    port: PortName = Field(
        default=PortName.PORT_A, description="The breakout board port the headstage is connected to."
    )
    port_voltage: PortVoltageConfiguration = Field(
        default=PortVoltageConfiguration(),
        description="The link voltage settings for the headstage port.",
    )
    rhd2164: Rhd2164Configuration = Field(
        default=Rhd2164Configuration(), description="The settings of the bioamplifier."
    )
    bno055: Bno055Configuration = Field(
        default=Bno055Configuration(), description="The settings of the inertial measurement unit."
    )
    ts4231: Ts4231Configuration = Field(
        default=Ts4231Configuration(), description="The settings of the lighthouse receiver array."
    )
    electrical_stimulator: ElectricalStimulatorConfiguration = Field(
        default=ElectricalStimulatorConfiguration(),
        description="The settings of the electrical stimulator.",
    )
    optical_stimulator: OpticalStimulatorConfiguration = Field(
        default=OpticalStimulatorConfiguration(),
        description="The settings of the optical stimulator.",
    )
    buffer_size: int = Field(
        default=3000, ge=1, description="The number of samples collected in each data frame."
    )


class HarpSyncSource(SchemaEnum, StrEnum, sgen_typename="OpenEphys.Onix1.HarpSyncSource"):
    """Specifies the physical input the Harp synchronisation signal arrives on."""

    BREAKOUT = "Breakout"
    CLOCK_ADAPTER = "ClockAdapter"


class NeuropixelsV2QuadShankReference(
    SchemaEnum, StrEnum, sgen_typename="OpenEphys.Onix1.NeuropixelsV2QuadShankReference"
):
    """Specifies the available reference sources of a Neuropixels 2.0 quad-shank probe."""

    EXTERNAL = "External"
    TIP_1 = "Tip1"
    TIP_2 = "Tip2"
    TIP_3 = "Tip3"
    TIP_4 = "Tip4"
    GROUND = "Ground"


class Bno055AxisMap(SchemaEnum, StrEnum, sgen_typename="OpenEphys.Onix1.Bno055AxisMap"):
    """Specifies the axis order compensating for the mounting orientation of a BNO055 unit."""

    XYZ = "XYZ"
    XZY = "XZY"
    YXZ = "YXZ"
    YZX = "YZX"
    ZXY = "ZXY"
    ZYX = "ZYX"


class Bno055AxisSign(SchemaEnum, StrEnum, sgen_typename="OpenEphys.Onix1.Bno055AxisSign"):
    """Specifies an axis inverted to compensate for the mounting orientation of a BNO055 unit."""

    DEFAULT = "Default"
    MIRROR_Z = "MirrorZ"
    MIRROR_Y = "MirrorY"
    MIRROR_X = "MirrorX"


class NeuropixelsV2QuadShankConfiguration(SchemaModel):
    """Represents the electrode settings of a Neuropixels 2.0 quad-shank probe."""

    invert_polarity: bool = Field(
        default=True, description="Specifies whether the acquired voltages are inverted."
    )
    reference: NeuropixelsV2QuadShankReference = Field(
        default=NeuropixelsV2QuadShankReference.EXTERNAL,
        description="The reference source of all enabled electrodes.",
    )
    gain_calibration_file_name: str = Field(
        default="", description="The path of the gain calibration file supplied with the probe."
    )
    probe_interface_file_name: str = Field(
        default="",
        description="The path of a probe interface file selecting the enabled electrodes. "
        "Left empty, the default channel map is used.",
    )


class NeuropixelsV2eProbeConfiguration(SchemaModel):
    """Represents the acquisition settings of a Neuropixels 2.0 probe."""

    enable: bool = Field(default=True, description="Specifies whether the probe produces data.")
    probe_configuration: NeuropixelsV2QuadShankConfiguration = Field(
        default=NeuropixelsV2QuadShankConfiguration(),
        description="The electrode settings of the probe.",
    )


class NeuropixelsV2BetaProbeConfiguration(SchemaModel):
    """Represents the acquisition settings of a beta Neuropixels 2.0 probe."""

    enable: bool = Field(default=True, description="Specifies whether the probe produces data.")
    enable_led: bool = Field(
        default=True, description="Specifies whether the headstage indicator LED is enabled."
    )
    probe_configuration: NeuropixelsV2QuadShankConfiguration = Field(
        default=NeuropixelsV2QuadShankConfiguration(),
        description="The electrode settings of the probe.",
    )


class PolledBno055Configuration(SchemaModel):
    """Represents the acquisition settings of a polled BNO055 inertial measurement unit."""

    enable: bool = Field(default=True, description="Specifies whether the unit produces data.")
    axis_map: Bno055AxisMap = Field(
        default=Bno055AxisMap.YZX,
        description="The axis order compensating for the mounting orientation of the unit.",
    )
    axis_sign: list[Bno055AxisSign] = Field(
        default=[Bno055AxisSign.MIRROR_Y, Bno055AxisSign.MIRROR_X],
        description="The axes inverted to compensate for the mounting orientation of the unit.",
    )


class NeuropixelsV2eConfiguration(SchemaModel):
    """Represents the acquisition settings of an ONIX NeuropixelsV2e headstage."""

    name: str = Field(
        default="HeadstageNeuropixelsV2e",
        description="The name identifying the devices in the headstage. "
        "It must match the device names expected by the data operators.",
    )
    port: PortName = Field(
        default=PortName.PORT_A, description="The breakout board port the headstage is connected to."
    )
    port_voltage: PortVoltageConfiguration = Field(
        default=PortVoltageConfiguration(),
        description="The link voltage settings for the headstage port.",
    )
    probe_a: NeuropixelsV2eProbeConfiguration = Field(
        default=NeuropixelsV2eProbeConfiguration(), description="The settings of probe A."
    )
    probe_b: NeuropixelsV2eProbeConfiguration = Field(
        default=NeuropixelsV2eProbeConfiguration(), description="The settings of probe B."
    )
    bno055: PolledBno055Configuration = Field(
        default=PolledBno055Configuration(),
        description="The settings of the inertial measurement unit.",
    )
    buffer_size: int = Field(
        default=3000, ge=1, description="The number of samples collected in each probe data frame."
    )


class NeuropixelsV2eBetaConfiguration(SchemaModel):
    """Represents the acquisition settings of an ONIX beta NeuropixelsV2e headstage."""

    name: str = Field(
        default="HeadstageNeuropixelsV2eBeta",
        description="The name identifying the devices in the headstage. "
        "It must match the device names expected by the data operators.",
    )
    port: PortName = Field(
        default=PortName.PORT_A, description="The breakout board port the headstage is connected to."
    )
    port_voltage: PortVoltageConfiguration = Field(
        default=PortVoltageConfiguration(),
        description="The link voltage settings for the headstage port.",
    )
    tracking_led: bool = Field(
        default=False, description="Specifies whether the tracking LED is enabled."
    )
    probe_a: NeuropixelsV2BetaProbeConfiguration = Field(
        default=NeuropixelsV2BetaProbeConfiguration(), description="The settings of probe A."
    )
    probe_b: NeuropixelsV2BetaProbeConfiguration = Field(
        default=NeuropixelsV2BetaProbeConfiguration(), description="The settings of probe B."
    )
    bno055: PolledBno055Configuration = Field(
        default=PolledBno055Configuration(),
        description="The settings of the inertial measurement unit.",
    )
    buffer_size: int = Field(
        default=3000, ge=1, description="The number of samples collected in each probe data frame."
    )


class Headstage(SchemaUnion):
    """The headstage connected to the breakout board, selected by the type property."""

    root: Annotated[
        Headstage64Configuration | NeuropixelsV2eConfiguration | NeuropixelsV2eBetaConfiguration,
        DiscriminatedUnion("type"),
    ]


class HarpSyncInputConfiguration(SchemaModel):
    """Represents the acquisition settings of the breakout board Harp synchronisation input."""

    enable: bool = Field(default=True, description="Specifies whether sync events produce data.")
    source: HarpSyncSource = Field(
        default=HarpSyncSource.CLOCK_ADAPTER,
        description="The physical input the Harp synchronisation signal arrives on.",
    )


class HeartbeatConfiguration(SchemaModel):
    """Represents the settings of the breakout board heartbeat."""

    beats_per_second: int = Field(
        default=100, ge=1, description="The rate at which heartbeats are produced, in Hz."
    )


class AnalogIOVoltageRange(SchemaEnum, StrEnum, sgen_typename="OpenEphys.Onix1.AnalogIOVoltageRange"):
    """Specifies the available input voltage ranges of an analog IO channel."""

    TEN_VOLTS = "TenVolts"
    TWO_POINT_FIVE_VOLTS = "TwoPointFiveVolts"
    FIVE_VOLTS = "FiveVolts"


class AnalogIODirection(SchemaEnum, StrEnum, sgen_typename="OpenEphys.Onix1.AnalogIODirection"):
    """Specifies the direction of an analog IO channel."""

    INPUT = "Input"
    OUTPUT = "Output"


# The breakout board has twelve analog channels, each with its own range and direction.
# They are modelled as two lists indexed by channel so a YAML file lists them in order.
class AnalogIOConfiguration(SchemaModel):
    """Represents the acquisition settings of the breakout board analog IO channels."""

    enable: bool = Field(default=True, description="Specifies whether the analog channels produce data.")
    input_range: list[AnalogIOVoltageRange] = Field(
        default=[AnalogIOVoltageRange.TEN_VOLTS] * 12,
        min_length=12,
        max_length=12,
        description="The input voltage range of each channel, in channel order.",
    )
    direction: list[AnalogIODirection] = Field(
        default=[AnalogIODirection.INPUT] * 12,
        min_length=12,
        max_length=12,
        description="The direction of each channel, in channel order.",
    )


class DigitalIOConfiguration(SchemaModel):
    """Represents the acquisition settings of the breakout board digital IO."""

    enable: bool = Field(default=True, description="Specifies whether the digital inputs produce data.")
    dead_time: float = Field(
        default=0,
        ge=0,
        description="The dead time between digital event detections, in us, when sampling asynchronously.",
    )
    sample_rate: float | None = Field(
        default=None,
        description="The sample rate of the digital inputs, in Hz. "
        "Left unset, data is produced on every change in input state.",
    )


class OutputClockConfiguration(SchemaModel):
    """Represents the settings of the breakout board output clock."""

    clock_gate: bool = Field(default=False, description="Specifies whether the clock gate signal is set.")
    frequency: float = Field(default=1e6, gt=0, description="The frequency of the output clock, in Hz.")
    duty_cycle: float = Field(
        default=50, ge=0, le=100, description="The duty cycle of the output clock, in percent."
    )
    delay: float = Field(
        default=0,
        ge=0,
        description="The delay following acquisition start before the clock becomes active, in seconds.",
    )


# The memory monitor is off in ONIX by default. It is on here because the acquisition
# workflows publish its data as OnixMemoryUsage and the backlog alert is silent without it.
class MemoryMonitorConfiguration(SchemaModel):
    """Represents the acquisition settings of the breakout board memory monitor."""

    enable: bool = Field(default=True, description="Specifies whether memory usage produces data.")
    samples_per_second: int = Field(
        default=10, ge=1, description="The rate at which memory usage is recorded, in Hz."
    )


# The breakout board has no name field. Its device names are fixed to what the monitoring
# workflows ask for, and it has no port, so nothing about its identity is configurable.
class BreakoutBoardConfiguration(SchemaModel):
    """Represents the acquisition settings of the ONIX breakout board."""

    heartbeat: HeartbeatConfiguration = Field(
        default=HeartbeatConfiguration(), description="The settings of the heartbeat."
    )
    analog_io: AnalogIOConfiguration = Field(
        default=AnalogIOConfiguration(), description="The settings of the analog IO channels."
    )
    digital_io: DigitalIOConfiguration = Field(
        default=DigitalIOConfiguration(), description="The settings of the digital IO."
    )
    clock_output: OutputClockConfiguration = Field(
        default=OutputClockConfiguration(), description="The settings of the output clock."
    )
    harp_input: HarpSyncInputConfiguration = Field(
        default=HarpSyncInputConfiguration(),
        description="The settings of the Harp synchronisation input.",
    )
    memory_monitor: MemoryMonitorConfiguration = Field(
        default=MemoryMonitorConfiguration(), description="The settings of the memory monitor."
    )


class EphysConfiguration(SchemaModel):
    """Represents the acquisition settings of an Aeon ephys rig."""

    subject: str = Field(
        description="The ID of the implanted subject, naming its surgery record file."
    )
    headstage: Headstage = Field(
        description="The settings of the connected headstage, selected by the type property."
    )
    breakout_board: BreakoutBoardConfiguration = Field(
        default=BreakoutBoardConfiguration(),
        description="The settings of the breakout board the headstage is connected to.",
    )
