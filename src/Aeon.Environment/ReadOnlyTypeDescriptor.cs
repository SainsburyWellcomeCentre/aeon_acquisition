using System;
using System.ComponentModel;
using System.Linq;

namespace Aeon.Environment
{
    class ReadOnlyTypeDescriptor : CustomTypeDescriptor
    {
        readonly string[] filterProperties;

        public ReadOnlyTypeDescriptor(ICustomTypeDescriptor parent, params string[] properties)
            : base(parent)
        {
            filterProperties = properties;
        }

        private PropertyDescriptorCollection MapProperties(PropertyDescriptorCollection properties)
        {
            return new PropertyDescriptorCollection(
                properties.OfType<PropertyDescriptor>().Select(property =>
                    Array.Exists(filterProperties, name => property.Name == name) ?
                    new ReadOnlyPropertyDescriptor(property) :
                    property).ToArray());
        }

        public override PropertyDescriptorCollection GetProperties()
        {
            var properties = base.GetProperties();
            return MapProperties(properties);
        }

        public override PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            var properties = base.GetProperties(attributes);
            return MapProperties(properties);
        }
    }

    class ReadOnlyPropertyDescriptor : PropertyDescriptor
    {
        public ReadOnlyPropertyDescriptor(PropertyDescriptor property)
            : base(property)
        {
            ParentDescriptor = property;
        }

        private PropertyDescriptor ParentDescriptor { get; }

        public override Type ComponentType => ParentDescriptor.ComponentType;

        public override bool IsReadOnly => true;

        public override Type PropertyType => ParentDescriptor.PropertyType;

        public override bool CanResetValue(object component)
        {
            return false;
        }

        public override object GetValue(object component)
        {
            return ParentDescriptor.GetValue(component);
        }

        public override void ResetValue(object component)
        {
        }

        public override void SetValue(object component, object value)
        {
        }

        public override bool ShouldSerializeValue(object component)
        {
            return ParentDescriptor.ShouldSerializeValue(component);
        }
    }
}
