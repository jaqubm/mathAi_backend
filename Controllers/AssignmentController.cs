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

        Console.WriteLine(assignment.Submissions.Count);
        
        return await assignmentRepository.SaveChangesAsync() ? Ok(assignment.Id) : Problem("Error occured while creating new assignment.");
    }
}