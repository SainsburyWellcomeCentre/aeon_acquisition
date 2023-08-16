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

        public IObservable<string> Process(IObservable<IRepository> source)
        {
            return source.Select(repo =>
            {
                var status = repo.RetrieveStatus();
                var untrackedChanges = status.Untracked.Any();
                if (!(status.IsDirty || untrackedChanges) &&
                    repo.Tags.FirstOrDefault(r => r.Target == repo.Head.Tip) is Tag tag)
                {
                    var name = tag.FriendlyName;
                    var lastDot = name.LastIndexOf('.');
                    if (lastDot >= 0) name = name.Substring(0, lastDot);
                    return Path.Combine(DataPath, name);
                }
                else
                {
                    var commitSha = repo.Head.Tip.Sha;
                    var shortSha = commitSha.Substring(commitSha.Length - 7);
                    var name = repo.Info.IsHeadDetached
                        ? $"detached-{shortSha}"
                        : $"{repo.Head.FriendlyName}-{shortSha}";
                    return Path.Combine(FallbackPath, name);
                }
            });
        }
    }
}
