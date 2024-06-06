using System.ComponentModel.DataAnnotations;

namespace TodoListApp.Services.Database;
public class TodoListEntity
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;

    public int TaskCount { get; set; }

    public ICollection<TaskEntity>? Tasks { get; set; }
}
