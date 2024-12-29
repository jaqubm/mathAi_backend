using System.Diagnostics.CodeAnalysis;
using mathAi_backend.Dtos;
using mathAi_backend.Helpers;
using mathAi_backend.Models;
using mathAi_backend.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace mathAi_backend.Controllers;

[ApiController]
[Route("[controller]")]
[Experimental("OPENAI001")]
public class AssignmentSubmissionController(IConfiguration config, IAssignmentSubmissionRepository assignmentSubmissionRepository) : ControllerBase
{
    private readonly AssistantClientHelper _assistantClientHelper = new(config);

    [HttpPut("AddExerciseAnswer")]
    public async Task<ActionResult<string>> AddExerciseAnswer([FromForm] ExerciseAnswerCreatorDto exerciseAnswerCreatorDto)
    {
        var userId = await AuthHelper.GetUserIdFromGoogleJwtTokenAsync(HttpContext);
        var exerciseDb = await assignmentSubmissionRepository.GetExerciseByIdAsync(exerciseAnswerCreatorDto.ExerciseId);
        
        if (exerciseDb is null) return NotFound("Exercise not found.");

        var assignmentSubmissionDb = await assignmentSubmissionRepository
            .GetAssignmentSubmissionByIdAsync(exerciseAnswerCreatorDto.AssignmentSubmissionId);
        
        if (assignmentSubmissionDb is null) return NotFound("AssignmentSubmission not found.");
        if (!assignmentSubmissionDb.StudentId.Equals(userId)) return Unauthorized("You don't have permission to add answer.");
        if (assignmentSubmissionDb.Completed) return Conflict("AssignmentSubmission is already completed.");
        if (assignmentSubmissionDb.Assignment is null) return NotFound("Assignment not found.");
        if (assignmentSubmissionDb.Assignment.StartDate < DateTime.Now) return Conflict("Time to provide answers has not started.");
        if (assignmentSubmissionDb.Assignment.DueDate > DateTime.Now) return Conflict("Time to provide answers has ended.");
        if (assignmentSubmissionDb.ExerciseAnswers.Any(x => x.ExerciseId == exerciseAnswerCreatorDto.ExerciseId))
            return Conflict("An answer to this exercise already exists.");
        
        if (exerciseAnswerCreatorDto.AnswerImageFile is null || exerciseAnswerCreatorDto.AnswerImageFile.Length == 0)
            return BadRequest("No file uploaded or file is empty.");

        var exerciseAnswerName = userId + "_" +
                                 exerciseAnswerCreatorDto.AssignmentSubmissionId + "_" +
                                 exerciseAnswerCreatorDto.ExerciseId;
        
        await using var memoryStream = new MemoryStream();
        await exerciseAnswerCreatorDto.AnswerImageFile.CopyToAsync(memoryStream);
        var uploadedFileBytes = memoryStream.ToArray();
        var uploadedFileExtension = Path.GetExtension(exerciseAnswerCreatorDto.AnswerImageFile.FileName);
        var uploadedFileName = exerciseAnswerName + uploadedFileExtension;
        
        var assistant = await _assistantClientHelper.CreateExerciseAssistant(exerciseAnswerName);
        var uploadedAnswerImage = await _assistantClientHelper.UploadSolutionImageAsync(uploadedFileBytes, uploadedFileName);
        
        var (grade, feedback) = await _assistantClientHelper.GradeExerciseSolutionAsync(assistant, exerciseDb.Content, uploadedAnswerImage);

        var exerciseAnswer = new ExerciseAnswer
        {
            AssistantId = assistant.Id,
            Grade = grade,
            Feedback = feedback,
            AssignmentSubmissionId = assignmentSubmissionDb.Id,
            ExerciseId = exerciseDb.Id
        };
        
        assignmentSubmissionDb.ExerciseAnswers.Add(exerciseAnswer);
        
        assignmentSubmissionRepository.UpdateEntity(assignmentSubmissionDb);
        
        return await assignmentSubmissionRepository.SaveChangesAsync() ? Ok() : Problem("An error when saving exercise answer occured.");
    }

    [HttpPut("MarkAsCompleted/{assignmentSubmissionId}")]
    public async Task<ActionResult<string>> MarkAsCompleted([FromRoute] string assignmentSubmissionId)
    {
        var userId = await AuthHelper.GetUserIdFromGoogleJwtTokenAsync(HttpContext);
        var assignmentSubmissionDb = await assignmentSubmissionRepository.GetAssignmentSubmissionByIdAsync(assignmentSubmissionId);
        
        if (assignmentSubmissionDb is null) return NotFound("AssignmentSubmission not found.");
        if (assignmentSubmissionDb.Completed) return Conflict("AssignmentSubmission is already completed.");
        if (assignmentSubmissionDb.Assignment is null) return NotFound("Assignment not found.");
        if (!assignmentSubmissionDb.StudentId.Equals(userId)) 
            return Unauthorized("You don't have permission to mark this assignment as completed.");
        
        var exerciseSetDb = await assignmentSubmissionRepository.GetExerciseSetByIdAsync(assignmentSubmissionDb.Assignment.ExerciseSetId);
        
        if (exerciseSetDb is null) return NotFound("ExerciseSet not found.");
        
        foreach (var exercise in exerciseSetDb.Exercises)
        {
            if (assignmentSubmissionDb.ExerciseAnswers.All(x => x.ExerciseId != exercise.Id))
            {
                assignmentSubmissionDb.ExerciseAnswers.Add(new ExerciseAnswer
                {
                    AssignmentSubmissionId = assignmentSubmissionDb.Id,
                    ExerciseId = exercise.Id,
                });
            }
        }
        
        assignmentSubmissionDb.Completed = true;
        
        assignmentSubmissionRepository.UpdateEntity(assignmentSubmissionDb);
        
        return await assignmentSubmissionRepository.SaveChangesAsync() ? Ok() : Problem("An error when marking assignment as completed.");
    }
}