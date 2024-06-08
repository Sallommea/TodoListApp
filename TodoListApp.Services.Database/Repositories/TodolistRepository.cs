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

    public async Task<PaginatedListResult<TodoListEntity>> GetPaginatedTodoListsAsync(int pageNumber, int itemsPerPage)
    {
        var result = new PaginatedListResult<TodoListEntity>
        {
            TotalRecords = await this.dbContext.TodoLists.CountAsync(),
        };
        result.TotalPages = (int)Math.Ceiling((double)result.TotalRecords / itemsPerPage);
        result.ResultList = await this.dbContext.TodoLists
            .OrderByDescending(x => x.Id)
            .Skip((pageNumber - 1) * itemsPerPage)
            .Take(itemsPerPage)
            .ToListAsync();

        return result;
    }

    public async Task<PaginatedTodoListResult> GetTodoListWithTasksAsync(int todoListId, int taskPageNumber, int tasksPerPage)
    {
        var todoList = await this.dbContext.TodoLists
            .Include(tl => tl.Tasks)
            .FirstOrDefaultAsync(tl => tl.Id == todoListId);

        if (todoList == null)
        {
            return new PaginatedTodoListResult
            {
                TodoList = null,
                PaginatedTasks = new PaginatedListResult<TaskEntity>
                {
                    TotalRecords = 0,
                    TotalPages = 0,
                    ResultList = new List<TaskEntity>(),
                },
            };
        }

        var totalTaskPages = (int)Math.Ceiling((double)todoList.TaskCount / tasksPerPage);

        var paginatedTasks = todoList.Tasks?
       .OrderBy(t => t.Id)
       .Skip((taskPageNumber - 1) * tasksPerPage)
       .Take(tasksPerPage)
       .ToList() ?? new List<TaskEntity>();

        var paginatedTasksResult = new PaginatedListResult<TaskEntity>
        {
            TotalRecords = todoList.TaskCount,
            TotalPages = totalTaskPages,
            ResultList = paginatedTasks,
        };

        return new PaginatedTodoListResult
        {
            TodoList = todoList,
            PaginatedTasks = paginatedTasksResult,
        };
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
