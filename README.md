# Spotify Web App (.NET MVC)

A .NET 8 MVC application that integrates with the Spotify Web API to display your personal Spotify data and control playback.

## Features

- **Top 10 Tracks** – View your most played tracks and start playback of any track*.
- **Now Playing** – Display the currently playing song with an option to stop playback*
- **Followed Artists** – List all artists you follow on Spotify.
- **Playback Control** – Start or stop songs directly from the app*.
  
  **(*Playback Control requires Spotify Premium)**
## Live Demo
Hosted on Render: **[https://spotifywebapps.onrender.com](https://spotifywebapps.onrender.com)**

> ⚠️ Requires you to log in with your own Spotify account to access personal data.

## Tech Stack

- **Backend:** .NET 8 MVC
- **Frontend:** Razor Views + Vanilla JavaScript
- **API:** Spotify Web API
- **Hosting:** [Render](https://render.com)

## Prerequisites

1. **Spotify Developer Account** – Create one at [developer.spotify.com](https://developer.spotify.com).
2. **Spotify App** – Create an app in the Spotify Dashboard and note:
   - **Client ID**
   - **Client Secret**
   - **Add Redirect URI:**  

## Notes
This app uses the Authorization Code Flow for secure user authentication.
Make sure your redirect URI matches exactly in both the Spotify dashboard and your code.
