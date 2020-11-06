namespace GitExtensions.GitLab
{
    using Microsoft.Extensions.Hosting;
    using System.IO;
    using System.IO.Pipes;
    using System.Threading;
    using System.Threading.Tasks;

	public class IPCListener : BackgroundService
	{
		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
            string receivedText;

            PipeSecurity ps = new PipeSecurity();
            System.Security.Principal.SecurityIdentifier sid = new System.Security.Principal.SecurityIdentifier(System.Security.Principal.WellKnownSidType.WorldSid, null);
            PipeAccessRule par = new PipeAccessRule(sid, PipeAccessRights.ReadWrite, System.Security.AccessControl.AccessControlType.Allow);
            ps.AddAccessRule(par);

            using (var pipeStream = new NamedPipeServerStream(
                "test",
                PipeDirection.InOut,
                1,
                PipeTransmissionMode.Message,
                PipeOptions.Asynchronous,
                4096,
                4096,
                ps))
            {
                await pipeStream.WaitForConnectionAsync(stoppingToken);

                using (var streamReader = new StreamReader(pipeStream))
                {
                    receivedText = await streamReader.ReadToEndAsync();
                }
            }
        }
	}
}
