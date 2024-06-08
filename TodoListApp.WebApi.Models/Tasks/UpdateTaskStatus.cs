namespace TodoListApp.WebApi.Models.Tasks;
public class UpdateTaskStatus
{
    public int TaskId { get; set; }

    public Status Status { get; set; }
}
