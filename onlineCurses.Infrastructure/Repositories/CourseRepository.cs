using Microsoft.EntityFrameworkCore;
using onlineCurses.Domain.Entities;
using onlineCurses.Domain.Interfaces;
using onlineCurses.Infrastructure.Data;

namespace onlineCurses.Infrastructure.Repositories;

public class CourseRepository : ICourseRepository
{
    private readonly AppDbContext _context;

        public CourseRepository(AppDbContext context) => _context = context;

        public async Task<Course?> GetByIdAsync(int id)
            => await _context.Courses.Include(c => c.Lessons).FirstOrDefaultAsync(c => c.Id == id);

        public async Task<IEnumerable<Course>> SearchAsync(string? query, CourseStatus? status, int page, int pageSize)
        {
            var q = _context.Courses.AsQueryable();
            if (!string.IsNullOrWhiteSpace(query))
                q = q.Where(c => c.Title.Contains(query));
            if (status.HasValue)
                q = q.Where(c => c.Status == status.Value);

            return await q.OrderBy(c => c.Title)
                          .Skip((page - 1) * pageSize)
                          .Take(pageSize)
                          .ToListAsync();
        }

        public async Task AddAsync(Course course)
        {
            _context.Courses.Add(course);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Course course)
        {
            _context.Courses.Update(course);
            await _context.SaveChangesAsync();
        }

        public async Task SoftDeleteAsync(int id)
        {
            var course = await GetByIdAsync(id);
            if (course != null)
            {
                course.IsDeleted = true;
                course.UpdatedAt = DateTime.UtcNow;
                await UpdateAsync(course);
            }
        }

        public async Task<int> GetActiveLessonCountAsync(int courseId)
            => await _context.Lessons.CountAsync(l => l.CourseId == courseId);

        public async Task<DateTime?> GetLastUpdatedAsync(int courseId)
        {
            var course = await GetByIdAsync(courseId);
            var lessonMax = await _context.Lessons
                .Where(l => l.CourseId == courseId)
                .MaxAsync(l => (DateTime?)l.UpdatedAt);

            return new[] { course?.UpdatedAt, lessonMax }.Max();
        }
}