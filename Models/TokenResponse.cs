﻿using Newtonsoft.Json;

namespace SpotifyWebApp.Models
{
    public class TokenResponse
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; } = string.Empty;

        [JsonProperty("token_type")]
        public string TokenType { get; set; } = string.Empty;

        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }

        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; } = string.Empty;
    }
}
