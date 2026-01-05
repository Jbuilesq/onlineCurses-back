
using Moq;
using onlineCurses.Application.Services;
using onlineCurses.Domain.Entities;
using onlineCurses.Domain.Interfaces;

namespace onlineCurses.Tests;

public class LessonServiceTests
{
    private readonly Mock<ILessonRepository> _lessonRepoMock;
    private readonly LessonService _lessonService;

    public LessonServiceTests()
    {
        _lessonRepoMock = new Mock<ILessonRepository>();
        _lessonService = new LessonService(_lessonRepoMock.Object);
    }

    [Fact]
    public async Task CreateLesson_WithUniqueOrder_ShouldSucceed()
    {
        // Arrange
        var courseId = 1;
        var order = 1;

        _lessonRepoMock.Setup(r => r.IsOrderDuplicateAsync(courseId, order, null))
            .ReturnsAsync(false); // Orden libre

        // Act
        var result = await _lessonService.CreateAsync(courseId, "Lección 1", order);

        // Assert
        Assert.Equal(order, result.Order);
        Assert.Equal("Lección 1", result.Title);
        _lessonRepoMock.Verify(r => r.AddAsync(It.IsAny<Lesson>()), Times.Once);
    }

    [Fact]
    public async Task CreateLesson_WithDuplicateOrder_ShouldFail()
    {
        // Arrange
        var courseId = 1;
        var order = 1;

        _lessonRepoMock.Setup(r => r.IsOrderDuplicateAsync(courseId, order, null))
            .ReturnsAsync(true); // Orden ya usado

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(
            () => _lessonService.CreateAsync(courseId, "Lección duplicada", order));

        Assert.Contains("orden ya existe", exception.Message);
    }
}