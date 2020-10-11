namespace GitExtensions.GitLab.Client.Repo
{
    using System;
    using System.Collections.Generic;
    using RestSharp;

    public class Issue
    {
        public int Number;

        internal RestClient _client;
        public Repository Repository { get; internal set; }


        public List<IssueComment> GetComments()
        {
            throw new NotImplementedException();
        }

        public IssueComment CreateComment(string body)
        {
            throw new NotImplementedException();
        }
    }

    public class IssueComment
    {
        public int Id { get; private set; }
        public string Body { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt { get; private set; }
        public User User { get; private set; }

        /// <summary>
        /// api.github.com/repos/{user}/{repo}/issues/{issue}/comments/{id}
        /// </summary>
        public string Url { get; private set; }
    }
}
