using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using Bonsai;
using LibGit2Sharp;

namespace Aeon.Acquisition
{
    [Combinator]
    [WorkflowElementCategory(ElementCategory.Transform)]
    [Description("Ensures a clean, untagged HEAD carries an auto-generated test tag pushed to the remote.")]
    public class AutoTestTag
    {
        [Description("The prefix used to name auto-generated test tags.")]
        public string Prefix { get; set; } = "test";

        [Description("The name of the remote the test tag is pushed to.")]
        public string Remote { get; set; } = "origin";

        public IObservable<IRepository> Process(IObservable<IRepository> source)
        {
            return source.Select(repo =>
            {
                var status = repo.RetrieveStatus();
                if (status.IsDirty || status.Untracked.Any()) return repo;
                if (repo.Info.IsHeadDetached) return repo;
                if (repo.Tags.Any(tag => tag.Target == repo.Head.Tip)) return repo;

                var prefix = string.IsNullOrEmpty(Prefix) ? "test" : Prefix;
                var tagBase = $"{prefix}-{SanitizeName(repo.Head.FriendlyName)}.";
                var nextNumber = repo.Tags
                    .Select(tag => tag.FriendlyName)
                    .Where(name => name.StartsWith(tagBase, StringComparison.Ordinal))
                    .Select(name => int.TryParse(name.Substring(tagBase.Length), out var number) ? number : 0)
                    .DefaultIfEmpty(0)
                    .Max() + 1;
                var tagName = tagBase + nextNumber;
                var tag = repo.ApplyTag(tagName);
                try
                {
                    PushTag(repo, Remote, tagName);
                }
                catch
                {
                    repo.Tags.Remove(tag);
                    throw;
                }
                Console.WriteLine($"Created test tag '{tagName}' and pushed it to '{Remote}'.");
                return repo;
            });
        }

        static string SanitizeName(string name)
        {
            var invalidChars = Path.GetInvalidFileNameChars();
            var sanitized = name.Select(c => invalidChars.Contains(c) ? '-' : c);
            return new string(sanitized.ToArray());
        }

        static void PushTag(IRepository repo, string remote, string tagName)
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = "git",
                Arguments = $"push {remote} refs/tags/{tagName}",
                WorkingDirectory = repo.Info.WorkingDirectory,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };
            using var process = System.Diagnostics.Process.Start(startInfo);
            var error = process.StandardError.ReadToEnd();
            process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            if (process.ExitCode != 0)
            {
                throw new InvalidOperationException(
                    $"Failed to push tag '{tagName}' to remote '{remote}'. {error.Trim()}");
            }
        }
    }
}
