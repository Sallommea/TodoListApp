using TodoListApp.Services.Models;

namespace TodoListApp.Services.Interfaces;
public interface ITodoListService
{
    Task<IEnumerable<TodoList>> GetAllTodoListsAsync();

    Task AddTodoListAsync(TodoList todoList);

    Task UpdateTodoListAsync(TodoList todoList);

    Task DeleteTodoListAsync(int id);
}
