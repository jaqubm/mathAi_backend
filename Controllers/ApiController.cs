using mathAi_backend.Data;
using mathAi_backend.Repositories;
using Microsoft.AspNetCore.Mvc;
using OpenAI.Chat;

namespace mathAi_backend.Controllers;

[Route("[controller]")]
public class ApiController(IConfiguration config, IOpenAiRepository openAiRepository) : ControllerBase
{
    private readonly DataContext _entityFramework = new(config);
    
    [HttpGet("status")]
    public async Task<ActionResult<Dictionary<string, string>>> GetStatus()
    {
        var databaseConnectionStatus = await _entityFramework.Database.CanConnectAsync();
        
        try
        {
            var client = openAiRepository.CreateChatClient();
            ChatCompletion openAiConnectionStatus = await client.CompleteChatAsync("Write OK if connection was successful");

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