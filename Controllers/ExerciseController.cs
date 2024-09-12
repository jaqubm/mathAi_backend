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
    
    [HttpGet("GetExercise/{exerciseId}")]
    public ActionResult<Exercise> GetExercise([FromRoute] string exerciseId)
    {
        var exerciseDb = exerciseRepository.GetExerciseById(exerciseId);
        
        if (exerciseDb is not null) return exerciseDb;
        
        return NotFound("Exercise not found.");
    }

    [HttpPut("UpdateExercise/{exerciseId}")]
    public ActionResult<Exercise> UpdateExercise([FromRoute] string exerciseId, [FromBody] ExerciseDto exercise)
    {
        var exerciseDb = exerciseRepository.GetExerciseById(exerciseId);
        
        _mapper.Map(exercise, exerciseDb);
        
        exerciseRepository.UpdateEntity(exerciseDb);
        
        return exerciseRepository.SaveChanges() ? Ok(exerciseDb) : Problem("Failed to update exercise.");
    }

    [HttpDelete("DeleteExercise/{exerciseId}")]
    public IActionResult DeleteExercise([FromRoute] string exerciseId)
    {
        var exerciseDb = exerciseRepository.GetExerciseById(exerciseId);
        
        exerciseRepository.DeleteEntity(exerciseDb);
        
        return exerciseRepository.SaveChanges() ? NoContent() : Problem("Failed to delete exercise.");
    }
}