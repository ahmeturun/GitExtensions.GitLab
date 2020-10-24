namespace GitExtensions.GitLab.Client.Repo
{
    using Newtonsoft.Json;
	
    public class GitLabDiff
	{
        [JsonProperty("old_path")]
        public string OldPath { get; set; }

        [JsonProperty("new_path")]
        public string NewPath { get; set; }

        [JsonProperty("a_mode")]
        public string AMode { get; set; }

        [JsonProperty("b_mode")]
        public string BMode { get; set; }

        [JsonProperty("diff")]
        public string DiffContent { get; set; }

        [JsonProperty("new_file")]
        public bool NewFile { get; set; }

        [JsonProperty("renamed_file")]
        public bool RenamedFile { get; set; }

        [JsonProperty("deleted_file")]
        public bool DeletedFile { get; set; }
    }
}
