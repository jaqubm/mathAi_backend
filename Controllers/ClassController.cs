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
public class ClassController(IClassRepository classRepository) : ControllerBase
{
    private readonly Mapper _mapper = new(new MapperConfiguration(c =>
    {
        c.CreateMap<Class, ClassDto>();
        c.CreateMap<User, UserDto>();
        c.CreateMap<Assignment, AssignmentListDto>();
    })); 
    
    [HttpPost("Create")]
    public async Task<ActionResult<string>> CreateClass([FromBody] ClassCreatorDto classCreatorDto)
    {
        var userId = await AuthHelper.GetUserIdFromGoogleJwtTokenAsync(HttpContext);
        var userDb = await classRepository.GetUserByIdAsync(userId);
        
        if (userDb is null) return Unauthorized();
        if (!userDb.IsTeacher) return Unauthorized("Only Teacher is allowed to create Class.");
        
        var newClass = new Class
        {
            Name = classCreatorDto.Name, 
            OwnerId = userId
        };

        foreach (var studentEmail in classCreatorDto.StudentEmailList)
        {
            var studentDb = await classRepository.GetUserByEmailAsync(studentEmail);

            if (studentDb is null || userDb.FirstTimeSignIn || studentDb.IsTeacher || userDb.Email.Equals(studentDb.Email)) continue;
            if (newClass.ClassStudentList.Exists(c => c.StudentId.Equals(studentDb.Email))) continue;
            
            var classStudent = new ClassStudent
            {
                ClassId = newClass.Id,
                StudentId = studentDb.Id
            };
            
            newClass.ClassStudentList.Add(classStudent);
        }
        
        await classRepository.AddEntityAsync(newClass);

        return await classRepository.SaveChangesAsync() ? Ok(newClass.Id) : Problem("Error occured while creating new class.");
    }

    [HttpGet("Get/{classId}")]
    public async Task<ActionResult<ClassDto>> GetClass([FromRoute] string classId)
    {
        var userId = await AuthHelper.GetUserIdFromGoogleJwtTokenAsync(HttpContext);
        var userDb = await classRepository.GetUserByIdAsync(userId);
        
        if (userDb is null) return Unauthorized();
        
        var classDb = await classRepository.GetClassByIdAsync(classId);

        if (classDb is null) return NotFound("Class with given ID not found.");
        if (!string.Equals(classDb.OwnerId, userId) && !classDb.ClassStudentList.Exists(cs => string.Equals(cs.StudentId, userId))) 
            return Unauthorized("User is not allowed to access this class.");
        
        var cClass = _mapper.Map<ClassDto>(classDb);
        if (classDb.OwnerId.Equals(userId)) cClass.IsOwner = true;

        foreach (var classStudent in classDb.ClassStudentList)
        {
            var studentDb = await classRepository.GetUserByIdAsync(classStudent.StudentId);
            if (studentDb is null) continue;
            cClass.StudentList.Add(_mapper.Map<UserDto>(studentDb));
        }

        return Ok(cClass);
    }
    
    [HttpPut("UpdateName/{classId}")]
    public async Task<ActionResult<ExerciseSet>> UpdateClassName([FromRoute] string classId, [FromBody] string className)
    {
        var userId = await AuthHelper.GetUserIdFromGoogleJwtTokenAsync(HttpContext);
        var classDb = await classRepository.GetClassByIdAsync(classId);

        if (classDb is null)
            return NotFound($"Could not find class with id {classId}.");
        
        if (!string.Equals(classDb.OwnerId, userId)) return Unauthorized("You are not authorized to update name of this class.");

        classDb.Name = className;

        classRepository.UpdateEntity(classDb);

        return await classRepository.SaveChangesAsync() ? Ok() : Problem("Failed to update name of class.");
    }

    [HttpDelete("Delete/{classId}")]
    public async Task<ActionResult<string>> DeleteClass([FromRoute] string classId)
    {
        var userId = await AuthHelper.GetUserIdFromGoogleJwtTokenAsync(HttpContext);
        var userDb = await classRepository.GetUserByIdAsync(userId);
        
        if (userDb is null) return Unauthorized();
        
        var classDb = await classRepository.GetClassByIdAsync(classId);

        if (classDb is null) return NotFound("Class with given ID was not found.");
        if (!string.Equals(classDb.OwnerId, userId)) return Unauthorized("User is not allowed to delete this class.");

        classRepository.DeleteEntity(classDb);

        return await classRepository.SaveChangesAsync() ? Ok() : Problem($"Error occured while deleting class.");
    }

    [HttpPut("AddStudent/{classId}")]
    public async Task<ActionResult<string>> AddStudentToClass([FromRoute] string classId, [FromBody] string studentEmail)
    {
        var studentDb = await classRepository.GetUserByEmailAsync(studentEmail);

        if (studentDb is null) return Unauthorized("Student account with given email does not exist.");
        if (studentDb.FirstTimeSignIn) return Unauthorized("Account with given email has not chosen account type yet.");
        if (studentDb.IsTeacher) return Unauthorized("Only Student can be added to Class.");

        var classDb = await classRepository.GetClassByIdAsync(classId);

        if (classDb is null) return NotFound("Class with given ID was not found.");
        if (classDb.ClassStudentList.Exists(cs => cs.StudentId.Equals(studentDb.Id))) 
            return BadRequest($"Student {studentDb.Name} is already a part of {classDb.Name} class.");

        classDb.ClassStudentList.Add(new ClassStudent
        {
            ClassId = classDb.Id,
            StudentId = studentDb.Id
        });

        foreach (var assignmentSubmission in from assignment in classDb.AssignmentList
                 where DateTime.Now < assignment.DueDate
                 select new AssignmentSubmission
                 {
                     AssignmentId = assignment.Id,
                     StudentId = studentDb.Id
                 })
        {
            await classRepository.AddEntityAsync(assignmentSubmission);
        }

        classRepository.UpdateEntity(classDb);

        return await classRepository.SaveChangesAsync() ? Ok() : Problem($"Error occured while adding user to {classDb.Name} class.");
    }
    
    [HttpDelete("RemoveStudent/{classId}/{studentEmail}")]
    public async Task<ActionResult<string>> RemoveStudentFromClass([FromRoute] string classId, [FromRoute] string studentEmail)
    {
        var userId = await AuthHelper.GetUserIdFromGoogleJwtTokenAsync(HttpContext);
        var userDb = await classRepository.GetUserByIdAsync(userId);
        
        if (userDb is null) return Unauthorized();

        var classDb = await classRepository.GetClassByIdAsync(classId);

        if (classDb is null) return NotFound("Class with given ID was not found.");
        if (!classDb.OwnerId.Equals(userId)) return Unauthorized("Only owner of this class can remove students.");
        
        var studentDb = await classRepository.GetUserByEmailAsync(studentEmail);
        
        if (studentDb is null) return BadRequest("Student account with given email does not exist.");
        if (classDb.OwnerId.Equals(studentDb.Id)) return BadRequest("Owner can not be removed from class.");
        if (!classDb.ClassStudentList.Exists(cs => cs.StudentId.Equals(studentDb.Id))) return NotFound($"Student {userDb.Name} is no part of {classDb.Name} class therefore he can not be removed.");

        classDb
            .ClassStudentList
            .Remove(classDb.ClassStudentList
                .Find(cs => cs.StudentId.Equals(studentDb.Id)) 
                    ?? throw new InvalidOperationException());

        classRepository.UpdateEntity(classDb);

        return await classRepository.SaveChangesAsync() ? Ok() : Problem($"Error occured while removing user from {classDb.Name} class.");
    }
}