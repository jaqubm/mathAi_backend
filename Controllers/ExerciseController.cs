using AutoMapper;
using mathAi_backend.Dtos;
using mathAi_backend.Helpers;
using mathAi_backend.Models;
using mathAi_backend.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace mathAi_backend.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class ExerciseController(IExerciseRepository exerciseRepository) : ControllerBase
{
    private readonly Mapper _mapper = new(new MapperConfiguration(c =>
    {
        c.CreateMap<ExerciseUpdateDto, Exercise>();
    }));

    [HttpPut("Update/{exerciseId}")]
    public async Task<IActionResult> UpdateExercise([FromRoute] string exerciseId, [FromBody] ExerciseUpdateDto exerciseUpdateDto)
    {
        var userId = await AuthHelper.GetUserIdFromGoogleJwtTokenAsync(HttpContext);
        var exerciseDb = await exerciseRepository.GetExerciseByIdAsync(exerciseId);
        
        if (exerciseDb is null) return NotFound("Exercise not found.");
        
        var exerciseSetDb = await exerciseRepository.GetExerciseSetByIdAsync(exerciseDb.ExerciseSetId);
        
        if (exerciseSetDb is null) return NotFound("Exercise set to which exercise belongs not found.");
        if (!string.Equals(userId, exerciseSetDb.UserId)) return Unauthorized();
        
        _mapper.Map(exerciseUpdateDto, exerciseDb);
        
        exerciseRepository.UpdateEntity(exerciseSetDb);
        
        return await exerciseRepository.SaveChangesAsync() ? Ok() : Problem("Failed to update exercise.");
    }

    [HttpDelete("Delete/{exerciseId}")]
    public async Task<IActionResult> DeleteExercise([FromRoute] string exerciseId)
    {
        var userId = await AuthHelper.GetUserIdFromGoogleJwtTokenAsync(HttpContext);
        var exerciseDb = await exerciseRepository.GetExerciseByIdAsync(exerciseId);
        
        if (exerciseDb is null) return NotFound("Exercise not found.");
        
        var exerciseSetDb = await exerciseRepository.GetExerciseSetByIdAsync(exerciseDb.ExerciseSetId);
        
        if (exerciseSetDb is null) return NotFound("Exercise set to which exercise belongs not found.");
        if (!string.Equals(userId, exerciseSetDb.UserId)) return Unauthorized();
        
        exerciseRepository.DeleteEntity(exerciseDb);
        
        return await exerciseRepository.SaveChangesAsync() ? Ok() : Problem("Failed to delete exercise.");
    }
}