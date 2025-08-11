using Newtonsoft.Json;

namespace SpotifyWebApp.Models
{
    public class Track
    {
        [JsonProperty("id")]
        public string Id { get; set; } = string.Empty;

        [JsonProperty("name")]
        public string Name { get; set; } = string.Empty;

        [JsonProperty("artists")]
        public List<Artist> Artists { get; set; } = new();

        [JsonProperty("album")]
        public Album Album { get; set; } = new();

        [JsonProperty("uri")]
        public string Uri { get; set; } = string.Empty;

        [JsonProperty("external_urls")]
        public ExternalUrls ExternalUrls { get; set; } = new();
    }
}
