using GitUIPluginInterfaces.RepositoryHosts;
using System.Web;

namespace GitExtensions.GitLab
{
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
        }

        public IHostedRepository GetHostedRepository()
        {
            if (repo == null)
            {
                repo = new GitLabRepo(GitLabPlugin.GitLab.getRepository(HttpUtility.UrlEncode($"{Owner}/{RemoteRepositoryName}")))
                {
                    CloneProtocol = CloneProtocol
                };
            }

            return repo;
        }

        /// <summary>
        /// Local name of the remote, 'origin'
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Owner of the remote repository, in
        /// git@github.com:mabako/Git.hub.git this is 'mabako'
        /// </summary>
        public string Owner { get; }

        /// <summary>
        /// Name of the remote repository, in
        /// git@github.com:mabako/Git.hub.git this is 'Git.hub'
        /// </summary>
        public string RemoteRepositoryName { get; }

        public string RemoteUrl { get; }

        public GitProtocol CloneProtocol { get; }

        public string Data => Owner + "/" + RemoteRepositoryName;
        public string DisplayData => Data;
        public bool IsOwnedByMe => GitHubLoginInfo.Username == Owner;

        public string GetBlameUrl(string commitHash, string fileName, int lineIndex)
        {
            return $"{GitHub3Plugin.Instance.GitHubEndpoint}/{Data}/blame/{commitHash}/{fileName}#L{lineIndex}";
        }
    }
}
