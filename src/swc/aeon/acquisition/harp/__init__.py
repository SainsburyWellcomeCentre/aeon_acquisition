"""Base models for Harp devices used by the acquisition packages.

The module declares no namespace, so these classes generate no types of their own and their
fields are generated in each device model deriving from them.
"""

from typing import ClassVar

from bonsai.sgen import SchemaModel
from pydantic import Field


class HarpDevice(SchemaModel):
    """A base class for creating Harp device models."""

    who_am_i: ClassVar[int]
    """The unique identifier for the device type."""

    port_name: str = Field(examples=["COM"], description="The name of the device serial port.")


class HarpInputExpander(HarpDevice):
    """Represents a Harp Input Expander device."""

    who_am_i: ClassVar[int] = 1106


class HarpOutputExpander(HarpDevice):
    """Represents a Harp Output Expander device."""

    who_am_i: ClassVar[int] = 1108


class HarpClockSynchronizer(HarpDevice):
    """Represents a Harp Clock Synchronizer device."""

    who_am_i: ClassVar[int] = 1152


class HarpTimestampGeneratorGen3(HarpDevice):
    """Represents a Harp Timestamp Generator Gen3 device."""

    who_am_i: ClassVar[int] = 1158


class HarpCameraController(HarpDevice):
    """Represents a Harp Camera Controller device."""

    who_am_i: ClassVar[int] = 1168


class HarpCameraControllerGen2(HarpDevice):
    """Represents a Harp Camera Controller Gen2 device."""

    who_am_i: ClassVar[int] = 1170


class HarpBehavior(HarpDevice):
    """Represents a Harp Behavior device."""

    who_am_i: ClassVar[int] = 1216


class HarpAudioSwitch(HarpDevice):
    """Represents a Harp Audio Switch device."""

    who_am_i: ClassVar[int] = 1248


class HarpSoundCard(HarpDevice):
    """Represents a Harp Sound Card device."""

    who_am_i: ClassVar[int] = 1280


class HarpRfidReader(HarpDevice):
    """Represents a Harp RFID Reader device."""

    who_am_i: ClassVar[int] = 2094
