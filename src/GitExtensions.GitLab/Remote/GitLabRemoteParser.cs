namespace GitExtensions.GitLab.Remote
{
    using System;
    using System.Text.RegularExpressions;
    public class GitLabRemoteParser
    {
        private readonly string gitlabHostDomain;

        public GitLabRemoteParser(string gitlabHostDomain)
        {
            this.gitlabHostDomain = !string.IsNullOrEmpty(gitlabHostDomain)
                ? gitlabHostDomain
                : throw new ArgumentNullException(nameof(gitlabHostDomain));
        }

        private Match MatchRegExes(string remoteUrl, string[] regExs)
        {
            Match m = null;
            foreach (var regex in regExs)
            {
                m = Regex.Match(remoteUrl, regex);
                if (m.Success)
                {
                    break;
                }
            }

            return m;
        }

        private string[] GetDomainSpecificRegexes()
        {
            var gitLabSshUrlRegex = $@"git(?:@|://){gitlabHostDomain}[:/](?<owner>[^/]+)/(?<repo>[\w_\.\-]+)\.git";
            var gitLabHttpsUrlRegex = $@"https?://(?:[^@:]+)?(?::[^/@:]+)?@?{gitlabHostDomain}/(?<owner>[^/]+)/(?<repo>[\w_\.\-]+)(?:.git)?";
            return new string[] { gitLabSshUrlRegex, gitLabHttpsUrlRegex };
        }

        public bool IsValidRemoteUrl(string remoteUrl)
        {
            return TryExtractGitHubDataFromRemoteUrl(remoteUrl, out _, out _);
        }

        public bool TryExtractGitHubDataFromRemoteUrl(string remoteUrl, out string owner, out string repository)
        {
            owner = null;
            repository = null;

            var m = MatchRegExes(remoteUrl, GetDomainSpecificRegexes());

            if (m == null || !m.Success)
            {
                return false;
            }

            owner = m.Groups["owner"].Value;
            repository = m.Groups["repo"].Value.Replace(".git", "");
            return true;
        }
    }
}
