using Microsoft.Extensions.Logging;
using TodoListApp.Services.Database;
using TodoListApp.Services.Database.Repositories;
using TodoListApp.Services.Exceptions;
using TodoListApp.Services.Interfaces;
using TodoListApp.Services.Logging;
using TodoListApp.WebApi.Models.Tasks;

namespace TodoListApp.Services.Services;
public class TaskService : ITaskService
{
    private readonly ITaskRepository taskRepository;
    private readonly ILogger<TaskService> logger;

    public TaskService(ITaskRepository taskRepository, ILogger<TaskService> logger)
    {
        this.taskRepository = taskRepository;
        this.logger = logger;
    }

    public async Task<TaskDetailsDto?> GetTaskDetailsAsync(int taskId)
    {
        try
        {
            var task = await this.taskRepository.GetTaskByIdAsync(taskId);
            if (task == null)
            {
                throw new TaskException($"Task with ID {taskId} not found.");
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
        catch (TaskException)
        {
            throw; // Propagate the known exception to the controller
        }
        catch (Exception ex)
        {
            // Log the exception (assuming logger is injected in the service)
            this.logger.LogError(ex, "An error occurred while getting task details.");
            throw; // Propagate the unexpected exception to the controller
        }
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

        try
        {
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

            TaskLoggerMessages.TaskCreated(this.logger, createdTask.Id);
            return taskDto;
        }
        catch (KeyNotFoundException ex)
        {
            TodoListLoggerMessages.TodoListNotFound(this.logger, ex);
            throw new TodoListException("Unable to create task due to missing TodoList.", ex);
        }
        catch (Exception ex)
        {
            TaskLoggerMessages.UnexpectedErrorCreatingTask(this.logger, ex);
            throw new TaskException("An unexpected error occurred while creating the task.", ex);
        }

    }

    public async Task<bool> DeleteTaskAsync(int taskId)
    {
        try
        {
            var result = await this.taskRepository.DeleteTaskAsync(taskId);
            if (!result)
            {
                this.logger.LogInformation("Task with ID {TaskId} not found in the repository.", taskId);
                return false;
            }

            this.logger.LogInformation("Task with ID {TaskId} deleted successfully from the repository.", taskId);
            return true;
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "An error occurred while deleting task with ID {TaskId}.", taskId);
            throw;
        }
    }

    public async Task<bool> UpdateTaskAsync(int taskId, UpdateTaskDto updateTaskDto)
    {
        try
        {
            var existingTask = await this.taskRepository.GetTaskByIdAsync(taskId);
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

            await this.taskRepository.UpdateTaskAsync(taskId, existingTask);
            return true;
        }
        catch (KeyNotFoundException ex)
        {
            this.logger.LogWarning(ex, "Task with ID {TaskId} not found when trying to update.", taskId);
            return false;
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "An error occurred while updating task with ID {TaskId}.", taskId);
            throw;
        }
    }
}
