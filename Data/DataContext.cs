using mathAi_backend.Models;
using Microsoft.EntityFrameworkCore;

namespace mathAi_backend.Data;

public class DataContext(IConfiguration config) : DbContext
{
    public virtual DbSet<User> User { get; set; }
    public virtual DbSet<Exercise> Exercise { get; set; }
    public virtual DbSet<ExerciseSet> ExerciseSet { get; set; }
    public virtual DbSet<Class> Class { get; set; }
    public virtual DbSet<ClassStudents> ClassStudents { get; set; }
    public virtual DbSet<Assignment> Assignment { get; set; }
    public virtual DbSet<AssignmentSubmission> AssignmentSubmissions { get; set; }
    public virtual DbSet<ExerciseAnswers> ExerciseAnswers { get; set; }


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (optionsBuilder.IsConfigured) return;

        optionsBuilder.UseSqlServer(config.GetConnectionString("AzureSQL"),
            o =>
            {
                o.EnableRetryOnFailure();
                o.CommandTimeout(180000);
            });
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("mathAi");

        // User and ExerciseSet
        modelBuilder.Entity<User>()
            .HasKey(u => u.Email);

        modelBuilder.Entity<ExerciseSet>()
            .HasKey(e => e.Id);

        modelBuilder.Entity<ExerciseSet>()
            .HasOne(es => es.User)
            .WithMany(u => u.ExerciseSets)
            .HasForeignKey(es => es.UserId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Exercise>()
            .HasOne(e => e.ExerciseSet)
            .WithMany(es => es.Exercises)
            .HasForeignKey(e => e.ExerciseSetId)
            .OnDelete(DeleteBehavior.Cascade);

        // Class and ClassStudents
        modelBuilder.Entity<Class>()
            .HasKey(c => c.Id);

        modelBuilder.Entity<Class>()
            .HasOne(c => c.Owner)
            .WithMany(u => u.Classes)
            .HasForeignKey(c => c.OwnerId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ClassStudents>()
            .HasKey(cs => new { cs.ClassId, cs.StudentId });

        modelBuilder.Entity<ClassStudents>()
            .HasOne(cs => cs.Class)
            .WithMany(c => c.ClassStudents)
            .HasForeignKey(cs => cs.ClassId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ClassStudents>()
            .HasOne(cs => cs.Student)
            .WithMany(u => u.StudentClasses)
            .HasForeignKey(cs => cs.StudentId)
            .OnDelete(DeleteBehavior.Restrict);  // Avoid multiple cascade paths

        // Assignment
        modelBuilder.Entity<Assignment>()
            .HasKey(a => a.Id);

        modelBuilder.Entity<Assignment>()
            .HasOne(a => a.Class)
            .WithMany(c => c.Assignments)
            .HasForeignKey(a => a.ClassId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Assignment>()
            .HasOne(a => a.ExerciseSet)
            .WithMany()
            .HasForeignKey(a => a.ExerciseSetId)
            .OnDelete(DeleteBehavior.Cascade);

        // AssignmentSubmission and ExerciseAnswer
        modelBuilder.Entity<AssignmentSubmission>()
            .HasKey(sub => sub.Id);

        modelBuilder.Entity<AssignmentSubmission>()
            .HasOne(sub => sub.Assignment)
            .WithMany(a => a.Submissions)
            .HasForeignKey(sub => sub.AssignmentId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<AssignmentSubmission>()
            .HasOne(sub => sub.Student)
            .WithMany(u => u.AssignmentSubmissions)
            .HasForeignKey(sub => sub.StudentId)
            .OnDelete(DeleteBehavior.Restrict);  // Prevent multiple cascade paths

        modelBuilder.Entity<ExerciseAnswers>()
            .HasKey(ea => ea.Id);

        modelBuilder.Entity<ExerciseAnswers>()
            .HasOne(ea => ea.AssignmentSubmission)
            .WithMany(sub => sub.ExerciseAnswers)
            .HasForeignKey(ea => ea.AssignmentSubmissionId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ExerciseAnswers>()
            .HasOne(ea => ea.Exercise)
            .WithMany(e => e.ExerciseAnswers)
            .HasForeignKey(ea => ea.ExerciseId)
            .OnDelete(DeleteBehavior.Restrict);  // Prevent multiple cascade paths
    }
}
