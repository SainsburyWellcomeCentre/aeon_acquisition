using System;
using System.ComponentModel;
using System.Reactive.Linq;
using Bonsai;

namespace Aeon.Acquisition
{
    [Description("Creates a room light controller message for the specified light panel.")]
    public class CreateRoomLightMessage : Source<RoomLightMessage>
    {
        const int NoChange = -1;
        const int MaxLightValue = 254;

        [Category(nameof(CategoryAttribute.Design))]
        [Description("The name of the room light panel.")]
        public string Name { get; set; }

        [Range(NoChange, MaxLightValue)]
        [Editor(DesignTypes.NumericUpDownEditor, DesignTypes.UITypeEditor)]
        [Description("The intensity to set on the cold white channel for the specified light panel.")]
        public int ColdWhite { get; set; }

        [Range(NoChange, MaxLightValue)]
        [Editor(DesignTypes.NumericUpDownEditor, DesignTypes.UITypeEditor)]
        [Description("The intensity to set on the warm white channel for the specified light panel.")]
        public int WarmWhite { get; set; }

        [Range(NoChange, MaxLightValue)]
        [Editor(DesignTypes.NumericUpDownEditor, DesignTypes.UITypeEditor)]
        [Description("The intensity to set on the red channel for the specified light panel.")]
        public int Red { get; set; }

        public override IObservable<RoomLightMessage> Generate()
        {
            return Observable.Return(new RoomLightMessage(Name, ColdWhite, WarmWhite, Red));
        }

        public IObservable<RoomLightMessage> Generate<TSource>(IObservable<TSource> source)
        {
            return source.Select(value => new RoomLightMessage(Name, ColdWhite, WarmWhite, Red));
        }
    }
}
