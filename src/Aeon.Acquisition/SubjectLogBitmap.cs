using Bonsai;
using System;
using System.ComponentModel;
using System.Reactive.Linq;
using Bonsai.Harp;
using System.Drawing;
using System.Windows.Forms;
using Bonsai.IO;

namespace Aeon.Acquisition
{
    [Combinator]
    [Description("Generates a timestamped screenshot of the monitoring interface.")]
    [WorkflowElementCategory(ElementCategory.Sink)]
    public class SubjectLogBitmap
    {
        // The default real-time reference is unix time in total seconds from 1904
        static readonly DateTime ReferenceTime = new DateTime(1904, 1, 1);

        [Editor(DesignTypes.FolderNameEditor, DesignTypes.UITypeEditor)]
        public string Path { get; set; }

        public IObservable<Timestamped<LogMetadata>> Process(IObservable<Timestamped<LogMetadata>> source)
        {
            return source.Do(value =>
            {
                using (Bitmap bitmap = new Bitmap(Screen.PrimaryScreen.Bounds.Width,
                                                  Screen.PrimaryScreen.Bounds.Height))
                {
                    using (Graphics g = Graphics.FromImage(bitmap))
                    {
                        g.CopyFromScreen(Screen.PrimaryScreen.Bounds.X,
                                         Screen.PrimaryScreen.Bounds.Y,
                                         0, 0,
                                         bitmap.Size,
                                         CopyPixelOperation.SourceCopy);
                    }
                    var dateTime = ReferenceTime.AddSeconds(value.Seconds);
                    var fileName = string.Format(Path + "\\" + "{0}_{1}_Summary.png", value.Value.Id, dateTime.ToString("yyyy-MM-ddThh-mm-ss"));
                    PathHelper.EnsureDirectory(fileName);
                    bitmap.Save(fileName);
                }
            });
        }
    }
}