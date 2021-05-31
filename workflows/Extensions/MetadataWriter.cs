using Bonsai;
using System.ComponentModel;
using Bonsai.IO;
using System.IO;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

[Combinator]
[Description("Writes experiment metadata into the specified YML file.")]
[WorkflowElementCategory(ElementCategory.Sink)]
public class MetadataWriter : StreamSink<Experiment, YamlTextWriter>
{
    protected override YamlTextWriter CreateWriter(Stream stream)
    {
        return new YamlTextWriter(stream);
    }

    protected override void Write(YamlTextWriter writer, Experiment input)
    {   
        writer.Write(input);
    }
}

public class YamlTextWriter : StreamWriter
{
    readonly ISerializer serializer;

    public YamlTextWriter(Stream stream)
        : base(stream)
    {
        serializer = new SerializerBuilder()
            .WithNamingConvention(HyphenatedNamingConvention.Instance)
            .Build();
    }
    
    public override void Write(object graph)
    {
        serializer.Serialize(this, graph);
    }
}