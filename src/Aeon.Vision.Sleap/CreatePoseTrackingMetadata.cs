using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using Bonsai;

namespace Aeon.Vision.Sleap
{
    [Description("Initializes a pose tracking metadata object from the specified model path.")]
    public class CreatePoseTrackingMetadata : Source<PoseTrackingMetadata>
    {
        [Description("The relative or absolute path of the folder containing the pretrained pose tracking model.")]
        [Editor("Bonsai.Design.FolderNameEditor, Bonsai.Design", DesignTypes.UITypeEditor)]
        public string ModelPath { get; set; }

        public override IObservable<PoseTrackingMetadata> Generate()
        {
            return Observable.Defer(() => Observable.Return(new PoseTrackingMetadata(ModelPath)));
        }

        public IObservable<PoseTrackingMetadata> Generate(IObservable<string> source)
        {
            return source.Select(prefix => new PoseTrackingMetadata(ModelPath, prefix));
        }
    }

    public class PoseTrackingMetadata
    {
        internal PoseTrackingMetadata(string modelPath, string prefix = default)
        {
            if (string.IsNullOrEmpty(modelPath))
            {
                throw new ArgumentNullException(nameof(modelPath));
            }

            var fullModelPath = prefix != null ? Path.Combine(prefix, modelPath) : modelPath;
            var directoryInfo = new DirectoryInfo(fullModelPath);
            if (!directoryInfo.Exists)
            {
                throw new ArgumentException("The specified model path does not exist.", nameof(modelPath));
            }

            var pbFiles = directoryInfo.GetFiles("*.pb");
            if (pbFiles.Length != 1)
            {
                throw new ArgumentException(
                    "The specified model path does not contain an exported graph; or has more than one pb file.",
                    nameof(modelPath));
            }

            var jsonConfigFiles = directoryInfo.GetFiles("confmap_config.json");
            if (jsonConfigFiles.Length != 1)
            {
                throw new ArgumentException(
                    "The specified model path does not contain a trained config file; or has more than one JSON file.",
                    nameof(modelPath));
            }

            if (Path.IsPathRooted(modelPath))
            {
                var separatorIndex = modelPath.IndexOf(Path.VolumeSeparatorChar);
                if (separatorIndex >= 0) modelPath = modelPath.Substring(separatorIndex + 1);
            }

            ModelFileName = pbFiles[0].FullName;
            TrainingConfig = jsonConfigFiles[0].FullName;
            LogName = modelPath
                .Trim(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
                .Replace(Path.DirectorySeparatorChar, '_')
                .Replace(Path.AltDirectorySeparatorChar, '_');
        }

        public string ModelFileName { get; }

        public string TrainingConfig { get; }

        public string LogName { get; }

        public override string ToString()
        {
            return $"{nameof(PoseTrackingMetadata)}: {LogName}";
        }
    }
}
