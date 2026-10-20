"""Models for the Aeon.Video package."""

from enum import StrEnum

from bonsai.sgen import SchemaEnum, SchemaModel
from pydantic import Field

from swc.aeon.acquisition.harp import HarpCameraControllerGen2

SGEN_NAMESPACE = "Aeon.Video"


class TriggerName(SchemaEnum, StrEnum):
    """Specifies the available camera trigger outputs."""

    TRIGGER0 = "Trigger0"
    TRIGGER1 = "Trigger1"


class CameraControllerTrigger(SchemaModel):
    """Represents the configuration of a camera trigger output."""

    frequency: int = Field(default=50, description="The frequency of the camera TTL trigger.")


class CameraController(HarpCameraControllerGen2):
    """Represents synchronized trigger generation for a set of cameras."""

    triggers: dict[TriggerName, CameraControllerTrigger] = Field(
        description="Specifies the configuration of each camera trigger output."
    )


class SpinnakerCamera(SchemaModel):
    """Represents a FLIR Spinnaker camera acquisition module."""

    serial_number: str = Field(examples=["00000"], description="The serial number of the camera.")
    exposure_time: float = Field(
        default=1000,
        ge=100,
        description="The exposure time of the sensor, in microseconds.",
    )
    gain: float = Field(default=0, ge=0, description="The camera gain.")
    binning: int = Field(default=1, ge=1, description="The camera binning configuration.")
