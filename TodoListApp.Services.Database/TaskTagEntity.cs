namespace TodoListApp.Services.Database;
public class TaskTagEntity
{
    public int TaskId { get; set; }

    public TaskEntity Task { get; set; } = null!;

    public int TagId { get; set; }

    public TagEntity Tag { get; set; } = null!;
}
