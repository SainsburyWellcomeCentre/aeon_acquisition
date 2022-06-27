using Bonsai;
using System;
using System.ComponentModel;
using System.Reactive.Linq;
using LibGit2Sharp;

namespace Aeon.Acquisition
{

    [Combinator]
    [Description("From a Repository object determines if the repository is dirty or uncommited changes exist")]
    [WorkflowElementCategory(ElementCategory.Transform)]

    public class CheckDirtyRepo
    {

        public IObservable<Boolean> Process(IObservable<IRepository> source)
        {
            return source.Select(value =>
            {
                RepositoryStatus status = value.RetrieveStatus();
                bool isDirtyRepostatus = status.IsDirty;
                bool isUncommitedFiles = value.Diff.Compare<TreeChanges>().Count > 0;
                return isDirtyRepostatus & isUncommitedFiles;
            });
        }


    }

}
