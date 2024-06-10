using TodoListApp.WebApi.Models.Tasks;

namespace TodoListApp.Services.Interfaces;
public interface ITaskService
{
    Task<TaskDetailsDto?> GetTaskDetailsAsync(int taskId);

    Task<TaskDetailsDto> CreateTaskAsync(CreateTaskDto createTaskDto);

    Task<bool> DeleteTaskAsync(int taskId);

    Task<bool> UpdateTaskAsync(int taskId, UpdateTaskDto updateTaskDto);
}
