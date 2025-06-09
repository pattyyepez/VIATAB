using DTOs;
namespace Blazor.Services;

public interface IStoryService
{

    Task<List<StoryDto>> GetStoriesAsync();
    
    Task CreateStoryAsync(CreateStoryDto story);
    Task DeleteStoryAsync(int id);

}