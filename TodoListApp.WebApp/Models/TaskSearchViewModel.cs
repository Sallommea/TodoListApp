using TodoListApp.WebApi.Models.Tasks;

namespace TodoListApp.WebApp.Models;

public class TaskSearchViewModel
{
    public string SearchText { get; set; } = string.Empty;

    public List<TaskSearchResultDto> Tasks { get; set; } = new List<TaskSearchResultDto>();

    public int CurrentPage { get; set; }

    public int? TotalPages { get; set; }

    public int? TotalRecords { get; set; }

    public int ItemsPerPage { get; set; }

    public bool SearchPerformed { get; set; }
}
