using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TodoListApp.Services.Database;
public class TagEntity
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    public ICollection<TaskTagEntity> TaskTags { get; set; } = new List<TaskTagEntity>();

    [ForeignKey("User")]
    public string? UserId { get; set; }

    public User? User { get; set; }
}
