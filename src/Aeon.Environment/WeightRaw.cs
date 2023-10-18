using System;
using System.ComponentModel;
using Bonsai;

namespace Aeon.Environment
{
    [Description("Provides a type signature for converting values into weight measurements.")]
    public class WeightRaw : Transform<WeightMeasurement, WeightMeasurement>
    {
        public override IObservable<WeightMeasurement> Process(IObservable<WeightMeasurement> source)
        {
            return source;
        }
    }
}
