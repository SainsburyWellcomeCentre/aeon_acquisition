"""Regenerate the schemas and serialization classes for every acquisition package.

Each Python package under ``src/swc/aeon/acquisition`` that declares an ``SGEN_NAMESPACE``
describes the acquisition package of the same name. This runs the two ``bonsai-sgen``
commands from the README over all of them, so the committed artefacts cannot drift from
the models one package at a time.

Usage:
    uv run python tools/regenerate.py
    uv run python tools/regenerate.py --check
    uv run python tools/regenerate.py --package ephys

``--check`` writes nothing. It regenerates into a temporary directory and compares the
result against the committed files, exiting non-zero when they differ, which is what a
continuous integration job should run.
"""

from __future__ import annotations

import argparse
import importlib
import pkgutil
import shutil
import subprocess
import sys
import tempfile
from pathlib import Path

ROOT = Path(__file__).resolve().parent.parent
PACKAGE_ROOT = ROOT / "src" / "swc" / "aeon" / "acquisition"
SCHEMA_DIR = ROOT / "schemas"


class Package:
    """An acquisition package and the two artefacts generated from its models."""

    def __init__(self, name: str, namespace: str) -> None:
        self.name = name
        self.namespace = namespace

    @property
    def module(self) -> str:
        return f"swc.aeon.acquisition.{self.name}"

    @property
    def schema(self) -> Path:
        return SCHEMA_DIR / f"{self.namespace}.json"

    @property
    def generated(self) -> Path:
        return ROOT / "src" / self.namespace / f"{self.namespace}.Generated.cs"


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


def run(package: Package, schema_dir: Path, generated_dir: Path) -> None:
    """Exports the package schema and generates its serialization classes."""
    commands = [
        ["bonsai-sgen", "export", package.module, "-o", str(schema_dir)],
        # fmt: off
        [
            "bonsai-sgen", "generate", package.module,
            "-o", str(generated_dir),
            "--serializer", "json", "yaml",
        ],
        # fmt: on
    ]
    for command in commands:
        subprocess.run(command, check=True, cwd=ROOT)


def regenerate(packages: list[Package]) -> int:
    for package in packages:
        print(f"regenerating {package.namespace}")
        run(package, SCHEMA_DIR, package.generated.parent)

    return 0


def matches(committed: Path, fresh: Path) -> bool:
    """Compares two generated files, ignoring line endings.

    The repository normalises text to LF while Windows checks it out as CRLF, so a
    byte comparison would report every file as stale on a Windows working copy.
    """
    return (
        committed.read_text(encoding="utf-8-sig").splitlines()
        == fresh.read_text(encoding="utf-8-sig").splitlines()
    )


def check(packages: list[Package]) -> int:
    """Regenerates into a temporary directory and reports any committed file that differs."""
    stale = []
    with tempfile.TemporaryDirectory() as directory:
        for package in packages:
            scratch = Path(directory) / package.name
            scratch.mkdir(parents=True)
            run(package, scratch, scratch)

            expected = [
                (package.schema, scratch / package.schema.name),
                (package.generated, scratch / package.generated.name),
            ]
            for committed, fresh in expected:
                if not committed.exists():
                    stale.append(f"{committed.relative_to(ROOT)} is missing")
                elif not matches(committed, fresh):
                    stale.append(f"{committed.relative_to(ROOT)} does not match its model")

    if stale:
        print("The committed artefacts are out of date:", file=sys.stderr)
        for entry in stale:
            print(f"  {entry}", file=sys.stderr)
        print("\nRun: uv run python tools/regenerate.py", file=sys.stderr)
        return 1

    counted = "package is" if len(packages) == 1 else "packages are"
    print(f"{len(packages)} {counted} up to date")
    return 0


def main() -> int:
    parser = argparse.ArgumentParser(description=__doc__.splitlines()[0])
    parser.add_argument(
        "--check",
        action="store_true",
        help="compare the committed artefacts against the models without writing",
    )
    parser.add_argument(
        "--package",
        action="append",
        metavar="NAME",
        help="restrict to one acquisition package, repeatable",
    )
    arguments = parser.parse_args()

    if shutil.which("bonsai-sgen") is None:
        print("bonsai-sgen is not on the path. Run: uv sync", file=sys.stderr)
        return 1

    packages = discover()
    if arguments.package:
        requested = set(arguments.package)
        known = {package.name for package in packages}
        unknown = requested - known
        if unknown:
            print(
                f"unknown package {', '.join(sorted(unknown))}. "
                f"Known packages: {', '.join(sorted(known))}",
                file=sys.stderr,
            )
            return 1
        packages = [package for package in packages if package.name in requested]

    if not packages:
        print("no acquisition package declares an SGEN_NAMESPACE", file=sys.stderr)
        return 1

    return check(packages) if arguments.check else regenerate(packages)


if __name__ == "__main__":
    sys.exit(main())
