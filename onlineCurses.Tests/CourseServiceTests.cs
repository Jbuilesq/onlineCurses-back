using System;
using System.Threading.Tasks;
using Xunit;
using Moq;
using onlineCurses.Application.Services;
using onlineCurses.Domain.Entities;
using onlineCurses.Domain.Interfaces;

namespace onlineCurses.Tests;

public class CourseServiceTests
{
    
    private readonly Mock<ICourseRepository> _courseRepoMock;
    private readonly Mock<ILessonRepository> _lessonRepoMock;
    private readonly CourseService _courseService;

    public CourseServiceTests()
    {
        _courseRepoMock = new Mock<ICourseRepository>();
        _lessonRepoMock = new Mock<ILessonRepository>();
        _courseService = new CourseService(_courseRepoMock.Object, _lessonRepoMock.Object);
    }

    [Fact]
    public async Task PublishCourse_WithLessons_ShouldSucceed()
    {
        // Arrange
        var courseId = 1;
        var course = new Course { Id = courseId, Title = "Test Course", Status = CourseStatus.Draft };

        _courseRepoMock.Setup(r => r.GetByIdAsync(courseId))
                       .ReturnsAsync(course);

        _courseRepoMock.Setup(r => r.GetActiveLessonCountAsync(courseId))
                       .ReturnsAsync(1);

        // Act
        await _courseService.PublishAsync(courseId);

        // Assert
        Assert.Equal(CourseStatus.Published, course.Status);
        _courseRepoMock.Verify(r => r.UpdateAsync(course), Times.Once);
    }

    [Fact]
    public async Task PublishCourse_WithoutLessons_ShouldFail()
    {
        // Arrange
        var courseId = 1;
        var course = new Course { Id = courseId, Status = CourseStatus.Draft };

        _courseRepoMock.Setup(r => r.GetByIdAsync(courseId))
                       .ReturnsAsync(course);

        _courseRepoMock.Setup(r => r.GetActiveLessonCountAsync(courseId))
                       .ReturnsAsync(0);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(() => _courseService.PublishAsync(courseId));
        Assert.Contains("lecciones", exception.Message);
        Assert.Equal(CourseStatus.Draft, course.Status);
    }

    [Fact]
    public async Task DeleteCourse_ShouldBeSoftDelete()
    {
        // Arrange
        var courseId = 1;
        var course = new Course { Id = courseId, IsDeleted = false };

        _courseRepoMock.Setup(r => r.GetByIdAsync(courseId))
                       .ReturnsAsync(course);

        // ← Aquí está la clave: mockear SoftDeleteAsync
        _courseRepoMock.Setup(r => r.SoftDeleteAsync(courseId))
                       .Returns(Task.CompletedTask)
                       .Callback(() => course.IsDeleted = true); // Simulamos el cambio

        // Act
        await _courseService.SoftDeleteAsync(courseId);

        // Assert
        Assert.True(course.IsDeleted);
        _courseRepoMock.Verify(r => r.SoftDeleteAsync(courseId), Times.Once);
    }
}