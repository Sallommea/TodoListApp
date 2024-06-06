namespace TodoListApp.Services.Database.Repositories;
public interface ITaskRepository
{
    Task<TaskEntity?> GetTaskByIdAsync(int todoListId, int taskId);

    Task<TaskEntity> AddTaskAsync(TaskEntity task);

    Task UpdateTaskAsync(int todoListId, int taskId, TaskEntity task);

    Task DeleteTaskAsync(TaskEntity task);
}
