using Bunit;
using DTOs;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Blazor.Components.Pages;
using Blazor.Services;
using FluentAssertions;
using Xunit;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

public class HomePageTests : TestContext
{
    private readonly Mock<IStoryService> _mockStoryService;
    private readonly Mock<IJSRuntime> _mockJsRuntime;

    public HomePageTests()
    {
        _mockStoryService = new Mock<IStoryService>();
        _mockJsRuntime = new Mock<IJSRuntime>();

        Services.AddSingleton(_mockStoryService.Object);
        Services.AddSingleton(_mockJsRuntime.Object);
    }

    [Fact]
    public void ShowsLoadingIndicator_WhileFetching()
    {
        var tcs = new TaskCompletionSource<List<StoryDto>>();
        _mockStoryService.Setup(s => s.GetStoriesAsync()).Returns(tcs.Task);

        var cut = RenderComponent<Home>();

        cut.Markup.Should().Contain("Loading stories...");
    }

    [Fact]
    public void ShowsNoStoriesMessage_WhenEmptyListReturned()
    {
        _mockStoryService.Setup(s => s.GetStoriesAsync()).ReturnsAsync(new List<StoryDto>());

        var cut = RenderComponent<Home>();

        cut.Markup.Should().Contain("No stories found");
    }

    [Fact]
    public void RendersStories_WhenReturned()
    {
        var stories = new List<StoryDto>
        {
            new() { Id = 1, Title = "Test Story", Content = "Blah", DepartmentId = 2, CreatedAt = DateTime.UtcNow }
        };

        _mockStoryService.Setup(s => s.GetStoriesAsync()).ReturnsAsync(stories);

        var cut = RenderComponent<Home>();

        cut.Markup.Should().Contain("Test Story");
        cut.Markup.Should().Contain("Blah");
    }

    [Fact]
    public async Task DeletesStory_WhenConfirmed()
    {
        var stories = new List<StoryDto>
        {
            new() { Id = 1, Title = "To Delete", Content = "Blah", DepartmentId = 2, CreatedAt = DateTime.UtcNow }
        };

        _mockStoryService.SetupSequence(s => s.GetStoriesAsync())
            .ReturnsAsync(stories)
            .ReturnsAsync(new List<StoryDto>());

        _mockStoryService.Setup(s => s.DeleteStoryAsync(1)).Returns(Task.CompletedTask);

        _mockJsRuntime
            .Setup(js => js.InvokeAsync<bool>("confirm", It.IsAny<object[]>()))
            .ReturnsAsync(true);

        var cut = RenderComponent<Home>();

        var deleteButton = cut.Find("button.btn-outline-danger");
        await cut.InvokeAsync(() => deleteButton.Click());

        _mockStoryService.Verify(s => s.DeleteStoryAsync(1), Times.Once);
        cut.Markup.Should().Contain("No stories found");
    }

    [Fact]
    public async Task DoesNotDeleteStory_WhenCancelled()
    {
        var stories = new List<StoryDto>
        {
            new() { Id = 1, Title = "Don't Delete", Content = "Blah", DepartmentId = 2, CreatedAt = DateTime.UtcNow }
        };

        _mockStoryService.Setup(s => s.GetStoriesAsync()).ReturnsAsync(stories);

        _mockJsRuntime
            .Setup(js => js.InvokeAsync<bool>("confirm", It.IsAny<object[]>()))
            .ReturnsAsync(false);

        var cut = RenderComponent<Home>();

        var deleteButton = cut.Find("button.btn-outline-danger");
        await cut.InvokeAsync(() => deleteButton.Click());

        _mockStoryService.Verify(s => s.DeleteStoryAsync(It.IsAny<int>()), Times.Never);
        cut.Markup.Should().Contain("Don't Delete");
    }
}