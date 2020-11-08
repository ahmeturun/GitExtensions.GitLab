namespace GitExtensions.GitLab.Forms
{
	using GitUI;
	using Microsoft.VisualStudio.Threading;
	using System;
	using System.Threading.Tasks;
	using System.Windows.Forms;

	public partial class LoginForm : GitExtensionsForm
	{
		private readonly JoinableTaskFactory joinableTaskFactory;
		public LoginForm()
		{
			GitLabPlugin.Instance.LoggedIn = false;
			joinableTaskFactory = new JoinableTaskContext().Factory;
			InitializeComponent();
			InitializeComplete();
		}

		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			if(keyData == Keys.Enter)
			{
				loginButtonClick(null, null);
			}
			return base.ProcessCmdKey(ref msg, keyData);
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
				GitLabPlugin.Instance.GitLabClient.UpdateAuthToken(loginResult.AccessToken);
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
			GitLabPlugin.Instance.LoggedIn = false;
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
	}
}
