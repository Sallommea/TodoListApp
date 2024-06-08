using TodoListApp.Services.Database;
using TodoListApp.Services.Models;
using TodoListApp.WebApi.Models;

namespace TodoListApp.Services.Interfaces;
public interface ITodoListService
{
    Task<PaginatedListResult<TodoListDto>> GetPaginatedTodoListsAsync(int pageNumber, int itemsPerPage);

    Task<TodoDetailsDto?> GetTodoListWithTasksAsync(int todoListId, int taskPageNumber, int tasksPerPage);

    Task<TodoListEntity> AddTodoListAsync(CreateTodoList todoList);

    Task UpdateTodoListAsync(UpdateTodo updateTodo);

    Task DeleteTodoListAsync(int id);
}
