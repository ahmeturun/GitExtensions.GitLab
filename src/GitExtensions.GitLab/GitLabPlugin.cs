using GitUIPluginInterfaces;
using ResourceManager;
using System;
using System.ComponentModel.Composition;
using System.Windows.Forms;
using GitExtensions.GitLab.Properties;
using GitUIPluginInterfaces.RepositoryHosts;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using GitUI;
using GitExtUtils;
using GitCommands;
using GitCommands.Config;
using GitExtensions.GitLab.Remote;
using GitExtensions.GitLab.Forms;
using GitUI.Properties;

namespace GitExtensions.GitLab
{
	/// <summary>
	/// A template for Git Extensions plugins.
	/// Find more documentation here: https://github.com/gitextensions/gitextensions.plugintemplate/wiki/GitPluginBase
	/// </summary>
	[Export(typeof(IGitPlugin))]
	public class GitLabPlugin : GitPluginBase, IRepositoryHostPlugin
	{
		#region Translation
		private readonly TranslationString authenticationError = new TranslationString("Authentication Failed.");
		private readonly TranslationString repoInitializationError = new TranslationString("Error on repo initialization.");
		#endregion

		public readonly StringSetting OAuthToken = new StringSetting("OAuth Token", "");
		public readonly StringSetting GitLabHost = new StringSetting("GitLab (Enterprise) hostname","https://gitlab.com");
		private readonly ArgumentString cmdInfo = new GitArgumentBuilder("status");
		private RepoType? repoType = null;
		private IGitUICommands currentGitUiCommands;

		internal static GitLabPlugin Instance;
		internal Client.Client GitLabClient { get; private set; }

		public bool ConfigurationOk => !string.IsNullOrEmpty(OAuthToken.ValueOrDefault(Settings)) 
			&& !string.IsNullOrEmpty(GitLabHost.ValueOrDefault(Settings));

		public GitLabPlugin() : base(true)
		{
			SetNameAndDescription("GitLab Merge Request");
			Icon = Resources.IconGitlab;
			Translate();
			if (Instance == null)
			{
				Instance = this;
			}
		}


		/// <summary>
		/// Is called when the plugin is loaded. This happens every time when a repository is opened.
		/// </summary>
		public override void Register(IGitUICommands gitUiCommands)
		{
			currentGitUiCommands = gitUiCommands;;
			currentGitUiCommands.PostSettings += CurrentGitUiCommands_PostSettings;
			InitializeConfiguredParameters();
			if (ConfigurationOk && IsGitLabRepo(gitUiCommands))
			{
				base.Register(gitUiCommands);
				GitLabPluginScriptManager.Initialize();
				ForceRefreshGE(gitUiCommands);
			}
			else
			{
				GitLabPluginScriptManager.Clean();
				ForceRefreshGE(gitUiCommands);
			}
		}

		private void CurrentGitUiCommands_PostSettings(object sender, GitUIPostActionEventArgs e)
		{
			Register(currentGitUiCommands);
		}

		private void InitializeConfiguredParameters()
		{
			var gitLabHostParsed = $"https://{GitLabHost.ValueOrDefault(Settings).Replace("https://", "").Replace("http://", "").Trim('/')}";
			GitLabClient = new Client.Client(
				gitLabHostParsed,
				Instance.OAuthToken.ValueOrDefault(Instance.Settings));
		}

		public override void Unregister(IGitUICommands gitUiCommands)
		{
			if (IsGitLabRepo(gitUiCommands))
			{
				GitLabPluginScriptManager.Clean();
			}

			repoType = null;
		}

		public override bool Execute(GitUIEventArgs gitUIEventArgs)
		{
			gitUIEventArgs.GitUICommands.StartSettingsDialog(this);
			return false;
		}

		private void ForceRefreshGE(IGitUICommands gitUiCommands)
		{
			gitUiCommands.RepoChangedNotifier.Notify();
		}

		public override IEnumerable<ISetting> GetSettings()
		{
			yield return OAuthToken;
			yield return GitLabHost;
		}

		#region IRepositoryHostPlugin definitions

		public string OwnerLogin => GitLabClient.GetCurrentUser()?.Username;

		public IReadOnlyList<IHostedRepository> SearchForRepository(string search)
		{
			throw new NotImplementedException();
		}

		public IReadOnlyList<IHostedRepository> GetRepositoriesOfUser(string user)
		{
			throw new NotImplementedException();
		}

		public IHostedRepository GetRepository(string user, string repositoryName)
		{
			throw new NotImplementedException();
		}

		public IReadOnlyList<IHostedRepository> GetMyRepos()
		{
			throw new NotImplementedException();
		}

		public void ConfigureContextMenu(ContextMenuStrip contextMenu)
		{
			throw new NotImplementedException();
		}

		public bool GitModuleIsRelevantToMe()
		{
			return GetHostedRemotesForModule().Count > 0;
		}

		public Task<string> AddUpstreamRemoteAsync()
		{
			throw new NotImplementedException();
		}

		#endregion


		private bool IsGitLabRepo(IGitUICommands gitUiCommands)
		{
			if (repoType == null)
			{
				ExecutionResult result = gitUiCommands.GitModule.GitExecutable.Execute(cmdInfo);
				repoType = (result.ExitCode == 0)
					? RepoType.git
					: RepoType.Unknown;
			}

			try
			{
				return repoType == RepoType.git && GetHostedRemotesForModule().Count() > 0;
			}
			catch (UnauthorizedAccessException)
			{
				new MergeRequsetFormStatus(
						$"GitLab Plugin -{authenticationError.Text}",
						string.Empty,
						Images.StatusBadgeError,
						"Given private key is not valid.").ShowDialog();
				return false;
			}
			catch(Exception ex)
			{
				new MergeRequsetFormStatus(
						$"GitLab Plugin -{repoInitializationError.Text}",
						string.Empty,
						Images.StatusBadgeError,
						ex.Message).ShowDialog();
				return false;
			}

		}

		/// <summary>
		/// Returns all relevant git-remotes for the current working directory
		/// </summary>
		public IReadOnlyList<IHostedRemote> GetHostedRemotesForModule()
		{
			if (currentGitUiCommands?.GitModule == null)
			{
				return Array.Empty<IHostedRemote>();
			}

			var gitModule = currentGitUiCommands.GitModule;
			return Remotes().ToList();

			IEnumerable<IHostedRemote> Remotes()
			{
				var set = new HashSet<IHostedRemote>();
				var gitlabDomain = GitLabHost.ValueOrDefault(Settings)
					.Replace("https://", "")
					.Replace("http://", "")
					.Trim('/');
				foreach (string remote in gitModule.GetRemoteNames())
				{
					var url = gitModule.GetSetting(string.Format(SettingKeyString.RemoteUrl, remote));

					if (string.IsNullOrEmpty(url))
					{
						continue;
					}

					if (new GitLabRemoteParser(gitlabDomain)
							.TryExtractGitLabDataFromRemoteUrl(url, out var owner, out var repository))
					{
						var hostedRemote = new GitLabHostedRemote(remote, owner, repository, url);

						if (set.Add(hostedRemote))
						{
							yield return hostedRemote;
						}
					}
				}
			}
		}


	}
}
