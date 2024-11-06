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
public class UserController(IUserRepository userRepository) : ControllerBase
{
    private readonly Mapper _mapper = new(new MapperConfiguration(c =>
    {
        c.CreateMap<User, UserDto>();
        c.CreateMap<ExerciseSet, ExerciseSetListDto>();
        c.CreateMap<Class, ClassListDto>();
    })); 

    [HttpGet("Get")]
    public async Task<ActionResult<UserDto>> GetUser()
    {
        var userId = await AuthHelper.GetUserIdFromGoogleJwtTokenAsync(HttpContext);
        var userDb = await userRepository.GetUserByIdAsync(userId);
        
        if (userDb is null)
            return NotFound("User not found.");
        
        var user = _mapper.Map<UserDto>(userDb);
        
        return Ok(user);
    }
    
    [HttpGet("Exist/{email}")]
    public async Task<ActionResult<bool>> GetUserExists([FromRoute] string email)
    {
        var userExists = await userRepository.CheckIfUserExistsByEmailAsync(email);
        
        return Ok(userExists);
    }

    [HttpGet("GetExerciseSetsList")]
    public async Task<ActionResult<List<ExerciseSetListDto>>> GetUserExerciseSetsList()
    {
        var userId = await AuthHelper.GetUserIdFromGoogleJwtTokenAsync(HttpContext);
        var exerciseSetsListDb = await userRepository.GetExerciseSetsListByUserIdAsync(userId);
        
        var exerciseSetsList = _mapper.Map<List<ExerciseSetListDto>>(exerciseSetsListDb);
        
        return Ok(exerciseSetsList);
    }

    [HttpGet("GetClassList")]
    public async Task<ActionResult<List<ClassListDto>>> GetUserClassList()
    {
        var userId = await AuthHelper.GetUserIdFromGoogleJwtTokenAsync(HttpContext);
        var userDb = await userRepository.GetUserByIdAsync(userId);
        
        if (userDb is null)
            return Unauthorized("User does not exist!");
        
        var classListDb = userDb.IsTeacher switch
        {
            true => await userRepository.GetClassListByOwnerIdAsync(userId),
            false => await userRepository.GetClassListByStudentIdAsync(userId)
        };
        
        var classList = _mapper.Map<List<ClassListDto>>(classListDb);
        
        return Ok(classList);
    }

    /*[HttpGet("GetAssignmentSubmissions/{email}")]
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
    }*/
    
    [HttpPut("UpdateAccountType")]
    public async Task<IActionResult> UpdateUserAccountType([FromBody] bool isTeacher)
    {
        var userId = await AuthHelper.GetUserIdFromGoogleJwtTokenAsync(HttpContext);
        var userDb = await userRepository.GetUserByIdAsync(userId);
        
        if (userDb is null)
            return Unauthorized("User does not exist!");
        
        userDb.IsTeacher = isTeacher;
        userDb.FirstTimeSignIn = false;
        
        userRepository.UpdateEntity(userDb);
        
        return await userRepository.SaveChangesAsync() ? Ok() : Problem("Failed to update user account type.");
    }
}