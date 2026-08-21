"""ONIX electrophysiology configuration models and schema emitter.

Single source of truth for the ephys YAML configuration format. Running this module emits
``EphysConfiguration.json``, from which ``Aeon.Ephys.Generated.cs`` is generated:

    python EphysConfiguration.py
    dotnet bonsai.sgen EphysConfiguration.json --namespace Aeon.Ephys --serializer json --serializer yaml

Requires pydantic >= 2.12 (for ``union_format`` in ``model_json_schema``).

Definitions tagged with ``x-sgen-typename`` bind to existing OpenEphys.Onix1 types instead of
generating equivalent classes. Device names and addresses are deliberately absent from the
schema so they can never be set from YAML: ONIX derives both from the headstage port.
"""

import json
from enum import StrEnum
from pathlib import Path
from typing import Annotated, Literal

from pydantic import BaseModel, ConfigDict, Field, GetJsonSchemaHandler, RootModel
from pydantic.alias_generators import to_pascal
from pydantic.json_schema import JsonSchemaValue
from pydantic_core import CoreSchema


def bind_typename(schema: JsonSchemaValue, typename: str) -> JsonSchemaValue:
    """Applies the `x-sgen-typename` tag, binding a definition to an existing type.

    Args:
        schema: The JSON schema definition to tag, modified in place.
        typename: Fully qualified name of the type to bind.

    Returns:
        The same schema, so it can be used inline in a `model_config` declaration.
    """
    schema["x-sgen-typename"] = typename
    return schema


class DiscriminatorTypeMixin:
    """Sets `discriminator_type` to the subclass name, for types in a discriminated union."""

    def __init_subclass__(cls, **kwargs):
        """Injects `discriminator_type` as a Literal of the subclass name."""
        super().__init_subclass__(**kwargs)
        name = cls.__name__
        cls.__annotations__["discriminator_type"] = Literal[name]
        cls.discriminator_type = name


class EphysSchema(BaseModel):
    """Base for ONIX configuration models, keyed in PascalCase."""

    model_config = ConfigDict(
        alias_generator=to_pascal,
        field_title_generator=lambda n, _: to_pascal(n),
        populate_by_name=True,
    )


# Bound to ONIX's own PortName enum: the port value determines every child device address
# ((port << 8) + index), so it must be the real enum rather than a generated copy.
class PortName(StrEnum):
    """The headstage port on the ONIX breakout board."""

    PORT_A = "PortA"
    PORT_B = "PortB"

    @classmethod
    def __get_pydantic_json_schema__(
        cls, core_schema: CoreSchema, handler: GetJsonSchemaHandler
    ) -> JsonSchemaValue:
        """Binds ONIX's `PortName` rather than generating an equivalent enum."""
        return bind_typename(handler(core_schema), "OpenEphys.Onix1.PortName")


# Same again: the generated code has to reference ONIX's own enum for
# ConfigureHarpSyncInput.Source to accept it.
class HarpSyncSource(StrEnum):
    """The hardware source of the Harp synchronisation signal."""

    BREAKOUT = "Breakout"
    CLOCK_ADAPTER = "ClockAdapter"

    @classmethod
    def __get_pydantic_json_schema__(
        cls, core_schema: CoreSchema, handler: GetJsonSchemaHandler
    ) -> JsonSchemaValue:
        """Binds ONIX's `HarpSyncSource` rather than generating an equivalent enum."""
        return bind_typename(handler(core_schema), "OpenEphys.Onix1.HarpSyncSource")


class Rhd2000AnalogHighCutoff(StrEnum):
    """Upper cutoff frequency of the RHD2164 analog bandwidth filter."""

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

    @classmethod
    def __get_pydantic_json_schema__(
        cls, core_schema: CoreSchema, handler: GetJsonSchemaHandler
    ) -> JsonSchemaValue:
        """Binds ONIX's `Rhd2000AnalogHighCutoff`."""
        return bind_typename(handler(core_schema), "OpenEphys.Onix1.Rhd2000AnalogHighCutoff")


class Rhd2000AnalogLowCutoff(StrEnum):
    """Lower cutoff frequency of the RHD2164 analog bandwidth filter."""

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

    @classmethod
    def __get_pydantic_json_schema__(
        cls, core_schema: CoreSchema, handler: GetJsonSchemaHandler
    ) -> JsonSchemaValue:
        """Binds ONIX's `Rhd2000AnalogLowCutoff`."""
        return bind_typename(handler(core_schema), "OpenEphys.Onix1.Rhd2000AnalogLowCutoff")


class Rhd2000DspCutoff(StrEnum):
    """Cutoff frequency of the RHD2164 DSP high-pass filter, or `OFF` to disable it."""

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

    @classmethod
    def __get_pydantic_json_schema__(
        cls, core_schema: CoreSchema, handler: GetJsonSchemaHandler
    ) -> JsonSchemaValue:
        """Binds ONIX's `Rhd2000DspCutoff`."""
        return bind_typename(handler(core_schema), "OpenEphys.Onix1.Rhd2000DspCutoff")


