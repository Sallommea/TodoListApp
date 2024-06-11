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
            TaskControllerLoggerMessages.TaskExceptionOccurredWhileGettingTaskDetails(this.logger, ex.Message, ex);
            return this.NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            TaskControllerLoggerMessages.UnexpectedErrorOccurredWhileGettingTaskDetails(this.logger, ex.Message, ex);
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
            return this.StatusCode((int)HttpStatusCode.InternalServerError, new { message = "An unexpected error occurred." + ex.Message });
        }
    }

    [HttpDelete("{taskId}")]
    public async Task<IActionResult> DeleteTask(int taskId)
    {
        if (taskId <= 0)
        {
            TaskControllerLoggerMessages.InvalidTaskIdForDeletion(this.logger, taskId);
            return this.BadRequest(new { message = "Invalid task ID." });
        }

        try
        {
            var result = await this.taskService.DeleteTaskAsync(taskId);
            if (!result)
            {
                TaskControllerLoggerMessages.TaskIdNotFoundToDelete(this.logger, taskId);
                return this.NotFound(new { message = $"Task with ID {taskId} not found." });
            }

            TaskControllerLoggerMessages.TaskDeletedSuccessfully(this.logger, taskId);
            return this.NoContent();
        }
        catch (Exception ex)
        {
            TaskControllerLoggerMessages.UnexpectedErrorOccurredWhileDeletingTask(this.logger, ex.Message, taskId, ex);
            return this.StatusCode((int)HttpStatusCode.InternalServerError, new { message = "An unexpected error occurred." + ex.Message });
        }
    }

    [HttpPut("{taskId}")]
    public async Task<IActionResult> UpdateTask(int taskId, UpdateTaskDto updateTaskDto)
    {
        if (taskId <= 0)
        {
            TaskControllerLoggerMessages.InvalidTaskIdForUpdate(this.logger, taskId);
            return this.BadRequest(new { message = "Invalid task ID." });
        }

        if (!this.ModelState.IsValid)
        {
            return this.BadRequest(this.ModelState);
        }

        try
        {
            var result = await this.taskService.UpdateTaskAsync(taskId, updateTaskDto);
            if (!result)
            {
                TaskControllerLoggerMessages.TaskIdNotFoundToUpdate(this.logger, taskId);
                return this.NotFound(new { message = $"Task with ID {taskId} not found." });
            }

            return this.Ok();
        }
        catch (Exception ex)
        {
            TaskControllerLoggerMessages.UnexpectedErrorOccurredWhileUpdatingTask(this.logger, ex.Message, taskId, ex);
            return this.StatusCode((int)HttpStatusCode.InternalServerError, new { message = "An unexpected error occurred." + ex.Message });
        }
    }
}
