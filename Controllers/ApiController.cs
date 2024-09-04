using Microsoft.AspNetCore.Mvc;
using OpenAI.Chat;

namespace mathAi_backend.Controllers;

[Route("[controller]")]
public class ApiController(IConfiguration config) : ControllerBase
{
    [HttpGet("status")]
    public IActionResult GetStatus()
    {
        try
        {
            ChatClient client = new(model: "gpt-4o-mini", config.GetSection("AppSettings:OpenAiApiKey").Value ??= "");
            
            ChatCompletion completion = client.CompleteChat("Write OK if connection was successful");

            return Ok(new Dictionary<string, string>
            {
                { "apiStatus", "OK" },
                { "openAIApiConnectionStatus", completion.ToString() }
            });
        }
        catch
        {
            return Ok(new Dictionary<string, string>
            {
                { "apiStatus", "OK" },
                { "openAIApiConnectionStatus", "Failed" }
            });
        }
    }
}