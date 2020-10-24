namespace GitExtensions.GitLab.Client.Repo
{
	using System;
	using Newtonsoft.Json;
	using RestSharp;
	public class MergeRequest
	{
		[JsonProperty("project_id")]
		public long ProjectId { get; set; }

		[JsonProperty("id")]
		public object Id { get; set; }

		[JsonProperty("iid")]
		public long InternalId { get; set; }

		[JsonProperty("title")]
		public string Title { get; set; }

		[JsonProperty("description")]
		public string Description { get; set; }

		[JsonProperty("created_at")]
		public DateTime CreatedAt { get; set; }

		[JsonProperty("updated_at")]
		public DateTime UpdatedAt { get; set; }

		[JsonProperty("web_url")]
		public string WebUrl { get; set; }

		[JsonProperty("author")]
		public User Author { get; set; }

		[JsonProperty("assignee")]
		public User Assignee { get; set; }

		[JsonProperty("assignee_id")]
		public int AssigneeId { get; set; }

		[JsonProperty("target_branch")]
		public string TargetBranch { get; set; }

		[JsonProperty("source_branch")]
		public string SourceBranch { get; set; }

		[JsonProperty("target_project_id")]
		public int TargetProjectId { get; set; }

		[JsonProperty("source_project_id")]
		public int SourceProjectId { get; set; }

		[JsonProperty("remove_source_branch")]
		public bool RemoveSourceBranch { get; set; }

		[JsonProperty("squash")]
		public bool Squash { get; set; }

		[JsonProperty("sha")]
		public string Sha { get; set; }

		[JsonProperty("merge_commit_sha")]
		public string MergeCommitSha { get; set; }
	}
}
