namespace GitExtensions.GitLab
{
    using GitUIPluginInterfaces;
    using ResourceManager;
    using System;
    using System.ComponentModel.Composition;

    [Export(typeof(IGitPlugin))]
    public class GitLabManagePullRequests : GitPluginBase
    {
        public static string GitLabManageMRDescription = "gitlabmanagemergerequest";
        public GitLabManagePullRequests() : base(false)
        {
            SetNameAndDescription(GitLabManageMRDescription);
        }

        public override bool Execute(GitUIEventArgs args)
        {
            // open form for viewing & managing pull requests
            throw new NotImplementedException();
        }
    }
}
