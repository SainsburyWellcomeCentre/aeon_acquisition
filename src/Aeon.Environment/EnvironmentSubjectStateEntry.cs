using Bonsai.Design;
using Bonsai.Expressions;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
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

            IEnumerable<EnvironmentSubjectStateEntry> GetFileDatabaseEntries(string databasePath)
            {
                return from row in File.ReadAllLines(databasePath).Skip(1)
                       let attributes = row?.Split(',')
                       where attributes?.Length > 1
                       select new EnvironmentSubjectStateEntry
                       {
                           Id = attributes[0],
                           ReferenceWeight = float.Parse(attributes[1])
                       };
            }

            IEnumerable<EnvironmentSubjectStateEntry> GetSqlDatabaseEntries(string databasePath)
            {
                return Observable.Using(
                    () => new MySqlConnection(databasePath),
                    connection =>
                    {
                        connection.Open();
                        return EnumerateColony.Query(connection)
                                              .SubscribeOn(Scheduler.Default);
                    })
                    .Where(record => record.Available)
                    .Select(record => new EnvironmentSubjectStateEntry
                    {
                        Id = record.Id
                    }).ToList().Wait();
            }

            IEnumerable<EnvironmentSubjectStateEntry> GetDatabaseEntries(EnvironmentSubjectState source)
            {
                var databasePath = source.DatabasePath;
                if (string.IsNullOrEmpty(databasePath)) return Enumerable.Empty<EnvironmentSubjectStateEntry>();
                else if (File.Exists(databasePath)) return GetFileDatabaseEntries(databasePath);
                else return GetSqlDatabaseEntries(databasePath);
            }

            public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
            {
                if (context != null && !context.PropertyDescriptor.IsReadOnly &&
                    context.Instance is EnvironmentSubjectStateEntry entry &&
                    !string.IsNullOrWhiteSpace(entry.Id))
                {
                    var source = GetSource(context);
                    var subject = GetDatabaseEntries(source).SingleOrDefault(group => group.Id == entry.Id);
                    if (subject != null)
                    {
                        entry.ReferenceWeight = subject.ReferenceWeight;
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
                return source != null;
            }

            public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
            {
                var source = GetSource(context);
                if (source == null) return base.GetStandardValues(context);
                var subjects = GetDatabaseEntries(source);
                return new StandardValuesCollection(subjects.Select(subject => subject.Id).ToArray());
            }
        }
    }
}
