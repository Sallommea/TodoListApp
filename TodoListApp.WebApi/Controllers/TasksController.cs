using System.Net;
using Microsoft.AspNetCore.Mvc;
using TodoListApp.Services.Exceptions;
using TodoListApp.Services.Interfaces;
using TodoListApp.WebApi.Logging;
using TodoListApp.WebApi.Models.Tasks;

namespace TodoListApp.WebApi.Controllers;
[Route("api/[controller]")]
[ApiController]
public class TasksController : ControllerBase
{
    private readonly ITaskService taskService;
    private readonly ILogger<TasksController> logger;

    public TasksController(ITaskService taskService, ILogger<TasksController> logger)
    {
        this.taskService = taskService;
        this.logger = logger;
    }

    [HttpGet("{taskId}")]
    public async Task<IActionResult> GetTaskDetails(int taskId)
    {
        if (taskId <= 0)
        {
            return this.BadRequest(new { message = "Invalid task ID." });
        }

        try
        {
            var taskDetails = await this.taskService.GetTaskDetailsAsync(taskId);

            return this.Ok(taskDetails);
        }
        catch (TaskException ex)
        {
            return this.NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            // Log the exception (assuming logger is injected in the controller)
            this.logger.LogError(ex, "An unexpected error occurred while getting task details.");
            return this.StatusCode((int)HttpStatusCode.InternalServerError, new { message = "An unexpected error occurred. Please try again later." });
        }


    }

    [HttpPost]
    public async Task<IActionResult> CreateTask(CreateTaskDto createTaskDto)
    {
        if (!this.ModelState.IsValid)
        {
            return this.BadRequest(this.ModelState);
        }

        try
        {
            var createdTask = await this.taskService.CreateTaskAsync(createTaskDto);
            TaskControllerLoggerMessages.TaskCreatedSuccessfully(this.logger, createdTask.Id, createdTask.TodoListId);
            return this.CreatedAtAction(
            nameof(this.GetTaskDetails),
            new { todoListId = createdTask.TodoListId, taskId = createdTask.Id },
            createdTask);
        }
        catch (TodoListException ex)
        {
            TaskControllerLoggerMessages.TodoListNotFound(this.logger, ex);
            return this.BadRequest(new { message = ex.Message });
        }
        catch (TaskException ex)
        {
            TaskControllerLoggerMessages.UnexpectedErrorCreatingTask(this.logger, ex);
            return this.StatusCode((int)HttpStatusCode.InternalServerError, new { message = ex.Message });
        }
        catch (Exception ex)
        {
            TaskControllerLoggerMessages.UnexpectedErrorCreatingTask(this.logger, ex);
            return this.StatusCode((int)HttpStatusCode.InternalServerError, new { message = "An unexpected error occurred. Please try again later." });
        }
    }

    [HttpDelete("{taskId}")]
    public async Task<IActionResult> DeleteTask(int taskId)
    {
        if (taskId <= 0)
        {
            this.logger.LogWarning("Invalid task ID provided for deletion: {TaskId}", taskId);
            return this.BadRequest(new { message = "Invalid task ID." });
        }

        try
        {
            var result = await this.taskService.DeleteTaskAsync(taskId);
            if (!result)
            {
                this.logger.LogInformation("Task with ID {TaskId} not found.", taskId);
                return this.NotFound(new { message = $"Task with ID {taskId} not found." });
            }

            this.logger.LogInformation("Task with ID {TaskId} deleted successfully.", taskId);
            return this.NoContent();
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "An unexpected error occurred while deleting task with ID {TaskId}.", taskId);
            return this.StatusCode((int)HttpStatusCode.InternalServerError, new { message = "An unexpected error occurred. Please try again later." });
        }
    }

    [HttpPut("{todoListId}/tasks/{taskId}")]
    public async Task<IActionResult> UpdateTask(int taskId, UpdateTaskDto updateTaskDto)
    {
        if (taskId <= 0)
        {
            this.logger.LogWarning("Invalid task ID provided for update: {TaskId}", taskId);
            return this.BadRequest(new { message = "Invalid task ID." });
        }

        if (!this.ModelState.IsValid)
        {
            this.logger.LogWarning("Validation failed for update task DTO: {ValidationErrors}", ModelState);
            return this.BadRequest(this.ModelState);
        }

        try
        {
            var result = await this.taskService.UpdateTaskAsync(taskId, updateTaskDto);
            if (!result)
            {
                return this.NotFound();
            }

            return this.Ok();
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "An unexpected error occurred while updating task with ID {TaskId}.", taskId);
            return this.StatusCode((int)HttpStatusCode.InternalServerError, new { message = "An unexpected error occurred. Please try again later." });
        }
    }
}
