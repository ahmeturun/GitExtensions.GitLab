using GitCommands;
using GitUI;
using GitUIPluginInterfaces;
using GitUIPluginInterfaces.RepositoryHosts;
using ResourceManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.VisualStudio.Threading;
using System.Drawing;
using System.Threading;
using GitExtensions.GitLab.Client.Repo;
using System.Web;
using GitUI.HelperDialogs;
using GitUI.UserControls;
using GitUI.Properties;

namespace GitExtensions.GitLab.Forms
{
	public partial class CreateMergeRequestForm : GitExtensionsForm
	{
		#region Translation
		private readonly TranslationString strLoading = new TranslationString("Loading...");
		private readonly TranslationString strFailedToCreatePullRequest = new TranslationString("Failed to create merge request.");
		private readonly TranslationString strPleaseCloneGitHubRep = new TranslationString("Please clone GitLab repository before merge request.");
		private readonly TranslationString strUserSearch = new TranslationString("Seach Users");
		private readonly TranslationString mergeRequestCompleted = new TranslationString("Merge Request Created");
		private readonly TranslationString mergeRequestError = new TranslationString("Merge Request Creation is Unsuccessfull");
		private readonly TranslationString errorDetails = new TranslationString("Error details");
		#endregion

		private readonly IGitModule gitModule;
		private readonly IRepositoryHostPlugin repoHost;
		private readonly GitModuleForm FormBrowse;
		private readonly AsyncLoader remoteLoader = new AsyncLoader();
		private readonly AsyncLoader userLoader = new AsyncLoader
		{
			Delay = TimeSpan.FromMilliseconds(1000)
		};
		private IReadOnlyList<IHostedRemote> hostedRemotes;

		private string selectedSourceBranch;
		private string selectedTargetBranch;
		private User[] assigneeUserList;
		private bool userSearchOngoing = false;
		private CancellationToken userSearchCancellationToken;
		private CancellationTokenSource userSearchCancellationTokenSource = new CancellationTokenSource();

		public CreateMergeRequestForm(
			IGitModule gitModule,
			IRepositoryHostPlugin repoHost,
			GitModuleForm formBrowse)
		{
			this.gitModule = gitModule ?? throw new ArgumentNullException(nameof(gitModule));
			this.repoHost = repoHost ?? throw new ArgumentNullException(nameof(repoHost));
			FormBrowse = formBrowse;
			InitializeComponent();
			InitializeComplete();
			sourceProjectCB.DisplayMember = nameof(IHostedRemote.RemoteRepositoryName);
			targetProjectCB.DisplayMember = nameof(IHostedRemote.RemoteRepositoryName);
			sourceBranchCB.DisplayMember = nameof(IHostedBranch.Name);
			targetBranchCB.DisplayMember = nameof(IHostedBranch.Name);
			assigneeCB.DisplayMember = nameof(Client.Repo.User.Name);
			assigneeCB.Text = strUserSearch.Text;
			mergeRequestCreateLoading.Visible = false;
			mergeRequestCreateLoading.IsAnimating = false;
		}

		private void CreateMergeRequestForm_Load(object sender, EventArgs e)
		{
			createMergeRequestBtn.Enabled = false;
			sourceBranchCB.Text = strLoading.Text;
			hostedRemotes = repoHost.GetHostedRemotesForModule();
			// TODO: show mask for loading.
			InitializeSources(hostedRemotes.Where(remote => remote.IsOwnedByMe).ToArray());
			remoteLoader.LoadAsync(
				() => hostedRemotes.Where(remote => remote.IsOwnedByMe).ToArray(),
				nonOwnedHostedRemotes =>
				{
					if (nonOwnedHostedRemotes.Length == 0)
					{
						MessageBox.Show(
							this,
							strFailedToCreatePullRequest.Text + Environment.NewLine + strPleaseCloneGitHubRep.Text,
							string.Empty,
							MessageBoxButtons.OK,
							MessageBoxIcon.Error);
						Close();
						return;
					}

					selectedSourceBranch = gitModule.IsValidGitWorkingDir()
						? gitModule.GetSelectedBranch()
						: string.Empty;
					InitializeTargets(nonOwnedHostedRemotes);
				});
			new AsyncLoader().LoadAsync(
				() => 
				{
					userSearchOngoing = true;
					return GitLabPlugin.GitLabClient.GetUsers().ToArray();
				},
				users =>
				{
					assigneeCB.Items.AddRange(users);
					userSearchOngoing = false;
				});
		}

		#region SourceManagement
		private void InitializeSources(IHostedRemote[] ownedHostedRemotes)
		{
			sourceProjectCB.Items.Clear();
			sourceProjectCB.Items.AddRange(ownedHostedRemotes);

			if (sourceProjectCB.Items.Count > 0)
			{
				sourceProjectCB.SelectedIndex = 0;
			}
		}

		private void sourceProjectCB_SelectedIndexChanged(object sender, EventArgs e)
		{
			var selectedTargetHostedRemote = sourceProjectCB.SelectedItem as IHostedRemote;
			sourceBranchCB.Items.Clear();
			sourceBranchCB.Text = strLoading.Text;
			var selectedBranch = string.Empty;
			var selectedRevision = FormBrowse.RevisionGridControl.GetSelectedRevisions();
			if (selectedRevision?.Count == 1 && !selectedRevision[0].IsArtificial)
			{
				selectedBranch = selectedRevision[0].Refs[0].Name.Split('/')?.LastOrDefault() ?? selectedBranch;
			}

			PopulateBranchesComboAndEnableCreateButton(
				selectedTargetHostedRemote,
				sourceBranchCB,
				selectedBranch);
		}

		#endregion

