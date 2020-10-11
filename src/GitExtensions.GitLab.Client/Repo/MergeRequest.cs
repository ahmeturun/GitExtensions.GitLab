namespace GitExtensions.GitLab.Client.Repo
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using RestSharp;
    
    public class MergeRequest
    {
        [JsonProperty("project_id")]
        public long ProjectId { get; set; }

        public long Id { get; set; }

        [JsonProperty("iid")]
        public long InternalId { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonProperty("updated_at")]
        public DateTime UpdatedAt { get; set; }

        [JsonProperty("web_url")]
        public string WebUrl { get; set; }

        public User Author { get; set; }

        internal RestClient client;

        /// <summary>
        /// Retrieves all Commits associated with this merge request.
        /// </summary>
        /// <returns></returns>
        public List<PullRequestCommit> GetCommits()
        {
            // https://gitlab.com/api/v4/projects/ahmeturun%2Ftestproj/merge_requests/1/commits?private_token=<token>
            var request = new RestRequest("projects/{projectId}/merge_requests/{mergeRequest}/commits");
            request.AddUrlSegment("projectId", ProjectId);
            request.AddUrlSegment("mergeRequest", InternalId);

            return client.GetList<PullRequestCommit>(request);
        }

        public bool Open()
        {
            return UpdateState("open");
        }

        public bool Close()
        {
            return UpdateState("closed");
        }

        private bool UpdateState(string state)
        {
            // https://gitlab.com/api/v4/projects/ahmeturun%2Ftestproj/merge_requests/1/commits?private_token=<token>
            ///projects/:id/merge_requests/:merge_request_iid
            var request = new RestRequest("projects/{projectId}/merge_requests/{mergeRequestId}");
            request.AddUrlSegment("projectId", ProjectId);
            request.AddUrlSegment("mergeRequestId", InternalId);

            request.RequestFormat = DataFormat.Json;
            request.AddJsonBody(new
            {
                state_event = state
            });
            return client.Patch(request).StatusCode == System.Net.HttpStatusCode.OK;
        }
    }
}
