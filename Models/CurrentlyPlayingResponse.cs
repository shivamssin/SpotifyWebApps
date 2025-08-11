using Newtonsoft.Json;

namespace SpotifyWebApp.Models
{
    public class CurrentlyPlayingResponse
    {
        [JsonProperty("is_playing")]
        public bool IsPlaying { get; set; }

        [JsonProperty("item")]
        public Track? Item { get; set; }

        [JsonProperty("progress_ms")]
        public int ProgressMs { get; set; }
    }
}
