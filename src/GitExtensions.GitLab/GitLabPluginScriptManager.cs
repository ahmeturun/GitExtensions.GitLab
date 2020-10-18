namespace GitExtensions.GitLab
{
    using System.ComponentModel;
    
    public class GitLabPluginScriptManager
    {
        private static BindingList<GitUI.Script.ScriptInfo> GitExtScriptList = new BindingList<GitUI.Script.ScriptInfo> { };
        private static readonly BindingList<GitUI.Script.ScriptInfo> GitLabPluginScriptList = new BindingList<GitUI.Script.ScriptInfo> { };
        static readonly private int FirstHotkeyCommandIdentifier = 10000; // Arbitrary choosen. GE by default starts at 9000 for it's own scripts.
        
        public static void AddNew(string name,
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
            GitExtScriptList = GitUI.Script.ScriptManager.GetScripts();

            GitUI.Script.ScriptInfo newScript = GitExtScriptList.AddNew();
            newScript.Enabled = enabled;
            newScript.Name = name;
            newScript.Command = command;
            newScript.Arguments = arguments;
            newScript.AddToRevisionGridContextMenu = addToRevisionGridContextMenu;
            newScript.OnEvent = onEvent;
            newScript.AskConfirmation = askConfirmation;
            newScript.RunInBackground = runInBackground;
            newScript.IsPowerShell = isPowerShell;
            newScript.HotkeyCommandIdentifier = FirstHotkeyCommandIdentifier + GitExtScriptList.Count;
            newScript.Icon = icon;

            GitLabPluginScriptList.Add(newScript);
        }

        public static void RemoveAll()
        {
            GitExtScriptList = GitUI.Script.ScriptManager.GetScripts();

            foreach (var script in GitLabPluginScriptList)
            {
                GitExtScriptList.Remove(script);
            }

            GitLabPluginScriptList.Clear();
        }
    }
}
