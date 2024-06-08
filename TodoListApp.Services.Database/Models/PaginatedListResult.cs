namespace TodoListApp.Services.Database.Models;
public class PaginatedListResult<T>
{
    public List<T>? ResultList { get; set; }

    public int? TotalPages { get; set; }

    public int? TotalRecords { get; set; }
}

public class PaginatedTodoListResult
{
    public TodoListEntity? TodoList { get; set; }

    public PaginatedListResult<TaskEntity>? PaginatedTasks { get; set; }
}
