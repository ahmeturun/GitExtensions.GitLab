namespace GitExtensions.GitLab
{
	using System.ComponentModel.Composition;
	using System.Windows.Forms;
	using GitExtensions.GitLab.Forms;
	using GitUI;
	using GitUI.CommandsDialogs;
	using GitUIPluginInterfaces;
	using ResourceManager;

	[Export(typeof(IGitPlugin))]
	public class GitLabCreateMergeRequest : GitPluginBase
	{
		#region Translation
		private readonly TranslationString ErrorString = new TranslationString("Error");
		private readonly TranslationString NoReposHostFound = new TranslationString("Could not find any relevant repository hosts for the currently open repository.");
		#endregion

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
			if (!CheckForGitModule(args.GitModule, args.OwnerForm))
			{
				return false;
			}
			var revisionGridControl = (args.OwnerForm as GitModuleForm)?.RevisionGridControl
				?? args.OwnerForm as RevisionGridControl
				?? ((args.OwnerForm as Form).Owner as FormBrowse)?.RevisionGridControl;

			var mergeRequestForm = new CreateMergeRequestForm(
				args.GitModule,
				revisionGridControl);
			mergeRequestForm.ShowDialog(args.OwnerForm);
			return false;
		}

		private bool CheckForGitModule(IGitModule module, IWin32Window owner)
		{
			if (!module.IsValidGitWorkingDir())
			{
				return false;
			}

			if (!GitLabPlugin.Instance.GitModuleIsRelevantToMe())
			{
				MessageBox.Show(
					owner,
					NoReposHostFound.Text,
					ErrorString.Text,
					MessageBoxButtons.OK,
					MessageBoxIcon.Error);
				return false;
			}

			return true;
		}
	}
}
