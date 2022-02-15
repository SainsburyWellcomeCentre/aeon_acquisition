using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace Aeon.Acquisition
{
    public static class StateRecovery<TState> where TState : new()
    {
        static string GetFileName(string name) => $"~{typeof(TState).Name}.{name}.tmp";

        public static void Serialize(string name, TState value)
        {
            var fileName = GetFileName(name);
            var serializer = new XmlSerializer(typeof(TState));
            using (var writer = XmlWriter.Create(fileName, new XmlWriterSettings { Indent = true }))
            {
                serializer.Serialize(writer, value);
            }
        }

        public static TState Deserialize(string name)
        {
            var fileName = GetFileName(name);
            if (!File.Exists(fileName))
            {
                return new TState();
            }

            var serializer = new XmlSerializer(typeof(TState));
            using (var reader = XmlReader.Create(fileName))
            {
                return (TState)serializer.Deserialize(reader);
            }
        }
    }
}
