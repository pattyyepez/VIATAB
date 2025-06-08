using Microsoft.EntityFrameworkCore;
using Entities;

namespace Database;

public class TabloidDataAccess : ITabloidDataAccess
{
    private readonly ViaTabloidDbContext _context;

    public TabloidDataAccess(ViaTabloidDbContext context)
    {
        _context = context;
    }

    public async Task<Story> CreateStoryAsync(Story story)
    {
        if (string.IsNullOrWhiteSpace(story.Title))
            throw new ArgumentException("Story title cannot be empty.");

        try
        {
            var existing = await _context.Stories.FirstOrDefaultAsync(s =>
                s.Title == story.Title && s.DepartmentId == story.DepartmentId);

            if (existing != null)
            {
                Console.WriteLine($"Story '{story.Title}' already exists.");
                return existing;
            }

            _context.Stories.Add(story);
            await _context.SaveChangesAsync();
            return story;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unexpected error: {ex.Message}");
            throw new Exception("An unexpected error occurred while creating story.");
        }
    }

    public async Task<List<Story>> GetAllStoriesAsync(int? departmentId = null)
    {
        try
        {
            var query = _context.Stories.Include(s => s.Department).AsQueryable();

            if (departmentId.HasValue)
                query = query.Where(s => s.DepartmentId == departmentId);

            var result = await query.ToListAsync();

            if (result.Count == 0)
                throw new KeyNotFoundException("No stories found matching the criteria.");

            return result;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching stories: {ex.Message}");
            throw;
        }
    }

    public async Task<Story> GetStoryByIdAsync(int id)
    {
        var story = await _context.Stories.Include(s => s.Department).FirstOrDefaultAsync(s => s.Id == id);
        return story ?? throw new KeyNotFoundException($"Story with ID {id} not found.");
    }

    public async Task<Story> UpdateStoryAsync(int id, Story updated)
    {
        var existing = await _context.Stories.FindAsync(id);
        if (existing == null) throw new KeyNotFoundException($"Story with ID {id} not found.");

        existing.Title = updated.Title;
        existing.Content = updated.Content;

        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task DeleteStoryAsync(int id)
    {
        var story = await _context.Stories.FindAsync(id);
        if (story == null) throw new KeyNotFoundException($"Story with ID {id} not found.");

        _context.Stories.Remove(story);
        await _context.SaveChangesAsync();
    }

    public async Task<List<Department>> GetDepartmentsAsync()
    {
        return await _context.Departments.Include(d => d.Stories).ToListAsync();
    }
}

