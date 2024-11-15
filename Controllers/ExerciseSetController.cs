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
public class ExerciseSetController(IConfiguration config, IExerciseSetRepository exerciseSetRepository) : ControllerBase
{
    private readonly OpenAiHelper _openAiHelper = new(config);
    
    private readonly Mapper _mapper = new(new MapperConfiguration(c =>
    {
        c.CreateMap<ExerciseSetSettingsDto, ExerciseSet>();
        c.CreateMap<ExerciseSet, ExerciseSet>();
        c.CreateMap<ExerciseSetDto, ExerciseSet>();
        c.CreateMap<ExerciseSet, ExerciseSetDto>();
        c.CreateMap<Exercise, ExerciseDto>();
        c.CreateMap<ExerciseDto, Exercise>();
    })); 
    
    [AllowAnonymous]
    [HttpPost("Generate")]
    public async Task<ActionResult<ExerciseSet>> GenerateExerciseSet([FromBody] ExerciseSetSettingsDto exerciseSetSettings)
    {
        string? userId;

        try
        {
            userId = await AuthHelper.GetUserIdFromGoogleJwtTokenAsync(HttpContext);
        }
        catch
        {
            userId = null;
        }
        
        try
        {
            var client = _openAiHelper.CreateChatClient();

            var exerciseSetName = "Zestaw Zadań";

            if (!string.IsNullOrEmpty(userId))
            {
                exerciseSetName += $" {await exerciseSetRepository.GetUserExerciseSetsCountAsync(userId) + 1}";
            }

            var exerciseSet = new ExerciseSet
            {
                Name = exerciseSetName
            };

            _mapper.Map(exerciseSetSettings, exerciseSet);
            exerciseSet.UserId = userId;

            for (var i = 0; i < exerciseSetSettings.NumberOfExercises; i++)
            {
                var chatCompletion = await client.CompleteChatAsync(OpenAiHelper.GenerateExerciseSetPrompt(exerciseSetSettings));

                try
                {
                    var exercise = new Exercise(chatCompletion.Value.Content[0].Text, exerciseSet.Id);

                    exerciseSet.Exercises.Add(exercise);
                }
                catch
                {
                    i--;
                }
            }
            
            await exerciseSetRepository.AddEntityAsync(exerciseSet);
            
            return await exerciseSetRepository.SaveChangesAsync() ? Ok(exerciseSet.Id) : Problem("Failed to generate exercise set.");
        }
        catch (Exception e)
        {
            return Problem(e.Message, statusCode: 500);
        }
    }
    
    [HttpPut("GenerateAdditionalExercise/{exerciseSetId}")]
    public async Task<ActionResult<ExerciseSet>> GenerateAdditionalExercise([FromRoute] string exerciseSetId)
    {
        var userId = await AuthHelper.GetUserIdFromGoogleJwtTokenAsync(HttpContext);
        
        try
        {
            var client = _openAiHelper.CreateChatClient();

            var exerciseSetDb = await exerciseSetRepository.GetExerciseSetByIdAsync(exerciseSetId);

            if (exerciseSetDb is null)
                return NotFound($"Could not find exercise set with id {exerciseSetId}");

            if (!string.Equals(exerciseSetDb.UserId, userId))
                return Unauthorized("You don't have permission to generate additional exercise for this exercise set.");

            var exerciseSetGenerator = new ExerciseSetSettingsDto
            {
                SchoolType = exerciseSetDb.SchoolType,
                Grade = exerciseSetDb.Grade,
                Subject = exerciseSetDb.Subject,
                NumberOfExercises = 1,
            };

            for (var i = 0; i < exerciseSetGenerator.NumberOfExercises; i++)
            {
                var chatCompletion = await client.CompleteChatAsync(OpenAiHelper.GenerateExerciseSetPrompt(exerciseSetGenerator));

                try
                {
                    var exercise = new Exercise(chatCompletion.Value.Content[0].Text, exerciseSetDb.Id);

                    exerciseSetDb.Exercises.Add(exercise);
                }
                catch
                {
                    i--;
                }
            }

            exerciseSetRepository.UpdateEntity(exerciseSetDb);

            return await exerciseSetRepository.SaveChangesAsync() ? Ok() : Problem("Failed to generate additional exercise and add it to the exercise set.");
        }
        catch (Exception e)
        {
            return Problem(e.Message, statusCode: 500);
        }
    }
    
