using GitUI;
using GitUI.Properties;
using GitUI.UserControls;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace GitExtensions.GitLab.Forms
{
	public partial class MergeRequsetFormStatus : GitModuleForm
	{
		private readonly string mergeRequestLink;

		[Obsolete]
		public MergeRequsetFormStatus() { }

		public MergeRequsetFormStatus(string text, string mergeRequestLink, Bitmap dialogIcon, params string[] output) : base(null)
		{
			this.mergeRequestLink = mergeRequestLink;
			SetIcon(dialogIcon);
			InitializeComponent();
			//Ok.Focus();
			//Ok.UseWaitCursor = false;
			if (!string.IsNullOrEmpty(mergeRequestLink))
			{
				mergeRequestUrlLabel.Click += MergeRequestUrlLabel_Click;
				mergeRequestUrlLabel.Dock = DockStyle.Bottom;
				mergeRequestUrlLabel.Visible = true;
			}
			Text = text;
			if (output?.Length > 0)
			{
				foreach (string line in output)
				{
					AppendMessage(line, consoleOutputControl);
				}
			}
			InitializeComplete();
		}

		private void MergeRequestUrlLabel_Click(object sender, EventArgs e)
		{
			System.Diagnostics.Process.Start(mergeRequestLink);
		}

		private void SetIcon(Bitmap image)
		{
			Icon oldIcon = Icon;
			Icon = BitmapToIcon(image);
			oldIcon?.Dispose();
		}

		private static Icon BitmapToIcon(Bitmap bitmap)
		{
			IntPtr handle = IntPtr.Zero;
			try
			{
				handle = bitmap.GetHicon();
				var icon = Icon.FromHandle(handle);

				return (Icon)icon.Clone();
			}
			finally
			{
				if (handle != IntPtr.Zero)
				{
					NativeMethods.DestroyIcon(handle);
				}
			}
		}

		private void Ok_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.OK;
			Close();
		}

        private protected void AppendMessage(string text, ConsoleOutputControl consoleOutputControl)
		{

			consoleOutputControl.AppendMessageFreeThreaded(text);
		}
	}
}
