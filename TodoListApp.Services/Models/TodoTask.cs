using System.ComponentModel.DataAnnotations;

namespace TodoListApp.Services.Models;
public class TodoTask
{
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;

    [Required]
    public DateTime CreatedDate { get; set; }

    public DateTime? DueDate { get; set; }

    [Required]
    public TaskStatus Status { get; set; }

    [Required]
    public string Assignee { get; set; } = string.Empty;

    public int TodoListId { get; set; }
}
