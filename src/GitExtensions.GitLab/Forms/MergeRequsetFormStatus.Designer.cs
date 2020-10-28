namespace GitExtensions.GitLab.Forms
{
	partial class MergeRequsetFormStatus
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.consoleOutputControl = new GitUI.UserControls.EditboxBasedConsoleOutputControl();
			this.mergeRequestUrlLabel = new System.Windows.Forms.LinkLabel();
			this.OKBtn = new System.Windows.Forms.Button();
			this.dontShowCheckBox = new System.Windows.Forms.CheckBox();
			this.consoleOutputControl.SuspendLayout();
			this.SuspendLayout();
			// 
			// consoleOutputControl
			// 
			this.consoleOutputControl.Controls.Add(this.mergeRequestUrlLabel);
			this.consoleOutputControl.Location = new System.Drawing.Point(12, 12);
			this.consoleOutputControl.Name = "consoleOutputControl";
			this.consoleOutputControl.Size = new System.Drawing.Size(524, 185);
			this.consoleOutputControl.TabIndex = 0;
			this.consoleOutputControl.Text = "editboxBasedConsoleOutputControl1";
			this.consoleOutputControl.UseWaitCursor = true;
			// 
			// mergeRequestUrlLabel
			// 
			this.mergeRequestUrlLabel.AutoSize = true;
			this.mergeRequestUrlLabel.Location = new System.Drawing.Point(367, 0);
			this.mergeRequestUrlLabel.Name = "mergeRequestUrlLabel";
			this.mergeRequestUrlLabel.Size = new System.Drawing.Size(154, 13);
			this.mergeRequestUrlLabel.TabIndex = 1;
			this.mergeRequestUrlLabel.TabStop = true;
			this.mergeRequestUrlLabel.Text = "Open merge request in browser";
			this.mergeRequestUrlLabel.UseWaitCursor = true;
			this.mergeRequestUrlLabel.Visible = false;
			// 
			// OKBtn
			// 
			this.OKBtn.Location = new System.Drawing.Point(457, 228);
			this.OKBtn.Name = "OKBtn";
			this.OKBtn.Size = new System.Drawing.Size(75, 23);
			this.OKBtn.TabIndex = 1;
			this.OKBtn.Text = "Ok";
			this.OKBtn.UseVisualStyleBackColor = true;
			this.OKBtn.Click += new System.EventHandler(this.Ok_Click);
			// 
			// dontShowCheckBox
			// 
			this.dontShowCheckBox.AutoSize = true;
			this.dontShowCheckBox.Location = new System.Drawing.Point(324, 232);
			this.dontShowCheckBox.Name = "dontShowCheckBox";
			this.dontShowCheckBox.Size = new System.Drawing.Size(108, 17);
			this.dontShowCheckBox.TabIndex = 2;
			this.dontShowCheckBox.Text = "Don\'t show again";
			this.dontShowCheckBox.UseVisualStyleBackColor = true;
			this.dontShowCheckBox.Visible = false;
			this.dontShowCheckBox.CheckedChanged += new System.EventHandler(this.dontShowCheckBox_CheckedChanged);
			// 
			// MergeRequsetFormStatus
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.ClientSize = new System.Drawing.Size(549, 263);
			this.Controls.Add(this.dontShowCheckBox);
			this.Controls.Add(this.OKBtn);
			this.Controls.Add(this.consoleOutputControl);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "MergeRequsetFormStatus";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Process";
			this.consoleOutputControl.ResumeLayout(false);
			this.consoleOutputControl.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		protected internal System.Windows.Forms.FlowLayoutPanel ControlsPanel;
		private GitUI.UserControls.EditboxBasedConsoleOutputControl consoleOutputControl;
		private System.Windows.Forms.LinkLabel mergeRequestUrlLabel;
		private System.Windows.Forms.Button OKBtn;
		private System.Windows.Forms.CheckBox dontShowCheckBox;
	}
}