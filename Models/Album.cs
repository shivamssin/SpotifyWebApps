using Newtonsoft.Json;

namespace SpotifyWebApp.Models
{
    public class Album
    {
        [JsonProperty("name")]
        public string Name { get; set; } = string.Empty;

        [JsonProperty("images")]
        public List<SpotifyImage> Images { get; set; } = new();
    }
}
