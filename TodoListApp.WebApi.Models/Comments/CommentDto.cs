namespace TodoListApp.WebApi.Models.Comments;
public class CommentDto
{
    public int Id { get; set; }

    public string Content { get; set; } = string.Empty;

    public DateTime CreatedDate { get; set; }

    public string UserFirstName { get; set; } = string.Empty;

    public string UserLastName { get; set; } = string.Empty;
}
