using Microsoft.EntityFrameworkCore;
using onlineCurses.Domain.Entities;

namespace onlineCurses.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public DbSet<Course> Courses { get; set; } = null!;
    public DbSet<Lesson> Lessons { get; set; } = null!;
    public DbSet<User> Users { get; set; } = null!;

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<Lesson>()
            .HasOne(l => l.Course)
            .WithMany(c => c.Lessons)
            .HasForeignKey(l => l.CourseId);

        // Soft delete global filters
        builder.Entity<Course>().HasQueryFilter(c => !c.IsDeleted);
        builder.Entity<Lesson>().HasQueryFilter(l => !l.IsDeleted);

        // Email único
        builder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();
    }
}