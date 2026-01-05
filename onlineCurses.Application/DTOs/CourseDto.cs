using onlineCurses.Domain.Entities;

namespace onlineCurses.Application.DTOs;

public class CourseDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public CourseStatus Status { get; set; }
}
public class CourseSummaryDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public int TotalLessons { get; set; }
    public DateTime LastModified { get; set; }
}