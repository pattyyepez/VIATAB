using DTOs;
using Entities;

namespace WebApi.Services;

public interface IStoryService
{ 
    Task<StoryDto> AddAsync(CreateStoryDto story);
    Task<StoryDto> GetByIdAsync(int id);
    Task<StoryDto> UpdateAsync(int id, UpdateStoryDto story);
    Task DeleteByIdAsync(int id);
    Task<List<StoryDto>> GetAll();

}