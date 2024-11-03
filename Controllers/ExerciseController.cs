using AutoMapper;
using mathAi_backend.Dtos;
using mathAi_backend.Models;
using mathAi_backend.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace mathAi_backend.Controllers;

[ApiController]
[Route("[controller]")]
public class ExerciseController(IExerciseRepository exerciseRepository) : ControllerBase
{
    private readonly Mapper _mapper = new(new MapperConfiguration(c =>
    {
        c.CreateMap<ExerciseDto, Exercise>();
    })); 
    
    [HttpGet("Get/{exerciseId}")]
    public async Task<ActionResult<Exercise>> GetExercise([FromRoute] string exerciseId)
    {
        var exerciseDb = await exerciseRepository.GetExerciseByIdAsync(exerciseId);
        
        if (exerciseDb is not null) return exerciseDb;
        
        return NotFound("Exercise not found.");
    }

    [HttpPut("Update/{exerciseId}")]
    public async Task<ActionResult<Exercise>> UpdateExercise([FromRoute] string exerciseId, [FromBody] ExerciseDto exercise)
    {
        var exerciseDb = await exerciseRepository.GetExerciseByIdAsync(exerciseId);
        
        _mapper.Map(exercise, exerciseDb);
        
        exerciseRepository.UpdateEntity(exerciseDb);
        
        return await exerciseRepository.SaveChangesAsync() ? Ok(exerciseDb) : Problem("Failed to update exercise.");
    }

    [HttpDelete("Delete/{exerciseId}")]
    public async Task<IActionResult> DeleteExercise([FromRoute] string exerciseId)
    {
        var exerciseDb = await exerciseRepository.GetExerciseByIdAsync(exerciseId);
        
        exerciseRepository.DeleteEntity(exerciseDb);
        
        return await exerciseRepository.SaveChangesAsync() ? NoContent() : Problem("Failed to delete exercise.");
    }
}