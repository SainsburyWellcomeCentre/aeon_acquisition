using Bonsai;
using System;
using System.ComponentModel;
using System.Reactive.Linq;
using LibGit2Sharp;


namespace Aeon.Acquisition
{
    [DefaultProperty(nameof(RepoPath))]
    [Description("Returns a Repository object from the target repository root path. " +
        "If no path is provided it will use the current working directory")]

    public class GetRepository : Source<IRepository>
    {
        [Description("The relative or absolute path of the repository directory.")]
        [Editor("Bonsai.Design.FolderNameEditor, Bonsai.Design", DesignTypes.UITypeEditor)]
        public string RepoPath { get; set; } = "";


        public override IObservable<IRepository> Generate()
        {

            return Observable.Defer(() =>
            {
                string cpath;
                if (RepoPath == "")
                {
                    cpath = Environment.CurrentDirectory;
                }
                else { cpath = RepoPath; }
                Repository repo = new Repository(cpath);
                return Observable.Return(repo);
            });
        }
    }
}