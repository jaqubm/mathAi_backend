using AutoMapper;
using mathAi_backend.Dtos;
using mathAi_backend.Models;
using mathAi_backend.Repositories;
using Microsoft.AspNetCore.Mvc;
using OpenAI.Chat;

namespace mathAi_backend.Controllers;

[ApiController]
[Route("[controller]")]
public class ExerciseSetController(IExerciseSetRepository exerciseSetRepository, IUserRepository userRepository, IOpenAiRepository openAiRepository) : Controller
{
    private readonly Mapper _mapper = new(new MapperConfiguration(c =>
    {
        c.CreateMap<ExerciseSetGeneratorDto, ExerciseSet>();
        c.CreateMap<ExerciseSet, ExerciseSet>();
    })); 
    
    private static string ExerciseAnswerFormat()
    {
        return "Odpowiedź odeślij w formacie JSON, tak jak w przykładzie, w formacie gotowym do wyświetlenia na stronie internetowej:" +
               "{Content: string," +
               "FirstHint: string," +
               "SecondHint: string," +
               "ThirdHint: string," +
               "Solution: string}";
    }
    
    private static string GenerateExerciseSetPrompt(ExerciseSetGeneratorDto exerciseSetGenerator)
    {
        return $"Wygeneruj zadanie z Matematyki, wraz z trzema podpowiedziami oraz odpowiedzią dla ucznia " +
               $"ze szkoły: {exerciseSetGenerator.SchoolType}, " +
               $"klasa: {exerciseSetGenerator.Grade}, " +
               $"o tematyce: {exerciseSetGenerator.Subject}. " + 
               $"Zadanie powinno być ciekawe i rozbudowane.\n" + 
               ExerciseAnswerFormat();
    }
    
    [HttpPost("Generate")]
    public async Task<ActionResult<ExerciseSet>> GenerateExerciseSet([FromBody] ExerciseSetGeneratorDto exerciseSetGenerator)
    {
        try
        {
            var client = openAiRepository.CreateChatClient();

            var exerciseSetName = "Zestaw Zadań";

            if (!string.IsNullOrEmpty(exerciseSetGenerator.UserId))
            {
                var userDb = userRepository.GetUserByEmail(exerciseSetGenerator.UserId);
                
                if (userDb is null)
                    return Unauthorized();
                
                exerciseSetName += $" {userRepository.UserExerciseSetsCount(userDb) + 1}";
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
                ChatCompletion chatCompletion = await client.CompleteChatAsync(GenerateExerciseSetPrompt(exerciseSetGenerator));

                try
                {
                    var exercise = new Exercise(chatCompletion.ToString(), exerciseSet.Id);

                    exerciseSet.Exercises.Add(exercise);
                }
                catch
                {
                    i--;
                }
            }
            
            exerciseSetRepository.AddEntity(exerciseSet);
            
            return exerciseSetRepository.SaveChanges() ? Ok(exerciseSet.Id) : Problem("Failed to create exercise set.");
        }
        catch (Exception e)
        {
            return Problem(e.Message, statusCode: 500);
        }
    }
    
    [HttpPost("Copy/{exerciseSetId}")]
    public ActionResult<ExerciseSet> CopyExerciseSet([FromRoute] string exerciseSetId, [FromBody] string email)
    {
        var userDb = userRepository.GetUserByEmail(email);
                
        if (userDb is null)
            return Unauthorized("You don't have permission to copy exercise set.");
        
        var exerciseSetDb = exerciseSetRepository.GetExerciseSetById(exerciseSetId);
            
        if (exerciseSetDb is null)
            return NotFound($"Could not find exercise set with id {exerciseSetId}");
        
        var exerciseSetName = $"Zestaw Zadań {userRepository.UserExerciseSetsCount(userDb) + 1}";

        var copiedExerciseSet = new ExerciseSet
        {
            Name = exerciseSetName,
            SchoolType = exerciseSetDb.SchoolType,
            Grade = exerciseSetDb.Grade,
            Subject = exerciseSetDb.Subject,
            UserId = userDb.Email,
        };

        foreach (var copiedExercise in exerciseSetDb.Exercises.Select(exercise => new Exercise
                 {
                     Content = exercise.Content,
                     FirstHint = exercise.FirstHint,
                     SecondHint = exercise.SecondHint,
                     ThirdHint = exercise.ThirdHint,
                     Solution = exercise.Solution,
                     ExerciseSetId = copiedExerciseSet.Id,
                 }))
        {
            copiedExerciseSet.Exercises.Add(copiedExercise);
        }
        
        exerciseSetRepository.AddEntity(copiedExerciseSet);
            
        return exerciseSetRepository.SaveChanges() ? Ok(copiedExerciseSet.Id) : Problem("Failed to copy exercise set.");
    }
    
    [HttpGet("Get/{exerciseSetId}")]
    public ActionResult<ExerciseSet> GetExerciseSet([FromRoute] string exerciseSetId)
    {
        var exerciseSetDb = exerciseSetRepository
            .GetExerciseSetById(exerciseSetId);
        
        return exerciseSetDb is not null ? Ok(exerciseSetDb) : NotFound("Exercise set not found.");
    }
    
    [HttpPut("Update")]
    public ActionResult<ExerciseSet> UpdateExerciseSet([FromBody] ExerciseSet exerciseSet)
    {
        if (string.IsNullOrEmpty(exerciseSet.UserId))
            return Unauthorized("You don't have permission to update exercise set.");
        
        var exerciseSetDb = exerciseSetRepository.GetExerciseSetById(exerciseSet.Id);
            
        if (exerciseSetDb is null)
            return NotFound($"Could not find exercise set with id {exerciseSet.Id}");
        
        var userDb = userRepository.GetUserByEmail(exerciseSet.UserId);
        
        if (userDb is null)
            return Unauthorized("You don't have permission to update the exercise set.");
        
        if (!userDb.IsTeacher)
            return Unauthorized("You don't have permission to update the exercise set.");
        
        if (!string.Equals(exerciseSetDb.UserId, userDb.Email))
            return Unauthorized("You don't have permission to update the exercise set.");
        
        _mapper.Map(exerciseSet, exerciseSetDb);
        
        exerciseSetRepository.UpdateEntity(exerciseSetDb);
        
        return exerciseSetRepository.SaveChanges() ? Ok() : Problem("Failed to update exercise set.");
    }

    [HttpPut("GenerateAdditionalExercise/{exerciseSetId}")]
    public async Task<ActionResult<ExerciseSet>> GenerateAdditionalExercise([FromRoute] string exerciseSetId, [FromBody] string email)
    {
        try
        {
            var client = openAiRepository.CreateChatClient();
            
            var exerciseSetDb = exerciseSetRepository.GetExerciseSetById(exerciseSetId);
            
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
                ChatCompletion chatCompletion = await client.CompleteChatAsync(GenerateExerciseSetPrompt(exerciseSetGenerator));

                try
                {
                    var exercise = new Exercise(chatCompletion.ToString(), exerciseSetDb.Id);

                    exerciseSetDb.Exercises.Add(exercise);
                }
                catch
                {
                    i--;
                }
            }
            
            exerciseSetRepository.UpdateEntity(exerciseSetDb);
            
            return exerciseSetRepository.SaveChanges() ? Ok() : Problem("Failed to generate additional exercise and add it to the exercise set.");
        }
        catch (Exception e)
        {
            return Problem(e.Message, statusCode: 500);
        }
    }
}