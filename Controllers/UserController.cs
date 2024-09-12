using mathAi_backend.Helpers;
using mathAi_backend.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace mathAi_backend.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController(IUserRepository userRepository) : ControllerBase
{
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
    public bool FirstTimeSignIn([FromRoute] string email)
    {
        var user = userRepository.GetUserByEmail(email);
        
        return user.FirstTimeSignIn;
    }

    [HttpGet("IsTeacher/{email}")]
    public bool IsTeacher([FromRoute] string email)
    {
        var user = userRepository.GetUserByEmail(email);
    
        return user.IsTeacher;
    }

    [HttpPut("UpdateToTeacher/{email}")]
    public ActionResult UpdateToTeacher([FromRoute] string email)
    {
        var userDb = userRepository.GetUserByEmail(email);
        
        userDb.IsTeacher = true;
        userDb.FirstTimeSignIn = false;

        userRepository.UpdateEntity(userDb);
        
        return userRepository.SaveChanges() ? Ok() : Problem("Failed to update account to teacher account.");
    }
    
    [HttpPut("UpdateToStudent/{email}")]
    public ActionResult UpdateToStudent([FromRoute] string email)
    {
        var userDb = userRepository.GetUserByEmail(email);
        
        userDb.IsTeacher = false;
        userDb.FirstTimeSignIn = false;

        userRepository.UpdateEntity(userDb);
        
        return userRepository.SaveChanges() ? Ok() : Problem("Failed to update account to student account.");
    }
}