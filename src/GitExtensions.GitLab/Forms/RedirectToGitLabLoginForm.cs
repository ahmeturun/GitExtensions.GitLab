using GitExtensions.GitLab.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GitExtensions.GitLab.Forms
{
	public partial class RedirectToGitLabLoginForm : Form
	{
		public bool dontAskForLoginAgain = false;
		public RedirectToGitLabLoginForm()
		{
			InitializeComponent();
		}

		private void RedirectToGitLabLoginForm_Load(object sender, EventArgs e)
		{

		}

		private void dontAskAgainCheckBox_CheckedChanged(object sender, EventArgs e)
		{

		}

		private void cancelButton_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.Cancel;
		}

		private void label2_Click(object sender, EventArgs e)
		{

		}

		private void continueButton_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.OK;
			Close();
		}
	}
}
