using TodoListApp.WebApi.Models;

namespace TodoListApp.WebApp.Models;

public class TodoListViewModel
{
    public List<TodoListDto>? TodoLists { get; set; }

    public int TotalPages { get; set; }

    public int CurrentPage { get; set; }

    public int TotalRecord { get; set; }
}
