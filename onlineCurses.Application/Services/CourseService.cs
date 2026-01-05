using onlineCurses.Application.DTOs;
using onlineCurses.Domain.Entities;
using onlineCurses.Domain.Interfaces;

namespace onlineCurses.Application.Services;

public class CourseService
{
    private readonly ICourseRepository _courseRepository;
    private readonly ILessonRepository _lessonRepository;

    public CourseService(ICourseRepository courseRepository, ILessonRepository lessonRepository)
    {
        _courseRepository = courseRepository;
        _lessonRepository = lessonRepository;
    }

    public async Task<IEnumerable<CourseDto>> SearchAsync(string? q, CourseStatus? status, int page, int pageSize)
    {
        var courses = await _courseRepository.SearchAsync(q, status, page, pageSize);
        return courses.Select(c => new CourseDto { Id = c.Id, Title = c.Title, Status = c.Status });
    }

    public async Task<CourseDto> CreateAsync(string title)
    {
        var course = new Course { Title = title };
        await _courseRepository.AddAsync(course);
        return new CourseDto { Id = course.Id, Title = course.Title, Status = course.Status };
    }

    public async Task UpdateAsync(int id, string title)
    {
        var course = await _courseRepository.GetByIdAsync(id) ?? throw new Exception("Curso no encontrado");
        course.Title = title;
        course.UpdatedAt = DateTime.UtcNow;
        await _courseRepository.UpdateAsync(course);
    }

    public async Task SoftDeleteAsync(int id)
    {
        await _courseRepository.SoftDeleteAsync(id);
    }

    public async Task PublishAsync(int id)
    {
        var course = await _courseRepository.GetByIdAsync(id) ?? throw new Exception("Curso no encontrado");
        var lessonCount = await _courseRepository.GetActiveLessonCountAsync(id);
        if (lessonCount == 0)
            throw new Exception("No se puede publicar un curso sin lecciones activas");

        course.Status = CourseStatus.Published;
        course.UpdatedAt = DateTime.UtcNow;
        await _courseRepository.UpdateAsync(course);
    }

    public async Task UnpublishAsync(int id)
    {
        var course = await _courseRepository.GetByIdAsync(id) ?? throw new Exception("Curso no encontrado");
        course.Status = CourseStatus.Draft;
        course.UpdatedAt = DateTime.UtcNow;
        await _courseRepository.UpdateAsync(course);
    }

    public async Task<CourseSummaryDto> GetSummaryAsync(int id)
    {
        var course = await _courseRepository.GetByIdAsync(id) ?? throw new Exception("Curso no encontrado");
        var lessonCount = await _courseRepository.GetActiveLessonCountAsync(id);
        var lastUpdated = await _courseRepository.GetLastUpdatedAsync(id) ?? course.UpdatedAt;

        return new CourseSummaryDto
        {
            Id = course.Id,
            Title = course.Title,
            TotalLessons = lessonCount,
            LastModified = lastUpdated
        };
    }
}