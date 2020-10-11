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

namespace GitExtensions.GitLab
{
    /// <summary>
    /// A template for Git Extensions plugins.
    /// Find more documentation here: https://github.com/gitextensions/gitextensions.plugintemplate/wiki/GitPluginBase
    /// </summary>
    [Export(typeof(IGitPlugin))]
    public class GitLabPlugin : GitPluginBase, IRepositoryHostPlugin
    {
        private readonly CredentialsSetting credentialsSettings;
        private readonly Func<IGitModule> getModule;
        public readonly StringSetting OAuthToken = new StringSetting("OAuth Token", "");
        public readonly StringSetting GitLabHost = new StringSetting(
            "GitLab (Enterprise) hostname", 
            "https://gitlab.com");

        private IGitUICommands currentGitUiCommands;

        public string GitLabApiEndpoint => $"{GitLabHost.ValueOrDefault(Settings).Trim('/')}/api/v4/";

        private IGitModule gitModule;

        internal static GitLabPlugin Instance;
        internal static Client.Client gitLab;
        internal static Client.Client GitLab => gitLab ?? (gitLab = new Client.Client(
            Instance.GitLabHost.ValueOrDefault(Instance.Settings).Trim('/'),
            Instance.OAuthToken.ValueOrDefault(Instance.Settings)));

        public bool ConfigurationOk => throw new NotImplementedException();

        public string OwnerLogin => throw new NotImplementedException();

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
            // Put your initialization logic here
            base.Register(gitUiCommands);
            currentGitUiCommands = gitUiCommands;
            gitModule = gitUiCommands.GitModule;
            // TODO: open a form to set the personal access token if it's not set
        }


        /// <summary>
        /// Is called when the plugin is unloaded. This happens every time when a repository is closed through one of the following events:
        ///   1. opening another repository
        ///   2. returning to Dashboard (Repository > Close (go to Dashboard))
        ///   3. closing Git Extensions
        /// </summary>
        public override void Unregister(IGitUICommands gitUiCommands)
        {
            // Put your cleaning logic here
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
            // Put your action logic here
            MessageBox.Show(gitUIEventArgs.OwnerForm, "Hello from the Plugin Template.", "Git Extensions");
            return false;
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
            throw new NotImplementedException();
        }

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
                    var url = gitModule.GetSetting(string.Format(StringKeySettings.RemoteUrl, remote));

                    if (string.IsNullOrEmpty(url))
                    {
                        continue;
                    }
                    var gitLabHostUri = new Uri(GitLabHost.ValueOrDefault(Settings));
                    if (new Remote.GitLabRemoteParser(gitLabHostUri.Host)
                            .TryExtractGitHubDataFromRemoteUrl(url, out var owner, out var repository))
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

        public Task<string> AddUpstreamRemoteAsync()
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<ISetting> GetSettings()
        {
            yield return OAuthToken;
            yield return GitLabHost;
        }
    }
}
