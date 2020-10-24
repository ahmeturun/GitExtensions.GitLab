namespace GitExtensions.GitLab.Client
{
	using System;
	using System.Collections.Generic;
	using System.Net;
	using System.Net.Http;
	using GitExtensions.GitLab.Client.Repo;
	using RestSharp;


	public class Client
	{
		private const string AuthenticationSchema = "Bearer";
		private readonly HttpClient httpClient;
		private readonly IRestClient client;
		private readonly string gitLabDomain = "https://gitlab.net";

		public Client(string gitLabDomain, string oAuthToken)
		{
			this.gitLabDomain = !string.IsNullOrEmpty(gitLabDomain)
				? gitLabDomain
				: throw new ArgumentNullException(nameof(gitLabDomain));
			client = new RestClient(this.gitLabDomain).UseSerializer(() => new JsonNetSerializer());
			client.AddDefaultHeader(HttpRequestHeader.Authorization.ToString(), $"{AuthenticationSchema} {oAuthToken}");
		}

		/// <summary>
		/// Retrieves the current user.
		/// Requires to be logged in (OAuth).
		/// </summary>
		/// <returns>current user</returns>
		public User GetCurrentUser()
		{
			var request = new RestRequest("/api/v4/user");

			var user = GetRequest<User>(request, false);

			return user;
		}

		/// <summary>
		/// Fetches a single repository from github.com/username/repositoryName.
		/// </summary>
		/// <param name="repositoryName">name of the repository</param>
		/// <returns>fetched repository</returns>
		public Repository GetRepository(string repositoryName)
		{
			var request = new RestRequest("/api/v4/projects/{repo}")
				.AddUrlSegment("repo", repositoryName);

			var repo = GetRequest<Repository>(request);
			if (repo == null)
				return null;

			return repo;
		}

		public IList<User> GetUsers(string searchKey = "")
		{
			var searchFilter = !string.IsNullOrEmpty(searchKey)
				? $"?search={searchKey}"
				: string.Empty;
			var request = new RestRequest($"/api/v4/users{searchFilter}");
			var users = GetRequest<List<User>>(request);
			return users;
		}

		public MergeRequest CreateMergeRequest(MergeRequest mergeRequest)
		{
			var request = new RestRequest($"/api/v4/projects/{mergeRequest.Id}/merge_requests")
				.AddJsonBody(mergeRequest);
			var createdMergeRequest = PostRequest<MergeRequest>(request);
			return createdMergeRequest;
		}

		public BranchDiff GetBranchDiff(int projectId, string sourceId, string targetId)
		{
			//https://gitlab.com/api/v4/projects/ahmeturun%2Ffork_test_path/repository/compare?from=master&to=merge_test?private_token=sgxDwASYHYxE15FhzVrk
			var request = new RestRequest($"/api/v4/projects/{projectId}/repository/compare?from={targetId}&to={sourceId}");
			return GetRequest<BranchDiff>(request);
		}

		private T GetRequest<T>(IRestRequest request, bool throwOnError = true) where T : new()
		{
			var response = client.Get<T>(request);
			if (response.IsSuccessful)
			{
				return response.Data;
			}

			if (!throwOnError)
			{
				return default;
			}

			if (response.StatusCode == HttpStatusCode.Unauthorized)
			{
				throw new UnauthorizedAccessException("The GitLab authentication token provided is not valid.");
			}

			if (response.StatusCode == HttpStatusCode.NotFound)
			{
				return default;
			}

			throw new Exception($"{response.StatusDescription}\n{response.Content}");
		}

		private T PostRequest<T>(IRestRequest request, bool throwOnError = true) where T : new()
		{
			var response = client.Post<T>(request);
			if (response.IsSuccessful)
			{
				return response.Data;
			}

			if (!throwOnError)
			{
				return default;
			}

			if (response.StatusCode == HttpStatusCode.Unauthorized)
			{
				throw new UnauthorizedAccessException("The GitLab authentication token provided is not valid.");
			}

			if (response.StatusCode == HttpStatusCode.NotFound)
			{
				return default;
			}

			throw new Exception($"{response.StatusDescription}\n{response.Content}");
		}

	}
}
