namespace GitExtensions.GitLab.Client.Repo
{
	using System;
	using Newtonsoft.Json;

	public class Commit
	{
		[JsonProperty("id")]
		public string Sha { get; set; }

		[JsonProperty("short_id")]
		public string ShortId { get; set; }

		[JsonProperty("title")]
		public string Title { get; set; }

		[JsonProperty("author_name")]
		public string AuthorName { get; set; }

		[JsonProperty("author_email")]
		public string AuthorEmail { get; set; }

		[JsonProperty("created_at")]
		public DateTime CreatedAt { get; set; }
	}
}
