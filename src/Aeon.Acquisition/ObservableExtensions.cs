using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;

namespace Aeon.Acquisition
{
    static class ObservableExtensions
    {
        public static IObservable<Unit> MergeUnit<TSource, TOther>(this IObservable<TSource> source, IObservable<TOther> other)
        {
            return source.Select(x => Unit.Default).Merge(other.Select(x => Unit.Default));
        }
    }
}
