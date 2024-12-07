using System.Diagnostics.CodeAnalysis;
using AutoMapper;
using mathAi_backend.Dtos;
using mathAi_backend.Helpers;
using mathAi_backend.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace mathAi_backend.Controllers;

[ApiController]
[Route("[controller]")]
[Experimental("OPENAI001")]
public class AssignmentSubmissionController(IConfiguration config, IAssignmentSubmissionRepository assignmentSubmissionRepository) : ControllerBase
{
    private readonly AssistantClientHelper _assistantClientHelper = new(config);
    
    private readonly Mapper _mapper = new(new MapperConfiguration(c =>
    {
        
    }));

    [HttpPut("AddAnswer")]
    public async Task<ActionResult<string>> AddExerciseAnswer([FromForm] ExerciseAnswerCreatorDto exerciseAnswerCreatorDto)
    {
        var userId = await AuthHelper.GetUserIdFromGoogleJwtTokenAsync(HttpContext);
        var exerciseDb = await assignmentSubmissionRepository.GetExerciseByIdAsync(exerciseAnswerCreatorDto.ExerciseId);
        
        if (exerciseDb is null) return NotFound("Exercise not found.");

        var assignmentDb = await assignmentSubmissionRepository
            .GetAssignmentSubmissionByIdAsync(exerciseAnswerCreatorDto.AssignmentSubmissionId);
        
        if (assignmentDb is null) return NotFound("AssignmentSubmission not found.");
        if (!assignmentDb.StudentId.Equals(userId)) return Unauthorized("You don't have permission to add answer.");
        
        if (exerciseAnswerCreatorDto.AnswerImageFile is null || exerciseAnswerCreatorDto.AnswerImageFile.Length == 0)
            return BadRequest("No file uploaded or file is empty.");
        
        await using var memoryStream = new MemoryStream();
        await exerciseAnswerCreatorDto.AnswerImageFile.CopyToAsync(memoryStream);
        var uploadedFileBytes = memoryStream.ToArray();
        var uploadedFileExtension = Path.GetExtension(exerciseAnswerCreatorDto.AnswerImageFile.FileName);
        var uploadedFileName = userId + "_" + 
                               exerciseAnswerCreatorDto.AssignmentSubmissionId + "_" + 
                               exerciseAnswerCreatorDto.ExerciseId +
                               uploadedFileExtension;
        
        var assistant = await _assistantClientHelper.CreateExerciseAssistant();
        var uploadedAnswerImage = await _assistantClientHelper.UploadSolutionImageAsync(uploadedFileBytes, uploadedFileName);
        
        var (grade, feedback) = await _assistantClientHelper.GradeExerciseSolutionAsync(assistant, exerciseDb.Content, uploadedAnswerImage);
        
        return Ok(new
        {
            Grade = grade,
            Feedback = feedback
        });
    }
}