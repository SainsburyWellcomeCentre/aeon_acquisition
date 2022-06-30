using Bonsai;
using System;
using System.ComponentModel;
using System.Reactive.Linq;
using LibGit2Sharp;

namespace Aeon.Acquisition
{
    [Combinator]
    [WorkflowElementCategory(ElementCategory.Transform)]
    [Description("Determines whether a specified repository is clean or if uncommitted or untracked changes exist")]
    public class IsRepositoryClean
    {
        public IObservable<bool> Process(IObservable<IRepository> source)
        {
            return source.Select(value =>
            {
                var status = value.RetrieveStatus();
                var untrackedChanges = value.Diff.Compare<TreeChanges>().Count > 0;
                return !(status.IsDirty || untrackedChanges);
            });
        }
    }
}
