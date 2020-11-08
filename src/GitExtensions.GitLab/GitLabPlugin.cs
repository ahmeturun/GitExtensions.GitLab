namespace GitExtensions.GitLab
{
	using GitUIPluginInterfaces;
	using ResourceManager;
	using System;
	using System.ComponentModel.Composition;
	using GitExtensions.GitLab.Properties;
	using GitUIPluginInterfaces.RepositoryHosts;
	using System.Collections.Generic;
	using System.Linq;
	using GitExtUtils;
	using GitCommands;
	using GitCommands.Config;
	using GitExtensions.GitLab.Remote;
	using GitExtensions.GitLab.Forms;

	[Export(typeof(IGitPlugin))]
	public class GitLabPlugin : GitPluginBase
	{
		#region Translation
		private readonly TranslationString authenticationError = new TranslationString("Authentication Failed.");
		private readonly TranslationString repoInitializationError = new TranslationString("Error on repo initialization.");
		#endregion

		private const string DefaultGitLabHost = "gitlab.com";

		private static bool dontShowErrors = false;
		//private static bool dontAskForLoginAgain = false;

		public bool LoggedIn = false;
		public readonly StringSetting OAuthToken = new StringSetting("OAuth Token", "");
		public readonly BoolSetting AskForMergeRequestAfterPush = new BoolSetting("Ask for merge request upon push to remote", false);
		private readonly ArgumentString cmdInfo = new GitArgumentBuilder("status");
		private RepoType? repoType = null;
		private IGitUICommands currentGitUiCommands;

		internal static GitLabPlugin Instance;
		internal Client.Client GitLabClient { get; private set; }
		internal bool AskForMergeRequestAfterPushValue { get; private set; }

		public GitLabPlugin() : base(true)
		{
			SetNameAndDescription("GitLab");
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
			//OAuth2Handler.RegisterURIHandler();
			currentGitUiCommands = gitUiCommands;;
			currentGitUiCommands.PostSettings += CurrentGitUiCommands_PostSettings;
			currentGitUiCommands.PostRegisterPlugin += GitUiCommands_PostRegisterPlugin;
			InitializeConfiguredParameters(gitUiCommands.GitModule);
			if (IsGitLabRepo(gitUiCommands))
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

		private void GitUiCommands_PostRegisterPlugin(object sender, GitUIEventArgs e)
		{
			GitLabPluginScriptManager.CleanPluginToolstripMenuItems(e.OwnerForm);
		}

		private void CurrentGitUiCommands_PostSettings(object sender, GitUIPostActionEventArgs e)
		{
			Register(currentGitUiCommands);
		}

		private void InitializeConfiguredParameters(IGitModule gitModule)
		{
			var firstRemoteName = gitModule.GetRemoteNames().FirstOrDefault();
			var url = gitModule.GetSetting(string.Format(SettingKeyString.RemoteUrl, firstRemoteName));
			var remoteHost = !string.IsNullOrEmpty(url) ? new Uri(url).Host : DefaultGitLabHost;
			var gitLabHostParsed = $"https://{remoteHost}";
			GitLabClient = new Client.Client(
				gitLabHostParsed,
				Instance.OAuthToken.ValueOrDefault(Instance.Settings));
			AskForMergeRequestAfterPushValue = AskForMergeRequestAfterPush.ValueOrDefault(Settings);
		}

		public override void Unregister(IGitUICommands gitUiCommands)
		{
			GitLabPluginScriptManager.Clean();
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
			yield return AskForMergeRequestAfterPush;
		}
		public bool GitModuleIsRelevantToMe()
		{
			return GetHostedRemotesForModule().Count > 0;
		}


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
				//if (dontAskForLoginAgain)
				//{
				//	return false;
				//}
				return LoginGitLab();
			}
			catch(Exception ex)
			{
				if (dontShowErrors)
				{
					return false;
				}
				using (var mergeRequestformStatus = new MergeRequsetFormStatus(
						$"GitLab Plugin -{repoInitializationError.Text}",
						string.Empty,
						GitUI.Properties.Images.StatusBadgeError,
						true,
						ex.Message))
				{
					mergeRequestformStatus.ShowDialog();
					dontShowErrors = mergeRequestformStatus.DontShowAgain;
				}
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
				foreach (string remote in gitModule.GetRemoteNames())
				{
					var url = gitModule.GetSetting(string.Format(SettingKeyString.RemoteUrl, remote));
					if (string.IsNullOrEmpty(url))
					{
						continue;
					}
					var remoteUri = new Uri(url);
					if (!remoteUri.Host.Contains("gitlab"))
					{
						continue;
					}

					if (new GitLabRemoteParser(remoteUri.Host)
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

		public bool LoginGitLab()
		{
			return StartLoginProcess();
			
			//if (!confirmRedirect)
			//{
			//	return StartLoginProcess();
			//}
			//else
			//{
			//	using (var confirmRedirectForm = new RedirectToGitLabLoginForm())
			//	{
			//		confirmRedirectForm.ShowDialog();
			//		dontAskForLoginAgain = confirmRedirectForm.dontAskForLoginAgain;
			//		if (confirmRedirectForm.DialogResult == DialogResult.OK)
			//		{
			//			return StartLoginProcess();
			//		}
			//		else
			//		{
			//			return false;
			//		}
			//	}
			//}
		}

		private bool StartLoginProcess()
		{
			using (var loginForm = new LoginForm())
			{
				loginForm.ShowDialog();
			}

			//Process.Start(GitLabClient.OAuthredirectURL);
			//var oAuthTask = new AsyncLoader();
			//oAuthTask.LoadAsync(() =>
			//{
			//	return new OAuthTokenRetriever().ServerThread();
			//}, loginResult =>
			//{
			//	if (loginResult.LoginSuccessFull)
			//	{
			//		Instance.Settings.SetString(OAuthToken.Name, loginResult.OAuthToken);
			//		Register(currentGitUiCommands);
			//	}
			//	else
			//	{
			//		if (loginResult.TryAgain)
			//		{
			//			LoginGitLab(false);
			//		}
			//	}
			//});
			return LoggedIn;
		}

		private void LoginForm_LoggedIn(object sender, LoginForm.LoggedInEventArgs e)
		{
			Register(e.GitUICommands);
		}
	}
}
