namespace GitExtensions.GitLab
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using GitExtensions.GitLab.Client.Repo;
    using GitUIPluginInterfaces.RepositoryHosts;

    public class GitLabRepo : IHostedRepository
    {
        private Repository repo;

        public GitLabRepo(Repository repo)
        {
            this.repo = repo;
        }

        public string Owner => repo.Owner?.Name;
        public string Name => repo.Name;
        public string Description => repo.Description;
        // TODO: skipped this thing, check what it's used for and do da implementation
        public bool IsAFork => false;
        public bool IsMine => Owner ==  GitHubLoginInfo.Username;
        public bool IsPrivate => repo.Private;
        public int Forks => repo.Forks;
        public string Homepage => repo.Homepage;

        public string ParentReadOnlyUrl
        {
            get
            {
                if (!repo.Fork)
                {
                    return null;
                }

                if (!repo.Detailed)
                {
                    if (repo.Organization != null)
                    {
                        return null;
                    }

                    repo = GitHub3Plugin.GitHub.getRepository(Owner, Name);
                }

                return CloneProtocol == GitProtocol.Ssh ? repo.Parent?.GitUrl : repo.Parent?.CloneUrl;
            }
        }

        public string ParentOwner
        {
            get
            {
                if (!repo.Fork)
                {
                    return null;
                }

                if (!repo.Detailed)
                {
                    if (repo.Organization != null)
                    {
                        return null;
                    }

                    repo = GitHub3Plugin.GitHub.getRepository(Owner, Name);
                }

                return repo.Parent?.Owner.Login;
            }
        }

        public string CloneReadWriteUrl => CloneProtocol == GitProtocol.Ssh ? repo.SshUrl : repo.CloneUrl;

        public string CloneReadOnlyUrl => CloneProtocol == GitProtocol.Ssh ? repo.GitUrl : repo.CloneUrl;

        public IReadOnlyList<IHostedBranch> GetBranches()
        {
            return repo.GetBranches()
                .Select(branch => new GitHubBranch(branch))
                .OrderBy(branch => branch.Name, StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        public string GetDefaultBranch()
        {
            return repo.GetDefaultBranch();
        }

        public IHostedRepository Fork()
        {
            return new GitLabRepo(repo.CreateFork());
        }

        public IReadOnlyList<IPullRequestInformation> GetPullRequests()
        {
            var pullRequests = repo?.GetPullRequests();

            if (pullRequests != null)
            {
                return pullRequests
                    .Select(pr => new GitHubPullRequest(pr))
                    .ToList();
            }

            return Array.Empty<IPullRequestInformation>();
        }

        public int CreatePullRequest(string myBranch, string remoteBranch, string title, string body)
        {
            var pullRequest = repo.CreatePullRequest(GitHubLoginInfo.Username + ":" + myBranch, remoteBranch, title, body);

            if (pullRequest == null || pullRequest.Number == 0)
            {
                throw new Exception("Failed to create pull request.");
            }

            return pullRequest.Number;
        }

        public GitProtocol CloneProtocol { get; set; } = GitProtocol.Ssh;

        public IReadOnlyList<GitProtocol> SupportedCloneProtocols { get; set; } = new[] { GitProtocol.Ssh, GitProtocol.Https };

        public override string ToString() => $"{Owner}/{Name}";
    }
}
