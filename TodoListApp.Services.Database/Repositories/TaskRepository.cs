using Microsoft.EntityFrameworkCore;
using TodoListApp.Services.Database.Models;

namespace TodoListApp.Services.Database.Repositories;
public class TaskRepository : ITaskRepository
{
    private readonly TodoListDbContext dbContext;

    public TaskRepository(TodoListDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<TaskEntity> AddTaskAsync(TaskEntity task)
    {
        _ = await this.dbContext.Tasks.AddAsync(task);

        var todoList = await this.dbContext.TodoLists.FindAsync(task.TodoListId);
        if (todoList != null)
        {
            todoList.TaskCount++;
            _ = this.dbContext.TodoLists.Update(todoList);
        }
        else
        {
            throw new KeyNotFoundException($"TodoList with ID {task.TodoListId} not found while adding task.");
        }

        _ = await this.dbContext.SaveChangesAsync();
        return task;
    }

    public async Task<bool> DeleteTaskAsync(int taskId)
    {
        var task = await this.dbContext.Tasks.FindAsync(taskId);
        if (task == null)
        {
            return false;
        }

        _ = this.dbContext.Tasks.Remove(task);

        var todoList = await this.dbContext.TodoLists.FindAsync(task.TodoListId);
        if (todoList != null && todoList.TaskCount > 0)
        {
            todoList.TaskCount--;
            _ = this.dbContext.TodoLists.Update(todoList);
        }

        _ = await this.dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<TaskEntity?> GetTaskByIdAsync(int taskId)
    {
        return await this.dbContext.Tasks
        .FirstOrDefaultAsync(t => t.Id == taskId);
    }

    public async Task UpdateTaskAsync(int taskId, TaskEntity task)
    {
        var existingTask = await this.dbContext.Tasks.FirstOrDefaultAsync(t => t.Id == taskId);

        if (existingTask == null)
        {
            throw new KeyNotFoundException($"Task with ID {taskId} not found in TodoList.");
        }

        existingTask.Title = task.Title;
        existingTask.Description = task.Description;
        existingTask.DueDate = task.DueDate;
        existingTask.Status = task.Status;
        existingTask.IsExpired = task.IsExpired;
        _ = await this.dbContext.SaveChangesAsync();
    }

    public async Task<PaginatedListResult<TaskEntity>> SearchTasksByTitleAsync(int pageNumber, int tasksPerPage, string searchText)
    {
        var result = new PaginatedListResult<TaskEntity>();
        var query = this.dbContext.Tasks.AsQueryable();

        if (!string.IsNullOrEmpty(searchText))
        {
            query = query.Where(t => t.Title.Contains(searchText));
        }

        result.TotalRecords = await query.CountAsync();
        result.TotalPages = (int)Math.Ceiling((double)result.TotalRecords / tasksPerPage);
        result.ResultList = await query
            .OrderByDescending(x => x.Id)
            .Skip((pageNumber - 1) * tasksPerPage)
            .Take(tasksPerPage)
            .ToListAsync();

        return result;
    }
}
