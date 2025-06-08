using Blazor.Services;
using DTOs;
using Moq;
using Moq.Protected;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;

public class StoryServiceTests
{
    private static HttpClient CreateMockHttpClient(HttpResponseMessage responseMessage, Action<HttpRequestMessage>? onRequest = null)
    {
        var handler = new Mock<HttpMessageHandler>(MockBehavior.Strict);

        handler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync((HttpRequestMessage request, CancellationToken _) =>
            {
                onRequest?.Invoke(request);
                return responseMessage;
            });

        return new HttpClient(handler.Object)
        {
            BaseAddress = new Uri("http://localhost/")
        };
    }

    [Fact]
    public async Task GetStoriesAsync_ReturnsList_WhenSuccessful()
    {
        // Arrange
        var expected = new List<StoryDto>
        {
            new() { Id = 1, Title = "Test", Content = "Story", DepartmentId = 1, CreatedAt = DateTime.UtcNow }
        };

        var json = JsonSerializer.Serialize(expected);
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json")
        };

        var http = CreateMockHttpClient(response);
        var service = new StoryService(http);

        // Act
        var result = await service.GetStoriesAsync();

        // Assert
        result.Should().HaveCount(1);
        result[0].Title.Should().Be("Test");
    }

    [Fact]
    public async Task GetStoriesAsync_ReturnsEmptyList_OnFailure()
    {
        var response = new HttpResponseMessage(HttpStatusCode.InternalServerError);
        var http = CreateMockHttpClient(response);
        var service = new StoryService(http);

        var result = await service.GetStoriesAsync();

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task CreateStoryAsync_SendsCorrectRequest()
    {
        CreateStoryDto? sentDto = null;

        var response = new HttpResponseMessage(HttpStatusCode.Created);
        var http = CreateMockHttpClient(response, req =>
        {
            sentDto = req.Content!.ReadFromJsonAsync<CreateStoryDto>().Result;
        });

        var service = new StoryService(http);
        var dto = new CreateStoryDto
        {
            Title = "New",
            Content = "Content",
            DepartmentId = 3
        };

        await service.CreateStoryAsync(dto);

        sentDto.Should().NotBeNull();
        sentDto!.Title.Should().Be("New");
    }

    [Fact]
    public async Task DeleteStoryAsync_SendsCorrectRequest()
    {
        HttpRequestMessage? sentRequest = null;

        var response = new HttpResponseMessage(HttpStatusCode.NoContent);
        var http = CreateMockHttpClient(response, req => sentRequest = req);

        var service = new StoryService(http);

        await service.DeleteStoryAsync(5);

        sentRequest.Should().NotBeNull();
        sentRequest!.Method.Should().Be(HttpMethod.Delete);
        sentRequest.RequestUri!.ToString().Should().EndWith("api/stories/5");
    }
}
