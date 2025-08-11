using Newtonsoft.Json;

namespace SpotifyWebApp.Models
{
    public class SpotifyImage
    {
        [JsonProperty("url")]
        public string Url { get; set; } = string.Empty;

        [JsonProperty("height")]
        public int Height { get; set; }

        [JsonProperty("width")]
        public int Width { get; set; }
    }
}
