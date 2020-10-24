namespace GitExtensions.GitLab
{
	using System;
	using System.ComponentModel.Composition;
	using System.Linq;
	using System.Windows.Forms;
	using GitCommands;
	using GitExtensions.GitLab.Forms;
	using GitUI;
	using GitUI.CommandsDialogs;
	using GitUIPluginInterfaces;
	using GitUIPluginInterfaces.RepositoryHosts;
	using ResourceManager;

	[Export(typeof(IGitPlugin))]
	public class GitLabCreateMergeRequest : GitPluginBase
	{
		public static string GitLabCreateMRDescription = "GitLab Create Merge Request";
		public GitLabCreateMergeRequest() : base(false)
		{
			SetNameAndDescription(GitLabCreateMRDescription);
		}

		public override void Register(IGitUICommands gitUiCommands)
		{
			base.Register(gitUiCommands);
		}

		public override bool Execute(GitUIEventArgs args)
		{
			var showMergeRequestFormChecked = GitLabPlugin.Instance.AskForMergeRequestAfterPushValue;
			if (args.OwnerForm is FormPush formPush && !formPush.ErrorOccurred)
			{
				if (showMergeRequestFormChecked)
				{
					// show confirmation dialog to confirm opening create merge request form
					var createMergeRequestCheckResult = MessageBox.Show(
						formPush,
						$"Would you like to create a merge request?",
						"GitLab Plugin",
						MessageBoxButtons.YesNo,
						MessageBoxIcon.Question);
					if (createMergeRequestCheckResult == DialogResult.Yes)
					{
						return InitializeMergeRequestForm(args);
					}
					return false;
				}
				return false;
			}
			else if(args.OwnerForm is FormCreateTag
				|| args.OwnerForm is FormDeleteRemoteBranch
				|| args.OwnerForm is FormDeleteTag)
			{
				return false;
			}
			else
			{
				return InitializeMergeRequestForm(args);
			}

		}

		private bool InitializeMergeRequestForm(GitUIEventArgs args)
		{
			if (!TryGetRepositoryHost(args.GitModule, args.OwnerForm, out var repoHostPlugin))
			{
				return false;
			}
			var revisionGridControl = (args.OwnerForm as GitModuleForm)?.RevisionGridControl
				?? args.OwnerForm as RevisionGridControl
				?? ((args.OwnerForm as Form).Owner as GitUI.CommandsDialogs.FormBrowse)?.RevisionGridControl;

			var mergeRequestForm = new CreateMergeRequestForm(
				args.GitModule,
				repoHostPlugin,
				revisionGridControl);
			mergeRequestForm.ShowDialog(args.OwnerForm);
			return false;
		}

		private bool TryGetRepositoryHost(IGitModule module, IWin32Window owner, out IRepositoryHostPlugin repoHost)
		{
			if (!module.IsValidGitWorkingDir())
			{
				repoHost = null;
				return false;
			}
			repoHost = PluginRegistry.GitHosters.FirstOrDefault(gitHosters => gitHosters.GitModuleIsRelevantToMe());

			if (repoHost == null)
			{
				var errorString = new TranslationString("Error");
				var noReposHostFound = new TranslationString("Could not find any relevant repository hosts for the currently open repository.");
				MessageBox.Show(
					owner,
					noReposHostFound.Text,
					errorString.Text,
					MessageBoxButtons.OK,
					MessageBoxIcon.Error);
				return false;
			}

			return true;
		}
	}
}
