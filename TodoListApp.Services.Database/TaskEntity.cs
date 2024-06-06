using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TodoListApp.Services.Database;
public class TaskEntity
{
    [Key]
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
    public Status Status { get; set; }

    [Required]
    public string Assignee { get; set; } = string.Empty;

    [ForeignKey("TodoListEntity")]
    public int TodoListId { get; set; }

    public TodoListEntity? TodoList { get; set; }
}

public enum Status
{
    NotStarted,
    InProgress,
    Completed,
}
