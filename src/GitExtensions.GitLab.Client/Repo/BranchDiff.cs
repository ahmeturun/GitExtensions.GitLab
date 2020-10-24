namespace GitExtensions.GitLab.Client.Repo
{
	using System.Collections.Generic;
	using Newtonsoft.Json;

	public class BranchDiff
	{
		[JsonProperty("commit")]
		public Commit Commit { get; set; }

		[JsonProperty("commits")]
		public IList<Commit> Commits { get; set; }

		[JsonProperty("diffs")]
		public IList<GitLabDiff> Diffs { get; set; }

		[JsonProperty("compare_timeout")]
		public bool CompareTimeout { get; set; }

		[JsonProperty("compare_same_ref")]
		public bool CompareSameRef { get; set; }
	}
}
