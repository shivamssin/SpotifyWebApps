using SpotifyWebApp.Models;
using SpotifyWebApp.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient<ISpotifyService, SpotifyService>();
builder.Services.AddScoped<ISpotifyService, SpotifyService>();

// Configure Spotify settings with environment variables (PRODUCTION READY)
builder.Services.Configure<SpotifyConfig>(options =>
{
    // Always prioritize environment variables over appsettings
    options.ClientId = Environment.GetEnvironmentVariable("SPOTIFY_CLIENT_ID") ?? "";
    options.ClientSecret = Environment.GetEnvironmentVariable("SPOTIFY_CLIENT_SECRET") ?? "";
    options.RedirectUri = Environment.GetEnvironmentVariable("SPOTIFY_REDIRECT_URI") ?? "";
    options.Scopes = "user-read-private user-read-email user-top-read user-read-playback-state user-modify-playback-state user-read-currently-playing user-follow-read";

    // Log configuration status (without revealing sensitive data)
    var logger = LoggerFactory.Create(config => config.AddConsole()).CreateLogger("Startup");
    logger.LogInformation("Spotify Config - ClientId: {HasClientId}, RedirectUri: {RedirectUri}",
        !string.IsNullOrEmpty(options.ClientId) ? "SET" : "MISSING",
        options.RedirectUri);
});

// Add session support with production settings
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SecurePolicy = builder.Environment.IsDevelopment()
        ? CookieSecurePolicy.SameAsRequest
        : CookieSecurePolicy.Always; // Force HTTPS in production
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.Cookie.Name = "SpotifyApp.Session";
});

// Add security headers and HSTS for production
if (!builder.Environment.IsDevelopment())
{
    builder.Services.AddHsts(options =>
    {
        options.Preload = true;
        options.IncludeSubDomains = true;
        options.MaxAge = TimeSpan.FromDays(365);
    });
}

// Configure Kestrel for production (if needed)
if (!builder.Environment.IsDevelopment())
{
    builder.WebHost.ConfigureKestrel(options =>
    {
        // Configure for cloud deployment
        var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
        options.ListenAnyIP(int.Parse(port));
    });
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();

    // Add security headers
    app.Use(async (context, next) =>
    {
        context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
        context.Response.Headers.Append("X-Frame-Options", "DENY");
        context.Response.Headers.Append("X-XSS-Protection", "1; mode=block");
        context.Response.Headers.Append("Referrer-Policy", "strict-origin-when-cross-origin");
        await next();
    });
}

// Force HTTPS redirect in production
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Health check endpoint for deployment monitoring
app.MapGet("/health", () => new {
    status = "healthy",
    timestamp = DateTime.UtcNow,
    environment = app.Environment.EnvironmentName
});

// Configuration check endpoint (for debugging deployment issues)
app.MapGet("/config-check", (IConfiguration config) =>
{
    if (app.Environment.IsDevelopment())
    {
        return Results.Json(new
        {
            hasClientId = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("SPOTIFY_CLIENT_ID")),
            hasClientSecret = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("SPOTIFY_CLIENT_SECRET")),
            redirectUri = Environment.GetEnvironmentVariable("SPOTIFY_REDIRECT_URI"),
            environment = app.Environment.EnvironmentName
        });
    }
    return Results.NotFound();
});

app.Run();