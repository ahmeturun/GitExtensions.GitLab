namespace GitExtensions.GitLab.Client
{
	using Newtonsoft.Json;
	using System;

	public class LoginRequest
	{
		public LoginRequest(
			string username,
			string password)
		{
			Username = username ?? throw new ArgumentNullException(nameof(username));
			Password = password ?? throw new ArgumentNullException(nameof(password));
		}

		[JsonProperty("grant_type")]
		public string GrantType { get; set; } = "password";

		[JsonProperty("username")]
		public string Username { get; set; }

		[JsonProperty("password")]
		public string Password { get; set; }
	}
}
