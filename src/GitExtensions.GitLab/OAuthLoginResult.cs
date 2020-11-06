namespace GitExtensions.GitLab
{
	public class OAuthLoginResult
	{
		public OAuthLoginResult(
			bool loginSuccessFull,
			bool tryAgain,
			string oAuthToken)
		{
			LoginSuccessFull = loginSuccessFull;
			TryAgain = tryAgain;
			OAuthToken = oAuthToken;
		}

		public bool LoginSuccessFull { get; }

		public bool TryAgain { get; }

		public string OAuthToken { get; }
	}
}
