"""Models for the Aeon.Foraging package."""

from enum import StrEnum

from bonsai.sgen import SchemaEnum, SchemaModel
from pydantic import Field

from swc.aeon.acquisition.harp import HarpOutputExpander

SGEN_NAMESPACE = "Aeon.Foraging"


class UndergroundFeeder(HarpOutputExpander):
    """Represents control and acquisition functionality for an underground feeder module."""

    pellet_delivery_retry_count: int = Field(
        default=2,
        ge=0,
        description="The number of times to retry a failed pellet delivery.",
    )
    pellet_delivery_timeout: float = Field(
        default=1,
        ge=0,
        description="The amount of time in seconds to wait for pellet detection "
        "before reporting a failure.",
    )
    wheel_radius: float = Field(default=-4.0, description="The radius of the wheel, in centimeters.")


class FeederCommand(SchemaEnum, StrEnum):
    """Specifies the available commands for an underground feeder controller."""

    DELIVER_PELLET = "DeliverPellet"
    RESET_FEEDER = "ResetFeeder"


class CreateFeederCommand(SchemaModel):
    """Creates a command to send to an underground feeder controller."""

    command: FeederCommand = Field(
        default=FeederCommand.DELIVER_PELLET,
        description="The command to send to the feeder device.",
    )
