using DTOs;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApi.Controllers;
using WebApi.Services;
using Xunit;


public class StoryControllerTests
{
   private readonly Mock<IStoryService> _mockService;
   private readonly Mock<ILogger<StoryController>> _mockLogger;
   private readonly StoryController _controller;


   public StoryControllerTests()
   {
       _mockService = new Mock<IStoryService>();
       _mockLogger = new Mock<ILogger<StoryController>>();
       _controller = new StoryController(_mockService.Object, _mockLogger.Object);
   }


   [Fact]
   public async Task Create_ReturnsOk_WithCreatedStory()
   {
       // Arrange
       var input = new CreateStoryDto { Title = "New Story", Content = "Content" };
       var output = new StoryDto { Id = 1, Title = input.Title, Content = input.Content };
       _mockService.Setup(s => s.AddAsync(input)).ReturnsAsync(output);


       // Act
       var result = await _controller.Create(input);


       // Assert
       var ok = result.Should().BeOfType<OkObjectResult>().Subject;
       ok.Value.Should().BeEquivalentTo(output);
   }


   [Fact]
   public async Task Get_ReturnsOk_WithListOfStories()
   {
       // Arrange
       var stories = new List<StoryDto>
       {
           new StoryDto { Id = 1, Title = "A", Content = "X" },
           new StoryDto { Id = 2, Title = "B", Content = "Y" }
       };
       _mockService.Setup(s => s.GetAll()).ReturnsAsync(stories);


       // Act
       var result = await _controller.Get();


       // Assert
       var ok = result.Should().BeOfType<OkObjectResult>().Subject;
       ok.Value.Should().BeEquivalentTo(stories);
   }


   [Fact]
   public async Task GetById_ReturnsOk_WithStory()
   {
       // Arrange
       int id = 42;
       var story = new StoryDto { Id = id, Title = "Sample", Content = "Data" };
       _mockService.Setup(s => s.GetByIdAsync(id)).ReturnsAsync(story);


       // Act
       var result = await _controller.Get(id);


       // Assert
       var ok = result.Should().BeOfType<OkObjectResult>().Subject;
       ok.Value.Should().BeEquivalentTo(story);
   }


   [Fact]
   public async Task Update_ReturnsOk_WithUpdatedStory()
   {
       // Arrange
       int id = 1;
       var update = new UpdateStoryDto { Title = "Updated", Content = "Updated Content" };
       var updatedStory = new StoryDto { Id = id, Title = update.Title, Content = update.Content };
       _mockService.Setup(s => s.UpdateAsync(id, update)).ReturnsAsync(updatedStory);


       // Act
       var result = await _controller.Update(id, update);


       // Assert
       var ok = result.Should().BeOfType<OkObjectResult>().Subject;
       ok.Value.Should().BeEquivalentTo(updatedStory);
   }


   [Fact]
   public async Task Delete_ReturnsOk()
   {
       // Arrange
       int id = 5;
       _mockService.Setup(s => s.DeleteByIdAsync(id)).Returns(Task.CompletedTask);


       // Act
       var result = await _controller.Delete(id);


       // Assert
       result.Should().BeOfType<OkResult>();
       _mockService.Verify(s => s.DeleteByIdAsync(id), Times.Once);
   }
}

