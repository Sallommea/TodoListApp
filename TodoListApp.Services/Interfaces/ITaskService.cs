using TodoListApp.WebApi.Models.Tasks;

namespace TodoListApp.Services.Interfaces;
public interface ITaskService
{
    Task<TaskDetailsDto?> GetTaskDetailsAsync(int todoListId, int taskId);

    Task<TaskDetailsDto> CreateTaskAsync(CreateTaskDto createTaskDto);

    Task<bool> DeleteTaskAsync(int todoListId, int taskId);

    Task<bool> UpdateTaskAsync(int todoListId, int taskId, UpdateTaskDto updateTaskDto);
}
