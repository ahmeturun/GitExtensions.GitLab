namespace GitExtensions.GitLab.Client.Repo
{
    using Newtonsoft.Json;
	using RestSharp;
	using System.Collections.Generic;

	public class Repository
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public string Description { get; set; }
        public string Homepage { get; set; }
        
        [JsonProperty("default_branch")]
        public string DefaultBranch { get; set; }
        [JsonProperty("namespace")]
        public User Owner { get; set; }

        [JsonProperty("http_url_to_repo")]
        public string GitUrl { get; set; }

        [JsonProperty("ssh_url_to_repo")]
        public string SshUrl { get; set; }

        [JsonProperty("can_create_merge_request_in")]
        public bool CanCreateMergeRequestIn { get; set; }


        internal IRestClient GitLabClient;
        public IList<Branch> GetBranches()
        {
            RestRequest request = new RestRequest($"/api/v4/projects/{Id}/repository/branches");
            return GitLabClient.GetList<Branch>(request);
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
            return Owner.Name + "/" + Name;
        }
    }
}
