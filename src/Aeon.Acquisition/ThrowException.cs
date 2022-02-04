using Bonsai;
using System;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;

namespace Aeon.Acquisition
{
    [Combinator]
    [Description("Raises an exception when the source sequence produces a value.")]
    [WorkflowElementCategory(ElementCategory.Combinator)]
    public class ThrowException
    {
        [Description("The error message describing the exception.")]
        public string Message { get; set; }

        public IObservable<TSource> Process<TSource>(IObservable<TSource> source)
        {
            return source.SelectMany(value => Observable.Throw<TSource>(new InvalidOperationException(Message)));
        }
    }
}