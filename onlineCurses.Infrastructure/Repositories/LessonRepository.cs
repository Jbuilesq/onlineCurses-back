using Microsoft.EntityFrameworkCore;
using onlineCurses.Domain.Entities;
using onlineCurses.Domain.Interfaces;
using onlineCurses.Infrastructure.Data;

namespace onlineCurses.Infrastructure.Repositories;

public class LessonRepository : ILessonRepository
{
    private readonly AppDbContext _context;

    public LessonRepository(AppDbContext context) => _context = context;

    public async Task<Lesson?> GetByIdAsync(int id)
        => await _context.Lessons.FirstOrDefaultAsync(l => l.Id == id);

    public async Task<IEnumerable<Lesson>> GetByCourseIdOrderedAsync(int courseId)
        => await _context.Lessons
            .Where(l => l.CourseId == courseId)
            .OrderBy(l => l.Order)
            .ToListAsync();

    public async Task AddAsync(Lesson lesson)
    {
        _context.Lessons.Add(lesson);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Lesson lesson)
    {
        _context.Lessons.Update(lesson);
        await _context.SaveChangesAsync();
    }

    public async Task SoftDeleteAsync(int id)
    {
        var lesson = await GetByIdAsync(id);
        if (lesson != null)
        {
            lesson.IsDeleted = true;
            lesson.UpdatedAt = DateTime.UtcNow;
            await UpdateAsync(lesson);
        }
    }

    public async Task<bool> IsOrderDuplicateAsync(int courseId, int order, int? excludeLessonId = null)
    {
        var query = _context.Lessons.Where(l => l.CourseId == courseId && l.Order == order);
        if (excludeLessonId.HasValue)
            query = query.Where(l => l.Id != excludeLessonId.Value);

        return await query.AnyAsync();
    }
}