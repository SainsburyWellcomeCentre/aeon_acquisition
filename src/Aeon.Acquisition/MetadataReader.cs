using Bonsai;
using System;
using System.ComponentModel;
using System.Reactive.Linq;
using System.IO;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Aeon.Acquisition
{
    [Combinator]
    [DefaultProperty("FileName")]
    [Description("Reads experiment metadata from the specified YML file.")]
    [WorkflowElementCategory(ElementCategory.Source)]
    public class MetadataReader
    {
        [Description("The name of the metadata file.")]
        [Editor(DesignTypes.OpenFileNameEditor, DesignTypes.UITypeEditor)]
        public string FileName { get; set; }

        public IObservable<Experiment> Process()
        {
            return Observable.Defer(() =>
            {
                var reader = new StringReader(File.ReadAllText(FileName));
                var deserializer = new DeserializerBuilder()
                    .WithNamingConvention(HyphenatedNamingConvention.Instance)
                    .Build();

                var metadata = deserializer.Deserialize<Experiment>(reader);
                return Observable.Return(metadata);
            });
        }
    }
}