namespace TodoListApp.WebApi.Models.Tasks;
public class TaskDto
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public DateTime? DueDate { get; set; }

    public Status Status { get; set; }
}

public enum Status
{
    NotStarted,
    InProgress,
    Completed,
}
