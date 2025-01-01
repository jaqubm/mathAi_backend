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
    [HttpPost("Create")]
    public async Task<ActionResult<string>> CreateAssignment([FromBody] AssignmentCreatorDto assignmentCreatorDto)
    {
        var userId = await AuthHelper.GetUserIdFromGoogleJwtTokenAsync(HttpContext);
        var classDb = await assignmentRepository.GetClassByIdAsync(assignmentCreatorDto.ClassId);
        
        if (classDb is null) return NotFound("Class not found.");
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
        
        classDb.ClassStudentList.ForEach(cs =>
        {
            assignment.AssignmentSubmissionList.Add(new AssignmentSubmission
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
        var userDb = await assignmentRepository.GetUserByIdAsync(userId);
        
        if (userDb is null) return NotFound("User not found.");
        if (!userDb.IsTeacher) return Unauthorized("You are not authorized to see this assignment.");
        
        var assignmentDb = await assignmentRepository.GetAssignmentByIdAsync(assignmentId);
        
        if (assignmentDb is null) return NotFound("Assignment not found.");
        if (assignmentDb.Class is null) return NotFound("Class not found.");
        if (assignmentDb.ExerciseSet is null) return NotFound("Exercise set not found.");
        if (!assignmentDb.Class.OwnerId.Equals(userId) && assignmentDb.AssignmentSubmissionList.All(s => s.StudentId != userId)) 
            return Unauthorized("You are not authorized to see this assignment.");

        var assignment = new AssignmentDto
        {
            Name = assignmentDb.Name,
            StartDate = assignmentDb.StartDate,
            DueDate = assignmentDb.DueDate,
            ClassId = assignmentDb.Class.Id,
            ClassName = assignmentDb.Class.Name,
            ExerciseSetId = assignmentDb.ExerciseSetId,
            ExerciseList = assignmentDb
                .ExerciseSet
                .ExerciseList
                .Select(exercise => new ExerciseDto 
                {
                    Id = exercise.Id,
                    Content = exercise.Content
                }).ToList()
        };
        
        foreach (var assignmentSubmissionDb in assignmentDb.AssignmentSubmissionList)
        {
            var gradeMaxSum = assignmentDb.ExerciseSet.ExerciseList.Count * 100;
            var gradeSum = assignmentSubmissionDb.ExerciseAnswerList.Sum(exerciseAnswer => exerciseAnswer.Grade);

            var studentDb = await assignmentRepository.GetUserByIdAsync(assignmentSubmissionDb.StudentId);
            
            if (studentDb is null) return NotFound("Student not found.");

            var assignmentSubmission = new AssignmentSubmissionListDto
            {
                Id = assignmentSubmissionDb.Id,
                SubmissionDate = assignmentSubmissionDb.SubmissionDate,
                Completed = assignmentSubmissionDb.Completed,
                StudentId = assignmentSubmissionDb.StudentId,
                Student = new UserDto
                {
                    Email = studentDb.Email,
                    Name = studentDb.Name,
                    IsTeacher = studentDb.IsTeacher,
                    FirstTimeSignIn = studentDb.FirstTimeSignIn
                },
                Score = (float)gradeSum / gradeMaxSum,
                ExerciseAnswerList = assignmentSubmissionDb
                    .ExerciseAnswerList
                    .Select(exerciseAnswer => new ExerciseAnswerDto
                    {
                        Id = exerciseAnswer.Id,
                        ExerciseId = exerciseAnswer.ExerciseId,
                        Grade = exerciseAnswer.Grade,
                        Feedback = exerciseAnswer.Feedback
                    }).ToList()
            };
            
            assignment.AssignmentSubmissionList.Add(assignmentSubmission);
        }
        
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