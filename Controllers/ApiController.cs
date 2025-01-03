using mathAi_backend.Data;
using mathAi_backend.Helpers;
using Microsoft.AspNetCore.Mvc;
using OpenAI.Chat;

namespace mathAi_backend.Controllers;

[Route("[controller]")]
public class ApiController(IConfiguration config) : ControllerBase
{
    private readonly DataContext _entityFramework = new(config);
    private readonly ChatClientHelper _chatClientHelper = new(config);
    
    [HttpGet("Status")]
    public async Task<ActionResult<Dictionary<string, string>>> GetStatus()
    {
        var databaseConnectionStatus = await _entityFramework.Database.CanConnectAsync();
        
        try
        {
            var client = _chatClientHelper.CreateChatClient();
            var openAiConnectionStatus = await client.CompleteChatAsync("Write OK if connection was successful");

            return Ok(new Dictionary<string, string>
            {
                { "apiStatus", "OK" },
                { "databaseConnectionStatus", databaseConnectionStatus ? "OK" : "Failed" },
                { "openAiApiConnectionStatus", openAiConnectionStatus.Value.Content[0].Text }
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

    [HttpGet("WakeUpDatabase")]
    public async Task<ActionResult> GetWakeUpDatabase()
    {
        var databaseWakeUpConnectAsync = await _entityFramework.Database.CanConnectAsync();
        
        return Ok(databaseWakeUpConnectAsync);
    }
}