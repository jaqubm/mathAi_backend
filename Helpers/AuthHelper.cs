using Google.Apis.Auth;
using mathAi_backend.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace mathAi_backend.Helpers;

public static class AuthHelper
{
    private static async Task<string> GetAccessTokenFromHttpContext(HttpContext httpContext)
    {
        return await httpContext.GetTokenAsync(JwtBearerDefaults.AuthenticationScheme, "access_token") ?? throw new UnauthorizedAccessException("No access token found!");
    }
    
    public static async Task<string> GetUserIdFromGoogleJwtTokenAsync(HttpContext httpContext)
    {
        var accessToken = await GetAccessTokenFromHttpContext(httpContext);
        var payload = await GoogleJsonWebSignature.ValidateAsync(accessToken);
        
        return payload.Subject;
    }

    public static async Task<User> CreateNewUserFromGoogleJwtTokenAsync(HttpContext httpContext)
    {
        var accessToken = await GetAccessTokenFromHttpContext(httpContext);
        var payload = await GoogleJsonWebSignature.ValidateAsync(accessToken);

        return new User
        {
            Id = payload.Subject,
            Email = payload.Email,
            Name = payload.Name
        };
    }
}