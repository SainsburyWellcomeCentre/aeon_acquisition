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


class LightCycle(SchemaModel):
    """Represents a network client for automated room light control."""

    event_socket: str = Field(
        default=">tcp://localhost:4303",
        description="Specifies the endpoint from which the Light Server publishes events.",
    )
    command_socket: str = Field(
        default=">tcp://localhost:4304",
        description="Specifies the endpoint to send commands to the Light Server.",
    )
    room_name: str = Field(description="The name of the room to monitor and control.")
    config_file_name: str = Field(
        default="lightcycle.config",
        description="The name of the CSV file describing the light model, "
        "where each row represents one whole minute and the red, cold white "
        "and warm white, light levels set for that minute.",
    )


class WeightScale(SchemaModel):
    """Represents acquisition functionality for automated habitat weighing scales."""

    port_name: str = Field(examples=["COM"], description="The name of the device serial port.")
    filter_window: int = Field(
        default=40, description="Sliding window size of the weight linear regression filter."
    )
