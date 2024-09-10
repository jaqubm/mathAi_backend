using mathAi_backend.Data;
using Microsoft.AspNetCore.Mvc;
using OpenAI.Chat;

namespace mathAi_backend.Controllers;

[Route("[controller]")]
public class ApiController(IConfiguration config) : ControllerBase
{
    private readonly DataContext _entityFramework = new(config);
    
    [HttpGet("status")]
    public async Task<IActionResult> GetStatus()
    {
        var databaseConnectionStatus = await _entityFramework.Database.CanConnectAsync();
        
        try
        {
            ChatClient client = new(model: "gpt-4o-mini", config.GetSection("AppSettings:OpenAiApiKey").Value ??= "");
            ChatCompletion openAiConnectionStatus = client.CompleteChat("Write OK if connection was successful");

            return Ok(new Dictionary<string, string>
            {
                { "apiStatus", "OK" },
                { "databaseConnectionStatus", databaseConnectionStatus ? "OK" : "Failed" },
                { "openAiApiConnectionStatus", openAiConnectionStatus.ToString() }
            });
        }
        catch
        {
            return Ok(new Dictionary<string, string>
            {
                { "apiStatus", "OK" },
                { "databaseConnectionStatus", databaseConnectionStatus ? "OK" : "Failed" },
                { "openAiApiConnectionStatus", "Failed" }
            });
        }
    }
}