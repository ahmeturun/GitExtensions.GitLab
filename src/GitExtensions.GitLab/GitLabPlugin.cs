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

namespace GitExtensions.GitLab
{
    /// <summary>
    /// A template for Git Extensions plugins.
    /// Find more documentation here: https://github.com/gitextensions/gitextensions.plugintemplate/wiki/GitPluginBase
    /// </summary>
    [Export(typeof(IGitPlugin))]
    public class GitLabPlugin : GitPluginBase, IRepositoryHostPlugin
    {
        private static readonly int FirstHotkeyCommandIdentifier = 10000; // Arbitrary choosen. GE by default starts at 9000 for it's own scripts.

        private readonly CredentialsSetting credentialsSettings;
        private readonly Func<IGitModule> getModule;
        public readonly StringSetting OAuthToken = new StringSetting("OAuth Token", "");
        public readonly StringSetting GitLabHost = new StringSetting(
            "GitLab (Enterprise) hostname", 
            "https://gitlab.com");
        private readonly ArgumentString cmdInfo = new GitArgumentBuilder("status");
        private RepoType? repoType = null;

        private IGitUICommands currentGitUiCommands;

        public string GitLabHostParsed => $"https://{GitLabHost.ValueOrDefault(Settings).Replace("https://", "").Replace("http://", "").Trim('/')}";
        public string GitLabApiEndpoint => $"{GitLabHostParsed}/api/v4/";

        private IGitModule gitModule;

        internal static GitLabPlugin Instance;
        internal static Client.Client gitLab;
        internal static Client.Client GitLabClient => gitLab ?? (gitLab = new Client.Client(
            Instance.GitLabHostParsed,
            Instance.OAuthToken.ValueOrDefault(Instance.Settings)));

        public bool ConfigurationOk => !string.IsNullOrEmpty(OAuthToken.ValueOrDefault(Settings));

        public string OwnerLogin => GitLabClient.GetCurrentUser()?.Username;

        public GitLabPlugin() : base(true)
        {
            SetNameAndDescription("GitLab Merge Request");
            Icon = Resources.IconGitlab;
            Translate();
            if (Instance == null)
            {
                Instance = this;
            }

            //var repoSettings = GetModule().GetEffectiveSettings();
            //GetModule().GetSetting
            //repoSettings.GetValue

            credentialsSettings = new CredentialsSetting(
                "GitLabCredentials", 
                "GitLab credentials", 
                () => gitModule?.WorkingDir);
        }


        /// <summary>
        /// Is called when the plugin is loaded. This happens every time when a repository is opened.
        /// </summary>
        public override void Register(IGitUICommands gitUiCommands)
        {
            currentGitUiCommands = gitUiCommands;
            if (IsGitLabRepo(gitUiCommands))
            {
                base.Register(gitUiCommands);

                GitLabPluginScriptManager.AddNew("Create Merge Request", string.Empty, string.Empty, command: $"plugin:{GitLabCreateMergeRequest.GitLabCreateMRDescription}");
                GitLabPluginScriptManager.AddNew("Manage Merge Requests", string.Empty, string.Empty, command: $"plugin:{GitLabManagePullRequests.GitLabManageMRDescription}");
                gitModule = gitUiCommands.GitModule;
                ForceRefreshGE(gitUiCommands);
            }
			else
			{
                GitLabPluginScriptManager.RemoveAll();
            }
        }

        public override void Unregister(IGitUICommands gitUiCommands)
        {
            if (IsGitLabRepo(gitUiCommands))
            {
                GitLabPluginScriptManager.RemoveAll();
            }

            repoType = null;
        }

        ///// <summary>
        ///// This is where you define the plugin setting page displayed in Git Extensions settings and that allows the user to configure the plugin settings.
        ///// You should return a collection of ISetting instances that could be of types:
        /////   * `BoolSetting` to store a boolean (display a Checkbox control),
        /////   * `StringSetting` to store a string (display a TextBox control),
        /////   * `NumberSetting` to store a number (display a TextBox control),
        /////   * `ChoiceSetting` to propose choices and store a string (display a ComboBox control),
        /////   * `PasswordSetting` to store a password (display a password TextBox control),
        /////   * `CredentialsSetting` to store a login and a password (display a login and a password fields),
        ///// See an example: https://github.com/gitextensions/gitextensions/blob/master/Plugins/JiraCommitHintPlugin/JiraCommitHintPlugin.cs
        ///// </summary>
        //public override IEnumerable<ISetting> GetSettings()
        //{
        //    // Uncomment and fill if your plugin have settings that the user could configure
        //}

        /// <summary>
        /// Is called when the plugin's name is clicked in Git Extensions' Plugins menu.
        /// Must return `true` if the revision grid should be refreshed after the execution of the plugin. false, otherwise.
        /// Help: You could call args.GitUICommands.StartSettingsDialog(this); in this method to open the setting page of the plugin.
        /// </summary>
        /// <returns>
        /// Returns <see langword="true"/> if the revision grid should be refreshed after the execution of the plugin;
        /// otherwise <see langword="false"/>.
        /// </returns>
        public override bool Execute(GitUIEventArgs gitUIEventArgs)
        {
            /*
             * open new form for below actions:
             *  Create Pull request
             */

            // Put your action logic here
            MessageBox.Show(gitUIEventArgs.OwnerForm, "Hello from the Plugin Template.", "Git Extensions");
            return false;
        }

        private void ForceRefreshGE(IGitUICommands gitUiCommands)
        {
            gitUiCommands.RepoChangedNotifier.Notify();
        }

        private IGitModule GetModule()
        {
            var module = getModule();

            if (module == null)
            {
                throw new ArgumentException($"Require a valid instance of {nameof(IGitModule)}");
            }

            return module;
        }

        public override IEnumerable<ISetting> GetSettings()
        {
            yield return OAuthToken;
            yield return GitLabHost;
        }

        #region IRepositoryHostPlugin definitions

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

            return repoType == RepoType.git && GetHostedRemotesForModule().Count() > 0;
        }

        /// <summary>
        /// Returns all relevant github-remotes for the current working directory
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