    [HttpPost("Copy/{exerciseSetId}")]
    public async Task<ActionResult<string>> CopyExerciseSet([FromRoute] string exerciseSetId)
    {
        var userId = await AuthHelper.GetUserIdFromGoogleJwtTokenAsync(HttpContext);
        var exerciseSetDb = await exerciseSetRepository.GetExerciseSetByIdAsync(exerciseSetId);
            
        if (exerciseSetDb is null)
            return NotFound($"Could not find exercise set with id {exerciseSetId}.");
        
        var exerciseSetName = $"Zestaw Zadań {await exerciseSetRepository.GetUserExerciseSetsCountAsync(userId) + 1}";

        var copiedExerciseSet = new ExerciseSet
        {
            Name = exerciseSetName,
            SchoolType = exerciseSetDb.SchoolType,
            Grade = exerciseSetDb.Grade,
            Subject = exerciseSetDb.Subject,
            UserId = userId,
        };

        foreach (var copiedExercise in exerciseSetDb.Exercises.Select(exercise => new Exercise(exercise)))
        {
            copiedExerciseSet.Exercises.Add(copiedExercise);
        }
        
        await exerciseSetRepository.AddEntityAsync(copiedExerciseSet);
            
        return await exerciseSetRepository.SaveChangesAsync() ? Ok(copiedExerciseSet.Id) : Problem("Failed to copy exercise set.");
    }
    
    [AllowAnonymous]
    [HttpGet("Get/{exerciseSetId}")]
    public async Task<ActionResult<ExerciseSetDto>> GetExerciseSet([FromRoute] string exerciseSetId)
    {
        string? userId;

        try
        {
            userId = await AuthHelper.GetUserIdFromGoogleJwtTokenAsync(HttpContext);
        }
        catch
        {
            userId = null;
        }
        
        var exerciseSetDb = await exerciseSetRepository
            .GetExerciseSetByIdAsync(exerciseSetId);
        
        if (exerciseSetDb is null) return NotFound($"Could not find exercise set with id {exerciseSetId}.");
        
        var exerciseSet = _mapper.Map<ExerciseSetDto>(exerciseSetDb);
        if (exerciseSetDb.UserId is not null && exerciseSetDb.UserId.Equals(userId)) exerciseSet.IsOwner = true;
        
        return Ok(exerciseSet);
    }

    [HttpPut("UpdateName/{exerciseSetId}")]
    public async Task<ActionResult<ExerciseSet>> UpdateExerciseSetName([FromRoute] string exerciseSetId, [FromBody] string exerciseSetName)
    {
        var userId = await AuthHelper.GetUserIdFromGoogleJwtTokenAsync(HttpContext);
        var exerciseSetDb = await exerciseSetRepository.GetExerciseSetByIdAsync(exerciseSetId);

        if (exerciseSetDb is null)
            return NotFound($"Could not find exercise set with id {exerciseSetId}.");
        
        if (!string.Equals(exerciseSetDb.UserId, userId)) return Unauthorized("You are not authorized to update name of this exercise set.");

        exerciseSetDb.Name = exerciseSetName;

        exerciseSetRepository.UpdateEntity(exerciseSetDb);

        return await exerciseSetRepository.SaveChangesAsync() ? Ok() : Problem("Failed to update name of exercise set.");
    }

    [HttpDelete("Delete/{exerciseSetId}")]
    public async Task<ActionResult> DeleteExerciseSet([FromRoute] string exerciseSetId)
    {
        var userId = await AuthHelper.GetUserIdFromGoogleJwtTokenAsync(HttpContext);
        var exerciseSetDb = await exerciseSetRepository.GetExerciseSetByIdAsync(exerciseSetId);
        
        if (exerciseSetDb is null)
            return NotFound($"Could not find exercise set with id {exerciseSetId}.");
        
        if (!string.Equals(exerciseSetDb.UserId, userId)) return Unauthorized("You are not authorized to delete this exercise set.");
        
        exerciseSetRepository.DeleteEntity(exerciseSetDb);
        
        return await exerciseSetRepository.SaveChangesAsync() ? Ok() : Problem("Failed to delete exercise set.");
    }
}