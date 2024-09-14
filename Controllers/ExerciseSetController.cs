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
    })); 
    
    private static string ExerciseAnswerFormat()
    {
        return "Odpowiedź odeślij w formacie JSON, tak jak w przykładzie, w formacie gotowym do wyświetlenia na stronie internetowej:\n" +
               "{Content: string,\nFirstHint: string,\nSecondHint: string,\nThirdHint: string,\nSolution: string}";
    }
    
    private static string GenerateExerciseSetPrompt(ExerciseSetGeneratorDto exerciseSetGenerator)
    {
        return $"Wygeneruj zadanie, wraz z trzema podpowiedziami oraz odpowiedzią dla ucznia " +
               $"ze szkoły: {exerciseSetGenerator.SchoolType}, " +
               $"klasa: {exerciseSetGenerator.Grade}, " +
               $"o tematyce: {exerciseSetGenerator.Subject}.\n" + 
               ExerciseAnswerFormat();
    }
    
    [HttpPost("GenerateExerciseSet")]
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

            var exerciseSet = new ExerciseSet()
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
            
            return exerciseSetRepository.SaveChanges() ? Ok(exerciseSet) : Problem("Failed to create exercise set.");
        }
        catch (Exception e)
        {
            return Problem(e.Message, statusCode: 500);
        }
    }

    [HttpPut("GenerateAdditionalExercise/{exerciseSetId}")]
    public async Task<ActionResult<ExerciseSet>> GenerateAdditionalExercise([FromRoute] string exerciseSetId,
        [FromBody] ExerciseSetGeneratorDto exerciseSetGenerator)
    {
        try
        {
            var client = openAiRepository.CreateChatClient();
            
            var exerciseSetDb = exerciseSetRepository.GetExerciseSetWithExercisesById(exerciseSetId);
            
            if (exerciseSetDb is null)
                return NotFound($"Could not find exercise set with id {exerciseSetId}");
            
            if (!string.Equals(exerciseSetDb.UserId, exerciseSetGenerator.UserId))
                return Unauthorized("You don't have permission to generate additional exercise for this exercise set.");
            
            for (var i = 0; i < 1; i++)
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
            
            return exerciseSetRepository.SaveChanges() ? Ok(exerciseSetDb) : Problem("Failed to generate additional exercise and add it to the exercise set.");
        }
        catch (Exception e)
        {
            return Problem(e.Message, statusCode: 500);
        }
    }

    [HttpPut("UpdateExerciseSet")]
    public ActionResult<ExerciseSet> UpdateExerciseSet([FromBody] ExerciseSet exerciseSet)
    {
        if (string.IsNullOrEmpty(exerciseSet.UserId))
            return Unauthorized("You don't have permission to update exercise set.");
        
        var userDb = userRepository.GetUserByEmail(exerciseSet.UserId);
        
        if (userDb is null)
            return Unauthorized("You don't have permission to update the exercise set.");
        
        if (!userDb.IsTeacher)
            return Unauthorized("You don't have permission to update the exercise set.");
        
        exerciseSetRepository.UpdateEntity(exerciseSet);
        
        return exerciseSetRepository.SaveChanges() ? Ok(exerciseSet) : Problem("Failed to update exercise set.");
    }
}