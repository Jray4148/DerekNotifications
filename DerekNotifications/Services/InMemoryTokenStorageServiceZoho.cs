using DerekNotifications.Interfaces;
using DerekNotifications.Models;
using Microsoft.Extensions.Options;

namespace DerekNotifications.Services;

public class InMemoryTokenStorageServiceZoho(
    IOptions<AppSettingsService> appSettingsService,
    ILogger<InMemoryTokenStorageServiceZoho> logger) : ITokenStorageService
{
    private TokenData _currentTokenData = new()
    {
        AccessToken = "xxx",
        RefreshToken = appSettingsService.Value.Zoho.RefreshToken,
        ExpiresAt = DateTimeOffset.UtcNow.AddMinutes(-5) // Force a refresh on first run
    };
    
    // A simple lock object to prevent race conditions during token refresh
    private readonly object _lock = new();

    public Task<TokenData> GetTokenDataAsync()
    {
        lock (_lock)
        {
            // Return a copy to avoid external modification
            return Task.FromResult(new TokenData 
            {
                AccessToken = _currentTokenData.AccessToken,
                RefreshToken = _currentTokenData.RefreshToken,
                ExpiresAt = _currentTokenData.ExpiresAt
            });
        }
    }

    public Task SaveTokenDataAsync(TokenData data)
    {
        lock (_lock)
        {
            _currentTokenData = data;
            // TODO: save this to a database here???
            logger.LogInformation("Token Refreshed: New token expires at {ExpiresAt}", data.ExpiresAt);
            return Task.CompletedTask;
        }
    }
}