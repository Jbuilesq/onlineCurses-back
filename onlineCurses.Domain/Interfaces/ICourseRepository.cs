
using onlineCurses.Domain.Entities;

namespace onlineCurses.Domain.Interfaces;

public interface ICourseRepository
{
    Task<Course?> GetByIdAsync(int id);
    Task<IEnumerable<Course>> SearchAsync(string? query, CourseStatus? status, int page, int pageSize);
    Task AddAsync(Course course);
    Task UpdateAsync(Course course);
    Task SoftDeleteAsync(int id);
    Task<int> GetActiveLessonCountAsync(int courseId);
    Task<DateTime?> GetLastUpdatedAsync(int courseId);
}