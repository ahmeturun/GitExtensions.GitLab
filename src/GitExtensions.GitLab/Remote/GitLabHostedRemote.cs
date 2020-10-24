namespace GitExtensions.GitLab
{
    using GitUIPluginInterfaces.RepositoryHosts;
    using System;
    using System.Web;

    internal class GitLabHostedRemote : IHostedRemote
    {
        private GitLabRepo repo;

        public GitLabHostedRemote(string name, string owner, string remoteRepositoryName, string url)
        {
            Name = name;
            Owner = owner;
            RemoteRepositoryName = remoteRepositoryName;
            RemoteUrl = url;
            CloneProtocol = url.IsUrlUsingHttp() ? GitProtocol.Https : GitProtocol.Ssh;
            GetHostedRepository();
        }

        public IHostedRepository GetHostedRepository()
        {
            if (repo == null)
            {
                repo = new GitLabRepo(GitLabPlugin.Instance.GitLabClient.GetRepository(
					HttpUtility.UrlEncode($"{Owner}/{RemoteRepositoryName}")))
                {
                    CloneProtocol = CloneProtocol
                };
            }

            return repo;
        }

        public int Id 
        {
            get
            {
                return repo?.Id ?? 0;
            }
        }

        /// <summary>
        /// Local name of the remote, 'origin'
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Owner of the remote repository, in
        /// git@gitlab.com:ahmeturun/GitLabPlugin.git this is ahmeturun
        /// </summary>
        public string Owner { get; }

        /// <summary>
        /// Name of the remote repository, in
        /// git@gitlab.com:ahmeturun/GitLabPlugin.git this is 'GitLabPlugin'
        /// </summary>
        public string RemoteRepositoryName { get; }

        public string RemoteUrl { get; }

        public GitProtocol CloneProtocol { get; }

        public string Data => Owner + "/" + RemoteRepositoryName;
        public string DisplayData => Data;
        public bool IsOwnedByMe => repo.CanCreateMergeRequestIn;

        public string GetBlameUrl(string commitHash, string fileName, int lineIndex)
        {
            throw new NotImplementedException();
            //return $"{GitHub3Plugin.Instance.GitHubEndpoint}/{Data}/blame/{commitHash}/{fileName}#L{lineIndex}";
        }
    }
}
