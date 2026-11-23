"""Regenerate the schemas and serialization classes for every acquisition package.

Each Python package under ``src/swc/aeon/acquisition`` that declares an ``SGEN_NAMESPACE``
describes the acquisition package of the same name. This runs the two ``bonsai-sgen``
commands from the README over all of them.

Usage:
    uv run python tools/regenerate.py
"""

from __future__ import annotations

import importlib
import pkgutil
import shutil
import subprocess
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parent.parent
PACKAGE_ROOT = ROOT / "src" / "swc" / "aeon" / "acquisition"
SCHEMA_DIR = ROOT / "schemas"


class Package:
    """An acquisition package and the namespace its models generate into."""

    def __init__(self, name: str, namespace: str) -> None:
        """Records the package name and the namespace it generates into."""
        self.name = name
        self.namespace = namespace

    @property
    def module(self) -> str:
        """The importable module holding the package models."""
        return f"swc.aeon.acquisition.{self.name}"

    @property
    def generated_dir(self) -> Path:
        """The project folder that receives the generated classes."""
        return ROOT / "src" / self.namespace


def discover() -> list[Package]:
    """Finds every acquisition package declaring an SGEN_NAMESPACE.

    Packages without one, such as the Harp device base classes, generate nothing of
    their own and are skipped.
    """
    packages = []
    for module in pkgutil.iter_modules([str(PACKAGE_ROOT)]):
        if not module.ispkg:
            continue
        imported = importlib.import_module(f"swc.aeon.acquisition.{module.name}")
        namespace = getattr(imported, "SGEN_NAMESPACE", None)
        if namespace:
            packages.append(Package(module.name, namespace))

    return sorted(packages, key=lambda package: package.name)


def regenerate(package: Package) -> None:
    """Exports the package schema and generates its serialization classes."""
    export = ["bonsai-sgen", "export", package.module, "-o", str(SCHEMA_DIR)]
    generate = ["bonsai-sgen", "generate", package.module, "-o", str(package.generated_dir)]
    generate += ["--serializer", "json", "yaml"]
    for command in (export, generate):
        subprocess.run(command, check=True, cwd=ROOT)


def main() -> int:
    """Regenerates every acquisition package."""
    if shutil.which("bonsai-sgen") is None:
        print("bonsai-sgen is not on the path. Run: uv sync", file=sys.stderr)
        return 1

    packages = discover()
    if not packages:
        print("no acquisition package declares an SGEN_NAMESPACE", file=sys.stderr)
        return 1

    for package in packages:
        print(f"regenerating {package.namespace}")
        regenerate(package)

    return 0


if __name__ == "__main__":
    sys.exit(main())
