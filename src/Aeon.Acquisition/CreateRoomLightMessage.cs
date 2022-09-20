using System;
using System.ComponentModel;
using System.Reactive.Linq;
using Bonsai;

namespace Aeon.Acquisition
{
    [Description("Creates a room light controller message for the specified light.")]
    public class CreateRoomLightMessage : Source<RoomLightMessage>
    {
        const int NoChange = -1;
        const int MaxLightValue = 254;

        [Description("The unique ID of the channel for this light.")]
        public int Channel { get; set; }

        [Range(NoChange, MaxLightValue)]
        [Editor(DesignTypes.NumericUpDownEditor, DesignTypes.UITypeEditor)]
        [Description("The intensity to set on the channel for the specified light.")]
        public int Value { get; set; }

        public override IObservable<RoomLightMessage> Generate()
        {
            return Observable.Return(new RoomLightMessage(Channel, Value));
        }

        public IObservable<RoomLightMessage> Generate<TSource>(IObservable<TSource> source)
        {
            return source.Select(value => new RoomLightMessage(Channel, Value));
        }
    }
}
