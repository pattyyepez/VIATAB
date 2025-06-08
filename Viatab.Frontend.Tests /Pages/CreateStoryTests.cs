using Bunit;
using DTOs;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Moq;
using Blazor.Services;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Blazor.Components.Pages;

public class CreateStoryTests : TestContext
{
    private readonly Mock<IStoryService> _mockStoryService;
    private readonly Mock<NavigationManager> _mockNavManager;

    public CreateStoryTests()
    {
        _mockStoryService = new Mock<IStoryService>();
        _mockNavManager = new Mock<NavigationManager>();

        // Register mocks
        Services.AddSingleton(_mockStoryService.Object);
        Services.AddSingleton<NavigationManager>(_mockNavManager.Object);
    }

    [Fact]
    public void RendersFormCorrectly()
    {
        var cut = RenderComponent<Create>();

        var labels = cut.FindAll("label").Select(l => l.TextContent).ToList();

        labels.Should().Contain("Title");
        labels.Should().Contain("Content");
        labels.Should().Contain("Department ID");

        cut.Find("button.btn-success").TextContent.Should().Contain("Submit");
        cut.Find("button.btn-secondary").TextContent.Should().Contain("Cancel");
    }
}
