using Microsoft.AspNetCore.Mvc;
using OpenAI.Chat;

namespace mathAi_backend.Controllers;

[Route("[controller]")]
public class ApiController(IConfiguration config) : ControllerBase
{
    [HttpGet("status")]
    public IActionResult GetStatus()
    {
        ChatClient client = new(model: "gpt-4o-mini", config.GetSection("AppSettings:OpenAIApiKey").Value ?? "");
        
        ChatCompletion completion = client.CompleteChat("Say 'this is a test.'");

        Console.WriteLine($"[ASSISTANT]: {completion}");
        
        return Ok(new Dictionary<string, string>
        {
            { "apiStatus", "OK" },
        });
    }
}