		#region TargetManagement
		private void InitializeTargets(IHostedRemote[] nonOwnedHostedRemotes)
		{
			targetProjectCB.Items.Clear();
			targetProjectCB.Items.AddRange(nonOwnedHostedRemotes);

			if (targetProjectCB.Items.Count > 0)
			{
				targetProjectCB.SelectedIndex = 0;
			}
		}
		private void targetProjectCB_SelectedIndexChanged(object sender, EventArgs e)
		{
			var selectedTargetHostedRemote = targetProjectCB.SelectedItem as IHostedRemote;

			targetBranchCB.Items.Clear();
			targetBranchCB.Text = strLoading.Text;

			PopulateBranchesComboAndEnableCreateButton(selectedTargetHostedRemote, targetBranchCB);
		}

		#endregion



		private void PopulateBranchesComboAndEnableCreateButton(
			IHostedRemote remote,
			ComboBox comboBox,
			string selectBranch = "")
		{
			ThreadHelper.JoinableTaskFactory.RunAsync(
					async () =>
					{
						await TaskScheduler.Default;

						IHostedRepository hostedRepository = remote.GetHostedRepository();
						var branches = hostedRepository.GetBranches();

						await this.SwitchToMainThreadAsync();

						comboBox.Items.Clear();

						var selectItem = 0;
						var defaultBranch = string.IsNullOrEmpty(selectBranch)
							? hostedRepository.GetDefaultBranch()
							: selectBranch;
						for (var i = 0; i < branches.Count; i++)
						{
							if (branches[i].Name == defaultBranch)
							{
								selectItem = i;
							}

							comboBox.Items.Add(branches[i]);
						}

						if (branches.Count > 0)
						{
							comboBox.SelectedIndex = selectItem;
						}

						createMergeRequestBtn.Enabled = true;
					})
				.FileAndForget();
		}

		private void assigneeCB_Enter(object sender, EventArgs e)
		{
			if (assigneeCB.Text.Length == 0)
			{
				assigneeCB.Text = strUserSearch.Text;
				assigneeCB.ForeColor = SystemColors.GrayText;
			}
		}

		private void assigneeCB_Leave(object sender, EventArgs e)
		{
			if (assigneeCB.Text == strUserSearch.Text)
			{
				assigneeCB.Text = "";
				assigneeCB.ForeColor = SystemColors.WindowText;
			}
		}

		private void assigneeCB_KeyUp(object sender, KeyEventArgs e)
		{
			if (assigneeCB.Text.Length < 3 || assigneeCB.Text == strUserSearch.Text || userSearchOngoing)
			{
				return;
			}
			var searchText = assigneeCB.Text;
			userSearchCancellationTokenSource.Cancel();
			userSearchCancellationTokenSource = new CancellationTokenSource();
			userSearchCancellationToken = userSearchCancellationTokenSource.Token;
			userLoader.Cancel();
			userLoader.LoadAsync(
				(userSearchCancellationToken) =>
				{
					userSearchOngoing = true;
					if (userSearchCancellationToken.IsCancellationRequested)
					{
						return assigneeUserList;
					}
					return GitLabPlugin.GitLabClient.GetUsers(searchText).ToArray();
				},
				users =>
				{
					assigneeUserList = users;
					assigneeCB.Items.Clear();
					assigneeCB.Items.AddRange(users);
					assigneeCB.DroppedDown = true;
					userSearchOngoing = false;
				});
		}

		private void createMergeRequestBtn_Click(object sender, EventArgs e)
		{
			createMergeRequestBtn.Enabled = false;
			var targetProject = targetProjectCB.SelectedItem as GitLabHostedRemote;
			var targetBranch = targetBranchCB.SelectedItem as IHostedBranch;
			var targetProjectId = targetProject.Id;
			var sourceProject = sourceProjectCB.SelectedItem as GitLabHostedRemote;
			var sourceBranch = sourceBranchCB.SelectedItem as IHostedBranch;
			var sourceProjectId = sourceProject.Id;
			var assignee = assigneeCB.SelectedItem as User;

			var mergeRequest = new MergeRequest
			{
				Id = sourceProjectId,
				SourceBranch = sourceBranch.Name,
				TargetBranch = targetBranch.Name,
				TargetProjectId = targetProjectId,
				SourceProjectId = sourceProjectId,
				Title = mergeRequestTitleTB.Text,
				AssigneeId = assignee.Id,
				Description = mergeRequestDetailTB.Text,
				RemoveSourceBranch = deleteSourceBranchChkBox.Checked,
				Squash = squashCommitsChkBox.Checked
			};

			var mergeRequestLoader = new AsyncLoader();
			mergeRequestLoader.LoadingError += MergeRequestLoader_LoadingError;
			mergeRequestCreateLoading.Visible = true;
			mergeRequestCreateLoading.IsAnimating = true;
			mergeRequestLoader.LoadAsync(
				() => GitLabPlugin.GitLabClient.CreateMergeRequest(mergeRequest),
				createdMergeRequest =>
				{
					new MergeRequsetFormStatus(
						mergeRequestCompleted.Text,
						createdMergeRequest.WebUrl,
						Images.StatusBadgeSuccess,
						"Merge request created.").ShowDialog(this);
					Close();
				});
		}

		private void MergeRequestLoader_LoadingError(object sender, AsyncErrorEventArgs e)
		{
			mergeRequestCreateLoading.Visible = false;
			mergeRequestCreateLoading.IsAnimating = false;

			new MergeRequsetFormStatus(
						mergeRequestError.Text,
						string.Empty,
						Images.StatusBadgeError,
						errorDetails.Text,
						e.Exception.Message).ShowDialog(this);
			createMergeRequestBtn.Enabled = true;
		}
	}
}
