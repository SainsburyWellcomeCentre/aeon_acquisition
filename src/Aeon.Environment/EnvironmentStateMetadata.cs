namespace Aeon.Environment
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

        public override string ToString()
        {
            return $"EnvironmentState({Type})";
        }
    }

    public enum EnvironmentStateType
    {
        Maintenance,
        Experiment
    }
}
