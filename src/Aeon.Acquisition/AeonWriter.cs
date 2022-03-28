using Bonsai.IO;
using System;
using System.ComponentModel;
using System.IO;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Aeon.Acquisition
{
    [Description("Writes all elements of the sequence to a text file, ensuring stream is flushed after source is silent for a specified duration.")]
    public class AeonWriter : CsvWriter
    {
        [XmlIgnore]
        [Description("The time duration after which to flush the stream if no new value is generated.")]
        public TimeSpan FlushDuration { get; set; }

        [Browsable(false)]
        [XmlElement(nameof(FlushDuration))]
        public string FlushDurationXml
        {
            get { return XmlConvert.ToString(FlushDuration); }
            set { FlushDuration = XmlConvert.ToTimeSpan(value); }
        }

        IObservable<TSource> Process<TSource>(IObservable<TSource> source, string header, Action<TSource, StreamWriter> writeAction)
        {
            return Observable.Create<TSource>(observer =>
            {
                var fileName = FileName;
                if (string.IsNullOrEmpty(fileName))
                {
                    throw new InvalidOperationException("A valid file path must be specified.");
                }

                PathHelper.EnsureDirectory(fileName);
                fileName = PathHelper.AppendSuffix(fileName, Suffix);
                if (File.Exists(fileName) && !Overwrite && !Append)
                {
                    throw new IOException(string.Format("The file '{0}' already exists.", fileName));
                }

                var disposable = new WriterDisposable();
                disposable.Schedule(() =>
                {
                    try
                    {
                        var writer = new StreamWriter(fileName, Append, Encoding.ASCII);
                        if (!string.IsNullOrEmpty(header)) writer.WriteLine(header);
                        disposable.Writer = writer;
                    }
                    catch (Exception ex)
                    {
                        observer.OnError(ex);
                    }
                });

                var process = source.Do(input =>
                {
                    disposable.Schedule(() =>
                    {
                        try { writeAction(input, disposable.Writer); }
                        catch (Exception ex)
                        {
                            observer.OnError(ex);
                        }
                    });
                });

                var flushInterval = FlushDuration;
                if (flushInterval > TimeSpan.Zero)
                {
                    process = process.Publish(ps => ps.Merge(
                        ps.Throttle(flushInterval).Do(value =>
                        {
                            disposable.Schedule(() =>
                            {
                                try { disposable.Writer.Flush(); }
                                catch (Exception ex)
                                {
                                    observer.OnError(ex);
                                }
                            });
                        }).IgnoreElements()));
                }

                return new CompositeDisposable(process.SubscribeSafe(observer), disposable);
            });
        }

        class WriterDisposable : IDisposable
        {
            readonly EventLoopScheduler scheduler = new EventLoopScheduler();

            public StreamWriter Writer { get; set; }

            public void Schedule(Action action)
            {
                scheduler.Schedule(action);
            }

            void DisposeWriter()
            {
                Writer?.Dispose();
            }

            public void Dispose()
            {
                scheduler.Schedule(() =>
                {
                    DisposeWriter();
                    scheduler.Dispose();
                });
            }
        }
    }
}
