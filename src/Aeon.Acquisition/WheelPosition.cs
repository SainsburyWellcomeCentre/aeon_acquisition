using Bonsai;
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;

namespace Aeon.Acquisition
{
    [Combinator]
    [Description("Decodes the absolute cumulative distance travelled on the wheel in metric units.")]
    [WorkflowElementCategory(ElementCategory.Transform)]
    public class WheelPosition
    {
        public WheelPosition()
        {
            Radius = 1;
        }

        [Description("The radius of the wheel, in metric units.")]
        public double Radius { get; set; }

        public IObservable<double> Process(IObservable<ushort> source)
        {
            // All math below is done assuming a 14-bit resolution for the encoder
            const int MaxValue = ushort.MaxValue >> 2;
            const int JumpThreshold = MaxValue / 2;
            return Observable.Defer(() =>
            {
                var turns = 0;
                var previous = default(ushort?);
                return source.Select(value =>
                {
                    int diff = 0;
                    if (previous.HasValue)
                    {
                        diff = value - (int)previous.Value;
                        if (diff < -JumpThreshold) turns++;
                        else if (diff > JumpThreshold) turns--;
                    }
                    previous = value;
                    return 2 * Math.PI * Radius * (turns + previous.Value / (double)MaxValue);
                });
            });
        }
    }
}