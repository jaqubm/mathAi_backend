using AutoMapper;
using mathAi_backend.Dtos;
using mathAi_backend.Models;
using mathAi_backend.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace mathAi_backend.Controllers;

[ApiController]
[Route("[controller]")]
public class AssignmentController(IAssignmentRepository assignmentRepository, IClassRepository classRepository, IExerciseSetRepository exerciseSetRepository) : ControllerBase
{
    private readonly Mapper _mapper = new(new MapperConfiguration(c =>
    {
        c.CreateMap<AssignmentDto, Assignment>();
    })); 
    
    [HttpPost("Create")]
    public ActionResult<string> CreateAssignment([FromBody] AssignmentDto assignmentDto)
    {
        var classDb = classRepository.GetClassById(assignmentDto.ClassId);
        
        if (classDb is null) return NotFound("Class not found.");
        
        var exerciseSetDb = exerciseSetRepository.GetExerciseSetById(assignmentDto.ExerciseSetId);
        
        if (exerciseSetDb is null) return NotFound("Exercise set not found.");
        
        var assignment = _mapper.Map<AssignmentDto, Assignment>(assignmentDto);
        
        assignmentRepository.AddEntity(assignment);
        
        return assignmentRepository.SaveChanges() ? Ok(assignment.Id) : Problem("Error occured while creating new assignment.");
    }

    [HttpGet("Get/{assignmentId}")]
    public ActionResult<Assignment> GetAssignment([FromRoute] string assignmentId)
    {
        var assignmentDb = assignmentRepository.GetAssignmentById(assignmentId);
        
        if (assignmentDb is null) return NotFound("Assignment with given ID not found.");
        
        return Ok(assignmentDb);
    }

    [HttpPut("Update/{assignmentId}")]
    public ActionResult<string> UpdateAssignment([FromRoute] string assignmentId, [FromBody] AssignmentDto assignmentDto)
    {
        var assignmentDb = assignmentRepository.GetAssignmentById(assignmentId);
        
        if (assignmentDb is null) return NotFound("Assignment with given ID not found.");
        
        var classDb = classRepository.GetClassById(assignmentDto.ClassId);
        
        if (classDb is null) return NotFound("Class not found.");
        
        var exerciseSetDb = exerciseSetRepository.GetExerciseSetById(assignmentDto.ExerciseSetId);
        
        if (exerciseSetDb is null) return NotFound("Exercise set not found.");
        
        _mapper.Map(assignmentDto, assignmentDb);
        
        assignmentRepository.UpdateEntity(assignmentDb);
        
        return assignmentRepository.SaveChanges() ? Ok(assignmentDb.Id) : Problem("Error occured while updating assignment.");
    }

    [HttpDelete("Delete/{assignmentId}")]
    public ActionResult<string> DeleteAssignment([FromRoute] string assignmentId)
    {
        var assignmentDb = assignmentRepository.GetAssignmentById(assignmentId);
        
        if (assignmentDb is null) return NotFound("Assignment with given ID not found.");
        
        assignmentRepository.DeleteEntity(assignmentDb);
        
        return assignmentRepository.SaveChanges() ? Ok() : Problem("Error occured while deleting assignment.");
    }
}