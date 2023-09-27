using Bonsai.Design;
using Bonsai.Expressions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace Aeon.Environment
{
    [TypeDescriptionProvider(typeof(EntryTypeDescriptionProvider))]
    public class EnvironmentSubjectStateEntry
    {
        [TypeConverter(typeof(EntryIdConverter))]
        public string Id { get; set; } = string.Empty;

        public float ReferenceWeight { get; set; }

        public float Weight { get; set; }

        [XmlIgnore]
        [Browsable(false)]
        public EnvironmentSubjectChangeType Type { get; set; }

        class EntryTypeDescriptionProvider : TypeDescriptionProvider
        {
            static readonly TypeDescriptionProvider parentProvider = TypeDescriptor.GetProvider(typeof(EnvironmentSubjectStateEntry));

            public EntryTypeDescriptionProvider()
                : base(parentProvider)
            {
            }

            public override ICustomTypeDescriptor GetTypeDescriptor(Type objectType, object instance)
            {
                var descriptor = base.GetTypeDescriptor(objectType, instance);
                if (instance is EnvironmentSubjectStateEntry metadata && metadata.Type == EnvironmentSubjectChangeType.Exit)
                {
                    return new ReadOnlyTypeDescriptor(descriptor, nameof(Id), nameof(ReferenceWeight));
                }

                return descriptor;
            }
        }

        class EntryIdConverter : StringConverter
        {
            EnvironmentSubjectState GetSource(ITypeDescriptorContext context)
            {
                var visualizerContext = (ITypeVisualizerContext)context?.GetService(typeof(ITypeVisualizerContext));
                if (visualizerContext == null) return null;
                var visualizerElement = ExpressionBuilder.GetVisualizerElement(visualizerContext.Source);
                return (EnvironmentSubjectState)ExpressionBuilder.GetWorkflowElement(visualizerElement.Builder);
            }

            IEnumerable<IGrouping<string, string[]>> GetDatabaseEntries(EnvironmentSubjectState source)
            {
                if (!File.Exists(source.DatabasePath)) return Enumerable.Empty<IGrouping<string, string[]>>();
                return from row in File.ReadAllLines(source.DatabasePath).Skip(1)
                       let attributes = row?.Split(',')
                       where attributes?.Length > 1
                       group attributes by attributes[0];
            }

            public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
            {
                if (context != null && !context.PropertyDescriptor.IsReadOnly &&
                    context.Instance is EnvironmentSubjectStateEntry entry &&
                    !string.IsNullOrWhiteSpace(entry.Id))
                {
                    var source = GetSource(context);
                    var subjects = GetDatabaseEntries(source).FirstOrDefault(group => group.Key == entry.Id);
                    if (subjects != null)
                    {
                        var attributes = subjects.Last();
                        if (float.TryParse(attributes[attributes.Length - 1], out float referenceWeight))
                        {
                            entry.ReferenceWeight = referenceWeight;
                        }
                    }
                }

                return base.ConvertTo(context, culture, value, destinationType);
            }

            public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
            {
                if (context.PropertyDescriptor.IsReadOnly)
                {
                    return false;
                }

                var source = GetSource(context);
                return File.Exists(source?.DatabasePath);
            }

            public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
            {
                var source = GetSource(context);
                if (source == null) return base.GetStandardValues(context);
                var subjects = GetDatabaseEntries(source);
                return new StandardValuesCollection(subjects.Select(group => group.Key).ToArray());
            }
        }
    }
}
