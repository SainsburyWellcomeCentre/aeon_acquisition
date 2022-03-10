using Bonsai;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reactive.Linq;
using YamlDotNet.Serialization;

namespace Aeon.Acquisition
{
    [Description("Constructs a key-value config dictionary from a YAML/JSON file.")]
    public class LoadConfigFile : Source<Dictionary<string, string>>
    {
        [Editor(DesignTypes.OpenFileNameEditor, DesignTypes.UITypeEditor)]
        [FileNameFilter("Config file (*.config)|*.config|YAML file (*.yaml;*.yml)|*.yaml;*.yml|JSON file (*.json)|*.json|All Files|*.*")]
        [Description("The path to the file containing the dictionary values.")]
        public string FileName { get; set; }

        public override IObservable<Dictionary<string, string>> Generate()
        {
            return Observable.Defer(() =>
            {
                var deserializer = new DeserializerBuilder().Build();
                using (var reader = File.OpenText(FileName))
                {
                    var dictionary = deserializer.Deserialize<Dictionary<string, string>>(reader);
                    return Observable.Return(dictionary);
                }
            });
        }
    }
}
