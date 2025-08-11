

namespace SpotifyWebApp.Models
{
    using Newtonsoft.Json;
    using SpotifyAPI.Web;
    
    public class TopTracksResponse
    {
        [JsonProperty("items")]
        public List<Track> Items { get; set; } = new();
    }
}
