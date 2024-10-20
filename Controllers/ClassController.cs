using mathAi_backend.Dtos;
using mathAi_backend.Models;
using mathAi_backend.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace mathAi_backend.Controllers;

[ApiController]
[Route("[controller]")]
public class ClassController(IClassRepository classRepository, IUserRepository userRepository) : ControllerBase
{
    [HttpPost("Create")]
    public ActionResult<string> CreateClass([FromBody] ClassDto classDto)
    {
        var owner = userRepository.GetUserByEmail(classDto.OwnerId);
        
        if (owner is null) return Unauthorized("User does not exist");
        if (!owner.IsTeacher) return Unauthorized("Only Teacher is allowed to create Class");
        
        var newClass = new Class(classDto.Name, classDto.OwnerId);
        
        classDto.ClassStudents.ForEach(s =>
        {
            var student = userRepository.GetUserByEmail(s);

            if (student is null || student.IsTeacher || owner.Email.Equals(student.Email)) return;
            if (newClass.ClassStudents.Exists(c => c.StudentId.Equals(student.Email))) return;
            
            var classStudent = new ClassStudents(newClass.Id, student.Email);
            newClass.ClassStudents.Add(classStudent);
        });
        
        classRepository.AddEntity(newClass);

        return classRepository.SaveChanges() ? Ok(newClass.Id) : Problem("Error occured while creating new class");
    }

    [HttpGet("Get/{classId}")]
    public ActionResult<Class> GetClass([FromRoute] string classId)
    {
        var classDb = classRepository.GetClassById(classId);
        
        if (classDb is null) return NotFound("Class with given ID not found");
        
        return Ok(classDb);
    }
}