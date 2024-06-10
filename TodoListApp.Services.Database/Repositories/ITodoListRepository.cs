namespace TodoListApp.Services.Database.Repositories;
public interface ITodoListRepository
{
    Task<IEnumerable<TodoListEntity>> GetAllTodoListsAsync();

    Task<TodoListEntity?> GetTodoListWithTasksAsync(int todoListId);

    Task AddTodoListAsync(TodoListEntity todoList);

    Task UpdateTodoListAsync(TodoListEntity todoList);

    Task DeleteTodoListAsync(int id);

    Task SaveChangesAsync();
}
