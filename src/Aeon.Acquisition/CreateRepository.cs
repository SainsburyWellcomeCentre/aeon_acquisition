using Bonsai;
using System;
using System.ComponentModel;
using System.Reactive.Linq;
using LibGit2Sharp;

namespace Aeon.Acquisition
{
    [DefaultProperty(nameof(Path))]
    [Description("Creates a repository object from the specified working directory or .git folder.")]
    public class CreateRepository : Source<IRepository>
    {
        [Description("The relative or absolute path of the repository directory.")]
        [Editor("Bonsai.Design.FolderNameEditor, Bonsai.Design", DesignTypes.UITypeEditor)]
        public string Path { get; set; }

        public override IObservable<IRepository> Generate()
        {
            return Observable.Defer(() =>
            {
                var path = string.IsNullOrEmpty(Path) ? Environment.CurrentDirectory : Path;
                Repository repo = new Repository(path);
                return Observable.Return(repo);
            });
        }
    }
}