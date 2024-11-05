using AutoMapper;
using mathAi_backend.Dtos;
using mathAi_backend.Helpers;
using mathAi_backend.Models;
using mathAi_backend.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace mathAi_backend.Controllers;

[ApiController]
[Route("[controller]")]
public class ExerciseSetController(IConfiguration config, IExerciseSetRepository exerciseSetRepository, IUserRepository userRepository) : Controller
{
    private readonly OpenAiHelper _openAiHelper = new(config);
    
    private readonly Mapper _mapper = new(new MapperConfiguration(c =>
    {
        c.CreateMap<ExerciseSetGeneratorDto, ExerciseSet>();
        c.CreateMap<ExerciseSet, ExerciseSet>();
    })); 
    
    [HttpPost("Generate")]
    public async Task<ActionResult<ExerciseSet>> GenerateExerciseSet([FromBody] ExerciseSetGeneratorDto exerciseSetGenerator)
    {
        try
        {
            var client = _openAiHelper.CreateChatClient();

            var exerciseSetName = "Zestaw Zadań";

            if (!string.IsNullOrEmpty(exerciseSetGenerator.UserId))
            {
                var userDb = await userRepository.GetUserByEmailAsync(exerciseSetGenerator.UserId);
                
                if (userDb is null)
                    return Unauthorized();
                
                exerciseSetName += $" {await userRepository.UserExerciseSetsCountAsync(userDb) + 1}";
            }
            else
            {
                exerciseSetGenerator.UserId = null;
            }

            var exerciseSet = new ExerciseSet
            {
                Name = exerciseSetName
            };
            
            _mapper.Map(exerciseSetGenerator, exerciseSet);

            for (var i = 0; i < exerciseSetGenerator.NumberOfExercises; i++)
            {
                var chatCompletion = await client.CompleteChatAsync(OpenAiHelper.GenerateExerciseSetPrompt(exerciseSetGenerator));

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
            
            return await exerciseSetRepository.SaveChangesAsync() ? Ok(exerciseSet.Id) : Problem("Failed to create exercise set.");
        }
        catch (Exception e)
        {
            return Problem(e.Message, statusCode: 500);
        }
    }
    
    [HttpPost("Copy/{exerciseSetId}")]
    public async Task<ActionResult<ExerciseSet>> CopyExerciseSet([FromRoute] string exerciseSetId, [FromBody] string email)
    {
        var userDb = await userRepository.GetUserByEmailAsync(email);
                
        if (userDb is null)
            return Unauthorized("You don't have permission to copy exercise set.");
        
        var exerciseSetDb = await exerciseSetRepository.GetExerciseSetByIdAsync(exerciseSetId);
            
        if (exerciseSetDb is null)
            return NotFound($"Could not find exercise set with id {exerciseSetId}");
        
        var exerciseSetName = $"Zestaw Zadań {await userRepository.UserExerciseSetsCountAsync(userDb) + 1}";

        var copiedExerciseSet = new ExerciseSet
        {
            Name = exerciseSetName,
            SchoolType = exerciseSetDb.SchoolType,
            Grade = exerciseSetDb.Grade,
            Subject = exerciseSetDb.Subject,
            UserId = userDb.Email,
        };

        foreach (var copiedExercise in exerciseSetDb.Exercises.Select(exercise => new Exercise(exercise)))
        {
            copiedExerciseSet.Exercises.Add(copiedExercise);
        }
        
        await exerciseSetRepository.AddEntityAsync(copiedExerciseSet);
            
        return await exerciseSetRepository.SaveChangesAsync() ? Ok(copiedExerciseSet.Id) : Problem("Failed to copy exercise set.");
    }
    
    [HttpGet("Get/{exerciseSetId}")]
    public async Task<ActionResult<ExerciseSet>> GetExerciseSet([FromRoute] string exerciseSetId)
    {
        var exerciseSetDb = await exerciseSetRepository
            .GetExerciseSetByIdAsync(exerciseSetId);
        
        return exerciseSetDb is not null ? Ok(exerciseSetDb) : NotFound("Exercise set not found.");
    }
    
    [HttpPut("Update")]
    public async Task<ActionResult<ExerciseSet>> UpdateExerciseSet([FromBody] ExerciseSet exerciseSet)
    {
        if (string.IsNullOrEmpty(exerciseSet.UserId))
            return Unauthorized("You don't have permission to update exercise set.");
        
        var exerciseSetDb = await exerciseSetRepository.GetExerciseSetByIdAsync(exerciseSet.Id);
            
        if (exerciseSetDb is null)
            return NotFound($"Could not find exercise set with id {exerciseSet.Id}");
        
        var userDb = await userRepository.GetUserByEmailAsync(exerciseSet.UserId);
        
        if (userDb is null)
            return Unauthorized("You don't have permission to update the exercise set.");
        
        if (!userDb.IsTeacher)
            return Unauthorized("You don't have permission to update the exercise set.");
        
        if (!string.Equals(exerciseSetDb.UserId, userDb.Email))
            return Unauthorized("You don't have permission to update the exercise set.");
        
        _mapper.Map(exerciseSet, exerciseSetDb);
        
        exerciseSetRepository.UpdateEntity(exerciseSetDb);
        
        return await exerciseSetRepository.SaveChangesAsync() ? Ok() : Problem("Failed to update exercise set.");
    }

    [HttpPut("GenerateAdditionalExercise/{exerciseSetId}")]
    public async Task<ActionResult<ExerciseSet>> GenerateAdditionalExercise([FromRoute] string exerciseSetId, [FromBody] string email)
    {
        try
        {
            var client = _openAiHelper.CreateChatClient();
            
            var exerciseSetDb = await exerciseSetRepository.GetExerciseSetByIdAsync(exerciseSetId);
            
            if (exerciseSetDb is null)
                return NotFound($"Could not find exercise set with id {exerciseSetId}");
            
            if (!string.Equals(exerciseSetDb.UserId, email))
                return Unauthorized("You don't have permission to generate additional exercise for this exercise set.");

            var exerciseSetGenerator = new ExerciseSetGeneratorDto
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
}