class NeuropixelsV2QuadShankProbeConfiguration(EphysSchema):
    """Per-probe settings for a Neuropixels 2.0 quad-shank probe."""

    model_config = {
        "json_schema_extra": bind_typename(
            {}, "OpenEphys.Onix1.NeuropixelsV2QuadShankProbeConfiguration"
        )
    }

    reference_serialized: str = Field(
        default="External",
        examples=["External", "Ground"],
        description="The probe reference to record against: External or Ground.",
    )
    invert_polarity: bool = Field(
        default=True, description="Whether to invert the polarity of the recorded signal."
    )
    gain_calibration_file_name: str = Field(
        default="",
        examples=["NP2_gain_calibration.csv"],
        description="Path to the gain calibration file supplied with the probe.",
    )
    probe_interface_file_name: str = Field(
        default="",
        examples=["NP2_probe_interface.json"],
        description="Path to the ProbeInterface file describing the probe geometry.",
    )


# Deliberately not bound to ConfigureNeuropixelsV2PsbDecoder: that type carries DeviceName and
# DeviceAddress which would be exposed by using this type directly. They should remain handled by ONIX.
class NeuropixelsV2Probe(EphysSchema):
    """One of the two probes addressed by a Neuropixels 2.0e headstage."""

    enable: bool = Field(default=False, description="Whether to acquire data from this probe.")
    probe_configuration: NeuropixelsV2QuadShankProbeConfiguration = Field(
        description="Calibration and reference settings for this probe."
    )


class AutoPortVoltage(EphysSchema):
    """The headstage port voltage, or auto-negotiation when left unset."""

    model_config = {"json_schema_extra": bind_typename({}, "OpenEphys.Onix1.AutoPortVoltage")}

    requested: float | None = Field(
        default=None,
        description="Requested port voltage in volts. Leave unset to auto-negotiate.",
    )


class NeuropixelsV2BetaProbe(EphysSchema):
    """One of the two probes addressed by a Neuropixels 2.0e beta headstage."""

    enable: bool = Field(default=False, description="Whether to acquire data from this probe.")
    enable_led: bool = Field(default=True, description="Whether to enable the probe's LED.")
    probe_configuration: NeuropixelsV2QuadShankProbeConfiguration = Field(
        description="Calibration and reference settings for this probe."
    )


class Rhd2164(EphysSchema):
    """The RHD2164 64-channel electrophysiology amplifier."""

    model_config = {"json_schema_extra": bind_typename({}, "OpenEphys.Onix1.ConfigureRhd2164")}

    enable: bool = Field(default=True, description="Whether to acquire data from the amplifier.")
    analog_high_cutoff: Rhd2000AnalogHighCutoff = Field(
        default=Rhd2000AnalogHighCutoff.HIGH_10000_HZ,
        description="Upper cutoff frequency of the analog bandwidth filter.",
    )
    analog_low_cutoff: Rhd2000AnalogLowCutoff = Field(
        default=Rhd2000AnalogLowCutoff.LOW_100_MHZ,
        description="Lower cutoff frequency of the analog bandwidth filter.",
    )
    dsp_cutoff: Rhd2000DspCutoff = Field(
        default=Rhd2000DspCutoff.OFF,
        description="Cutoff frequency of the DSP high-pass filter.",
    )


class TS4231(EphysSchema):
    """An array of TS4231 lighthouse receivers for 3D position tracking."""

    model_config = {"json_schema_extra": bind_typename({}, "OpenEphys.Onix1.ConfigureTS4231V1")}

    enable: bool = Field(default=True, description="Whether to acquire position data.")


class Bno055(EphysSchema):
    """The BNO055 inertial measurement unit."""

    model_config = {"json_schema_extra": bind_typename({}, "OpenEphys.Onix1.ConfigureBno055")}

    enable: bool = Field(default=True, description="Whether to acquire orientation data.")


class ElectricalStimulator(EphysSchema):
    """The Headstage64 electrical stimulator."""

    model_config = {
        "json_schema_extra": bind_typename(
            {}, "OpenEphys.Onix1.ConfigureHeadstage64ElectricalStimulator"
        )
    }

    arm: bool = Field(
        default=False,
        description="Whether to power the stimulator's supplies and respect triggers.",
    )
    trigger_delay: int = Field(default=0, ge=0, description="Delay from trigger to stimulus, in µs.")
    phase_one_duration: int = Field(default=0, ge=0, description="Duration of phase one, in µs.")
    phase_one_current: float = Field(default=0, description="Current during phase one, in µA.")
    inter_phase_interval: int = Field(default=0, ge=0, description="Interval between phases, in µs.")
    inter_phase_current: float = Field(default=0, description="Current between phases, in µA.")
    phase_two_duration: int = Field(default=0, ge=0, description="Duration of phase two, in µs.")
    phase_two_current: float = Field(default=0, description="Current during phase two, in µA.")
    inter_pulse_interval: int = Field(default=0, ge=0, description="Interval between pulses, in µs.")
    burst_pulse_count: int = Field(default=0, ge=0, description="Number of pulses per burst.")
    inter_burst_interval: int = Field(default=0, ge=0, description="Interval between bursts, in µs.")
    train_burst_count: int = Field(default=0, ge=0, description="Number of bursts per train.")


