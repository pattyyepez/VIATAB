using System.ComponentModel.DataAnnotations;

namespace Entities;

public class Department
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string Name { get; set; }

    // Navigation Property
    public ICollection<Story> Stories { get; set; }
}