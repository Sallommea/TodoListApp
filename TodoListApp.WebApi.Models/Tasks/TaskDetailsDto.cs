using TodoListApp.WebApi.Models.Comments;
using TodoListApp.WebApi.Models.Tags;

namespace TodoListApp.WebApi.Models.Tasks;
public class TaskDetailsDto
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public DateTime CreatedDate { get; set; }

    public DateTime? DueDate { get; set; }

    public Status Status { get; set; }

    public string Assignee { get; set; } = string.Empty;

    public int TodoListId { get; set; }

    public bool IsExpired { get; set; }

    public List<TagDto> Tags { get; set; } = new List<TagDto>();

    public List<CommentDto> Comments { get; set; } = new List<CommentDto>();
}
