"""Models for the Aeon.Environment package."""

from enum import StrEnum

from bonsai.sgen import SchemaEnum, SchemaModel
from pydantic import Field

SGEN_NAMESPACE = "Aeon.Environment"


class InterpolationMethod(SchemaEnum, StrEnum):
    """Specifies the available interpolation methods."""

    NONE = "None"
    ZERO = "Zero"
    LINEAR = "Linear"


class Fixture(SchemaModel):
    """Represents the channel assignment and calibration of a single light fixture."""

    channels: list[int] = Field(description="Specifies the collection of channels assigned to the fixture.")
    interpolation_method: InterpolationMethod = Field(
        description="Specifies the method used to interpolate light values for a fixture."
    )
    calibration_file: str | None = Field(
        default=None, description="Specifies the path to the calibration file for this fixture."
    )


class RoomFixtures(SchemaModel):
    """Specifies the channel map for every light fixture in the room."""

    cold_white: Fixture | None = None

    warm_white: Fixture | None = None

    red: Fixture | None = None


class ChannelMap(SchemaModel):
    """Represents channel map configuration used by the light controller."""

    rooms: dict[str, RoomFixtures] = Field(
        description="Specifies the collection of light channel maps for all rooms."
    )
