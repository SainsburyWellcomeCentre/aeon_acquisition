using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Aeon.Acquisition
{
    [TypeDescriptionProvider(typeof(EntryTypeDescriptionProvider))]
    public class SubjectChangeEntry
    {
        public string Id { get; set; } = string.Empty;

        public float ReferenceWeight { get; set; }

        public float Weight { get; set; }

        [XmlIgnore]
        [Browsable(false)]
        public SubjectChangeType Type { get; set; }

        class EntryTypeDescriptionProvider : TypeDescriptionProvider
        {
            static readonly TypeDescriptionProvider parentProvider = TypeDescriptor.GetProvider(typeof(SubjectChangeEntry));

            public EntryTypeDescriptionProvider()
                : base(parentProvider)
            {
            }

            public override ICustomTypeDescriptor GetTypeDescriptor(Type objectType, object instance)
            {
                var descriptor = base.GetTypeDescriptor(objectType, instance);
                if (instance is SubjectChangeEntry metadata && metadata.Type == SubjectChangeType.Exit)
                {
                    return new ReadOnlyTypeDescriptor(descriptor, nameof(Id), nameof(ReferenceWeight));
                }

                return descriptor;
            }
        }
    }
}
