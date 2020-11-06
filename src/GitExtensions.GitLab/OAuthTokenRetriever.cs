namespace GitExtensions.GitLab
{
	using Microsoft.VisualStudio.Threading;
	using System;
	using System.IO.Pipes;
	using System.Threading;

	public class OAuthTokenRetriever
	{
		public string ServerThread()
		{
			NamedPipeServerStream pipeServer =
					new NamedPipeServerStream("GitExtensionsGitlab", PipeDirection.InOut, 1);
			var pipeResult = pipeServer.WaitForConnectionAsync();
			pipeResult.WithTimeout(TimeSpan.FromSeconds(60));
			pipeResult.ConfigureAwait(false).GetAwaiter().GetResult();
			if (!pipeServer.IsConnected || !pipeResult.IsCompleted)
			{
				return string.Empty;
			}
			try
			{
				StreamString ss = new StreamString(pipeServer);
				var oAuthToken = ss.ReadString();
				return oAuthToken;
			}
			catch (Exception)
			{
				throw;
			}
			finally
			{
				pipeServer.Close();
			}
		}
	}
}
