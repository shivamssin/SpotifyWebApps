using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SpotifyWebApp.Models;
using System.Text;
using System.Web;

namespace SpotifyWebApp.Services
{
    public class SpotifyService : ISpotifyService
    {
        private readonly SpotifyConfig _config;
        private readonly HttpClient _httpClient;
        private readonly ILogger<SpotifyService> _logger;
        private readonly IWebHostEnvironment _environment;

        public SpotifyService(
            IOptions<SpotifyConfig> config,
            HttpClient httpClient,
            ILogger<SpotifyService> logger,
            IWebHostEnvironment environment)
        {
            _config = config.Value;
            _httpClient = httpClient;
            _logger = logger;
            _environment = environment;

            // Validate configuration
            if (string.IsNullOrEmpty(_config.ClientId))
            {
                _logger.LogError("Spotify Client ID is not configured");
                throw new InvalidOperationException("Spotify Client ID is required");
            }

            if (string.IsNullOrEmpty(_config.ClientSecret))
            {
                _logger.LogError("Spotify Client Secret is not configured");
                throw new InvalidOperationException("Spotify Client Secret is required");
            }

            if (string.IsNullOrEmpty(_config.RedirectUri))
            {
                _logger.LogError("Spotify Redirect URI is not configured");
                throw new InvalidOperationException("Spotify Redirect URI is required");
            }
        }

        public string GetAuthorizationUrl()
        {
            try
            {
                var state = Guid.NewGuid().ToString();
                var queryParams = HttpUtility.ParseQueryString(string.Empty);

                queryParams.Add("client_id", _config.ClientId);
                queryParams.Add("response_type", "code");
                queryParams.Add("redirect_uri", _config.RedirectUri);
                queryParams.Add("scope", _config.Scopes);
                queryParams.Add("state", state);

                var authUrl = $"https://accounts.spotify.com/authorize?{queryParams}";
                _logger.LogInformation("Generated authorization URL for client_id: {ClientId}", _config.ClientId[..8] + "...");

                return authUrl;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating authorization URL");
                throw;
            }
        }

        public async Task<TokenResponse?> GetAccessTokenAsync(string code)
        {
            try
            {
                var tokenRequest = new List<KeyValuePair<string, string>>
                {
                    new("grant_type", "authorization_code"),
                    new("code", code),
                    new("redirect_uri", _config.RedirectUri),
                    new("client_id", _config.ClientId),
                    new("client_secret", _config.ClientSecret)
                };

                var content = new FormUrlEncodedContent(tokenRequest);

                _logger.LogInformation("Requesting access token from Spotify");
                var response = await _httpClient.PostAsync("https://accounts.spotify.com/api/token", content);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(json);
                    _logger.LogInformation("Successfully obtained access token");
                    return tokenResponse;
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError("Failed to get access token. Status: {StatusCode}, Error: {Error}",
                    response.StatusCode, errorContent);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting access token");
                return null;
            }
        }

        public async Task<ApiResponse<List<Track>>> GetTopTracksAsync(string accessToken)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");

                var response = await _httpClient.GetAsync("https://api.spotify.com/v1/me/top/tracks?limit=10&time_range=short_term");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var topTracks = JsonConvert.DeserializeObject<TopTracksResponse>(json);

                    return new ApiResponse<List<Track>>
                    {
                        Success = true,
                        Data = topTracks?.Items ?? new List<Track>()
                    };
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("Failed to get top tracks. Status: {StatusCode}, Error: {Error}",
                    response.StatusCode, errorContent);

                return new ApiResponse<List<Track>>
                {
                    Success = false,
                    ErrorMessage = $"API Error: {response.StatusCode}"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting top tracks");
                return new ApiResponse<List<Track>>
                {
                    Success = false,
                    ErrorMessage = "Failed to load top tracks"
                };
            }
        }

        public async Task<ApiResponse<CurrentlyPlayingResponse?>> GetCurrentlyPlayingAsync(string accessToken)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");

                var response = await _httpClient.GetAsync("https://api.spotify.com/v1/me/player/currently-playing");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    if (!string.IsNullOrEmpty(json))
                    {
                        var currentlyPlaying = JsonConvert.DeserializeObject<CurrentlyPlayingResponse>(json);
                        return new ApiResponse<CurrentlyPlayingResponse?>
                        {
                            Success = true,
                            Data = currentlyPlaying
                        };
                    }
                }

                // No content or error - return success with null data
                return new ApiResponse<CurrentlyPlayingResponse?>
                {
                    Success = true,
                    Data = null
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting currently playing");
                return new ApiResponse<CurrentlyPlayingResponse?>
                {
                    Success = false,
                    ErrorMessage = "Failed to load currently playing track"
                };
            }
        }

        public async Task<ApiResponse<List<Artist>>> GetFollowingArtistsAsync(string accessToken)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");

                var response = await _httpClient.GetAsync("https://api.spotify.com/v1/me/following?type=artist&limit=50");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var following = JsonConvert.DeserializeObject<FollowingArtistsResponse>(json);

                    return new ApiResponse<List<Artist>>
                    {
                        Success = true,
                        Data = following?.Artists?.Items ?? new List<Artist>()
                    };
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("Failed to get following artists. Status: {StatusCode}, Error: {Error}",
                    response.StatusCode, errorContent);

                return new ApiResponse<List<Artist>>
                {
                    Success = false,
                    ErrorMessage = $"API Error: {response.StatusCode}"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting following artists");
                return new ApiResponse<List<Artist>>
                {
                    Success = false,
                    ErrorMessage = "Failed to load following artists"
                };
            }
        }

        public async Task<ApiResponse<bool>> StartPlaybackAsync(string accessToken, string trackUri)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");

                var requestBody = new { uris = new[] { trackUri } };
                var json = JsonConvert.SerializeObject(requestBody);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync("https://api.spotify.com/v1/me/player/play", content);

                if (response.IsSuccessStatusCode)
                {
                    return new ApiResponse<bool>
                    {
                        Success = true,
                        Data = true
                    };
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("Failed to start playback. Status: {StatusCode}, Error: {Error}",
                    response.StatusCode, errorContent);

                return new ApiResponse<bool>
                {
                    Success = false,
                    ErrorMessage = response.StatusCode == System.Net.HttpStatusCode.NotFound
                        ? "No active device found. Please open Spotify on a device first."
                        : $"Playback error: {response.StatusCode}"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error starting playback");
                return new ApiResponse<bool>
                {
                    Success = false,
                    ErrorMessage = "Failed to start playback"
                };
            }
        }

        public async Task<ApiResponse<bool>> StopPlaybackAsync(string accessToken)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");

                var response = await _httpClient.PutAsync("https://api.spotify.com/v1/me/player/pause", null);

                if (response.IsSuccessStatusCode)
                {
                    return new ApiResponse<bool>
                    {
                        Success = true,
                        Data = true
                    };
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("Failed to stop playback. Status: {StatusCode}, Error: {Error}",
                    response.StatusCode, errorContent);

                return new ApiResponse<bool>
                {
                    Success = false,
                    ErrorMessage = response.StatusCode == System.Net.HttpStatusCode.NotFound
                        ? "No active device found"
                        : $"Playback error: {response.StatusCode}"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error stopping playback");
                return new ApiResponse<bool>
                {
                    Success = false,
                    ErrorMessage = "Failed to stop playback"
                };
            }
        }
    }
}