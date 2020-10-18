using GitExtensions.GitLab.Client.Repo;
using GitUIPluginInterfaces.RepositoryHosts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitExtensions.GitLab.Remote
{
    /*
    {
        "id": 685,
        "iid": 16,
        "project_id": 53,
        "title": "Convert SA1200 StyleCop rule from warning to error",
        "description": "",
        "state": "merged",
        "created_at": "2020-09-14T09:01:15.948Z",
        "updated_at": "2020-09-15T04:47:06.637Z",
        "merged_by": {
            "id": 24,
            "name": "Aleksandr Nekrasov",
            "username": "aleksandr.nekrasov",
            "state": "active",
            "avatar_url": "https://secure.gravatar.com/avatar/9e6679913d7f9864db19e1ce081a3950?s=80&d=identicon",
            "web_url": "https://gitlab.piworks.net/aleksandr.nekrasov"
        },
        "merged_at": "2020-09-15T04:47:06.680Z",
        "closed_by": null,
        "closed_at": null,
        "target_branch": "master",
        "source_branch": "make-sa1200-error",
        "user_notes_count": 0,
        "upvotes": 0,
        "downvotes": 0,
        "author": {
            "id": 7,
            "name": "Ismail Arilik",
            "username": "ismailarilik",
            "state": "active",
            "avatar_url": "https://gitlab.piworks.net/uploads/-/system/user/avatar/7/avatar.png",
            "web_url": "https://gitlab.piworks.net/ismailarilik"
        },
        "assignees": [
            {
                "id": 24,
                "name": "Aleksandr Nekrasov",
                "username": "aleksandr.nekrasov",
                "state": "active",
                "avatar_url": "https://secure.gravatar.com/avatar/9e6679913d7f9864db19e1ce081a3950?s=80&d=identicon",
                "web_url": "https://gitlab.piworks.net/aleksandr.nekrasov"
            }
        ],
        "assignee": {
            "id": 24,
            "name": "Aleksandr Nekrasov",
            "username": "aleksandr.nekrasov",
            "state": "active",
            "avatar_url": "https://secure.gravatar.com/avatar/9e6679913d7f9864db19e1ce081a3950?s=80&d=identicon",
            "web_url": "https://gitlab.piworks.net/aleksandr.nekrasov"
        },
        "source_project_id": 53,
        "target_project_id": 53,
        "labels": [],
        "work_in_progress": false,
        "milestone": null,
        "merge_when_pipeline_succeeds": false,
        "merge_status": "can_be_merged",
        "sha": "9aa3f43e0b8786e8b11bb33bca71542ee255500e",
        "merge_commit_sha": "8a1113cf6e66128755eda6c9fd913bd29aaf06f4",
        "squash_commit_sha": null,
        "discussion_locked": null,
        "should_remove_source_branch": null,
        "force_remove_source_branch": true,
        "reference": "!16",
        "references": {
            "short": "!16",
            "relative": "!16",
            "full": "pi-product/cm-service!16"
        },
        "web_url": "https://gitlab.piworks.net/pi-product/cm-service/-/merge_requests/16",
        "time_stats": {
            "time_estimate": 0,
            "total_time_spent": 0,
            "human_time_estimate": null,
            "human_total_time_spent": null
        },
        "squash": true,
        "task_completion_status": {
            "count": 0,
            "completed_count": 0
        },
        "has_conflicts": false,
        "blocking_discussions_resolved": true,
        "approvals_before_merge": null
    }
    */
    public class GitLabMergeRequest : IPullRequestInformation
    {
        private readonly MergeRequest mergeRequest;
        public GitLabMergeRequest(MergeRequest mergeRequest)
        {
            this.mergeRequest = mergeRequest;
        }

        public string Title => mergeRequest.Title;

        public string Body => mergeRequest.Description;

        public string Owner => mergeRequest.Author.Name;

        public DateTime Created => mergeRequest.CreatedAt;

        public IHostedRepository BaseRepo => throw new NotImplementedException();

        public IHostedRepository HeadRepo => throw new NotImplementedException();

        public string BaseSha => mergeRequest.Sha;

        public string HeadSha => mergeRequest.MergeCommitSha;

        public string BaseRef => throw new NotImplementedException();

        public string HeadRef => throw new NotImplementedException();

        public string Id => mergeRequest.InternalId.ToString();

        public string DetailedInfo => throw new NotImplementedException();

        public string FetchBranch => throw new NotImplementedException();

        public void Close()
        {
            mergeRequest.Close();
        }

        public Task<string> GetDiffDataAsync()
        {
            throw new NotImplementedException();
        }

        public IPullRequestDiscussion GetDiscussion()
        {
            throw new NotImplementedException();
        }
    }
}
