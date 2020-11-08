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
		private const string GitLabOAuthClientId = "4c720c2b1df2547b7c661ba8328d0a0713765aad5f7ee6a023a8a6ae24c1013a";
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

		public LoginResult Login(
			string userName,
			string password)
		{
			var request = new RestRequest("/oauth/token")
				.AddJsonBody(new LoginRequest(userName, password));
			var result = PostRequest<LoginResult>(request);
			return result;
		}


		public User GetCurrentUser()
		{
			var request = new RestRequest("/api/v4/user");

			var user = GetRequest<User>(request, false);

			return user;
		}

		public Repository GetRepository(string repositoryName)
		{
			var request = new RestRequest("/api/v4/projects/{repo}")
				.AddUrlSegment("repo", repositoryName);

			var repo = GetRequest<Repository>(request);
			if (repo == null)
				return null;
			repo.GitLabClient = client;
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
			var request = new RestRequest($"/api/v4/projects/{projectId}/repository/compare?from={targetId}&to={sourceId}");
			return GetRequest<BranchDiff>(request);
		}

		public string OAuthredirectURL => $"{gitLabDomain}/oauth/authorize?client_id=" +
			$"{GitLabOAuthClientId}&redirect_uri=gitextensionsgitlab://auth&response_type=token";

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
