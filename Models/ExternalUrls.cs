using Newtonsoft.Json;

namespace SpotifyWebApp.Models
{
    public class ExternalUrls
    {
        [JsonProperty("spotify")]
        public string Spotify { get; set; } = string.Empty;
    }
}
