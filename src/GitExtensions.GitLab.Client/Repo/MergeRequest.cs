﻿namespace GitExtensions.GitLab.Client.Repo
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using RestSharp;


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
    public class MergeRequest
    {
        [JsonProperty("project_id")]
        public long ProjectId { get; set; }

        [JsonProperty("id")]
        public object Id { get; set; }

        [JsonProperty("iid")]
        public long InternalId { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonProperty("updated_at")]
        public DateTime UpdatedAt { get; set; }

        [JsonProperty("web_url")]
        public string WebUrl { get; set; }

        [JsonProperty("author")]
        public User Author { get; set; }

        [JsonProperty("assignee")]
        public User Assignee { get; set; }

        [JsonProperty("assignee_id")]
        public int AssigneeId { get; set; }

        [JsonProperty("target_branch")]
        public string TargetBranch { get; set; }

        [JsonProperty("source_branch")]
        public string SourceBranch { get; set; }

        [JsonProperty("target_project_id")]
        public int TargetProjectId { get; set; }

        [JsonProperty("source_project_id")]
        public int SourceProjectId { get; set; }

        [JsonProperty("remove_source_branch")]
        public bool RemoveSourceBranch { get; set; }

        [JsonProperty("squash")]
        public bool Squash { get; set; }

        [JsonProperty("sha")]
        public string Sha { get; set; }

        [JsonProperty("merge_commit_sha")]
        public string MergeCommitSha { get; set; }

        internal IRestClient client;

        /// <summary>
        /// Retrieves all Commits associated with this merge request.
        /// </summary>
        /// <returns></returns>
        public List<PullRequestCommit> GetCommits()
        {
            // https://gitlab.com/api/v4/projects/ahmeturun%2Ftestproj/merge_requests/1/commits?private_token=<token>
            var request = new RestRequest("projects/{projectId}/merge_requests/{mergeRequest}/commits");
            request.AddUrlSegment("projectId", ProjectId);
            request.AddUrlSegment("mergeRequest", InternalId);

            return client.GetList<PullRequestCommit>(request);
        }

        public bool Open()
        {
            return UpdateState("open");
        }

        public bool Close()
        {
            return UpdateState("closed");
        }

        private bool UpdateState(string state)
        {
            // https://gitlab.com/api/v4/projects/ahmeturun%2Ftestproj/merge_requests/1/commits?private_token=<token>
            ///projects/:id/merge_requests/:merge_request_iid
            var request = new RestRequest("projects/{projectId}/merge_requests/{mergeRequestId}");
            request.AddUrlSegment("projectId", ProjectId);
            request.AddUrlSegment("mergeRequestId", InternalId);

            request.RequestFormat = DataFormat.Json;
            request.AddJsonBody(new
            {
                state_event = state
            });
            return client.Patch(request).StatusCode == System.Net.HttpStatusCode.OK;
        }
    }
}
