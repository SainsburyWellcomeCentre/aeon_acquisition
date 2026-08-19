using System.ComponentModel;

namespace Aeon.Acquisition
{
    [Description("Represents provenance metadata for a repository.")]
    public class RepositoryMetadata
    {
        public RepositoryMetadata(string commit, string url)
        {
            Commit = commit;
            Url = url;
        }

        [Description("The commit hash at the current tip of the repository.")]
        public string Commit { get; }

        [Description("The URL of the origin remote of the repository.")]
        public string Url { get; }

        public override string ToString()
        {
            return $"RepositoryMetadata(Commit:{Commit}, Url:{Url})";
        }
    }
}
