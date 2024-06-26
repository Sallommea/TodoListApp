using System.ComponentModel.DataAnnotations;

namespace TodoListApp.WebApi.Models.Comments;
public class AddCommentDto
{
    [Required]
    public int TaskId { get; set; }

    [Required(ErrorMessage = "Content is required.")]
    public string Content { get; set; } = string.Empty;

    public string? UserName { get; set; }
}
