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
			StartPosition = FormStartPosition.CenterParent;
			SetIcon(dialogIcon);
			InitializeComponent();
			Ok.Enabled = true;
			Ok.Focus();
			AcceptButton = Ok;
			var consoleOutputControl = new EditboxBasedConsoleOutputControl
			{
				Dock = DockStyle.Top,
				Size = new Size(50,50)
			};
			MainPanel.Controls.Add(consoleOutputControl);
			if (!string.IsNullOrEmpty(mergeRequestLink))
			{
				var mergeRequestUrlLabel = new LinkLabel
				{
					Text = "Open merge request in browser"
				};
				mergeRequestUrlLabel.Click += MergeRequestUrlLabel_Click;
				mergeRequestUrlLabel.Dock = DockStyle.Bottom;
				MainPanel.Controls.Add(mergeRequestUrlLabel);
			}
			Text = text;
			if (output?.Length > 0)
			{
				foreach (string line in output)
				{
					AppendMessage(line, consoleOutputControl);
				}
			}
			ControlsPanel.Controls.Add(Ok);
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
