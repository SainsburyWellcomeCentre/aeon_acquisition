"""Models for the Aeon.Ephys package."""

from enum import StrEnum

from bonsai.sgen import SchemaEnum, SchemaModel
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

    enable: bool = Field(default=True, description="Specifies whether the receivers produce data.")


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

    arm: bool = Field(default=True, description="Specifies whether the stimulator responds to triggers.")
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
