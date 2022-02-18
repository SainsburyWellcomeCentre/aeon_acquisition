using Bonsai;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reactive.Linq;

namespace Aeon.Acquisition
{
    [Combinator]
    [Description("Generates a sequence with the total number of bytes available in the specified drive.")]
    [WorkflowElementCategory(ElementCategory.Transform)]
    public class AvailableDiskSpace
    {
        [Description("Specifies the root directory of the drive to check for available disk space.")]
        public string DrivePath { get; set; }

        public IObservable<long> Process<TSource>(IObservable<TSource> source)
        {
            return Observable.Defer(() =>
            {
                var rootPath = Path.GetPathRoot(DrivePath);
                var driveInfo = DriveInfo
                    .GetDrives()
                    .FirstOrDefault(drive => drive.RootDirectory.FullName == rootPath);
                if (driveInfo == null)
                {
                    throw new InvalidOperationException("The specified drive could not be found.");
                }

                return source.Select(_ => driveInfo.AvailableFreeSpace);
            });
        }
    }
}
