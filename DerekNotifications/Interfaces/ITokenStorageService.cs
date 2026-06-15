using DerekNotifications.Models;

namespace DerekNotifications.Interfaces;

public interface ITokenStorageService
{
    Task<TokenData> GetTokenDataAsync();
    Task SaveTokenDataAsync(TokenData data);
}
