using Microsoft.AspNetCore.Mvc;
using SpotifyWebApp.Services;

namespace SpotifyWebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ISpotifyService _spotifyService;

        public HomeController(ISpotifyService spotifyService)
        {
            _spotifyService = spotifyService;
        }

        public IActionResult Index()
        {
            var accessToken = HttpContext.Session.GetString("SpotifyAccessToken");
            ViewBag.IsAuthenticated = !string.IsNullOrEmpty(accessToken);
            return View();
        }

        public IActionResult Login()
        {
            var authUrl = _spotifyService.GetAuthorizationUrl();
            return Redirect(authUrl);
        }

        [Route("callback")]
        public async Task<IActionResult> Callback(string code, string state, string error)
        {
            if (!string.IsNullOrEmpty(error))
            {
                ViewBag.Error = error;
                return View("Error");
            }

            if (string.IsNullOrEmpty(code))
            {
                ViewBag.Error = "Authorization code not received";
                return View("Error");
            }

            var tokenResponse = await _spotifyService.GetAccessTokenAsync(code);

            if (tokenResponse != null)
            {
                HttpContext.Session.SetString("SpotifyAccessToken", tokenResponse.AccessToken);
                return RedirectToAction("Index");
            }

            ViewBag.Error = "Failed to get access token";
            return View("Error");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Remove("SpotifyAccessToken");
            return RedirectToAction("Index");
        }
    }
}
