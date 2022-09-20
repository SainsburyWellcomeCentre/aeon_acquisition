using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Bonsai;

namespace Aeon.Acquisition
{
    public class Experiment
    {
        public Experiment()
        {
            Cameras = new List<ExperimentCamera>();
            Patches = new List<ExperimentPatchController>();
        }

        public string Name { get; set; }

        public DateTime StartTime { get; set; }

        public string Description { get; set; }

        public string DataPath { get; set; }

        public int TimeChunkSize { get; set; }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public ExperimentArena Arena { get; set; }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public ExperimentClockSynchronizer ClockSynchronizer { get; set; }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public ExperimentMicrophone AmbientMicrophone { get; set; }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public ExperimentVideoController VideoController { get; set; }

        public List<ExperimentCamera> Cameras { get; set; }

        public List<ExperimentPatchController> Patches { get; set; }
    }

    public class ExperimentClockSynchronizer
    {
        public string SerialNumber { get; set; }

        public string PortName { get; set; }

        public string Description { get; set; }
    }

    public class ExperimentVideoController
    {
        public string SerialNumber { get; set; }

        public string PortName { get; set; }

        public string Description { get; set; }

        public double StandardTriggerFrequency { get; set; }

        public double HighSpeedTriggerFrequency { get; set; }
    }

    public class ExperimentMicrophone
    {
        public string SerialNumber { get; set; }

        public string Description { get; set; }

        public int SampleRate { get; set; }
    }

    public class ExperimentCamera
    {
        public string SerialNumber { get; set; }

        public string Description { get; set; }

        public ExperimentVector3 Position { get; set; }

        public ExperimentVideoTriggerSource TriggerSource { get; set; }

        public double Gain { get; set; }
    }

    public class ExperimentPatchController
    {
        public string SerialNumber { get; set; }

        public string PortName { get; set; }

        public string Description { get; set; }

        public ExperimentVector3 Position { get; set; }

        public double Radius { get; set; }

        public double StartingTorque { get; set; }

        public string WorkflowPath { get; set; }
    }

    public enum ExperimentVideoTriggerSource
    {
        StandardTrigger,
        HighSpeedTrigger
    }

    public class ExperimentArena
    {
        public ExperimentArena()
        {
            HousingChambers = new List<ExperimentHousingChamber>();
            Gates = new List<ExperimentArenaGate>();
        }

        public ExperimentVector3 Dimensions { get; set; }

        public List<ExperimentHousingChamber> HousingChambers { get; set; }

        public List<ExperimentArenaGate> Gates { get; set; }
    }

    public class ExperimentHousingChamber
    {
        public ExperimentVector3 Position { get; set; }
    }

    public class ExperimentArenaGate
    {
        public ExperimentVector3 Position { get; set; }
    }

    [TypeConverter(typeof(NumericRecordConverter))]
    public struct ExperimentVector3
    {
        public double X;

        public double Y;

        public double Z;
    }
}
