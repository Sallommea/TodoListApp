namespace TodoListApp.Services.Database.Repositories;
public interface ITodoListRepository
{
    Task<IEnumerable<TodoListEntity>> GetAllTodoListsAsync(string userId);

    Task<TodoListEntity?> GetTodoListWithTasksAsync(int todoListId, string userId);

    Task AddTodoListAsync(TodoListEntity todoList, string userId);

    Task UpdateTodoListAsync(TodoListEntity todoList, string userId);

    Task DeleteTodoListAsync(int id, string userId);

    Task SaveChangesAsync();
}
