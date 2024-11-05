using System.Text.RegularExpressions;
using AutoMapper;
using mathAi_backend.Dtos;
using mathAi_backend.Helpers;
using mathAi_backend.Models;
using mathAi_backend.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace mathAi_backend.Controllers;

[ApiController]
[Route("[controller]")]
public partial class UserController(IUserRepository userRepository, IClassRepository classRepository, IAssignmentRepository assignmentRepository) : ControllerBase
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
            
            if (await userRepository.UserExistAsync(user.Email)) return Ok();
        
            await userRepository.AddEntityAsync(user);
            
            return await userRepository.SaveChangesAsync() ? Ok() : Problem("Failed to add user to database.");
        }
        catch (Exception e)
        {
            return Unauthorized(e.Message);
        }
    }
    
    [HttpGet("Exist/{email}")]
    public async Task<ActionResult<bool>> UserExists([FromRoute] string email)
    {
        return Ok(await userRepository.UserExistAsync(email));
    }

    [HttpGet("FirstTimeSignIn/{email}")]
    public async Task<ActionResult<bool>> FirstTimeSignIn([FromRoute] string email)
    {
        var userDb = await userRepository.GetUserByEmailAsync(email);
        
        if (userDb is null)
            return NotFound("User not found.");
        
        return Ok(userDb.FirstTimeSignIn);
    }

    [HttpGet("IsTeacher/{email}")]
    public async Task<ActionResult<bool>> IsTeacher([FromRoute] string email)
    {
        var userDb = await userRepository.GetUserByEmailAsync(email);
        
        if (userDb is null)
            return NotFound("User not found.");
    
        return Ok(userDb.IsTeacher);
    }

    [HttpPut("UpdateToTeacher/{email}")]
    public async Task<ActionResult> UpdateToTeacher([FromRoute] string email)
    {
        var userDb = await userRepository.GetUserByEmailAsync(email);
        
        if (userDb is null)
            return NotFound("User not found.");
        
        userDb.IsTeacher = true;
        userDb.FirstTimeSignIn = false;

        userRepository.UpdateEntity(userDb);
        
        return await userRepository.SaveChangesAsync() ? Ok() : Problem("Failed to update account to teacher account.");
    }
    
    [HttpPut("UpdateToStudent/{email}")]
    public async Task<ActionResult> UpdateToStudent([FromRoute] string email)
    {
        var userDb = await userRepository.GetUserByEmailAsync(email);
        
        if (userDb is null)
            return NotFound("User not found.");
        
        userDb.IsTeacher = false;
        userDb.FirstTimeSignIn = false;

        userRepository.UpdateEntity(userDb);
        
        return await userRepository.SaveChangesAsync() ? Ok() : Problem("Failed to update account to student account.");
    }

    [HttpGet("GetExerciseSets/{email}")]
    public async Task<ActionResult<List<ExerciseSetDto>>> GetExerciseSets([FromRoute] string email)
    {
        var userDb = await userRepository.GetUserByEmailAsync(email);
        
        if (userDb is null)
            return NotFound("User not found.");

        var exerciseSets = await userRepository.GetUsersExerciseSetsByEmailAsync(email);
        
        var sortedExerciseSets = exerciseSets
            .OrderBy(x => ExtractExerciseSetNumber(x.Name))
            .Select(x => _mapper.Map<ExerciseSetDto>(x))
            .ToList();
        
        return Ok(sortedExerciseSets);
    }
    
    [HttpGet("GetClasses/{email}")]
    public async Task<ActionResult<List<Class>>> GetClasses([FromRoute] string email)
    {
        var userDb = await userRepository.GetUserByEmailAsync(email);
        
        if (userDb is null) 
            return NotFound("User not found.");

        var userClasses = userDb.IsTeacher switch
        {
            true => classRepository.GetClassesByOwnerIdAsync(userDb.Email),
            false => classRepository.GetClassesByStudentIdAsync(userDb.Email)
        };

        return Ok(userClasses);
    }

    [HttpGet("GetAssignmentSubmissions/{email}")]
    public async Task<ActionResult<List<AssignmentSubmission>>> GetAssignmentSubmissions([FromRoute] string email)
    {
        var userDb = await userRepository.GetUserByEmailAsync(email);
        
        if (userDb is null)
            return NotFound("User not found.");
        
        var assignmentSubmissions = await userRepository.GetAssignmentSubmissionsByEmailAsync(email);

        foreach (var assignmentSubmission in assignmentSubmissions)
        {
            assignmentSubmission.Assignment =
                await assignmentRepository.GetAssignmentByIdAsync(assignmentSubmission.AssignmentId) ??
                throw new InvalidOperationException();
        }
        
        var sortedAssignmentSubmissions = assignmentSubmissions
            .OrderBy(sub => sub.Assignment.DueDate)
            .ToList();
        
        return Ok(sortedAssignmentSubmissions);
    }
}