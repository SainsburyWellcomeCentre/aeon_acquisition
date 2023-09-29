using System;
using System.ComponentModel;
using System.Reactive.Linq;
using Bonsai;

namespace Aeon.Environment
{
    [TypeConverter(typeof(SettingsConverter))]
    [Description("Creates a light controller preset for the specified channel map.")]
    public class CreateRoomLightPreset : Source<RoomLightPreset>
    {
        [Description("The unique ID of the channel map on which to apply this preset.")]
        public string Name { get; set; }

        [Range(RoomLightMessage.NoChange, RoomLightMessage.MaxLightValue)]
        [Editor(DesignTypes.NumericUpDownEditor, DesignTypes.UITypeEditor)]
        [Description("The normalized light level to set on the cold-white channels.")]
        public float ColdWhite { get; set; }

        [Range(RoomLightMessage.NoChange, RoomLightMessage.MaxLightValue)]
        [Editor(DesignTypes.NumericUpDownEditor, DesignTypes.UITypeEditor)]
        [Description("The normalized light level to set on the warm-white channels.")]
        public float WarmWhite { get; set; }

        [Range(RoomLightMessage.NoChange, RoomLightMessage.MaxLightValue)]
        [Editor(DesignTypes.NumericUpDownEditor, DesignTypes.UITypeEditor)]
        [Description("The normalized light level to set on the red light channels.")]
        public float Red { get; set; }

        public override IObservable<RoomLightPreset> Generate()
        {
            return Observable.Return(new RoomLightPreset(Name, ColdWhite, WarmWhite, Red));
        }

        public IObservable<RoomLightPreset> Generate<TSource>(IObservable<TSource> source)
        {
            return source.Select(value => new RoomLightPreset(Name, ColdWhite, WarmWhite, Red));
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
                        nameof(Name),
                        nameof(ColdWhite),
                        nameof(WarmWhite),
                        nameof(Red)
                    });
            }
        }
    }
}
