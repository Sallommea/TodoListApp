using TodoListApp.Services.Database;
using TodoListApp.Services.Models;
using TodoListApp.WebApi.Models;

namespace TodoListApp.Services.Interfaces;
public interface ITodoListService
{
    Task<PaginatedListResult<TodoListDto>> GetPaginatedTodoListsAsync(string userId, int pageNumber, int itemsPerPage);

    Task<TodoDetailsDto?> GetTodoListWithTasksAsync(int todoListId, string userId, int taskPageNumber, int tasksPerPage);

    Task<TodoListEntity> AddTodoListAsync(CreateTodoList todoList, string userId);

    Task UpdateTodoListAsync(UpdateTodo updateTodo, string userId);

    Task DeleteTodoListAsync(int id, string userId);
}
