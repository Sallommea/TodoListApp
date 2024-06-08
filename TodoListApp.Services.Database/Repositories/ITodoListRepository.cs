using TodoListApp.Services.Database.Models;

namespace TodoListApp.Services.Database.Repositories;
public interface ITodoListRepository
{
    Task<PaginatedListResult<TodoListEntity>> GetPaginatedTodoListsAsync(int pageNumber, int itemsPerPage);

    Task<PaginatedTodoListResult> GetTodoListWithTasksAsync(int todoListId, int taskPageNumber, int tasksPerPage);

    Task AddTodoListAsync(TodoListEntity todoList);

    Task UpdateTodoListAsync(TodoListEntity todoList);

    Task DeleteTodoListAsync(int id);

    Task SaveChangesAsync();
}
