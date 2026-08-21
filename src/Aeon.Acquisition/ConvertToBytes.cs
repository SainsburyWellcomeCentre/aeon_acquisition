using Bonsai;
using System;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using System.Numerics;

namespace Aeon.Acquisition
{
    [Combinator]
    [Description("Convert various data representations to a serializable byte array.")]
    [WorkflowElementCategory(ElementCategory.Transform)]
    public class ConvertToBytes
    {
        public IObservable<float[]> Process(IObservable<Quaternion> source)
        {
            return source.Select(value => new[] { value.W, value.X, value.Y, value.Z });
        }

        public IObservable<float[]> Process(IObservable<Vector3> source)
        {
            return source.Select(value => new[] { value.X, value.Y, value.Z });
        }

        public IObservable<byte[]> Process(IObservable<ulong> source)
        {
            return source.Select(value => BitConverter.GetBytes(value));
        }

        public IObservable<byte[]> Process(IObservable<int> source)
        {
            return source.Select(value => BitConverter.GetBytes(value));
        }
    }
}
