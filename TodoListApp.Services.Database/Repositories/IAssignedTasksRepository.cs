using TodoListApp.Services.Database.Models;

namespace TodoListApp.Services.Database.Repositories;
public interface IAssignedTasksRepository
{
    Task<PaginatedListResult<TaskEntity>> GetTasksByAssigneeAsync(string userId, int pageNumber, int tasksPerPage, Status? status = null, string? sortCriteria = null);

    Task<bool> UpdateTaskStatusAsync(int taskId, Status newStatus, string userId);

    Task SaveChangesAsync();
}
