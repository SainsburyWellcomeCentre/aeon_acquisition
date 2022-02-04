using Bonsai;
using System;
using System.ComponentModel;
using System.Net;
using System.Reactive.Linq;

namespace Aeon.Acquisition
{
    [Combinator]
    [Description("Sends a message card to the specified incoming webhook.")]
    [WorkflowElementCategory(ElementCategory.Sink)]
    public class SendMessageCard
    {
        [Description("The address of the incoming webhook to which to send the card.")]
        public string Address { get; set; }

        public IObservable<TSource> Process<TSource>(IObservable<TSource> source)
        {
            return source.Do(value =>
            {
                using (var client = new WebClient())
                {
                    var data = $"{{ \"text\": \"{value}\" }}";
                    client.Headers["content-type"] = "application/json";
                    client.UploadString(Address, "post", data);
                }
            });
        }
    }
}
