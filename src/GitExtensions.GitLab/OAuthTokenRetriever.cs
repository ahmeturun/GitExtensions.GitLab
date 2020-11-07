namespace GitExtensions.GitLab
{
	using Microsoft.VisualStudio.Threading;
	using System;
	using System.IO.Pipes;
	using System.Windows.Forms;

	public class OAuthTokenRetriever
	{
		private static NamedPipeServerStream pipeServer;
		public OAuthLoginResult ServerThread()
		{
			try
			{
				pipeServer = new NamedPipeServerStream("GitExtensionsGitlab", PipeDirection.InOut, 1);
				var pipeResult = pipeServer.WaitForConnectionAsync();
				pipeResult.WithTimeout(TimeSpan.FromSeconds(60)).AttachToParent();
				pipeResult.ConfigureAwait(false).GetAwaiter().GetResult();
				if (!pipeServer.IsConnected || !pipeResult.IsCompleted)
				{
					return ShowUnsuccessfullDialog();
				}
				StreamString ss = new StreamString(pipeServer);
				var oAuthToken = ss.ReadString();
				return new OAuthLoginResult(true, false, oAuthToken);
			}
			catch (System.IO.IOException)
			{
				// and existing login is ongoing, skipping
				return new OAuthLoginResult(false, false, string.Empty);
			}
			catch (Exception)
			{
				return ShowUnsuccessfullDialog();
			}
			finally
			{
				pipeServer.Close();
			}
		}

		private static OAuthLoginResult ShowUnsuccessfullDialog()
		{
			var tryAgainResult = MessageBox.Show(
								"GitLab login unsuccessful, Try again?",
								"GitExtensions GitLab Plugin",
								MessageBoxButtons.YesNo,
								MessageBoxIcon.Question);
			if (tryAgainResult == DialogResult.Yes)
			{
				return new OAuthLoginResult(false, true, string.Empty);
			}
			else
			{
				return new OAuthLoginResult(false, false, string.Empty);
			}
		}
	}
}
