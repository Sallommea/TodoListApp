namespace TodoListApp.Services.Models;
public class TodoList
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public DateTime CreatedDate { get; set; }

    public DateTime? DueDate { get; set; }

    public int TaskCount { get; set; }

    public bool IsShared { get; set; }
}
