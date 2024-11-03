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
    public async Task<ActionResult<string>> CreateAssignment([FromBody] AssignmentDto assignmentDto)
    {
        var classDb = await classRepository.GetClassByIdAsync(assignmentDto.ClassId);
        
        if (classDb is null) return NotFound("Class not found.");
        
        var exerciseSetDb = await exerciseSetRepository.GetExerciseSetByIdAsync(assignmentDto.ExerciseSetId);
        
        if (exerciseSetDb is null) return NotFound("Exercise set not found.");
        
        var assignment = _mapper.Map<AssignmentDto, Assignment>(assignmentDto);
        
        classDb.ClassStudents.ForEach(cs =>
        {
            assignment.Submissions.Add(new AssignmentSubmission
            {
                AssignmentId = assignment.Id, 
                StudentId = cs.StudentId
            });
        });
        
        await assignmentRepository.AddEntityAsync(assignment);
        
        return await assignmentRepository.SaveChangesAsync() ? Ok(assignment.Id) : Problem("Error occured while creating new assignment.");
    }

    [HttpGet("Get/{assignmentId}")]
    public async Task<ActionResult<Assignment>> GetAssignment([FromRoute] string assignmentId)
    {
        var assignmentDb = await assignmentRepository.GetAssignmentByIdAsync(assignmentId);
        
        if (assignmentDb is null) return NotFound("Assignment with given ID not found.");
        
        return Ok(assignmentDb);
    }

    [HttpPut("Update/{assignmentId}")]
    public async Task<ActionResult<string>> UpdateAssignment([FromRoute] string assignmentId, [FromBody] AssignmentDto assignmentDto)
    {
        var assignmentDb = await assignmentRepository.GetAssignmentByIdAsync(assignmentId);
        
        if (assignmentDb is null) return NotFound("Assignment with given ID not found.");
        
        var classDb = await classRepository.GetClassByIdAsync(assignmentDto.ClassId);
        
        if (classDb is null) return NotFound("Class not found.");
        
        var exerciseSetDb = await exerciseSetRepository.GetExerciseSetByIdAsync(assignmentDto.ExerciseSetId);
        
        if (exerciseSetDb is null) return NotFound("Exercise set not found.");
        
        _mapper.Map(assignmentDto, assignmentDb);
        
        assignmentRepository.UpdateEntity(assignmentDb);
        
        return await assignmentRepository.SaveChangesAsync() ? Ok(assignmentDb.Id) : Problem("Error occured while updating assignment.");
    }

    [HttpDelete("Delete/{assignmentId}")]
    public async Task<ActionResult<string>> DeleteAssignment([FromRoute] string assignmentId)
    {
        var assignmentDb = await assignmentRepository.GetAssignmentByIdAsync(assignmentId);
        
        if (assignmentDb is null) return NotFound("Assignment with given ID not found.");
        
        assignmentRepository.DeleteEntity(assignmentDb);
        
        return await assignmentRepository.SaveChangesAsync() ? Ok() : Problem("Error occured while deleting assignment.");
    }
}