using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using Bonsai;
using LibGit2Sharp;

namespace Aeon.Acquisition
{
    [Combinator]
    [WorkflowElementCategory(ElementCategory.Transform)]
    [Description("Retrieves the name of the data folder depending on the state of the specified repository.")]
    public class GetDataPath
    {
        [Description("The relative or absolute path of the clean data directory.")]
        [Editor("Bonsai.Design.FolderNameEditor, Bonsai.Design", DesignTypes.UITypeEditor)]
        public string DataPath { get; set; }

        [Description("The relative or absolute path of the fallback data directory.")]
        [Editor("Bonsai.Design.FolderNameEditor, Bonsai.Design", DesignTypes.UITypeEditor)]
        public string FallbackPath { get; set; }

        [Description("The optional name of a remote used to verify the tag before the data path is used. " +
                     "If the tag is missing from the remote or the remote cannot be queried, the session " +
                     "is routed to the fallback path. An error is raised if the tag on the remote points " +
                     "to a different commit. If empty, no verification is performed.")]
        public string Remote { get; set; }

        public IObservable<string> Process(IObservable<IRepository> source)
        {
            return source.Select(repo =>
            {
                var status = repo.RetrieveStatus();
                var untrackedChanges = status.Untracked.Any();
                if (!(status.IsDirty || untrackedChanges) &&
                    repo.Tags.FirstOrDefault(r => r.Target == repo.Head.Tip) is Tag tag &&
                    (string.IsNullOrEmpty(Remote) || IsTagOnRemote(repo, Remote, tag)))
                {
                    var name = tag.FriendlyName;
                    var lastDot = name.LastIndexOf('.');
                    if (lastDot >= 0) name = name.Substring(0, lastDot);
                    return Path.Combine(DataPath, name);
                }
                else
                {
                    var commitSha = repo.Head.Tip.Sha;
                    var shortSha = commitSha.Substring(0, 7);
                    var name = repo.Info.IsHeadDetached
                        ? $"detached-{shortSha}"
                        : $"{repo.Head.FriendlyName}-{shortSha}";
                    return Path.Combine(FallbackPath, name);
                }
            });
        }

        static bool IsTagOnRemote(IRepository repo, string remote, Tag tag)
        {
            var tagName = tag.FriendlyName;
            var startInfo = new System.Diagnostics.ProcessStartInfo
            {
                FileName = "git",
                Arguments = $"ls-remote --tags {remote} refs/tags/{tagName}",
                WorkingDirectory = repo.Info.WorkingDirectory,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };
            using var process = System.Diagnostics.Process.Start(startInfo);
            var output = process.StandardOutput.ReadToEnd();
            var error = process.StandardError.ReadToEnd();
            process.WaitForExit();
            if (process.ExitCode != 0)
            {
                Console.Error.WriteLine(
                    $"Failed to query remote '{remote}' for tag '{tagName}', " +
                    $"falling back to the test data path. {error.Trim()}");
                return false;
            }

            string remoteSha = null;
            foreach (var line in output.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries))
            {
                var parts = line.Trim().Split('\t');
                if (parts.Length == 2 && parts[1] == $"refs/tags/{tagName}")
                {
                    remoteSha = parts[0];
                    break;
                }
            }

            if (remoteSha == null)
            {
                Console.Error.WriteLine(
                    $"Tag '{tagName}' does not exist on remote '{remote}', " +
                    "falling back to the test data path. Push the tag to use the data path.");
                return false;
            }

            var localSha = tag.Annotation != null ? tag.Annotation.Sha : tag.Target.Sha;
            if (remoteSha != localSha)
            {
                throw new InvalidOperationException(
                    $"Tag '{tagName}' on remote '{remote}' points to {remoteSha} " +
                    $"but the local tag points to {localSha}.");
            }
            return true;
        }
    }
}
