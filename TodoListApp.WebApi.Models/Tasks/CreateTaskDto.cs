namespace TodoListApp.WebApi.Models.Tasks;
public class CreateTaskDto
{
    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public DateTime? DueDate { get; set; }

    public int TodoListId { get; set; }
}
