using Microsoft.AspNetCore.Mvc;
using SpotifyWebApp.Services;

namespace SpotifyWebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SpotifyController : ControllerBase
    {
        private readonly ISpotifyService _spotifyService;

        public SpotifyController(ISpotifyService spotifyService)
        {
            _spotifyService = spotifyService;
        }

        [HttpGet("top-tracks")]
        public async Task<IActionResult> GetTopTracks()
        {
            var accessToken = HttpContext.Session.GetString("SpotifyAccessToken");
            if (string.IsNullOrEmpty(accessToken))
            {
                return Unauthorized(new { error = "Not authenticated with Spotify" });
            }

            var result = await _spotifyService.GetTopTracksAsync(accessToken);

            if (result.Success)
            {
                return Ok(new { data = result.Data });
            }

            return BadRequest(new { error = result.ErrorMessage });
        }

        [HttpGet("now-playing")]
        public async Task<IActionResult> GetNowPlaying()
        {
            var accessToken = HttpContext.Session.GetString("SpotifyAccessToken");
            if (string.IsNullOrEmpty(accessToken))
            {
                return Unauthorized(new { error = "Not authenticated with Spotify" });
            }

            var result = await _spotifyService.GetCurrentlyPlayingAsync(accessToken);

            if (result.Success)
            {
                return Ok(new { data = result.Data });
            }

            return BadRequest(new { error = result.ErrorMessage });
        }

        [HttpGet("following-artists")]
        public async Task<IActionResult> GetFollowingArtists()
        {
            var accessToken = HttpContext.Session.GetString("SpotifyAccessToken");
            if (string.IsNullOrEmpty(accessToken))
            {
                return Unauthorized(new { error = "Not authenticated with Spotify" });
            }

            var result = await _spotifyService.GetFollowingArtistsAsync(accessToken);

            if (result.Success)
            {
                return Ok(new { data = result.Data });
            }

            return BadRequest(new { error = result.ErrorMessage });
        }

        [HttpPost("play")]
        public async Task<IActionResult> StartPlayback([FromBody] PlayTrackRequest request)
        {
            var accessToken = HttpContext.Session.GetString("SpotifyAccessToken");
            if (string.IsNullOrEmpty(accessToken))
            {
                return Unauthorized(new { error = "Not authenticated with Spotify" });
            }

            var result = await _spotifyService.StartPlaybackAsync(accessToken, request.TrackUri);

            if (result.Success)
            {
                return Ok(new { success = true });
            }

            return BadRequest(new { error = result.ErrorMessage });
        }

        [HttpPost("stop")]
        public async Task<IActionResult> StopPlayback()
        {
            var accessToken = HttpContext.Session.GetString("SpotifyAccessToken");
            if (string.IsNullOrEmpty(accessToken))
            {
                return Unauthorized(new { error = "Not authenticated with Spotify" });
            }

            var result = await _spotifyService.StopPlaybackAsync(accessToken);

            if (result.Success)
            {
                return Ok(new { success = true });
            }

            return BadRequest(new { error = result.ErrorMessage });
        }
    }

    public class PlayTrackRequest
    {
        public string TrackUri { get; set; } = string.Empty;
    }
}
