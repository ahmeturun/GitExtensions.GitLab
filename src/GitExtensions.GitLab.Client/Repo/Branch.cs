using Newtonsoft.Json;

namespace GitExtensions.GitLab.Client.Repo
{
    public class BranchCommitRef
    {
        [JsonProperty("id")]
        public string Sha { get; set; }

        [JsonProperty("short_id")]
        public string ShordId { get; set; }

        public string Title { get; set; }
        public string Message { get; set; }
        
        [JsonProperty("author_name")]
        public string AuthorName { get; set; }

        [JsonProperty("author_email")]
        public string AuthorEmail { get; set; }
        
        [JsonProperty("web_url")]
        public string WebUrl { get; set; }
    }

    public class Branch
    {
        public string Name { get; set; }

        public BranchCommitRef Commit { get; set; }
    }
}
