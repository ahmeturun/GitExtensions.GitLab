namespace GitExtensions.OAuthProcesses
{
	using System.IO.Pipes;
	using System.Security.Principal;
	using System.Text.RegularExpressions;

	public class Program
	{
		private const string TokenRegex = "#access_token=(.*)&token_type=Bearer";

		static void Main(string[] args)
		{
			if (args.Length > 0)
			{
				var pipeClient =
					new NamedPipeClientStream(".", "GitExtensionsGitlab",
						PipeDirection.Out, PipeOptions.None,
						TokenImpersonationLevel.Impersonation);
				try
				{
					pipeClient.Connect(10000);

					var ss = new StreamString(pipeClient);
					var token = Regex.Match(args[0], TokenRegex).Groups[1].Value;
					ss.WriteString(token);
					pipeClient.Close();
					return;
				}
				catch (System.TimeoutException)
				{
					// operation timeout.
				}
			}
		}
	}
}
