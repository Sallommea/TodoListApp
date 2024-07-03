using Microsoft.EntityFrameworkCore;

namespace TodoListApp.Services.Database.Repositories;
public class TodolistRepository : ITodoListRepository
{
    private readonly TodoListDbContext dbContext;

    public TodolistRepository(TodoListDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<IEnumerable<TodoListEntity>> GetAllTodoListsAsync(string userId)
    {
        return await this.dbContext.TodoLists
        .Where(tl => tl.UserId == userId)
       .OrderByDescending(x => x.Id)
       .ToListAsync();
    }

    public async Task<TodoListEntity?> GetTodoListWithTasksAsync(int todoListId, string userId)
    {
        var todoList = await this.dbContext.TodoLists
            .Include(tl => tl.Tasks)
            .FirstOrDefaultAsync(tl => tl.Id == todoListId && tl.UserId == userId);

        return todoList;
    }

    public async Task AddTodoListAsync(TodoListEntity todoList, string userId)
    {
        todoList.UserId = userId;
        _ = await this.dbContext.TodoLists.AddAsync(todoList);
        _ = await this.dbContext.SaveChangesAsync();
    }

    public async Task DeleteTodoListAsync(int id, string userId)
    {
        var entity = await this.dbContext.TodoLists
            .FirstOrDefaultAsync(tl => tl.Id == id && tl.UserId == userId);

        if (entity is null)
        {
            throw new KeyNotFoundException($"TodoList with ID {id} not found for this user.");
        }

        _ = this.dbContext.TodoLists.Remove(entity);
        _ = await this.dbContext.SaveChangesAsync();
    }

    public async Task UpdateTodoListAsync(TodoListEntity todoList, string userId)
    {
        var entity = await this.dbContext.TodoLists
           .FirstOrDefaultAsync(tl => tl.Id == todoList.Id && tl.UserId == userId);
        if (entity == null)
        {
            throw new KeyNotFoundException($"TodoList with ID {todoList.Id} not found for this user.");
        }

        entity.Name = todoList.Name;
        entity.Description = todoList.Description;

        _ = this.dbContext.TodoLists.Update(entity);
        _ = await this.dbContext.SaveChangesAsync();
    }

    public async Task SaveChangesAsync()
    {
        _ = await this.dbContext.SaveChangesAsync();
    }
}
