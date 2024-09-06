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
        c.CreateMap<User, UserDto>();
    }));

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] string accessToken)
    {
        if (string.IsNullOrEmpty(accessToken))
            return Unauthorized("Access token is required.");

        try
        {
            var user = await AuthHelper.GetUserFromGoogleToken(accessToken);
            
            if (userRepository.UserAlreadyExist(user.UserId)) return Ok();
        
            userRepository.AddEntity(user);
        
            if (userRepository.SaveChanges()) return Ok();

            throw new Exception("Failed to add user to database");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return Unauthorized(e.Message);
        }
    }
}