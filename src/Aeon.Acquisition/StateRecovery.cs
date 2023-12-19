using System.IO;
using Newtonsoft.Json;

namespace Aeon.Acquisition
{
    public static class StateRecovery<TState> where TState : new()
    {
        static string GetFileName(string name) => !string.IsNullOrEmpty(name)
            ? $"~{name}.{typeof(TState).Name}.tmp"
            : $"~{typeof(TState).Name}.tmp";

        public static void Serialize(string name, TState value)
        {
            var fileName = GetFileName(name);
            var json = JsonConvert.SerializeObject(value, Formatting.Indented);
            File.WriteAllText(fileName, json);
        }

        public static TState Deserialize(string name)
        {
            var fileName = GetFileName(name);
            if (!File.Exists(fileName))
            {
                return new TState();
            }

            var json = File.ReadAllText(fileName);
            try
            {
                return JsonConvert.DeserializeObject<TState>(json);
            }
            catch (JsonException)
            {
                return new TState();
            }
        }
    }
}
