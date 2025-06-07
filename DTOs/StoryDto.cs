namespace DTOs;

public class StoryDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public DateTime CreatedAt { get; set; }
    public int DepartmentId { get; set; }
}