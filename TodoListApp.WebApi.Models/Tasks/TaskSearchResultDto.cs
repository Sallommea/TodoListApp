namespace TodoListApp.WebApi.Models.Tasks;
public class TaskSearchResultDto
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public DateTime? DueDate { get; set; }

    public bool IsExpired { get; set; }
}
