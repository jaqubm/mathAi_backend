using AutoMapper;
using mathAi_backend.Dtos;
using mathAi_backend.Helpers;
using mathAi_backend.Models;
using mathAi_backend.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace mathAi_backend.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController(IUserRepository userRepository) : ControllerBase
{
    private readonly Mapper _mapper = new(new MapperConfiguration(c =>
    {
        c.CreateMap<ExerciseSet, ExerciseSetDto>();
    })); 
    
    [HttpPost("SignIn")]
    public async Task<IActionResult> SignIn([FromBody] string accessToken)
    {
        if (string.IsNullOrEmpty(accessToken))
            return Unauthorized("Access token is required.");

        try
        {
            var user = await AuthHelper.GetUserFromGoogleToken(accessToken);
            
            if (userRepository.UserAlreadyExist(user.Email)) return Ok();
        
            userRepository.AddEntity(user);
            
            return userRepository.SaveChanges() ? Ok() : Unauthorized("Failed to add user to database.");
        }
        catch (Exception e)
        {
            return Unauthorized(e.Message);
        }
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
    public ActionResult<List<ExerciseSet>> GetExerciseSets([FromRoute] string email)
    {
        var userDb = userRepository.GetUserByEmail(email);
        
        if (userDb is null)
            return NotFound("User not found.");

        var exerciseSets = userRepository.GetUsersExerciseSetsByEmail(email);
        
        var exerciseSetsDto = exerciseSets.Select(x => _mapper.Map<ExerciseSetDto>(x)).ToList();
        
        return Ok(exerciseSetsDto);
    }
}