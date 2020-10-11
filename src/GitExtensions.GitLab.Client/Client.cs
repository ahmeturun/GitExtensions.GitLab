namespace GitExtensions.GitLab.Client
{
    using System;
    using System.Net;
    using GitExtensions.GitLab.Client.Repo;
    using RestSharp;
    
    public class Client
    {
        private const string AuthenticationSchema = "Bearer";
        private readonly RestClient client;
        private readonly string gitLabDomain = "https://gitlab.net";

        public Client()
        {
            client = new RestClient(gitLabDomain);
        }

        public Client(string gitLabDomain, string oAuthToken)
        {
            this.gitLabDomain = !string.IsNullOrEmpty(gitLabDomain)
                ? gitLabDomain
                : throw new ArgumentNullException(nameof(gitLabDomain));
            client = new RestClient(this.gitLabDomain);
            client.AddDefaultHeader(HttpRequestHeader.Authorization.ToString(), $"{AuthenticationSchema} {oAuthToken}");
        }

        /// <summary>
        /// Retrieves the current user.
        /// Requires to be logged in (OAuth).
        /// </summary>
        /// <returns>current user</returns>
        public User getCurrentUser()
        {
            var request = new RestRequest("/user");

            var user = DoRequest<User>(request, false);

            return user;
        }

        /// <summary>
        /// Fetches a single repository from github.com/username/repositoryName.
        /// </summary>
        /// <param name="repositoryName">name of the repository</param>
        /// <returns>fetched repository</returns>
        public Repository getRepository(string repositoryName)
        {
            var request = new RestRequest("/projects/{repo}")
                .AddUrlSegment("repo", repositoryName);

            var repo = DoRequest<Repository>(request);
            if (repo == null)
                return null;

            repo.client = client;
            return repo;
        }

        private T DoRequest<T>(IRestRequest request, bool throwOnError = true) where T : new()
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
                throw new UnauthorizedAccessException("The GitHub authentication token provided is not valid.");
            }

            throw new Exception(response.StatusDescription);
        }

    }
}
