using Newtonsoft.Json;

namespace SpotifyWebApp.Models
{
    public class ArtistsPaged
    {
        [JsonProperty("items")]
        public List<Artist> Items { get; set; } = new();

        [JsonProperty("total")]
        public int Total { get; set; }
    }
}
