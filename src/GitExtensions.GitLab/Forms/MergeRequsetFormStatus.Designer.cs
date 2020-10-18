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
			this.Ok = new System.Windows.Forms.Button();
			this.pnlOutput = new System.Windows.Forms.Panel();
			this.MainPanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// MainPanel
			// 
			this.MainPanel.Controls.Add(this.pnlOutput);
			this.MainPanel.Controls.Add(this.Ok);
			this.MainPanel.Padding = new System.Windows.Forms.Padding(0);
			this.MainPanel.Size = new System.Drawing.Size(549, 43);
			// 
			// Ok
			// 
			this.Ok.Location = new System.Drawing.Point(314, 234);
			this.Ok.Name = "Ok";
			this.Ok.Size = new System.Drawing.Size(118, 25);
			this.Ok.TabIndex = 0;
			this.Ok.Text = "OK";
			this.Ok.UseCompatibleTextRendering = true;
			this.Ok.UseVisualStyleBackColor = true;
			this.Ok.Click += new System.EventHandler(this.Ok_Click);
			// 
			// pnlOutput
			// 
			this.pnlOutput.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pnlOutput.Location = new System.Drawing.Point(0, 0);
			this.pnlOutput.Name = "pnlOutput";
			this.pnlOutput.Padding = new System.Windows.Forms.Padding(27);
			this.pnlOutput.Size = new System.Drawing.Size(549, 43);
			this.pnlOutput.TabIndex = 0;
			// 
			// MergeRequsetFormStatus
			// 
			this.AcceptButton = this.Ok;
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.ClientSize = new System.Drawing.Size(549, 88);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(500, 100);
			this.Name = "MergeRequsetFormStatus";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Process";
			this.MainPanel.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		protected System.Windows.Forms.Button Ok;
		protected System.Windows.Forms.Panel pnlOutput;
	}
}