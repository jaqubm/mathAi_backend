using Microsoft.AspNetCore.Mvc;

namespace mathAi_backend.Controllers;

[Route("[controller]")]
public class ApiController(IConfiguration config) : ControllerBase
{
    [HttpGet("status")]
    public IActionResult GetStatus()
    {
        ChatClient client = new(model: "gpt-4o-mini", Environment.GetEnvironmentVariable("OPENAI_API_KEY") ?? "");

        try
        {
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