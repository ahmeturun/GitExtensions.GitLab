namespace GitExtensions.GitLab.Remote
{
	using System;
	using System.Threading.Tasks;
	using GitUIPluginInterfaces.RepositoryHosts;
	using GitExtensions.GitLab.Client.Repo;

    public class GitLabMergeRequest : IPullRequestInformation
    {
        private readonly MergeRequest mergeRequest;
        public GitLabMergeRequest(MergeRequest mergeRequest)
        {
            this.mergeRequest = mergeRequest;
        }

        public string Title => mergeRequest.Title;

        public string Body => mergeRequest.Description;

        public string Owner => mergeRequest.Author.Name;

        public DateTime Created => mergeRequest.CreatedAt;

        public IHostedRepository BaseRepo => throw new NotImplementedException();

        public IHostedRepository HeadRepo => throw new NotImplementedException();

        public string BaseSha => mergeRequest.Sha;

        public string HeadSha => mergeRequest.MergeCommitSha;

        public string BaseRef => throw new NotImplementedException();

        public string HeadRef => throw new NotImplementedException();

        public string Id => mergeRequest.InternalId.ToString();

        public string DetailedInfo => throw new NotImplementedException();

        public string FetchBranch => throw new NotImplementedException();

		public void Close()
		{
			throw new NotImplementedException();
		}

		public Task<string> GetDiffDataAsync()
		{
			throw new NotImplementedException();
		}

		public IPullRequestDiscussion GetDiscussion()
		{
			throw new NotImplementedException();
		}
	}
}
