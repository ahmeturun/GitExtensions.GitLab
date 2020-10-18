namespace GitExtensions.GitLab.Remote
{
    using GitCommands.Remotes;
    using System;

    public class GitLabRemoteParser : RemoteParser
    {
        private readonly string GitLabHostName;

        public GitLabRemoteParser(string gitLabHostName)
        {
            GitLabHostName = !string.IsNullOrEmpty(gitLabHostName) 
                ? gitLabHostName
                : throw new ArgumentNullException(nameof(gitLabHostName));
        }

        public bool IsValidRemoteUrl(string remoteUrl)
        {
            return TryExtractGitLabDataFromRemoteUrl(remoteUrl, out _, out _);
        }

        public bool TryExtractGitLabDataFromRemoteUrl(string remoteUrl, out string owner, out string repository)
        {
            owner = null;
            repository = null;

            var m = MatchRegExes(remoteUrl, GetGitLabRegexes());

            if (m == null || !m.Success)
            {
                return false;
            }

            owner = m.Groups["owner"].Value;
            repository = m.Groups["repo"].Value.Replace(".git", "");
            return true;
        }

        private string[] GetGitLabRegexes()
        {
            var gitLabSshUrlRegex = $@"git(?:@|://){GitLabHostName}[:/](?<owner>[^/]+)/(?<repo>[\w_\.\-]+)\.git";
            var gitLabHttpsUrlRegex = $@"https?://(?:[^@:]+)?(?::[^/@:]+)?@?{GitLabHostName}/(?<owner>[^/]+)/(?<repo>[\w_\.\-]+)(?:.git)?";
            return new string[] { gitLabSshUrlRegex, gitLabHttpsUrlRegex };

        }
    }
}
