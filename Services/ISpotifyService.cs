using SpotifyWebApp.Models;

namespace SpotifyWebApp.Services
{
    public interface ISpotifyService
    {
        string GetAuthorizationUrl();
        Task<TokenResponse?> GetAccessTokenAsync(string code);
        Task<ApiResponse<List<Track>>> GetTopTracksAsync(string accessToken);
        Task<ApiResponse<CurrentlyPlayingResponse?>> GetCurrentlyPlayingAsync(string accessToken);
        Task<ApiResponse<List<Artist>>> GetFollowingArtistsAsync(string accessToken);
        Task<ApiResponse<bool>> StartPlaybackAsync(string accessToken, string trackUri);
        Task<ApiResponse<bool>> StopPlaybackAsync(string accessToken);
    }
}
