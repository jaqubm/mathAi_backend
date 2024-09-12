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
            o => o.EnableRetryOnFailure());
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("mathAi");
        
        // Connecting Entities with DB
        modelBuilder.Entity<User>()
            .HasKey(u => u.Email);
        
        modelBuilder.Entity<Exercise>()
            .HasOne(e => e.ExerciseSet)
            .WithMany(es => es.Exercises)
            .HasForeignKey(e => e.ExerciseSetId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ExerciseSet>()
            .HasKey(e => e.ExerciseSetId);
    }
}