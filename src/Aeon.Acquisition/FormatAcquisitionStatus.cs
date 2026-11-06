using Bonsai;
using System;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using LibGit2Sharp;

namespace Aeon.Acquisition
{
    [Combinator]
    [WorkflowElementCategory(ElementCategory.Transform)]
    [Description("Formats a status report announcing the start of an acquisition. " +
                 "The report states when the repository is untagged or has uncommitted changes.")]
    public class FormatAcquisitionStatus
    {
        public IObservable<string> Process(IObservable<IRepository> source)
        {
            return source.Select(repo =>
            {
                var status = repo.RetrieveStatus();
                var isClean = !(status.IsDirty || status.Untracked.Any());
                var tip = repo.Head.Tip;
                var tag = tip != null ? repo.Tags.FirstOrDefault(t => t.Target == tip) : null;
                var version = tag != null ? tag.FriendlyName : "untagged";
                var commit = tip != null ? tip.Sha.Substring(0, 7) : "none";
                if (!isClean)
                {
                    commit += " (uncommitted changes)";
                }

                return "## Acquisition Started\n" +
                       $"**Host**: {Environment.MachineName}\n" +
                       $"**Version**: {version}\n" +
                       $"**Commit**: {commit}";
            });
        }
    }
}
