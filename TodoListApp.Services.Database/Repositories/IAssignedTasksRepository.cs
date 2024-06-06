namespace TodoListApp.Services.Database.Repositories;
public interface IAssignedTasksRepository
{
    Task<List<TaskEntity>> GetTasksByAssigneeAsync(string assignee, Status? status = null, string? sortCriteria = null);
}
