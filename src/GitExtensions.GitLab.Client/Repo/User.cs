using Newtonsoft.Json;

namespace GitExtensions.GitLab.Client.Repo
{
    public class User
    {
        /// <summary>
        /// The GitLab username
        /// </summary>
        public string Id { get; set; }
        public string Name { get; set; }
        public string Username { get; set; }
        public string State { get; set; }

        [JsonProperty("avatar_url")]
        public string AvatarUrl { get; set; }

        [JsonProperty("web_url")]
        public string WebUrl { get; set; }
        public string State { get; set; }
        public User()
        {
        }

        public override bool Equals(object obj)
        {
            return GetHashCode() == obj.GetHashCode();
        }

        public override int GetHashCode()
        {
            return GetType().GetHashCode() + Id.GetHashCode();
        }

        public override string ToString()
        {
            return Id;
        }

    }
}

