namespace Aeon.Acquisition
{
    public class EnvironmentStateMetadata
    {
        public EnvironmentStateMetadata(string name, EnvironmentStateType state)
        {
            Name = name;
            Type = state;
        }

        public string Name { get; }

        public EnvironmentStateType Type { get; }
    }

    public enum EnvironmentStateType
    {
        Experiment,
        Maintenance
    }
}
