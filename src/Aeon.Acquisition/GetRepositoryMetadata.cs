using Bonsai;
using System;
using System.ComponentModel;
using System.Reactive.Linq;
using LibGit2Sharp;

namespace Aeon.Acquisition
{
    [Combinator]
    [WorkflowElementCategory(ElementCategory.Transform)]
    [Description("Retrieves provenance metadata for a repository.")]
    public class GetRepositoryMetadata
    {
        public IObservable<RepositoryMetadata> Process(IObservable<IRepository> source)
        {
            return source.Select(value =>
            {
                var tip = value.Head.Tip;
                if (tip == null)
                {
                    throw new InvalidOperationException("The repository has no commits at the current head.");
                }

                var remote = value.Network.Remotes["origin"];
                if (remote == null)
                {
                    throw new InvalidOperationException("The repository has no remote named 'origin'.");
                }

                return new RepositoryMetadata(tip.Sha, remote.Url);
            });
        }
    }
}
