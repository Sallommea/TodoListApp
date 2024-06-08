using TodoListApp.WebApi.Models.Tasks;

namespace TodoListApp.Services.Interfaces;
public interface IAssignedTasksService
{
    Task<List<TaskDetailsDto>> GetTasksByAssigneeAsync(string assignee, Status? status = null, string? sortCriteria = null);

    Task<bool> UpdateTaskStatusAsync(UpdateTaskStatus updateTaskStatusDto);
}
