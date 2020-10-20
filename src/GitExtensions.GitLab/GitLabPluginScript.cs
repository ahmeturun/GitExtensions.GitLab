using System;

namespace GitExtensions.GitLab
{
	public class GitLabPluginScript
	{
		public GitLabPluginScript(string name, string command)
		{
			Name = name ?? throw new ArgumentNullException(nameof(name));
			Command = command ?? throw new ArgumentNullException(nameof(command));
		}

		public string Name { get; }

		public string Command { get; }
	}
}
