using TodoListApp.WebApi.Models.Tasks;

namespace TodoListApp.WebApi.Models;
public class TodoDetailsDto
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public ICollection<TaskDto> Tasks { get; set; } = new List<TaskDto>();

    public int? TotalTasks { get; set; }

    public int? TotalTaskPages { get; set; }

    public int? CurrentTaskPage { get; set; }
}
