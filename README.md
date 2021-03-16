# Project Aeon - Acquisition

The Project Aeon acquisition repository contains the set of standardized data acquisition systems, protocols, operation instructions and metadata necessary for reproducible task control and acquisition on the foraging arena assay. The scripts contained in this repository should always represent as accurately as possible the automation routines and operational instructions used to log the experimental raw data for Project Aeon. Each acquired dataset should have a reference to the specific hash or release from this repository which was used in the experiment.

## Deployment Instructions

The Project Aeon acquisition framework runs on the [Bonsai](https://bonsai-rx.org/) visual programming language. This repository includes installation scripts which will automatically download and configure a reproducible, self-contained, Bonsai environment to run all acquisition systems on the foraging arena. It is necessary, however, to install a few system dependencies and device drivers which need to be installed separately, before runnning the environment configuration script.

### Prerequisites

These should only need to be installed once on a fresh new system, and are not required if simply refreshing the install or deploying to a new folder.

 * Windows 10
 * [Git for Windows](https://gitforwindows.org/) (recommended for cloning and manipulating this repository)
 * [Visual C++ Redistributable for Visual Studio 2012](https://www.microsoft.com/en-us/download/details.aspx?id=30679) (native dependency for OpenCV)
 * [FTDI CDM Driver 2.12.28](https://www.ftdichip.com/Drivers/CDM/CDM21228_Setup.zip) (serial port drivers for HARP devices)
 * [Spinnaker SDK 1.29.0.5](https://www.flir.co.uk/support/products/spinnaker-sdk/#Downloads) (device drivers for FLIR cameras)

### Environment Setup

The `bonsai` folder contains a snapshot of the runtime environment required to run experiments on the foraging arena. The `setup.cmd` batch script is included in this repository to automate the download and configuration of this environment. Simply double-clicking on this script should launch the necessary powershell commands as long as an active connection to the internet is available.

In case the configuration of the environment ever gets corrupted, you can revert the `bonsai` folder to its original state by deleting all the executable and package files and folders and re-running the `setup.cmd` script. This process may be automated in the future.
