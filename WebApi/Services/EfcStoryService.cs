using Database;
using DTOs;
using Entities;

namespace WebApi.Services;

public class EfcStoryService : IStoryService
{
    private readonly TabloidDataAccess _access;
    private readonly ILogger<EfcStoryService> _logger;
    
    public EfcStoryService(ViaTabloidDbContext dbContext, ILogger<EfcStoryService> logger)
    {
        _access = new TabloidDataAccess(dbContext);
        _logger = logger;
    }

    public async Task<StoryDto> AddAsync(CreateStoryDto story)
    {
        Story temp = new Story()
        {
            Title = story.Title,
            Content = story.Content,
            DepartmentId = story.DepartmentId,
        };
        
        temp = await _access.CreateStoryAsync(temp);
        
        return ToDto(temp);
    }

    public async Task<StoryDto> GetByIdAsync(int id)
    {
        return ToDto(await _access.GetStoryByIdAsync(id));
    }

    public async Task<StoryDto> UpdateAsync(int id, UpdateStoryDto story)
    {
        Story temp = new Story()
        {
            Title = story.Title,
            Content = story.Content,
        };
        return ToDto(await _access.UpdateStoryAsync(id, temp));
    }

    public async Task DeleteByIdAsync(int id)
    {
        await _access.DeleteStoryAsync(id);
    }

    public async Task<List<StoryDto>> GetAll()
    {
        return (await _access.GetAllStoriesAsync())
            .Select(ToDto)
            .ToList();
    }

    private StoryDto ToDto(Story story)
    {
        return new StoryDto()
        {
            Id = story.Id,
            Title = story.Title,
            Content = story.Content,
            CreatedAt = story.CreatedAt,
            DepartmentId = story.DepartmentId,
        };
    }
}