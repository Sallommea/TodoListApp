using TodoListApp.Services.Database;
using TodoListApp.Services.Database.Repositories;
using TodoListApp.Services.Interfaces;
using TodoListApp.WebApi.Models.Tasks;

namespace TodoListApp.Services.Services;
public class TaskService : ITaskService
{
    private readonly ITaskRepository taskRepository;

    public TaskService(ITaskRepository taskRepository)
    {
        this.taskRepository = taskRepository;
    }

    public async Task<TaskDetailsDto?> GetTaskDetailsAsync(int todoListId, int taskId)
    {
        var task = await this.taskRepository.GetTaskByIdAsync(todoListId, taskId);
        if (task == null)
        {
            return null;
        }

        var taskDetailsDto = new TaskDetailsDto
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            CreatedDate = task.CreatedDate,
            DueDate = task.DueDate,
            Status = (WebApi.Models.Tasks.Status)task.Status,
            Assignee = task.Assignee,
            TodoListId = task.TodoListId,
            IsExpired = task.IsExpired,
        };

        return taskDetailsDto;
    }

    public async Task<TaskDetailsDto> CreateTaskAsync(CreateTaskDto createTaskDto)
    {
        var currentDate = DateTime.UtcNow;

        var task = new TaskEntity
        {
            Title = createTaskDto.Title,
            Description = createTaskDto.Description,
            CreatedDate = DateTime.UtcNow,
            DueDate = createTaskDto.DueDate,
            Status = Database.Status.NotStarted,
            Assignee = "DefaultUser",
            TodoListId = createTaskDto.TodoListId,
            IsExpired = createTaskDto.DueDate.HasValue && createTaskDto.DueDate.Value < currentDate,
        };

        var createdTask = await this.taskRepository.AddTaskAsync(task);

        var taskDto = new TaskDetailsDto
        {
            Id = createdTask.Id,
            Title = createdTask.Title,
            Description = createdTask.Description,
            CreatedDate = createdTask.CreatedDate,
            DueDate = createdTask.DueDate,
            Status = (WebApi.Models.Tasks.Status)createdTask.Status,
            Assignee = createdTask.Assignee,
            TodoListId = createdTask.TodoListId,
            IsExpired = createdTask.IsExpired,
        };

        return taskDto;
    }

    public async Task<bool> DeleteTaskAsync(int taskId)
    {
        return await this.taskRepository.DeleteTaskAsync(taskId);
    }

    public async Task<bool> UpdateTaskAsync(int todoListId, int taskId, UpdateTaskDto updateTaskDto)
    {
        var existingTask = await this.taskRepository.GetTaskByIdAsync(todoListId, taskId);
        if (existingTask == null)
        {
            return false;
        }

        existingTask.Title = updateTaskDto.Title;
        existingTask.Description = updateTaskDto.Description;
        existingTask.DueDate = updateTaskDto.DueDate;
        existingTask.Status = (Database.Status)updateTaskDto.Status;

        var currentDateTime = DateTime.UtcNow;
        existingTask.IsExpired = updateTaskDto.DueDate.HasValue && updateTaskDto.DueDate.Value < currentDateTime;

        await this.taskRepository.UpdateTaskAsync(todoListId, taskId, existingTask);
        return true;
    }
}
