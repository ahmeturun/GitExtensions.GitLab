namespace GitExtensions.GitLab.Forms
{
	using GitUI;
	using GitUIPluginInterfaces;
	using Microsoft.VisualStudio.Threading;
	using System;
	using System.Threading.Tasks;

	public partial class LoginForm : GitExtensionsForm
	{
		private readonly JoinableTaskFactory joinableTaskFactory;
		public LoginForm()
		{
			joinableTaskFactory = new JoinableTaskContext().Factory;
			InitializeComponent();
			InitializeComplete();
		}

		private void loginButtonClick(object sender, EventArgs e)
		{
			try
			{
				var loginResult = joinableTaskFactory.Run(() =>
					Task.FromResult(GitLabPlugin.Instance.GitLabClient.Login(
						usernameTextBox.Text, passwordTextbox.Text)));
				GitLabPlugin.Instance.Settings.SetString(
					GitLabPlugin.Instance.OAuthToken.Name,
					loginResult.AccessToken);
				GitLabPlugin.Instance.LoggedIn = true;
				loadingControl1.Visible = false;
				Close();

			}
			catch (Exception ex)
			{
				LoginError(ex);
			}
		}

		private void LoginError(Exception e)
		{
			Close();
			using (var mergeRequestformStatus = new MergeRequsetFormStatus(
						$"GitLab Plugin - Login Error",
						string.Empty,
						GitUI.Properties.Images.StatusBadgeError,
						true,
						e.Message))
			{
				mergeRequestformStatus.ShowDialog();
			}
		}
		public sealed class LoggedInEventArgs : EventArgs
		{
			public IGitUICommands GitUICommands { get; }


			public LoggedInEventArgs(IGitUICommands gitUICommands)
			{
				GitUICommands = gitUICommands;
			}
		}
	}
}
