using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TodoListApp.Services.Database;
public class CommentEntity
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(1000)]
    public string Content { get; set; } = string.Empty;

    [Required]
    public DateTime CreatedDate { get; set; }

    [Required]
    public string UserName { get; set; } = "Anonymous";

    public string? UserId { get; set; }

    [ForeignKey("TaskEntity")]
    public int TaskId { get; set; }

    public TaskEntity? Task { get; set; }
}
