namespace GitExtensions.GitLab.Forms
{
	partial class LoginForm
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
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.loginButton = new System.Windows.Forms.Button();
			this.passwordLabel = new System.Windows.Forms.Label();
			this.usernameLabel = new System.Windows.Forms.Label();
			this.passwordTextbox = new System.Windows.Forms.TextBox();
			this.usernameTextBox = new System.Windows.Forms.TextBox();
			this.loadingControl1 = new GitUI.UserControls.RevisionGrid.LoadingControl();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.loadingControl1);
			this.groupBox1.Controls.Add(this.loginButton);
			this.groupBox1.Controls.Add(this.passwordLabel);
			this.groupBox1.Controls.Add(this.usernameLabel);
			this.groupBox1.Controls.Add(this.passwordTextbox);
			this.groupBox1.Controls.Add(this.usernameTextBox);
			this.groupBox1.Location = new System.Drawing.Point(12, 12);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(265, 168);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			// 
			// loginButton
			// 
			this.loginButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.loginButton.Location = new System.Drawing.Point(6, 129);
			this.loginButton.Name = "loginButton";
			this.loginButton.Size = new System.Drawing.Size(251, 33);
			this.loginButton.TabIndex = 4;
			this.loginButton.Text = "Login";
			this.loginButton.UseVisualStyleBackColor = true;
			this.loginButton.Click += new System.EventHandler(this.loginButtonClick);
			// 
			// passwordLabel
			// 
			this.passwordLabel.AutoSize = true;
			this.passwordLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.passwordLabel.Location = new System.Drawing.Point(6, 67);
			this.passwordLabel.Name = "passwordLabel";
			this.passwordLabel.Size = new System.Drawing.Size(73, 17);
			this.passwordLabel.TabIndex = 3;
			this.passwordLabel.Text = "Password:";
			// 
			// usernameLabel
			// 
			this.usernameLabel.AutoSize = true;
			this.usernameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.usernameLabel.Location = new System.Drawing.Point(6, 11);
			this.usernameLabel.Name = "usernameLabel";
			this.usernameLabel.Size = new System.Drawing.Size(77, 17);
			this.usernameLabel.TabIndex = 2;
			this.usernameLabel.Text = "Username:";
			// 
			// passwordTextbox
			// 
			this.passwordTextbox.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.passwordTextbox.Location = new System.Drawing.Point(6, 87);
			this.passwordTextbox.Name = "passwordTextbox";
			this.passwordTextbox.Size = new System.Drawing.Size(251, 23);
			this.passwordTextbox.TabIndex = 1;
			// 
			// usernameTextBox
			// 
			this.usernameTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.usernameTextBox.Location = new System.Drawing.Point(6, 31);
			this.usernameTextBox.Name = "usernameTextBox";
			this.usernameTextBox.Size = new System.Drawing.Size(251, 23);
			this.usernameTextBox.TabIndex = 0;
			// 
			// loadingControl1
			// 
			this.loadingControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.loadingControl1.IsAnimating = true;
			this.loadingControl1.Location = new System.Drawing.Point(3, 16);
			this.loadingControl1.Name = "loadingControl1";
			this.loadingControl1.Size = new System.Drawing.Size(259, 149);
			this.loadingControl1.TabIndex = 5;
			this.loadingControl1.Visible = false;
			// 
			// LoginForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(289, 192);
			this.Controls.Add(this.groupBox1);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "LoginForm";
			this.Text = "GitLab Login";
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Button loginButton;
		private System.Windows.Forms.Label passwordLabel;
		private System.Windows.Forms.Label usernameLabel;
		private System.Windows.Forms.TextBox passwordTextbox;
		private System.Windows.Forms.TextBox usernameTextBox;
		private GitUI.UserControls.RevisionGrid.LoadingControl loadingControl1;
	}
}