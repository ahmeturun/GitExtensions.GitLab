namespace GitExtensions.GitLab.Forms
{
	using System;
	using System.Collections.Generic;
	using System.Drawing;
	using System.Linq;
	using System.Threading.Tasks;
	using System.Windows.Forms;
	using GitCommands;
	using GitExtensions.GitLab.Client.Repo;
	using GitUI;
	using GitUI.Properties;
	using GitUIPluginInterfaces;
	using GitUIPluginInterfaces.RepositoryHosts;
	using Microsoft.VisualStudio.Threading;
	using ResourceManager;

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
		private readonly TranslationString strUnableUnderstandPatch = new TranslationString("Error: Unable to understand patch");
		private readonly TranslationString error = new TranslationString("Error");
		private readonly TranslationString strFailedToLoadDiffData = new TranslationString("Failed to load diff data!");
		#endregion

		private readonly IGitModule gitModule;
		private readonly RevisionGridControl RevisionGridControl;
		private readonly AsyncLoader remoteLoader = new AsyncLoader();
		private readonly AsyncLoader userLoader = new AsyncLoader
		{
			Delay = TimeSpan.FromMilliseconds(1000)
		};
		private IReadOnlyList<IHostedRemote> hostedRemotes;
		private User[] assigneeUserList;
		private bool userSearchOngoing = false;

		private Dictionary<string, string> diffCache;

		public CreateMergeRequestForm(
			IGitModule gitModule,
			RevisionGridControl revisionGridControl)
		{
			this.gitModule = gitModule ?? throw new ArgumentNullException(nameof(gitModule));
			RevisionGridControl = revisionGridControl;
			InitializeComponent();
			InitializeComplete();
			sourceProjectCB.DisplayMember = nameof(IHostedRemote.RemoteRepositoryName);
			targetProjectCB.DisplayMember = nameof(IHostedRemote.RemoteRepositoryName);
			sourceBranchCB.DisplayMember = nameof(IHostedBranch.Name);
			targetBranchCB.DisplayMember = nameof(IHostedBranch.Name);
			assigneeCB.DisplayMember = nameof(Client.Repo.User.Name);
			assigneeCB.Text = strUserSearch.Text;
			assigneeCB.AutoCompleteMode = AutoCompleteMode.None;
			mergeRequestCreateLoading.Visible = false;
			mergeRequestCreateLoading.IsAnimating = false;
			fileStatusList.UICommandsSource = revisionGridControl.UICommandsSource;
			diffViewer.UICommandsSource = revisionGridControl.UICommandsSource;
		}

		private void CreateMergeRequestForm_Load(object sender, EventArgs e)
		{
			fileStatusList.SelectedIndexChanged += fileStatusList_SelectedIndexChanged;
			createMergeRequestBtn.Enabled = false;
			sourceBranchCB.Text = strLoading.Text;
			hostedRemotes = GitLabPlugin.Instance.GetHostedRemotesForModule();
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
					InitializeTargets(nonOwnedHostedRemotes);
				});
			new AsyncLoader().LoadAsync(
				() => 
				{
					userSearchOngoing = true;
					return GitLabPlugin.Instance.GitLabClient.GetUsers().ToArray();
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
			var selectedRevision = RevisionGridControl?.GetSelectedRevisions();
			if (selectedRevision?.Count == 1 && !selectedRevision[0].IsArtificial)
			{
				selectedBranch = selectedRevision[0].Refs?.ElementAtOrDefault(0)?.Name?.Split('/')?.LastOrDefault() ?? selectedBranch;
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

						SetCreateMergeButtonState();
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
			userLoader.Cancel();
			userLoader.LoadAsync(
				(userSearchCancellationToken) =>
				{
					userSearchOngoing = true;
					if (userSearchCancellationToken.IsCancellationRequested)
					{
						return assigneeUserList;
					}
					return GitLabPlugin.Instance.GitLabClient.GetUsers(searchText).ToArray();
				},
				users =>
				{
					assigneeUserList = users;
					assigneeCB.Items.Clear();
					assigneeCB.Items.AddRange(users);
					assigneeCB.DroppedDown = true;
					var currentText = assigneeCB.Text;
					assigneeCB.SelectedText = string.Empty;
					assigneeCB.Text = currentText;
					assigneeCB.SelectionStart = currentText.Length;
					assigneeCB.Capture = false;
					userSearchOngoing = false;
				});
		}


		private void AssigneeCB_Click(object sender, System.EventArgs e)
		{
			SetCreateMergeButtonState();
		}

		private void AssigneeCB_SelectedValueChanged(object sender, System.EventArgs e)
		{
			SetCreateMergeButtonState();
		}

		private void AssigneeCB_TextChanged(object sender, System.EventArgs e)
		{
			SetCreateMergeButtonState();
		}
		private void MergeRequestTitleTB_TextChanged(object sender, System.EventArgs e)
		{
			SetCreateMergeButtonState();
		}

		private void SetCreateMergeButtonState()
		{
			createMergeRequestBtn.Enabled = assigneeCB.SelectedItem != null 
				&& sourceBranchCB.SelectedItem != null 
				&& targetBranchCB.SelectedItem != null 
				&& !string.IsNullOrEmpty(mergeRequestTitleTB.Text);
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
				() => GitLabPlugin.Instance.GitLabClient.CreateMergeRequest(mergeRequest),
				createdMergeRequest =>
				{
					new MergeRequsetFormStatus(
						mergeRequestCompleted.Text,
						createdMergeRequest.WebUrl,
						Images.StatusBadgeSuccess,
						false,
						"Merge request created.").ShowDialog(this);
					Close();
				});
		}

		private void MergeRequestLoader_LoadingError(object sender, AsyncErrorEventArgs e)
		{
			mergeRequestCreateLoading.Visible = false;
			mergeRequestCreateLoading.IsAnimating = false;

			using (var errorForm = new MergeRequsetFormStatus(
						mergeRequestError.Text,
						string.Empty,
						Images.StatusBadgeError,
						true,
						errorDetails.Text,
						e.Exception.Message))
			{
				errorForm.ShowDialog(this);
			}

			createMergeRequestBtn.Enabled = true;
		}

		#region merge request diff management
		private void branchCB_SelectedIndexChanged(object sender, EventArgs e)
		{
			diffViewer.Clear();
			fileStatusList.ClearDiffs();
			if (sourceBranchCB.SelectedItem == null || targetBranchCB.SelectedItem == null)
			{
				return;
			}
			LoadDiffPatch();
		}

		private void LoadDiffPatch()
		{
			ThreadHelper.JoinableTaskFactory.RunAsync(
				async () =>
				{
					try
					{
						var sourceProject = sourceProjectCB.SelectedItem as GitLabHostedRemote;
						var sourceBranch = sourceBranchCB.SelectedItem as IHostedBranch;
						var targetBranch = targetBranchCB.SelectedItem as IHostedBranch;
						var branchDiff = GetDiffData(
							sourceProject.Id,
							sourceBranch.Sha.ToString(),
							targetBranch.Sha.ToString());

						await this.SwitchToMainThreadAsync();

						SplitAndLoadDiff(
							branchDiff,
							sourceBranch.Sha.ToString(),
							targetBranch.Sha.ToString());
					}
					catch (Exception ex) when (!(ex is OperationCanceledException))
					{
						MessageBox.Show(this, strFailedToLoadDiffData.Text + Environment.NewLine + ex.Message, error.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
					}
				})
				.FileAndForget();
		}

		private BranchDiff GetDiffData(int projectId, string sourceId, string targetId)
			=> GitLabPlugin.Instance.GitLabClient.GetBranchDiff(projectId, sourceId, targetId);

		private void SplitAndLoadDiff(BranchDiff diffData, string baseSha, string secondSha)
		{
			diffCache = new Dictionary<string, string>();

			var giss = new List<GitItemStatus>();
			var firstRev = ObjectId.TryParse(baseSha, out var firstId)
				? new GitRevision(firstId)
				: null;
			var secondRev = ObjectId.TryParse(secondSha, out var secondId)
				? new GitRevision(secondId)
				: null;
			if (secondRev == null)
			{
				MessageBox.Show(this, strUnableUnderstandPatch.Text, error.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}

			foreach (var diff in diffData.Diffs)
			{
				var gis = new GitItemStatus {
					IsChanged = true,
					IsNew = diff.NewFile,
					IsDeleted = diff.DeletedFile,
					IsTracked = true,
					Name = diff.NewPath,
					Staged = StagedStatus.None
				};

				giss.Add(gis);
				diffCache.Add(gis.Name, diff.DiffContent);
			}

			// Note: Commits in PR may not exist in the local repo
			fileStatusList.SetDiffs(firstRev, secondRev, items: giss);
		}

		private void fileStatusList_SelectedIndexChanged(object sender, EventArgs e)
		{
			var gis = fileStatusList.SelectedItem?.Item;
			if (gis == null)
			{
				return;
			}

			var data = diffCache[gis.Name];

			if (gis.IsSubmodule)
			{
				diffViewer.ViewText(gis.Name, text: data);
			}
			else
			{
				diffViewer.ViewPatch(gis.Name, text: data);
			}
		}
		#endregion

		private void cancelMergeRequestFormBtn_Click(object sender, EventArgs e)
		{
			Close();
		}
	}
}
