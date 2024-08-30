using Microsoft.AspNetCore.Mvc;

namespace mathAi_backend.Controllers;

[Route("[controller]")]
public class ApiController(IConfiguration config) : ControllerBase
{
    [HttpGet("status")]
    public IActionResult GetStatus()
    {
        // TODO: Add OpenAI Api Connection status
        return Ok(new Dictionary<string, string>
        {
            { "apiStatus", "OK" },
        });
    }
}