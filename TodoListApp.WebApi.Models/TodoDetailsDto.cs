using TodoListApp.WebApi.Models.Tasks;

namespace TodoListApp.WebApi.Models;
public class TodoDetailsDto
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public int TaskCount { get; set; }

    public ICollection<TaskDto> Tasks { get; set; } = new List<TaskDto>();
}
