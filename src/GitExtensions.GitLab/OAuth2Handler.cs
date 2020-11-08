namespace GitExtensions.GitLab
{
	using System;
	using System.IO;
	using System.Threading;
	using Microsoft.Win32;

	public class OAuth2Handler
	{
		public static void RegisterURIHandler()
		{
			var pluginAssemblylocation = System.Reflection.Assembly.GetAssembly(typeof(GitLabPlugin)).Location;
			var pluginDllFileInfo = new FileInfo(pluginAssemblylocation);
			var oAuthProcessLocation = Path.Combine(pluginDllFileInfo.Directory.FullName, "GitExtensions.OAuthProcesses.exe");
			EnsureKeyExists(Registry.CurrentUser, "Software/Classes/gitextensionsgitlab", "GitExtensions GitLab Plugin");
			SetValue(Registry.CurrentUser, "Software/Classes/gitextensionsgitlab", "URL Protocol", string.Empty);
			EnsureKeyExists(Registry.CurrentUser, "Software/Classes/gitextensionsgitlab/DefaultIcon", $"{oAuthProcessLocation},1");
			EnsureKeyExists(Registry.CurrentUser, "Software/Classes/gitextensionsgitlab/shell/open/command", $"\"{oAuthProcessLocation}\" \"%1\"");

			var ipcListener = new IPCListener();
			ipcListener.StartAsync(CancellationToken.None);
		}

		private static void SetValue(RegistryKey rootKey, string keys, string valueName, string value)
		{
			var key = EnsureKeyExists(rootKey, keys);
			key.SetValue(valueName, value);
		}

		private static RegistryKey EnsureKeyExists(RegistryKey rootKey, string keys, string defaultValue = null)
		{
			if (rootKey == null)
			{
				throw new Exception("Root key is (null)");
			}

			var currentKey = rootKey;
			foreach (var key in keys.Split('/'))
			{
				currentKey = currentKey.OpenSubKey(key, RegistryKeyPermissionCheck.ReadWriteSubTree)
							 ?? currentKey.CreateSubKey(key, RegistryKeyPermissionCheck.ReadWriteSubTree);

				if (currentKey == null)
				{
					throw new Exception("Could not get or create key");
				}
			}

			if (defaultValue != null)
			{
				currentKey.SetValue(string.Empty, defaultValue);
			}

			return currentKey;
		}
	}
}
