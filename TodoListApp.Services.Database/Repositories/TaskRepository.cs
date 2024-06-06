using Microsoft.EntityFrameworkCore;

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
        _ = await this.dbContext.SaveChangesAsync();
        return task;
    }

    public async Task DeleteTaskAsync(TaskEntity task)
    {
        _ = this.dbContext.Tasks.Remove(task);
        _ = await this.dbContext.SaveChangesAsync();
    }

    public async Task<TaskEntity?> GetTaskByIdAsync(int todoListId, int taskId)
    {
        return await this.dbContext.Tasks
        .FirstOrDefaultAsync(t => t.TodoListId == todoListId && t.Id == taskId);
    }

    public async Task UpdateTaskAsync(int todoListId, int taskId, TaskEntity task)
    {
        var existingTask = await this.dbContext.Tasks.FirstOrDefaultAsync(t => t.TodoListId == todoListId && t.Id == taskId);

        if (existingTask == null)
        {
            throw new KeyNotFoundException($"Task with ID {taskId} not found in TodoList with ID {todoListId}.");
        }

        existingTask.Title = task.Title;
        existingTask.Description = task.Description;
        existingTask.DueDate = task.DueDate;
        existingTask.Status = task.Status;

        _ = await this.dbContext.SaveChangesAsync();
    }
}
