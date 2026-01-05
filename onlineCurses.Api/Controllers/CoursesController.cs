using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using onlineCurses.Application.Services;
using onlineCurses.Domain.Entities;

namespace onlineCurses.Api.Controllers;

[Route("api/courses")]
[ApiController]
[Authorize]
public class CoursesController: ControllerBase
{
    private readonly CourseService _courseService;
        private readonly LessonService _lessonService;

        public CoursesController(CourseService courseService, LessonService lessonService)
        {
            _courseService = courseService;
            _lessonService = lessonService;
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string? q, [FromQuery] string? status, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            CourseStatus? parsedStatus = status != null ? Enum.Parse<CourseStatus>(status, true) : null;
            var courses = await _courseService.SearchAsync(q, parsedStatus, page, pageSize);
            return Ok(courses);
        }

        [HttpGet("{id}/summary")]
        public async Task<IActionResult> Summary(int id)
            => Ok(await _courseService.GetSummaryAsync(id));

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCourseRequest req)
            => Ok(await _courseService.CreateAsync(req.Title));

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateCourseRequest req)
        {
            await _courseService.UpdateAsync(id, req.Title);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _courseService.SoftDeleteAsync(id);
            return Ok();
        }

        [HttpPatch("{id}/publish")]
        public async Task<IActionResult> Publish(int id)
        {
            await _courseService.PublishAsync(id);
            return Ok();
        }

        [HttpPatch("{id}/unpublish")]
        public async Task<IActionResult> Unpublish(int id)
        {
            await _courseService.UnpublishAsync(id);
            return Ok();
        }

        // Lecciones
        [HttpGet("{courseId}/lessons")]
        public async Task<IActionResult> GetLessons(int courseId)
            => Ok(await _lessonService.GetByCourseAsync(courseId));

        [HttpPost("{courseId}/lessons")]
        public async Task<IActionResult> CreateLesson(int courseId, [FromBody] CreateLessonRequest req)
            => Ok(await _lessonService.CreateAsync(courseId, req.Title, req.Order));

        [HttpPut("lessons/{id}")]
        public async Task<IActionResult> UpdateLesson(int id, [FromBody] UpdateLessonRequest req)
        {
            await _lessonService.UpdateAsync(id, req.Title, req.Order);
            return Ok();
        }

        [HttpDelete("lessons/{id}")]
        public async Task<IActionResult> DeleteLesson(int id)
        {
            await _lessonService.SoftDeleteAsync(id);
            return Ok();
        }
    }

    public class CreateCourseRequest { public string Title { get; set; } = string.Empty; }
    public class UpdateCourseRequest { public string Title { get; set; } = string.Empty; }
    public class CreateLessonRequest { public string Title { get; set; } = string.Empty; public int Order { get; set; } }
    public class UpdateLessonRequest { public string Title { get; set; } = string.Empty; public int Order { get; set; } }

