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
        c.CreateMap<UserDto, User>();
    }));

    [HttpPost("Login")]
    public async Task<IActionResult> Login([FromBody] string accessToken)
    {
        if (string.IsNullOrEmpty(accessToken))
            return Unauthorized("Access token is required.");

        try
        {
            var user = await AuthHelper.GetUserFromGoogleToken(accessToken);
            
            if (userRepository.UserAlreadyExist(user.Email)) return Ok();
        
            userRepository.AddEntity(user);
        
            if (userRepository.SaveChanges()) return Ok();

            throw new Exception("Failed to add user to database!");
        }
        catch (Exception e)
        {
            return Unauthorized(e.Message);
        }
    }

    [HttpGet("IsTeacher/{email}")]
    public bool IsTeacher([FromRoute] string email)
    {
        var user = userRepository.GetUserByEmail(email);
    
        return user.IsTeacher;
    }

    [HttpPut("UpdateToTeacher/{email}")]
    public IActionResult UpdateToTeacher([FromRoute] string email)
    {
        var userDb = userRepository.GetUserByEmail(email);
        
        userDb.IsTeacher = true;

        userRepository.UpdateEntity(userDb);
        
        if (userRepository.SaveChanges()) return Ok();

        throw new Exception("Failed to update account to teacher account!");
    }
}