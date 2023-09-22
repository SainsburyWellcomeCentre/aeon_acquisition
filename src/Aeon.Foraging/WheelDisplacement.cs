using Bonsai;
using System;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;

namespace Aeon.Foraging
{
    [Combinator]
    [Description("Generates a sequence of all changes in wheel position, in metric units.")]
    [WorkflowElementCategory(ElementCategory.Transform)]
    public class WheelDisplacement
    {
        [Description("The radius of the wheel, in metric units.")]
        public double Radius { get; set; } = 1;

        public IObservable<double> Process(IObservable<ushort> source)
        {
            // All math below is done assuming a 14-bit resolution for the encoder
            const int MaxValue = ushort.MaxValue >> 2;
            const int JumpThreshold = MaxValue / 2;
            return Observable.Defer(() =>
            {
                var previous = default(ushort?);
                return source.Select(value =>
                {
                    int diff = 0;
                    if (previous.HasValue)
                    {
                        diff = value - previous.Value;
                        if (diff < -JumpThreshold) diff += MaxValue;
                        else if (diff > JumpThreshold) diff -= MaxValue;
                    }
                    previous = value;
                    return 2 * Math.PI * Radius * (diff / (double)MaxValue);
                });
            });
        }
    }
}
