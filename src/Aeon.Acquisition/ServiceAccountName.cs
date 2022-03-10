using Bonsai;
using System;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;

namespace Aeon.Acquisition
{
    [Description("Returns the service account name used to record data acquired by the local computer.")]
    public class ServiceAccountName : Source<string>
    {
        public override IObservable<string> Generate()
        {
            return Observable.Return(Environment.MachineName);
        }

        public IObservable<string> Generate<TSource>(IObservable<TSource> source)
        {
            return source.Select(_ => Environment.MachineName);
        }
    }
}
