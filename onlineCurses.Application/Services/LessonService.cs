using onlineCurses.Application.DTOs;
using onlineCurses.Domain.Entities;
using onlineCurses.Domain.Interfaces;

namespace onlineCurses.Application.Services;

public class LessonService
{
    private readonly ILessonRepository _lessonRepository;

    public LessonService(ILessonRepository lessonRepository)
    {
        _lessonRepository = lessonRepository;
    }

    public async Task<IEnumerable<LessonDto>> GetByCourseAsync(int courseId)
    {
        var lessons = await _lessonRepository.GetByCourseIdOrderedAsync(courseId);
        return lessons.Select(l => new LessonDto
        {
            Id = l.Id,
            CourseId = l.CourseId,
            Title = l.Title,
            Order = l.Order
        });
    }

    public async Task<LessonDto> CreateAsync(int courseId, string title, int order)
    {
        if (await _lessonRepository.IsOrderDuplicateAsync(courseId, order))
            throw new Exception("El orden ya existe en este curso");

        var lesson = new Lesson { CourseId = courseId, Title = title, Order = order };
        await _lessonRepository.AddAsync(lesson);

        return new LessonDto { Id = lesson.Id, CourseId = courseId, Title = title, Order = order };
    }

    public async Task UpdateAsync(int id, string title, int order)
    {
        var lesson = await _lessonRepository.GetByIdAsync(id) ?? throw new Exception("Lección no encontrada");

        if (lesson.Order != order && await _lessonRepository.IsOrderDuplicateAsync(lesson.CourseId, order, id))
            throw new Exception("El nuevo orden ya está en uso");

        lesson.Title = title;
        lesson.Order = order;
        lesson.UpdatedAt = DateTime.UtcNow;
        await _lessonRepository.UpdateAsync(lesson);
    }

    public async Task SoftDeleteAsync(int id)
    {
        await _lessonRepository.SoftDeleteAsync(id);
    }
}