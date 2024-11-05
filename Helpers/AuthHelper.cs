using Google.Apis.Auth;
using mathAi_backend.Models;

namespace mathAi_backend.Helpers;

public static class AuthHelper
{
    public static async Task<User> GetUserFromGoogleToken(string token)
    {
        var payload = await GoogleJsonWebSignature.ValidateAsync(token);

        Console.WriteLine(payload);

        return new User
        {
            Name = payload.Name,
            Email = payload.Email
        };
    }
}