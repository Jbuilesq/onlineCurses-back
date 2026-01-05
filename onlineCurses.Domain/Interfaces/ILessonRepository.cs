using onlineCurses.Domain.Entities;

namespace onlineCurses.Domain.Interfaces;

public interface ILessonRepository
{
    Task<Lesson?> GetByIdAsync(int id);
    Task<IEnumerable<Lesson>> GetByCourseIdOrderedAsync(int courseId);
    Task AddAsync(Lesson lesson);
    Task UpdateAsync(Lesson lesson);
    Task SoftDeleteAsync(int id);
    Task<bool> IsOrderDuplicateAsync(int courseId, int order, int? excludeLessonId = null);

}