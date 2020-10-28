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

		public bool DontShowAgain { get; set; }

		[Obsolete]
		public MergeRequsetFormStatus() { }

		public MergeRequsetFormStatus(
			string text,
			string mergeRequestLink,
			Bitmap dialogIcon,
			bool isError,
			params string[] output) : base(null)
		{
			this.mergeRequestLink = mergeRequestLink;
			SetIcon(dialogIcon);
			InitializeComponent();
			if (!string.IsNullOrEmpty(mergeRequestLink))
			{
				mergeRequestUrlLabel.Click += MergeRequestUrlLabel_Click;
				mergeRequestUrlLabel.Dock = DockStyle.Bottom;
				mergeRequestUrlLabel.Visible = true;
			}
			Text = text;
			DontShowAgain = dontShowCheckBox.Checked;
			if (output?.Length > 0)
			{
				foreach (string line in output)
				{
					AppendMessage(line, consoleOutputControl);
				}
			}
			if (isError)
			{
				dontShowCheckBox.Visible = true;
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

		private void dontShowCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			DontShowAgain = dontShowCheckBox.Checked;
		}
	}
}
