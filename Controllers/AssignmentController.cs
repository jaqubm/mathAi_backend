using AutoMapper;
using mathAi_backend.Dtos;
using mathAi_backend.Helpers;
using mathAi_backend.Models;
using mathAi_backend.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace mathAi_backend.Controllers;

[ApiController]
[Route("[controller]")]
public class AssignmentController(IAssignmentRepository assignmentRepository) : ControllerBase
{
    private readonly Mapper _mapper = new(new MapperConfiguration(c =>
    {
        c.CreateMap<Assignment, AssignmentDto>();
    }));

    [HttpPost("Create")]
    public async Task<ActionResult<string>> CreateAssignment([FromBody] AssignmentCreatorDto assignmentCreatorDto)
    {
        var userId = await AuthHelper.GetUserIdFromGoogleJwtTokenAsync(HttpContext);
        var classDb = await assignmentRepository.GetClassByIdAsync(assignmentCreatorDto.ClassId);
        
        if (classDb is null) return NotFound("Class not found");
        if (!classDb.OwnerId.Equals(userId)) 
            return Unauthorized("You are not authorized to create an assignment for this class.");
        
        var exerciseSetDb = await assignmentRepository.GetExerciseSetByIdAsync(assignmentCreatorDto.ExerciseSetId);
        
        if (exerciseSetDb is null) return NotFound("Exercise set not found");
        if (exerciseSetDb.UserId is null || !exerciseSetDb.UserId.Equals(userId)) 
            return Unauthorized("You are not authorized to create an assignment with exercise set you do not own.");
        
        if (assignmentCreatorDto.DueDate < assignmentCreatorDto.StartDate) return BadRequest("Due date cannot be earlier than start date.");

        var assignment = new Assignment
        {
            Name = assignmentCreatorDto.Name,
            StartDate = assignmentCreatorDto.StartDate,
            DueDate = assignmentCreatorDto.DueDate,
            ClassId = assignmentCreatorDto.ClassId,
            ExerciseSetId = assignmentCreatorDto.ExerciseSetId
        };
        
        await assignmentRepository.AddEntityAsync(assignment);
        
        classDb.ClassStudents.ForEach(cs =>
        {
            assignment.Submissions.Add(new AssignmentSubmission
            {
                AssignmentId = assignment.Id,
                StudentId = cs.StudentId,
            });
        });
        
        return await assignmentRepository.SaveChangesAsync() ? Ok(assignment.Id) : Problem("Error occured while creating new assignment.");
    }

    [HttpGet("Get/{assignmentId}")]
    public async Task<ActionResult<AssignmentDto>> GetAssignment(string assignmentId)
    {
        var userId = await AuthHelper.GetUserIdFromGoogleJwtTokenAsync(HttpContext);
        var assignmentDb = await assignmentRepository.GetAssignmentByIdAsync(assignmentId);
        
        if (assignmentDb is null) return NotFound("Assignment not found.");
        if (assignmentDb.Class is null) return NotFound("Class not found.");
        if (!assignmentDb.Class.OwnerId.Equals(userId) && assignmentDb.Submissions.All(s => s.StudentId != userId)) 
            return Unauthorized("You are not authorized to see this assignment.");
        
        var assignment = _mapper.Map<AssignmentDto>(assignmentDb);
        
        return Ok(assignment);
    }
    
    [HttpPut("UpdateName/{assignmentId}")]
    public async Task<ActionResult<string>> UpdateAssignmentName([FromRoute] string assignmentId, [FromBody] string assignmentName)
    {
        var userId = await AuthHelper.GetUserIdFromGoogleJwtTokenAsync(HttpContext);
        var assignmentDb = await assignmentRepository.GetAssignmentByIdAsync(assignmentId);
        
        if (assignmentDb is null) return NotFound("Assignment not found.");
        if (assignmentDb.Class is null) return NotFound("Class not found.");
        if (!assignmentDb.Class.OwnerId.Equals(userId)) 
            return Unauthorized("You are not authorized to update name of this assignment in this class.");
        
        assignmentDb.Name = assignmentName;
        
        assignmentRepository.UpdateEntity(assignmentDb);
        
        return await assignmentRepository.SaveChangesAsync() ? Ok() : Problem("Error occured while updating an assignment from this class.");
    }

    [HttpPut("UpdateDueDate/{assignmentId}")]
    public async Task<ActionResult<string>> UpdateAssignmentDueDate([FromRoute] string assignmentId, [FromBody] DateTime assignmentDueDate)
    {
        var userId = await AuthHelper.GetUserIdFromGoogleJwtTokenAsync(HttpContext);
        var assignmentDb = await assignmentRepository.GetAssignmentByIdAsync(assignmentId);
        
        if (assignmentDb is null) return NotFound("Assignment not found.");
        if (assignmentDb.Class is null) return NotFound("Class not found.");
        if (!assignmentDb.Class.OwnerId.Equals(userId)) 
            return Unauthorized("You are not authorized to update due date of this assignment in this class.");
        
        if (assignmentDueDate < assignmentDb.StartDate) return BadRequest("Due date cannot be earlier than start date.");
        
        assignmentDb.DueDate = assignmentDueDate;
        
        assignmentRepository.UpdateEntity(assignmentDb);
        
        return await assignmentRepository.SaveChangesAsync() ? Ok() : Problem("Error occured while updating an assignment from this class.");
    }

    [HttpDelete("Delete/{assignmentId}")]
    public async Task<ActionResult<string>> DeleteAssignment([FromRoute] string assignmentId)
    {
        var userId = await AuthHelper.GetUserIdFromGoogleJwtTokenAsync(HttpContext);
        var assignmentDb = await assignmentRepository.GetAssignmentByIdAsync(assignmentId);
        
        if (assignmentDb is null) return NotFound("Assignment not found.");
        if (assignmentDb.Class is null) return NotFound("Class not found.");
        if (!assignmentDb.Class.OwnerId.Equals(userId)) 
            return Unauthorized("You are not authorized to delete an assignment from this class.");
        
        assignmentRepository.DeleteEntity(assignmentDb);
        
        return await assignmentRepository.SaveChangesAsync() ? Ok() : Problem("Error occured while deleting an assignment from this class.");
    }
}