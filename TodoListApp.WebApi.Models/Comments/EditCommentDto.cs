using System.ComponentModel.DataAnnotations;

namespace TodoListApp.WebApi.Models.Comments;
public class EditCommentDto
{
    [Required]
    public int CommentId { get; set; }

    [Required]
    public int TaskId { get; set; }

    [Required(ErrorMessage = "Content is required.")]
    [StringLength(1500, MinimumLength = 1, ErrorMessage = "Content must be between 1 and 1500 characters.")]
    public string Content { get; set; } = string.Empty;
}
