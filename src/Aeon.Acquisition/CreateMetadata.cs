using System;
using System.ComponentModel;
using System.Reactive.Linq;
using Bonsai;

[Description("Initializes an experiment metadata object using the specified properties.")]
public class CreateMetadata : Source<Experiment>
{
    public CreateMetadata()
    {
        Experiment = new Experiment
        {
            Arena = new ExperimentArena(),
            ClockSynchronizer = new ExperimentClockSynchronizer(),
            VideoController = new ExperimentVideoController(),
            AmbientMicrophone = new ExperimentMicrophone()
        };
    }

    [TypeConverter(typeof(ExpandableObjectConverter))]
    public Experiment Experiment { get; set; }

    public override IObservable<Experiment> Generate()
    {
        return Observable.Return(Experiment);
    }
}