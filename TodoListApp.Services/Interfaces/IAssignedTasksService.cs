using TodoListApp.Services.Database.Models;
using TodoListApp.WebApi.Models.Tasks;

namespace TodoListApp.Services.Interfaces;
public interface IAssignedTasksService
{
    Task<PaginatedListResult<AssignedTasksdto>> GetTasksByAssigneeAsync(string userId, int pageNumber, int tasksPerPage, Status? status = null, string? sortCriteria = null);

    Task<bool> UpdateTaskStatusAsync(UpdateTaskStatus updateTaskStatusDto, string userId);
}
