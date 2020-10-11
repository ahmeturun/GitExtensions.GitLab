namespace GitExtensions.GitLab
{
    public class GitLabLoginInfo
    {
        private static string username;

        public static string Username
        {
            get
            {
                if (username == "")
                {
                    return null;
                }

                if (username != null)
                {
                    return username;
                }

                try
                {
                    var user = GitLabPlugin.GitLab.getCurrentUser();
                    if (user != null)
                    {
                        username = user.Name;
                        return username;
                    }
                    else
                    {
                        username = "";
                    }

                    return null;
                }
                catch
                {
                    return null;
                }
            }
        }
    }
}
