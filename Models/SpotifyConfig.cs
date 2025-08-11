namespace SpotifyWebApp.Models
{
    public class SpotifyConfig
    {
        public string ClientId { get; set; } = string.Empty;
        public string ClientSecret { get; set; } = string.Empty;
        public string RedirectUri { get; set; } = string.Empty;
        public string Scopes { get; set; } = string.Empty;
    }
}
