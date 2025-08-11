# Use the official .NET 8 runtime as base image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

# Use the .NET 8 SDK for building
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["SpotifyWebApp.csproj", "."]
RUN dotnet restore "./SpotifyWebApp.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "SpotifyWebApp.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SpotifyWebApp.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Create a non-root user
RUN adduser --disabled-password --gecos '' appuser && chown -R appuser /app
USER appuser

ENTRYPOINT ["dotnet", "SpotifyWebApp.dll"]