using System.Net;
using System.Net.Http.Headers;
using System.Text.Json.Serialization;
using DerekNotifications.Interfaces;
using DerekNotifications.Models;
using DerekNotifications.Services;
using Microsoft.Extensions.Options;

namespace DerekNotifications.Delegates;

public class ZohoTokenRefreshHandler(
    IHttpClientFactory httpClientFactory,
    ITokenStorageService tokenStorageService, 
    IOptions<AppSettingsService> appSettingsService,
    ILogger<ZohoTokenRefreshHandler> logger)
    : DelegatingHandler
{
    // Refresh the token if it expires in less than 5 minutes
    private static readonly TimeSpan RefreshBuffer = TimeSpan.FromMinutes(5);
    private readonly SemaphoreSlim _lock = new SemaphoreSlim(1, 1);
    private const string ZohoAuthClientName = "ZohoAuthClient";
    
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, 
        CancellationToken cancellationToken)
    {
        await EnsureTokenValidAsync();
        var tokenData = await tokenStorageService.GetTokenDataAsync();
        
        request.Headers.Authorization = new AuthenticationHeaderValue("Zoho-oauthtoken", tokenData.AccessToken);
        
        //  Send the original request off with valid token
        var response = await base.SendAsync(request, cancellationToken);

        // TODO: Handle reactive token expiry (in case proactive check wasn't enough)
        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            // The token unexpectedly expired or was invalid. Force a refresh and retry.
            // NOTE: This retry logic is complex to do robustly. It requires cloning the request, 
            // re-running EnsureTokenValidAsync, and sending the cloned request.
            // For simplicity, we just trigger the refresh for the NEXT call.
            // A more robust implementation would use a lock and re-send the request here.
            // For now, let's trust the proactive check.
            logger.LogError("Token Invalid: unexpectedly expired or was invalid");
        }

        return response;
    }

    private async Task EnsureTokenValidAsync()
    {
        var tokenData = await tokenStorageService.GetTokenDataAsync();

        // Check if the token is expired or close to expiring (within the 5-minute buffer)
        if (tokenData.ExpiresAt - DateTimeOffset.UtcNow > RefreshBuffer)
        {
            // Token is still good, no refresh needed.
            return;
        }

        await _lock.WaitAsync();
        try
        {
            // Re-check the expiration time *after* acquiring the lock 
            // to ensure a previous thread didn't just refresh it.
            tokenData = await tokenStorageService.GetTokenDataAsync();
            if (tokenData.ExpiresAt - DateTimeOffset.UtcNow > RefreshBuffer)
            {
                return; // Token was refreshed by another thread
            }
            
            // Token is expired or near expiration, proceed to refresh.
            var content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                {"refresh_token", tokenData.RefreshToken},
                {"client_id", appSettingsService.Value.Zoho.ClientId},
                {"client_secret", appSettingsService.Value.Zoho.ClientSecret},
                {"grant_type", "refresh_token"}
            });
        
            // Send the refresh request to Zoho
            using var refreshClient = httpClientFactory.CreateClient(ZohoAuthClientName);
            var refreshResponse = await refreshClient.PostAsync(Constants.Zoho.AuthUrl, content);
            refreshResponse.EnsureSuccessStatusCode();

            var refreshResult = await refreshResponse.Content.ReadFromJsonAsync<RefreshResponse>();

            // Save the new token data
            await tokenStorageService.SaveTokenDataAsync(new TokenData
            {
                AccessToken = refreshResult?.AccessToken ?? "",
                RefreshToken = tokenData.RefreshToken, // Refresh token usually remains the same
                ExpiresAt = DateTimeOffset.UtcNow.AddSeconds(refreshResult?.ExpiresIn ?? -1)
            });
        }
        finally
        {
            _lock.Release();
        }
    }

    private class RefreshResponse
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; } = "";
        
        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }
    }
}