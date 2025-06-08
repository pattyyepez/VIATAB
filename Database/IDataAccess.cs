using Entities;

namespace Database;

public interface ITabloidDataAccess
{
    Task<Story> CreateStoryAsync(Story story);
    Task<List<Story>> GetAllStoriesAsync(int? departmentId = null);
    Task<Story> GetStoryByIdAsync(int id);
    Task<Story> UpdateStoryAsync(int id, Story updated);
    Task DeleteStoryAsync(int id);
    Task<List<Department>> GetDepartmentsAsync();
}