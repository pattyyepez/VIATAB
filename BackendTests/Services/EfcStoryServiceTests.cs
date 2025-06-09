using WebApi.Services;
using Database;
using DTOs;
using Entities;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;


public class EfcStoryServiceTests
{ 
    private readonly Mock<ITabloidDataAccess> _dataAccessMock;
   private readonly EfcStoryService _service;


   public EfcStoryServiceTests()
   {
       _dataAccessMock = new Mock<ITabloidDataAccess>();
       var loggerMock = new Mock<ILogger<EfcStoryService>>();
       _service = new EfcStoryService(_dataAccessMock.Object, loggerMock.Object);
   }


   [Fact]
   public async Task AddAsync_ShouldReturnStoryDto_WhenValidInput()
   {
       // Arrange
       var input = new CreateStoryDto { Title = "Test", Content = "Content", DepartmentId = 1 };
       var savedStory = new Story
       {
           Id = 123,
           Title = input.Title,
           Content = input.Content,
           DepartmentId = input.DepartmentId,
           CreatedAt = DateTime.UtcNow
       };


       _dataAccessMock
           .Setup(d => d.CreateStoryAsync(It.IsAny<Story>()))
           .ReturnsAsync(savedStory);


       // Act
       var result = await _service.AddAsync(input);


       // Assert
       result.Id.Should().Be(123);
       result.Title.Should().Be(input.Title);
       result.Content.Should().Be(input.Content);
       result.DepartmentId.Should().Be(input.DepartmentId);
   }


   [Fact]
   public async Task GetByIdAsync_ShouldReturnDto_WhenStoryExists()
   {
       var story = new Story
       {
           Id = 10,
           Title = "X",
           Content = "Y",
           DepartmentId = 1,
           CreatedAt = DateTime.UtcNow
       };


       _dataAccessMock.Setup(d => d.GetStoryByIdAsync(10)).ReturnsAsync(story);


       var result = await _service.GetByIdAsync(10);


       result.Id.Should().Be(10);
       result.Title.Should().Be("X");
   }


   [Fact]
   public async Task UpdateAsync_ShouldReturnUpdatedDto()
   {
       var update = new UpdateStoryDto { Title = "New", Content = "Updated" };
       var updated = new Story
       {
           Id = 7,
           Title = "New",
           Content = "Updated",
           DepartmentId = 1,
           CreatedAt = DateTime.UtcNow
       };


       _dataAccessMock.Setup(d => d.UpdateStoryAsync(7, It.IsAny<Story>())).ReturnsAsync(updated);


       var result = await _service.UpdateAsync(7, update);


       result.Title.Should().Be("New");
   }


   [Fact]
   public async Task DeleteByIdAsync_ShouldCallDataAccess()
   {
       _dataAccessMock.Setup(d => d.DeleteStoryAsync(5)).Returns(Task.CompletedTask);


       await _service.DeleteByIdAsync(5);


       _dataAccessMock.Verify(d => d.DeleteStoryAsync(5), Times.Once);
   }


   [Fact]
   public async Task GetAll_ShouldReturnListOfDtos()
   {
       var stories = new List<Story>
       {
           new() { Id = 1, Title = "A", Content = "C", DepartmentId = 1, CreatedAt = DateTime.UtcNow },
           new() { Id = 2, Title = "B", Content = "D", DepartmentId = 2, CreatedAt = DateTime.UtcNow }
       };


       _dataAccessMock.Setup(d => d.GetAllStoriesAsync(null)).ReturnsAsync(stories);


       var result = await _service.GetAll();


       result.Should().HaveCount(2);
       result[0].Title.Should().Be("A");
   }
}

