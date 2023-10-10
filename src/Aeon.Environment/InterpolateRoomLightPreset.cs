using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Xml.Serialization;
using Bonsai;
using MathNet.Numerics;
using MathNet.Numerics.Interpolation;

namespace Aeon.Environment
{
    [Description("Maps room light presets to a sequence of channel-value messages to the room light controller.")]
    public class InterpolateRoomLightPreset : Combinator<RoomLightPreset, RoomLightMessage>
    {
        [XmlIgnore]
        [Description("Specifies the channel map for the fixtures in the room.")]
        public RoomFixtures Fixtures { get; set; }

        static IInterpolation CreateFixtureInterpolation(InterpolationMethod method, string calibrationFile)
        {
            switch (method)
            {
                case InterpolationMethod.None: return new AnonymousInterpolation(t => t);
                case InterpolationMethod.Zero: return new AnonymousInterpolation(t => 0);
                case InterpolationMethod.Linear:
                    var calibrationContents = File.ReadAllLines(
                        calibrationFile ??
                        throw new ArgumentNullException(nameof(calibrationFile)));
                    var points = new List<double>();
                    var samples = new List<double>();
                    foreach (var row in calibrationContents.Skip(1))
                    {
                        var values = row.Split(',');
                        if (values.Length != 2 ||
                            !double.TryParse(values[0], out double level) ||
                            !double.TryParse(values[1], out double lux))
                        {
                            throw new ArgumentException(
                                "Calibration file should be in 2-column comma-separated text format.",
                                nameof(calibrationFile));
                        }

                        points.Add(lux);
                        samples.Add(level);
                    }
                    return Interpolate.Linear(points, samples);
                default: throw new ArgumentException("Unsupported interpolation method.", nameof(method));
            }
        }

        static void OnNextPreset(
            float preset,
            Fixture fixture,
            IInterpolation interpolation,
            IObserver<RoomLightMessage> observer)
        {
            if (fixture == null) throw new ArgumentNullException(nameof(fixture));
            if (fixture.Channels != null)
            {
                var value = (int)interpolation.Interpolate(preset);
                for (int i = 0; i < fixture.Channels.Count; i++)
                {
                    observer.OnNext(new RoomLightMessage(
                        channel: fixture.Channels[i],
                        value: value));
                }
            }
        }

        public override IObservable<RoomLightMessage> Process(IObservable<RoomLightPreset> source)
        {
            var fixtures = Fixtures ?? throw new InvalidOperationException("No fixtures have been specified.");
            return Observable.Create<RoomLightMessage>(observer =>
            {
                var coldWhiteLookup = CreateFixtureInterpolation(
                    fixtures.ColdWhite.InterpolationMethod,
                    fixtures.ColdWhite.CalibrationFile);
                var warmWhiteLookup = CreateFixtureInterpolation(
                    fixtures.WarmWhite.InterpolationMethod,
                    fixtures.WarmWhite.CalibrationFile);
                var redLookup = CreateFixtureInterpolation(
                    fixtures.Red.InterpolationMethod,
                    fixtures.Red.CalibrationFile);

                var presetObserver = Observer.Create<RoomLightPreset>(
                    value =>
                    {
                        OnNextPreset(value.ColdWhite, fixtures.ColdWhite, coldWhiteLookup, observer);
                        OnNextPreset(value.WarmWhite, fixtures.WarmWhite, warmWhiteLookup, observer);
                        OnNextPreset(value.Red, fixtures.Red, redLookup, observer);
                    },
                    observer.OnError,
                    observer.OnCompleted);
                return source.SubscribeSafe(presetObserver);
            });
        }

        class AnonymousInterpolation : IInterpolation
        {
            readonly Func<double, double> interpolate;

            public AnonymousInterpolation(Func<double, double> interpolator)
            {
                interpolate = interpolator;
            }

            public double Interpolate(double t) => interpolate(t);
            public bool SupportsDifferentiation => false;
            public bool SupportsIntegration => false;
            public double Differentiate(double t) => throw new NotSupportedException();
            public double Differentiate2(double t) => throw new NotSupportedException();
            public double Integrate(double t) => throw new NotSupportedException();
            public double Integrate(double a, double b) => throw new NotSupportedException();
        }
    }
}
