namespace GitExtensions.GitLab
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using GitExtensions.GitLab.Client.Repo;
    using GitExtensions.GitLab.Remote;
    using GitUIPluginInterfaces.RepositoryHosts;

    public class GitLabRepo : IHostedRepository
    {
        private readonly Repository repo;

        public GitLabRepo(Repository repo)
        {
            this.repo = repo;
        }

        public int Id => repo.Id;
        public string Owner => repo.Owner?.Name;
        public string Name => repo.Name;
        public string Description => repo.Description;
        // TODO: skipped this thing, check what it's used for and do da implementation
        public bool IsAFork => false;
        public bool IsMine => false;
        public bool IsPrivate => false;
        public int Forks => 0;
        public string Homepage => repo.Homepage;
        public bool CanCreateMergeRequestIn => repo.CanCreateMergeRequestIn;

        public string ParentReadOnlyUrl
        {
            get
            {
                return "";
            }
        }

        public string ParentOwner
        {
            get
            {
                return "";
            }
        }

        public string CloneReadWriteUrl => "";

        public string CloneReadOnlyUrl => "";

        public IReadOnlyList<IHostedBranch> GetBranches()
        {
            return repo.GetBranches()
                .Select(branch => new GitLabBranch(branch))
                .OrderBy(branch => branch.Name, StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        public string GetDefaultBranch()
        {
            return repo.GetDefaultBranch();
        }

        public IHostedRepository Fork()
        {
            throw new NotImplementedException();
        }

        public IReadOnlyList<IPullRequestInformation> GetMergeRequests()
        {
            throw new NotImplementedException();
            //var pullRequests = repo?.GetMergeRequests();

            //if (pullRequests != null)
            //{
            //    return pullRequests
            //        .Select(pr => new  GitHubPullRequest(pr))
            //        .ToList();
            //}

            //return Array.Empty<IPullRequestInformation>();
        }

        public int CreatePullRequest(string myBranch, string remoteBranch, string title, string body)
        {
            throw new NotImplementedException();
            //var pullRequest = repo.CreatePullRequest(GitHubLoginInfo.Username + ":" + myBranch, remoteBranch, title, body);

            //if (pullRequest == null || pullRequest.Number == 0)
            //{
            //    throw new Exception("Failed to create pull request.");
            //}

            //return pullRequest.Number;
        }

        public GitProtocol CloneProtocol { get; set; } = GitProtocol.Ssh;

        public IReadOnlyList<GitProtocol> SupportedCloneProtocols { get; set; } = new[] { GitProtocol.Ssh, GitProtocol.Https };

        public override string ToString() => $"{Owner}/{Name}";

        public IReadOnlyList<IPullRequestInformation> GetPullRequests()
        {
            throw new NotImplementedException();
        }
    }
}
