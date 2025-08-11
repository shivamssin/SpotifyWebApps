using Newtonsoft.Json;
using SpotifyAPI.Web;

namespace SpotifyWebApp.Models
{
    public class Artist
    {
        [JsonProperty("id")]
        public string Id { get; set; } = string.Empty;

        [JsonProperty("name")]
        public string Name { get; set; } = string.Empty;

        [JsonProperty("followers")]
        public Followers? Followers { get; set; }

        [JsonProperty("images")]
        public List<SpotifyImage> Images { get; set; } = new();

        [JsonProperty("external_urls")]
        public ExternalUrls ExternalUrls { get; set; } = new();
    }
}
