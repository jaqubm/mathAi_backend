using Google.Apis.Auth;
using mathAi_backend.Models;

namespace mathAi_backend.Helpers;

public static class AuthHelper
{
    public static async Task<User> GetUserFromGoogleToken(string token)
    {
        var payload = await GoogleJsonWebSignature.ValidateAsync(token);

        return new User(
            name: payload.Name,
            email: payload.Email
        );
    }
}