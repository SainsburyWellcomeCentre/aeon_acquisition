using Bonsai;
using Bonsai.Harp;
using System;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;

namespace Aeon.Acquisition
{
    [Description("Tests which values lie within the specified range.")]
    public class RangeThreshold : Transform<double, bool>
    {
        [Description("The inclusive lower bound of the range.")]
        public double? Lower { get; set; }

        [Description("The exclusive upper bound of the range.")]
        public double? Upper { get; set; }

        public override IObservable<bool> Process(IObservable<double> source)
        {
            return source.Select(value => !(Lower > value || value >= Upper) && !double.IsNaN(value));
        }

        public IObservable<Timestamped<bool>> Process(IObservable<Timestamped<double>> source)
        {
            return source.Select(input => Timestamped.Create(
                !(Lower > input.Value || input.Value >= Upper) && !double.IsNaN(input.Value),
                input.Seconds));
        }
    }
}
