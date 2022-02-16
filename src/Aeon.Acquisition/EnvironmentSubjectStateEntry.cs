using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Aeon.Acquisition
{
    [TypeDescriptionProvider(typeof(EntryTypeDescriptionProvider))]
    public class EnvironmentSubjectStateEntry
    {
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
    }
}
