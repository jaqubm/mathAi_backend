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
        c.CreateMap<ExerciseSet, UserExerciseSetListDto>();
        c.CreateMap<Class, UserClassListDto>();
    })); 

    [HttpGet("Get")]
    public async Task<ActionResult<UserDto>> GetUser()
    {
        var userId = await AuthHelper.GetUserIdFromGoogleJwtTokenAsync(HttpContext);
        var userDb = await userRepository.GetUserByIdAsync(userId);
        
        if (userDb is null) return NotFound("User not found.");
        
        var user = _mapper.Map<UserDto>(userDb);
        
        return Ok(user);
    }

    [HttpGet("GetExistsAndIsStudent/{email}")]
    public async Task<ActionResult<bool>> GetUserExistsAndIsStudent(string email)
    {
        var userDb = await userRepository.GetUserByEmailAsync(email);

        return Ok(userDb is not null && !userDb.IsTeacher);
    }
    
    [HttpPut("UpdateAccountType")]
    public async Task<IActionResult> UpdateUserAccountType([FromBody] bool isTeacher)
    {
        var userId = await AuthHelper.GetUserIdFromGoogleJwtTokenAsync(HttpContext);
        var userDb = await userRepository.GetUserByIdAsync(userId);
        
        if (userDb is null) return Unauthorized("User does not exist!");
        
        userDb.IsTeacher = isTeacher;
        userDb.FirstTimeSignIn = false;
        
        userRepository.UpdateEntity(userDb);
        
        return await userRepository.SaveChangesAsync() ? Ok() : Problem("Failed to update user account type.");
    }

    [HttpGet("GetExerciseSetList")]
    public async Task<ActionResult<List<UserExerciseSetListDto>>> GetUserExerciseSetsList()
    {
        var userId = await AuthHelper.GetUserIdFromGoogleJwtTokenAsync(HttpContext);
        var exerciseSetsListDb = await userRepository.GetExerciseSetListByUserIdAsync(userId);
        
        var exerciseSetsList = _mapper.Map<List<UserExerciseSetListDto>>(exerciseSetsListDb);
        
        return Ok(exerciseSetsList);
    }

    [HttpGet("GetClassList")]
    public async Task<ActionResult<List<UserClassListDto>>> GetUserClassList()
    {
        var userId = await AuthHelper.GetUserIdFromGoogleJwtTokenAsync(HttpContext);
        var userDb = await userRepository.GetUserByIdAsync(userId);
        
        if (userDb is null) return Unauthorized("User does not exist!");
        
        var classListDb = userDb.IsTeacher switch
        {
            true => await userRepository.GetClassListByOwnerIdAsync(userId),
            false => await userRepository.GetClassListByStudentIdAsync(userId)
        };
        
        var classList = _mapper.Map<List<UserClassListDto>>(classListDb);
        
        classList.ForEach(cl => cl.IsOwner = classListDb.Find(c => c.Id == cl.Id)?.Owner?.Id == userId);
        
        return Ok(classList);
    }

    [HttpGet("GetAssignmentSubmissionList/")]
    public async Task<ActionResult<List<UserAssignmentSubmissionListDto>>> GetUserAssignmentSubmissionList()
    {
        var userId = await AuthHelper.GetUserIdFromGoogleJwtTokenAsync(HttpContext);
        var userDb = await userRepository.GetUserByIdAsync(userId);
        
        if (userDb is null) return Unauthorized("User does not exist!");
        if (userDb.IsTeacher) return Conflict("Teacher does not have any AssignmentSubmissions!");
        
        var assignmentSubmissionListDb = await userRepository.GetAssignmentSubmissionListByUserIdAsync(userId);

        var assignmentSubmissionList = new List<UserAssignmentSubmissionListDto>();
        
        assignmentSubmissionListDb.ForEach(s =>
        {
            assignmentSubmissionList.Add(new UserAssignmentSubmissionListDto
            {
                Id = s.Id,
                Completed = s.Completed,
                AssignmentId = s.AssignmentId,
                AssignmentName = s.Assignment?.Name ?? throw new Exception("Assignment not found!"),
                ClassName = s.Assignment?.Class?.Name ?? throw new Exception("Class not found!"),
                StartDate = s.Assignment.StartDate,
                DueDate = s.Assignment.DueDate
            });
        });
        
        var sortedAssignmentSubmissionList = assignmentSubmissionList
            .OrderByDescending(s => s.DueDate)
            .ToList();
        
        return Ok(sortedAssignmentSubmissionList);
    }
}