using System.ComponentModel;

namespace Aeon.Acquisition
{
    [Description("Represents provenance metadata for a repository.")]
    public class RepositoryMetadata
    {
        [Description("The commit hash at the current tip of the repository.")]
        public string Commit { get; set; }

        [Description("The URL of the origin remote of the repository.")]
        public string Url { get; set; }
    }
}
