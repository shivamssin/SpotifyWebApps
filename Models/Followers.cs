using Newtonsoft.Json;

namespace SpotifyWebApp.Models
{
    public class Followers
    {
        [JsonProperty("total")]
        public int Total { get; set; }
    }
}
