namespace TodoListApp.WebApi.Models.Tasks;
public class UpdateTaskDto
{
    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public DateTime DueDate { get; set; }

    public Status Status { get; set; }
}
