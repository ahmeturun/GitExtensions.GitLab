namespace GitExtensions.GitLab.Remote
{
    using GitExtensions.GitLab.Client.Repo;
    using GitUIPluginInterfaces;
    using GitUIPluginInterfaces.RepositoryHosts;

    public class GitLabBranch : IHostedBranch
    {
        public GitLabBranch(Branch branch)
        {
            Name = branch.Name;
            Sha = ObjectId.Parse(branch.Commit.Sha);
        }
        public string Name { get; }

        public ObjectId Sha { get; }
    }
}
