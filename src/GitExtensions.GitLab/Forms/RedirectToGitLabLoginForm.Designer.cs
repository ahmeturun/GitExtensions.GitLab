namespace GitExtensions.GitLab.Forms
{
	partial class RedirectToGitLabLoginForm
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RedirectToGitLabLoginForm));
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.dontAskAgainCheckBox = new System.Windows.Forms.CheckBox();
			this.continueButton = new System.Windows.Forms.Button();
			this.cancelButton = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.Location = new System.Drawing.Point(12, 18);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(320, 38);
			this.label1.TabIndex = 0;
			this.label1.Text = "You need to login to GitLab to be able to use GitLab Plugin.";
			// 
			// label2
			// 
			this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label2.Location = new System.Drawing.Point(12, 60);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(320, 23);
			this.label2.TabIndex = 1;
			this.label2.Text = "Continue to login to GitLab.";
			this.label2.Click += new System.EventHandler(this.label2_Click);
			// 
			// dontAskAgainCheckBox
			// 
			this.dontAskAgainCheckBox.AutoSize = true;
			this.dontAskAgainCheckBox.Location = new System.Drawing.Point(68, 110);
			this.dontAskAgainCheckBox.Name = "dontAskAgainCheckBox";
			this.dontAskAgainCheckBox.Size = new System.Drawing.Size(102, 17);
			this.dontAskAgainCheckBox.TabIndex = 2;
			this.dontAskAgainCheckBox.Text = "Don\'t Ask Again";
			this.dontAskAgainCheckBox.UseVisualStyleBackColor = true;
			this.dontAskAgainCheckBox.CheckedChanged += new System.EventHandler(this.dontAskAgainCheckBox_CheckedChanged);
			// 
			// continueButton
			// 
			this.continueButton.Location = new System.Drawing.Point(257, 106);
			this.continueButton.Name = "continueButton";
			this.continueButton.Size = new System.Drawing.Size(75, 23);
			this.continueButton.TabIndex = 3;
			this.continueButton.Text = "Continue";
			this.continueButton.UseVisualStyleBackColor = true;
			this.continueButton.Click += new System.EventHandler(this.continueButton_Click);
			// 
			// cancelButton
			// 
			this.cancelButton.Location = new System.Drawing.Point(176, 106);
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.Size = new System.Drawing.Size(75, 23);
			this.cancelButton.TabIndex = 4;
			this.cancelButton.Text = "Cancel";
			this.cancelButton.UseVisualStyleBackColor = true;
			this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
			// 
			// RedirectToGitLabLoginForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(344, 141);
			this.Controls.Add(this.cancelButton);
			this.Controls.Add(this.continueButton);
			this.Controls.Add(this.dontAskAgainCheckBox);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MaximumSize = new System.Drawing.Size(360, 180);
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(360, 180);
			this.Name = "RedirectToGitLabLoginForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "GitLab Plugin Login";
			this.Load += new System.EventHandler(this.RedirectToGitLabLoginForm_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.CheckBox dontAskAgainCheckBox;
		private System.Windows.Forms.Button continueButton;
		private System.Windows.Forms.Button cancelButton;
	}
}