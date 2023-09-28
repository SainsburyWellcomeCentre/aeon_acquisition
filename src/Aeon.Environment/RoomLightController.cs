using System;
using System.ComponentModel;
using System.IO.Ports;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Bonsai;
using Bonsai.IO.Ports;

namespace Aeon.Environment
{
    [Combinator]
    [Description("Creates and configures a connection to the room light controller over Brainboxes Ethernet to Serial.")]
    public class RoomLightController
    {
        readonly CreateSerialPort serialPort = new();

        [TypeConverter(typeof(SerialPortNameConverter))]
        [Description("The name of the room light controller serial port.")]
        public string PortName
        {
            get { return serialPort.PortName; }
            set { serialPort.PortName = value; }
        }

        [XmlIgnore]
        [Description("The delay between each message sent to the room light controller serial port.")]
        public TimeSpan MessageDelay { get; set; }

        [Browsable(false)]
        [XmlElement(nameof(MessageDelay))]
        public string MessageDelayXml
        {
            get { return XmlConvert.ToString(MessageDelay); }
            set { MessageDelay = XmlConvert.ToTimeSpan(value); }
        }

        async Task SetRoomLightAsync(SerialPort serial, int channel, int value, CancellationToken cancellationToken)
        {
            if (value >= 0 && !cancellationToken.IsCancellationRequested)
            {
                serial.WriteLine($"{channel:00}{value:000}");
                serial.ReadLine();
                var delay = MessageDelay;
                if (delay > TimeSpan.Zero)
                {
                    await Observable.Timer(delay);
                }
            }
        }

        public IObservable<RoomLightMessage> Process(IObservable<RoomLightMessage> source)
        {
            return serialPort.Generate().SelectMany(serial =>
            {
                return source.Select(message => Observable.FromAsync(async cancellationToken =>
                {
                    await SetRoomLightAsync(serial, message.Channel, message.Value, cancellationToken);
                    return message;
                }))
                .Concat();
            });
        }
    }
}
