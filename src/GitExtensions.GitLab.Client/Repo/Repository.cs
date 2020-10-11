namespace GitExtensions.GitLab.Client.Repo
{
    using System;
    using System.Collections.Generic;
    using System.Web;
    using Newtonsoft.Json;
    using RestSharp;

    public class Repository
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public string Description { get; set; }
        public string Homepage { get; set; }
        
        [JsonProperty("default_branch")]
        public string DefaultBranch { get; set; }
        public User Owner { get; set; }

        /// <summary>
        /// Read-only clone url
        /// git://github.com/{user}/{repo}.git
        /// </summary>
        [JsonProperty("http_url_to_repo")]
        public string GitUrl { get; internal set; }

        /// <summary>
        /// Read/Write clone url via SSH
        /// git@github.com/{user}/{repo.git}
        /// </summary>
        [JsonProperty("ssh_url_to_repo")]
        public string SshUrl { get; internal set; }

        internal RestClient client;

        /// <summary>
        /// Forks this repository into your own account.
        /// </summary>
        /// <returns></returns>
        public Repository CreateFork(string forkProjectPath, string forkProjectName)
        {
            // https://gitlab.com/api/v4/projects/ahmeturun%2Ftestproj/merge_requests/1/commits?private_token=<token>
            // /projects/:id/fork
            RestRequest request = new AuthenticatedRestRequest("/repos/{user}/{repo}/forks");
            request.AddUrlSegment("user", Owner.Id);
            request.AddUrlSegment("repo", Name);
            request.AddJsonBody(new
            {
                path = forkProjectPath,
                name = forkProjectName
            });

            Repository forked = client.Post<Repository>(request).Data;
            forked.client = client;
            return forked;
        }

        /// <summary>
        /// Lists all branches
        /// </summary>
        /// <remarks>Not really sure if that's even useful, mind the 'git branch'</remarks>
        /// <returns>list of all branches</returns>
        public IList<Branch> GetBranches()
        {
            // /projects/:id/repository/branches
            // https://gitlab.com/api/v4/projects/ahmeturun%2Ftestproj/merge_requests/1/commits?private_token=<token>
            RestRequest request = new AuthenticatedRestRequest("/projects/{user}{repo}/repository/branches");
            request.AddUrlSegment("user", HttpUtility.UrlEncode(Owner.Id));
            request.AddUrlSegment("repo", HttpUtility.UrlEncode(Name));

            return client.GetList<Branch>(request);
        }

        /// <summary>
        /// Retrieves the name of the default branch
        /// </summary>
        /// <returns>The name of the default branch</returns>
        public string GetDefaultBranch()
        {
            RestRequest request = new AuthenticatedRestRequest("/repos/{user}/{repo}");
            request.AddUrlSegment("user", Owner.Id);
            request.AddUrlSegment("repo", Name);

            var repo = client.Get<Repository>(request).Data;

            if (repo == null)
            {
                return null;
            }

            return repo.DefaultBranch;
        }

        /// <summary>
        /// Lists all open pull requests
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>list of all open pull requests assigned to given <paramref name="userId"/></returns>
        public IList<MergeRequest> GetMergeRequests(int userId)
        {
            var request = new AuthenticatedRestRequest("/projects/{user}{repo}/merge_requests?state=opened&assignee_id={userId}");
            request.AddUrlSegment("user", HttpUtility.UrlEncode(Owner.Id));
            request.AddUrlSegment("repo", HttpUtility.UrlEncode(Name));
            request.AddUrlSegment("userId", userId);

            var list = client.GetList<MergeRequest>(request);
            if (list == null)
                return null;

            list.ForEach(pr => { pr.client = client; pr.ProjectId = Id; });
            return list;
        }

        /// <summary>
        /// Returns a single pull request.
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns>the single pull request</returns>
        public MergeRequest GetMergeRequest(int id)
        {
            var request = new AuthenticatedRestRequest("/projects/{user}{repo}/merge_requests/{merge_request_id}");
            request.AddUrlSegment("user", Owner.Id);
            request.AddUrlSegment("repo", Name);
            request.AddUrlSegment("merge_request_id", id.ToString());

            var pullrequest = client.Get<MergeRequest>(request).Data;
            if (pullrequest == null)
                return null;

            pullrequest.client = client;
            pullrequest.ProjectId = Id;
            return pullrequest;
        }

        public MergeRequest CreateMergeRequest(
            string sourceBranch,
            string targetBranch,
            int assigneeId,
            string title,
            string description)
        {
            var request = new AuthenticatedRestRequest("/projects/{name}{repo}/merge_requests");
            request.AddUrlSegment("name", HttpUtility.UrlEncode(Owner.Id));
            request.AddUrlSegment("repo", HttpUtility.UrlEncode(Name));

            request.RequestFormat = DataFormat.Json;
            request.AddJsonBody(new
            {
                title = title,
                description = description,
                target_branch = targetBranch,
                source_branch = sourceBranch,
                assignee_id = assigneeId
            });

            var pullrequest = client.Post<MergeRequest>(request).Data;
            if (pullrequest == null)
                return null;

            pullrequest.client = client;
            pullrequest.ProjectId = Id;
            return pullrequest;
        }

        public override bool Equals(object obj)
        {
            return obj is Repository && GetHashCode() == obj.GetHashCode();
        }

        public override int GetHashCode()
        {
            return GetType().GetHashCode() + ToString().GetHashCode();
        }

        public override string ToString()
        {
            return Owner.Id + "/" + Name;
        }
    }
}
