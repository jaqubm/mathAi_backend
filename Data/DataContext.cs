using mathAi_backend.Models;
using Microsoft.EntityFrameworkCore;

namespace mathAi_backend.Data;

public class DataContext(IConfiguration config) : DbContext
{
    public virtual DbSet<User> User { get; set; }
    public virtual DbSet<Exercise> Exercise { get; set; }
    public virtual DbSet<ExerciseSet> ExerciseSet { get; set; }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Returning if connection is already set up
        if (optionsBuilder.IsConfigured) return;
        
        // Setting up connection with DB
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
        
        // Connecting Entities with DB
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
    }
}