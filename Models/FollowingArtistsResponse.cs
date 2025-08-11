using Newtonsoft.Json;

namespace SpotifyWebApp.Models
{
    public class FollowingArtistsResponse
    {
        [JsonProperty("artists")]
        public ArtistsPaged Artists { get; set; } = new();
    }
}
