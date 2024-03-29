﻿using System;
using System.ComponentModel;
using System.Reactive.Linq;
using Bonsai;

namespace Aeon.Environment
{
    [TypeConverter(typeof(SettingsConverter))]
    [Description("Creates a light controller preset.")]
    public class CreateRoomLightPreset : Source<RoomLightPreset>
    {
        [Description("The normalized light level to set on the cold-white channels.")]
        public float ColdWhite { get; set; }

        [Description("The normalized light level to set on the warm-white channels.")]
        public float WarmWhite { get; set; }

        [Description("The normalized light level to set on the red light channels.")]
        public float Red { get; set; }

        public override IObservable<RoomLightPreset> Generate()
        {
            return Observable.Return(new RoomLightPreset(ColdWhite, WarmWhite, Red));
        }

        public IObservable<RoomLightPreset> Generate<TSource>(IObservable<TSource> source)
        {
            return source.Select(value => new RoomLightPreset(ColdWhite, WarmWhite, Red));
        }

        class SettingsConverter : ExpandableObjectConverter
        {
            public override PropertyDescriptorCollection GetProperties(
                ITypeDescriptorContext context,
                object value,
                Attribute[] attributes)
            {
                return base
                    .GetProperties(context, value, attributes)
                    .Sort(new[]
                    {
                        nameof(ColdWhite),
                        nameof(WarmWhite),
                        nameof(Red)
                    });
            }
        }
    }
}
