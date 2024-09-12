using mathAi_backend.Models;
using mathAi_backend.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace mathAi_backend.Controllers;

[ApiController]
[Route("[controller]")]
public class ExerciseSetController(IExerciseSetRepository exerciseSetRepository) : Controller
{
    [HttpPost("GenerateExerciseSet")]
    public async Task<ActionResult<ExerciseSet>> GenerateExerciseSet([FromBody] ExerciseSetGenerator exerciseSetGenerator)
    {
        try
        {
            //var client = exerciseSetRepository.CreateChatClient();
            //ChatCompletion chatCompletion = await client.CompleteChatAsync(JsonSerializer.Serialize(exerciseSetGenerator));
            
            //return Ok(chatCompletion);

            return Ok();
        }
        catch (Exception e)
        {
            return Problem(e.Message, statusCode: 500);
        }
    }
}