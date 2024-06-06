using System.ComponentModel.DataAnnotations;

namespace TodoListApp.Services.Models;
public class TodoList
{
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;

    public int TaskCount { get; set; }

    public bool IsShared { get; set; }

    public ICollection<TodoTask>? Tasks { get; set; }
}
