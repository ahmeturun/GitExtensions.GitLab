namespace GitExtensions.GitLab
{
	using GitExtensions.GitLab.Properties;
	using GitUI.CommandsDialogs;
	using GitUI.Properties;
	using System.Linq;
	using System.Reflection;
	using System.Windows.Forms;

	public class GitLabPluginScriptManager
	{
		private static readonly GitLabPluginScript CreateMergeRequestPluginScript =
			new GitLabPluginScript("Create Merge Request", $"plugin:{GitLabCreateMergeRequest.GitLabCreateMRDescription}");

		static readonly private int FirstHotkeyCommandIdentifier = 10000; // Arbitrary choosen. GE by default starts at 9000 for it's own scripts.

		public static void Initialize()
		{
			Clean();
			AddNew(CreateMergeRequestPluginScript.Name, string.Empty, nameof(Images.RemoteAdd), command: CreateMergeRequestPluginScript.Command, onEvent: GitUI.Script.ScriptEvent.AfterPush, addToRevisionGridContextMenu: false);
			AddNew(CreateMergeRequestPluginScript.Name, string.Empty, nameof(Images.RemoteAdd), command: CreateMergeRequestPluginScript.Command, onEvent: GitUI.Script.ScriptEvent.ShowInUserMenuBar);
		}

		public static void Clean()
		{
			Remove(CreateMergeRequestPluginScript);
		}

		public static void CleanPluginToolstripMenuItems(IWin32Window ownerForm)
		{
			if(ownerForm is FormBrowse formBrowse)
			{
				FieldInfo fi = typeof(FormBrowse).GetField("pluginsToolStripMenuItem", BindingFlags.NonPublic | BindingFlags.Instance);
				if (fi.GetValue(formBrowse) is ToolStripMenuItem toolStripMenuItem && toolStripMenuItem != null)
				{
					foreach (ToolStripMenuItem item in toolStripMenuItem.DropDownItems)
					{
						if(item.Text == GitLabCreateMergeRequest.GitLabCreateMRDescription)
						{
							toolStripMenuItem.DropDownItems.Remove(item);
							break;
						}
					}
				}
			}
		}

		private static void Remove(GitLabPluginScript gitLabPluginScript)
		{
			var gitExtScriptList = GitUI.Script.ScriptManager.GetScripts();
			var currentRegisteredPluginScript = gitExtScriptList
				.FirstOrDefault(item => item.Name == gitLabPluginScript.Name 
					&& item.Command == gitLabPluginScript.Command);
			while (currentRegisteredPluginScript != null)
			{
				gitExtScriptList.Remove(currentRegisteredPluginScript);
				currentRegisteredPluginScript = gitExtScriptList
				.FirstOrDefault(item => item.Name == gitLabPluginScript.Name
					&& item.Command == gitLabPluginScript.Command);
			}
		}

		private static void AddNew(
			string name,
			string arguments,
			string icon,
			bool enabled = true,
			string command = "plugin:",
			bool addToRevisionGridContextMenu = true,
			GitUI.Script.ScriptEvent onEvent = GitUI.Script.ScriptEvent.ShowInUserMenuBar,
			bool askConfirmation = false,
			bool runInBackground = false,
			bool isPowerShell = false)
		{
			var gitExtScriptList = GitUI.Script.ScriptManager.GetScripts();

			GitUI.Script.ScriptInfo newScript = gitExtScriptList.AddNew();
			newScript.Enabled = enabled;
			newScript.Name = name;
			newScript.Command = command;
			newScript.Arguments = arguments;
			newScript.AddToRevisionGridContextMenu = addToRevisionGridContextMenu;
			newScript.OnEvent = onEvent;
			newScript.AskConfirmation = askConfirmation;
			newScript.RunInBackground = runInBackground;
			newScript.IsPowerShell = isPowerShell;
			newScript.Icon = icon;
			newScript.HotkeyCommandIdentifier = FirstHotkeyCommandIdentifier + gitExtScriptList.Count;
		}
	}
}
