//----------------------
// <auto-generated>
//     Generated using the NJsonSchema v10.9.0.0 (Newtonsoft.Json v9.0.0.0) (http://NJsonSchema.org)
// </auto-generated>
//----------------------


namespace Aeon.Environment
{
    #pragma warning disable // Disable all warnings

    /// <summary>
    /// Specifies the channel map for every light fixture in the room.
    /// </summary>
    [System.ComponentModel.DescriptionAttribute("Specifies the channel map for every light fixture in the room.")]
    [Bonsai.CombinatorAttribute()]
    [Bonsai.WorkflowElementCategoryAttribute(Bonsai.ElementCategory.Source)]
    public partial class RoomFixtures
    {
    
        private Fixture _coldWhite;
    
        private Fixture _warmWhite;
    
        private Fixture _red;
    
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [YamlDotNet.Serialization.YamlMemberAttribute(Alias="coldWhite")]
        public Fixture ColdWhite
        {
            get
            {
                return _coldWhite;
            }
            set
            {
                _coldWhite = value;
            }
        }
    
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [YamlDotNet.Serialization.YamlMemberAttribute(Alias="warmWhite")]
        public Fixture WarmWhite
        {
            get
            {
                return _warmWhite;
            }
            set
            {
                _warmWhite = value;
            }
        }
    
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [YamlDotNet.Serialization.YamlMemberAttribute(Alias="red")]
        public Fixture Red
        {
            get
            {
                return _red;
            }
            set
            {
                _red = value;
            }
        }
    
        public System.IObservable<RoomFixtures> Process()
        {
            return System.Reactive.Linq.Observable.Defer(() => System.Reactive.Linq.Observable.Return(
                new RoomFixtures
                {
                    ColdWhite = _coldWhite,
                    WarmWhite = _warmWhite,
                    Red = _red
                }));
        }
    }


    [Bonsai.CombinatorAttribute()]
    [Bonsai.WorkflowElementCategoryAttribute(Bonsai.ElementCategory.Source)]
    public partial class Fixture
    {
    
        private System.Collections.Generic.List<int> _channels = new System.Collections.Generic.List<int>();
    
        private InterpolationMethod _interpolationMethod;
    
        private string _calibrationFile;
    
        /// <summary>
        /// Specifies the collection of channels assigned to the fixture.
        /// </summary>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [YamlDotNet.Serialization.YamlMemberAttribute(Alias="channels")]
        [System.ComponentModel.DescriptionAttribute("Specifies the collection of channels assigned to the fixture.")]
        public System.Collections.Generic.List<int> Channels
        {
            get
            {
                return _channels;
            }
            set
            {
                _channels = value;
            }
        }
    
        /// <summary>
        /// Specifies the method used to interpolate light values for a fixture.
        /// </summary>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [YamlDotNet.Serialization.YamlMemberAttribute(Alias="interpolationMethod")]
        [System.ComponentModel.DescriptionAttribute("Specifies the method used to interpolate light values for a fixture.")]
        public InterpolationMethod InterpolationMethod
        {
            get
            {
                return _interpolationMethod;
            }
            set
            {
                _interpolationMethod = value;
            }
        }
    
        /// <summary>
        /// Specifies the path to the calibration file for this fixture.
        /// </summary>
        [YamlDotNet.Serialization.YamlMemberAttribute(Alias="calibrationFile")]
        [System.ComponentModel.DescriptionAttribute("Specifies the path to the calibration file for this fixture.")]
        public string CalibrationFile
        {
            get
            {
                return _calibrationFile;
            }
            set
            {
                _calibrationFile = value;
            }
        }
    
        public System.IObservable<Fixture> Process()
        {
            return System.Reactive.Linq.Observable.Defer(() => System.Reactive.Linq.Observable.Return(
                new Fixture
                {
                    Channels = _channels,
                    InterpolationMethod = _interpolationMethod,
                    CalibrationFile = _calibrationFile
                }));
        }
    }


    /// <summary>
    /// Specifies the method used to interpolate light values for a fixture.
    /// </summary>
    public enum InterpolationMethod
    {
    
        [YamlDotNet.Serialization.YamlMemberAttribute(Alias="None")]
        None = 0,
    
        [YamlDotNet.Serialization.YamlMemberAttribute(Alias="Zero")]
        Zero = 1,
    
        [YamlDotNet.Serialization.YamlMemberAttribute(Alias="Linear")]
        Linear = 2,
    }


