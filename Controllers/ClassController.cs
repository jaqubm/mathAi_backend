using mathAi_backend.Dtos;
using mathAi_backend.Models;
using mathAi_backend.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace mathAi_backend.Controllers;

/*[ApiController]
[Route("[controller]")]
public class ClassController(IClassRepository classRepository, IUserRepository userRepository) : ControllerBase
{
    [HttpPost("Create")]
    public async Task<ActionResult<string>> CreateClass([FromBody] ClassDto classListDto)
    {
        var owner = await userRepository.GetUserByEmailAsync(classListDto.OwnerId);
        
        if (owner is null) return Unauthorized("User does not exist.");
        if (!owner.IsTeacher) return Unauthorized("Only Teacher is allowed to create Class.");
        
        var newClass = new Class
        {
            Name = classListDto.Name, 
            OwnerId = classListDto.OwnerId
        };

        foreach (var studentEmail in classListDto.ClassStudents)
        {
            var student = await userRepository.GetUserByEmailAsync(studentEmail);

            if (student is null || student.IsTeacher || owner.Email.Equals(student.Email)) continue;
            if (newClass.ClassStudents.Exists(c => c.StudentId.Equals(student.Email))) continue;
            
            var classStudent = new ClassStudents
            {
                ClassId = newClass.Id,
                StudentId = student.Email
            };
            
            newClass.ClassStudents.Add(classStudent);
        }
        
        await classRepository.AddEntityAsync(newClass);

        return await classRepository.SaveChangesAsync() ? Ok(newClass.Id) : Problem("Error occured while creating new class.");
    }

    [HttpGet("Get/{classId}")]
    public async Task<ActionResult<Class>> GetClass([FromRoute] string classId)
    {
        var classDb = await classRepository.GetClassByIdAsync(classId);
        
        if (classDb is null) return NotFound("Class with given ID not found.");
        
        return Ok(classDb);
    }
    
    [HttpDelete("Delete/{classId}")]
    public async Task<ActionResult<string>> DeleteClass([FromRoute] string classId)
    {
        var classDb = await classRepository.GetClassByIdAsync(classId);
        
        if (classDb is null) return NotFound("Class with given ID was not found.");
        
        classRepository.DeleteEntity(classDb);
        
        return await classRepository.SaveChangesAsync() ? Ok() : Problem($"Error occured while deleting class.");
    }

    [HttpPut("AddStudent/{classId}")]
    public async Task<ActionResult<string>> AddStudent([FromRoute] string classId, [FromBody] string studentEmail)
    {
        var userDb = await userRepository.GetUserByEmailAsync(studentEmail);
        
        if (userDb is null) return Unauthorized("User does not exist.");
        if (userDb.IsTeacher) return Unauthorized("Only Student can be added to Class.");
        
        var classDb = await classRepository.GetClassByIdAsync(classId);
        
        if (classDb is null) return NotFound("Class with given ID was not found.");
        if (classDb.ClassStudents.Exists(cs => cs.StudentId.Equals(userDb.Email))) return BadRequest($"Student {userDb.Name} is already a part of {classDb.Name} class.");
        
        classDb.ClassStudents.Add(new ClassStudents
        {
            ClassId = classDb.Id,
            StudentId = userDb.Email
        });
        
        classRepository.UpdateEntity(classDb);
        
        return await classRepository.SaveChangesAsync() ? Ok() : Problem($"Error occured while adding user to {classDb.Name} class.");
    }

    [HttpDelete("RemoveStudent/{classId}")]
    public async Task<ActionResult<string>> RemoveStudent([FromRoute] string classId, [FromBody] string studentEmail)
    {
        var userDb = await userRepository.GetUserByEmailAsync(studentEmail);
        
        if (userDb is null) return Unauthorized("User does not exist.");
        if (userDb.IsTeacher) return Unauthorized("Only Student can be removed from Class.");
        
        var classDb = await classRepository.GetClassByIdAsync(classId);
        
        if (classDb is null) return NotFound("Class with given ID was not found.");
        if (classDb.OwnerId.Equals(userDb.Email)) return Unauthorized("Only Student can be removed from Class.");
        if (!classDb.ClassStudents.Exists(cs => cs.StudentId.Equals(userDb.Email))) return NotFound($"Student {userDb.Name} is no part of {classDb.Name} class therefore he can not be removed.");
        
        classDb.ClassStudents.Remove(classDb.ClassStudents.Find(cs => cs.StudentId.Equals(userDb.Email)) ?? throw new InvalidOperationException());
        
        classRepository.UpdateEntity(classDb);
        
        return await classRepository.SaveChangesAsync() ? Ok() : Problem($"Error occured while removing user from {classDb.Name} class.");
    }
}*/