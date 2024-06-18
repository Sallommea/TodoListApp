namespace TodoListApp.WebApi.Models.Tasks;
public class AssignedTasksdto
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
}
