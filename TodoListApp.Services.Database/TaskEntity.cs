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

    [ForeignKey("TodoListEntity")]
    public int TodoListId { get; set; }

    public TodoListEntity? TodoList { get; set; }

    public bool IsExpired { get; set; }

    public ICollection<TaskTagEntity> TaskTags { get; set; } = new List<TaskTagEntity>();

    public ICollection<CommentEntity> Comments { get; set; } = new List<CommentEntity>();

    [ForeignKey("User")]
    public string? UserId { get; set; }

    public User? User { get; set; }
}

public enum Status
{
    NotStarted,
    InProgress,
    Completed,
}
