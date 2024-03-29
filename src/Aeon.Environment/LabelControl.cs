﻿using System;
using System.ComponentModel;
using Bonsai;

namespace Aeon.Environment
{
    [Combinator]
    [TypeVisualizer(typeof(LabelVisualizer))]
    [Description("Visualizes input values as text labels of configurable size.")]
    public class LabelControl
    {
        public LabelControl()
        {
            FontSize = 14;
        }

        public float FontSize { get; set; }

        public IObservable<TSource> Process<TSource>(IObservable<TSource> source)
        {
            return source;
        }
    }
}