    /// <summary>
    /// Represents channel map configuration used by the light controller.
    /// </summary>
    [System.ComponentModel.DescriptionAttribute("Represents channel map configuration used by the light controller.")]
    [Bonsai.CombinatorAttribute()]
    [Bonsai.WorkflowElementCategoryAttribute(Bonsai.ElementCategory.Source)]
    public partial class ChannelMap
    {
    
        private System.Collections.Generic.IDictionary<string, RoomFixtures> _rooms;
    
        /// <summary>
        /// Specifies the collection of light channel maps for all rooms.
        /// </summary>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [YamlDotNet.Serialization.YamlMemberAttribute(Alias="rooms")]
        [System.ComponentModel.DescriptionAttribute("Specifies the collection of light channel maps for all rooms.")]
        public System.Collections.Generic.IDictionary<string, RoomFixtures> Rooms
        {
            get
            {
                return _rooms;
            }
            set
            {
                _rooms = value;
            }
        }
    
        public System.IObservable<ChannelMap> Process()
        {
            return System.Reactive.Linq.Observable.Defer(() => System.Reactive.Linq.Observable.Return(
                new ChannelMap
                {
                    Rooms = _rooms
                }));
        }
    }


    /// <summary>
    /// Serializes a sequence of data model objects into YAML strings.
    /// </summary>
    [Bonsai.CombinatorAttribute()]
    [Bonsai.WorkflowElementCategoryAttribute(Bonsai.ElementCategory.Transform)]
    [System.ComponentModel.DescriptionAttribute("Serializes a sequence of data model objects into YAML strings.")]
    public partial class SerializeToYaml
    {
    
        private System.IObservable<string> Process<T>(System.IObservable<T> source)
        {
            return System.Reactive.Linq.Observable.Defer(() =>
            {
                var serializer = new YamlDotNet.Serialization.SerializerBuilder().Build();
                return System.Reactive.Linq.Observable.Select(source, value => serializer.Serialize(value)); 
            });
        }

        public System.IObservable<string> Process(System.IObservable<RoomFixtures> source)
        {
            return Process<RoomFixtures>(source);
        }

        public System.IObservable<string> Process(System.IObservable<Fixture> source)
        {
            return Process<Fixture>(source);
        }

        public System.IObservable<string> Process(System.IObservable<ChannelMap> source)
        {
            return Process<ChannelMap>(source);
        }
    }


    /// <summary>
    /// Deserializes a sequence of YAML strings into data model objects.
    /// </summary>
    [System.ComponentModel.DefaultPropertyAttribute("Type")]
    [Bonsai.WorkflowElementCategoryAttribute(Bonsai.ElementCategory.Transform)]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(Bonsai.Expressions.TypeMapping<RoomFixtures>))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(Bonsai.Expressions.TypeMapping<Fixture>))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(Bonsai.Expressions.TypeMapping<ChannelMap>))]
    [System.ComponentModel.DescriptionAttribute("Deserializes a sequence of YAML strings into data model objects.")]
    public partial class DeserializeFromYaml : Bonsai.Expressions.SingleArgumentExpressionBuilder
    {
    
        public DeserializeFromYaml()
        {
            Type = new Bonsai.Expressions.TypeMapping<ChannelMap>();
        }

        public Bonsai.Expressions.TypeMapping Type { get; set; }

        public override System.Linq.Expressions.Expression Build(System.Collections.Generic.IEnumerable<System.Linq.Expressions.Expression> arguments)
        {
            var typeMapping = (Bonsai.Expressions.TypeMapping)Type;
            var returnType = typeMapping.GetType().GetGenericArguments()[0];
            return System.Linq.Expressions.Expression.Call(
                typeof(DeserializeFromYaml),
                "Process",
                new System.Type[] { returnType },
                System.Linq.Enumerable.Single(arguments));
        }

        private static System.IObservable<T> Process<T>(System.IObservable<string> source)
        {
            return System.Reactive.Linq.Observable.Defer(() =>
            {
                var serializer = new YamlDotNet.Serialization.DeserializerBuilder().Build();
                return System.Reactive.Linq.Observable.Select(source, value =>
                {
                    var reader = new System.IO.StringReader(value);
                    var parser = new YamlDotNet.Core.MergingParser(new YamlDotNet.Core.Parser(reader));
                    return serializer.Deserialize<T>(parser);
                });
            });
        }
    }
}