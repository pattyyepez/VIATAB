using DTOs;

namespace Blazor.Services;

public class StoryService : IStoryService
{
    private readonly HttpClient _http;

    public StoryService(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<StoryDto>> GetStoriesAsync()
    {
        try
        {
            return await _http.GetFromJsonAsync<List<StoryDto>>("api/stories");
        }
        catch (Exception ex)
        {
            return new List<StoryDto>();
        }
    }


    public async Task CreateStoryAsync(CreateStoryDto story)
    {
        var response = await _http.PostAsJsonAsync("api/stories", story);
        response.EnsureSuccessStatusCode();
    }


    public async Task DeleteStoryAsync(int id)
    {
        await _http.DeleteAsync($"api/stories/{id}");

    }
}