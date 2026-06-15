using DerekNotifications.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;

namespace DerekNotifications.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class ApiKeyAttribute : Attribute, IAsyncAuthorizationFilter
{
    public Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var appSettings = context.HttpContext.RequestServices.GetRequiredService<IOptions<AppSettingsService>>().Value;        
        var apiKey = appSettings.Rsi.ApiToken;
        
        if (!context.HttpContext.Request.Headers.TryGetValue(Constants.Rsi.ApiKeyHeaderName, out var extractedApiKey) ||
            apiKey != extractedApiKey)
        {
            context.Result = new ContentResult()
            {
                StatusCode = 401,
                Content = "Api Key is not valid"
            };
        }
        
        return Task.CompletedTask;
    }
}