class OpticalStimulator(EphysSchema):
    """The Headstage64 optical stimulator."""

    model_config = {
        "json_schema_extra": bind_typename(
            {}, "OpenEphys.Onix1.ConfigureHeadstage64OpticalStimulator"
        )
    }

    arm: bool = Field(default=True, description="Whether the stimulator should respect triggers.")
    max_current: float = Field(default=100, description="Maximum current per channel, in mA.")
    channel_one_current: float = Field(
        default=100, description="Percentage of maximum current on channel one."
    )
    channel_two_current: float = Field(
        default=0, description="Percentage of maximum current on channel two."
    )
    delay: float = Field(default=0, description="Delay from trigger to stimulus, in ms.")
    pulse_duration: float = Field(default=5, description="Duration of each pulse, in ms.")
    pulses_per_second: float = Field(default=50, description="Pulse frequency within a burst, in Hz.")
    pulses_per_burst: int = Field(default=20, ge=0, description="Number of pulses per burst.")
    bursts_per_train: int = Field(default=1, ge=0, description="Number of bursts per train.")
    inter_burst_interval: float = Field(
        default=0, description="Interval between bursts, in ms."
    )


class HeadstageBase(EphysSchema):
    """Settings shared by every ONIX headstage."""

    port: PortName = Field(
        default=PortName.PORT_A, description="The breakout-board port the headstage is connected to."
    )
    port_voltage: AutoPortVoltage = Field(
        description="Port voltage settings for the headstage link."
    )
    buffer_size: int = Field(
        default=30, gt=0, description="Number of frames buffered per probe read."
    )


class NeuropixelsV2eHeadstage(DiscriminatorTypeMixin, HeadstageBase):
    """A Neuropixels 2.0e headstage carrying two quad-shank probes."""

    probe_a: NeuropixelsV2Probe = Field(description="Configuration for probe A.")
    probe_b: NeuropixelsV2Probe = Field(description="Configuration for probe B.")


class NeuropixelsV2eBetaHeadstage(DiscriminatorTypeMixin, HeadstageBase):
    """A Neuropixels 2.0e beta headstage carrying two quad-shank probes."""

    tracking_led: bool = Field(
        default=False, description="Whether to enable the headstage's tracking LED."
    )
    probe_a: NeuropixelsV2BetaProbe = Field(description="Configuration for probe A.")
    probe_b: NeuropixelsV2BetaProbe = Field(description="Configuration for probe B.")


class Headstage64(DiscriminatorTypeMixin, HeadstageBase):
    """A 64-channel headstage carrying an RHD2164 amplifier, IMU and stimulators."""

    rhd2164: Rhd2164 = Field(description="Configuration for the RHD2164 amplifier.")
    bno055: Bno055 = Field(description="Configuration for the inertial measurement unit.")
    ts4231: TS4231 = Field(description="Configuration for the optical position sensor.")
    electrical_stimulator: ElectricalStimulator = Field(
        description="Configuration for the electrical stimulator."
    )
    optical_stimulator: OpticalStimulator = Field(
        description="Configuration for the optical stimulator."
    )


class Headstage(
    RootModel[
        Annotated[
            NeuropixelsV2eHeadstage | NeuropixelsV2eBetaHeadstage | Headstage64,
            Field(discriminator="discriminator_type"),
        ]
    ]
):
    """Any supported ONIX headstage, selected by its discriminator."""


class HarpSyncInput(EphysSchema):
    """The Harp clock synchronisation input on the ONIX breakout board."""

    model_config = {
        "json_schema_extra": bind_typename({}, "OpenEphys.Onix1.ConfigureHarpSyncInput")
    }

    enable: bool = Field(default=True, description="Whether to acquire Harp synchronisation data.")
    source: HarpSyncSource = Field(
        description="The hardware source of the synchronisation signal.",
    )

    device_name: str = Field(
        default="BreakoutBoard/HarpSyncInput",
        description="Device name, qualified by the breakout board name.",
    )
    device_address: int = Field(
        default=12,
        description="Fixed hardware address of the Harp sync input on the breakout board.",
    )


class EphysConfiguration(EphysSchema):
    """Top-level ONIX electrophysiology configuration loaded from YAML."""

    headstage: Headstage = Field(description="Configuration for the connected headstage.")
    harp_input: HarpSyncInput = Field(
        description="Configuration for the breakout board's Harp synchronisation input."
    )


def main():
    """Emits the JSON schema next to this file."""
    schema = EphysConfiguration.model_json_schema(union_format="primitive_type_array")
    path = Path(__file__).with_name("EphysConfiguration.json")
    path.write_text(json.dumps(schema, indent=2))


if __name__ == "__main__":
    main()
