using Microsoft.EntityFrameworkCore;
using TodoListApp.Services.Database.Models;

namespace TodoListApp.Services.Database.Repositories;
public class TodolistRepository : ITodoListRepository
{
    private readonly TodoListDbContext dbContext;

    public TodolistRepository(TodoListDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<IEnumerable<TodoListEntity>> GetAllTodoListsAsync()
    {
        return await this.dbContext.TodoLists
       .OrderByDescending(x => x.Id)
       .ToListAsync();
    }

    public async Task<TodoListEntity?> GetTodoListWithTasksAsync(int todoListId)
    {
        var todoList = await this.dbContext.TodoLists
            .Include(tl => tl.Tasks)
            .FirstOrDefaultAsync(tl => tl.Id == todoListId);

        return todoList;
    }

    public async Task AddTodoListAsync(TodoListEntity todoList)
    {
        _ = await this.dbContext.TodoLists.AddAsync(todoList);
        _ = await this.dbContext.SaveChangesAsync();
    }

    public async Task DeleteTodoListAsync(int id)
    {
        var entity = await this.dbContext.TodoLists.FindAsync(id);

        if (entity is null)
        {
            throw new KeyNotFoundException($"TodoList with ID {id} not found.");
        }

        _ = this.dbContext.TodoLists.Remove(entity);
        _ = await this.dbContext.SaveChangesAsync();
    }

    public async Task UpdateTodoListAsync(TodoListEntity todoList)
    {
        var entity = await this.dbContext.TodoLists.FindAsync(todoList.Id);
        if (entity == null)
        {
            throw new KeyNotFoundException($"TodoList with ID {todoList.Id} not found.");
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
