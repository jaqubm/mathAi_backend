using System.Text.RegularExpressions;
using AutoMapper;
using mathAi_backend.Dtos;
using mathAi_backend.Helpers;
using mathAi_backend.Models;
using mathAi_backend.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace mathAi_backend.Controllers;

[ApiController]
[Route("[controller]")]
public partial class UserController(IUserRepository userRepository, IClassRepository classRepository) : ControllerBase
{
    private readonly Mapper _mapper = new(new MapperConfiguration(c =>
    {
        c.CreateMap<ExerciseSet, ExerciseSetDto>();
        c.CreateMap<Class, ClassDto>();
    })); 
    
    [GeneratedRegex(@"\d+")]
    private static partial Regex ExerciseSetNameRegex();
    
    private static int ExtractExerciseSetNumber(string exerciseSetName)
    {
        var match = ExerciseSetNameRegex().Match(exerciseSetName);
        return match.Success ? int.Parse(match.Value) : int.MaxValue;
    }
    
    [HttpPost("SignIn")]
    public async Task<IActionResult> SignIn([FromBody] string idToken)
    {
        if (string.IsNullOrEmpty(idToken))
            return Unauthorized("Access token is required.");

        try
        {
            var user = await AuthHelper.GetUserFromGoogleToken(idToken);
            
            if (userRepository.UserExist(user.Email)) return Ok();
        
            userRepository.AddEntity(user);
            
            return userRepository.SaveChanges() ? Ok() : Problem("Failed to add user to database.");
        }
        catch (Exception e)
        {
            return Unauthorized(e.Message);
        }
    }
    
    [HttpGet("Exist/{email}")]
    public ActionResult<bool> UserExists([FromRoute] string email)
    {
        return Ok(userRepository.UserExist(email));
    }

    [HttpGet("FirstTimeSignIn/{email}")]
    public ActionResult<bool> FirstTimeSignIn([FromRoute] string email)
    {
        var userDb = userRepository.GetUserByEmail(email);
        
        if (userDb is null)
            return NotFound("User not found.");
        
        return Ok(userDb.FirstTimeSignIn);
    }

    [HttpGet("IsTeacher/{email}")]
    public ActionResult<bool> IsTeacher([FromRoute] string email)
    {
        var userDb = userRepository.GetUserByEmail(email);
        
        if (userDb is null)
            return NotFound("User not found.");
    
        return Ok(userDb.IsTeacher);
    }

    [HttpPut("UpdateToTeacher/{email}")]
    public ActionResult UpdateToTeacher([FromRoute] string email)
    {
        var userDb = userRepository.GetUserByEmail(email);
        
        if (userDb is null)
            return NotFound("User not found.");
        
        userDb.IsTeacher = true;
        userDb.FirstTimeSignIn = false;

        userRepository.UpdateEntity(userDb);
        
        return userRepository.SaveChanges() ? Ok() : Problem("Failed to update account to teacher account.");
    }
    
    [HttpPut("UpdateToStudent/{email}")]
    public ActionResult UpdateToStudent([FromRoute] string email)
    {
        var userDb = userRepository.GetUserByEmail(email);
        
        if (userDb is null)
            return NotFound("User not found.");
        
        userDb.IsTeacher = false;
        userDb.FirstTimeSignIn = false;

        userRepository.UpdateEntity(userDb);
        
        return userRepository.SaveChanges() ? Ok() : Problem("Failed to update account to student account.");
    }

    [HttpGet("GetExerciseSets/{email}")]
    public ActionResult<List<ExerciseSetDto>> GetExerciseSets([FromRoute] string email)
    {
        var userDb = userRepository.GetUserByEmail(email);
        
        if (userDb is null)
            return NotFound("User not found.");

        var exerciseSets = userRepository.GetUsersExerciseSetsByEmail(email);
        
        var sortedExerciseSets = exerciseSets
            .OrderBy(x => ExtractExerciseSetNumber(x.Name))
            .Select(x => _mapper.Map<ExerciseSetDto>(x))
            .ToList();
        
        return Ok(sortedExerciseSets);
    }
    
    [HttpGet("GetClasses/{email}")]
    public ActionResult<List<Class>> GetClasses([FromRoute] string email)
    {
        var userDb = userRepository.GetUserByEmail(email);
        
        if (userDb is null) return NotFound("User not found.");

        var userClasses = userDb.IsTeacher switch
        {
            true => classRepository.GetClassesByOwnerId(userDb.Email),
            false => classRepository.GetClassesByStudentId(userDb.Email)
        };

        return Ok(userClasses);
    }
}