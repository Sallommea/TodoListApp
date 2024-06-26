using System.Net;
using Microsoft.AspNetCore.Mvc;
using TodoListApp.Services.Exceptions;
using TodoListApp.Services.Interfaces;
using TodoListApp.Services.Models;
using TodoListApp.WebApi.Logging;
using TodoListApp.WebApi.Models.Comments;
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

    [HttpGet("bysearchtext")]
    public async Task<ActionResult<PaginatedListResult<TaskSearchResultDto>>> GetPaginatedTasks(string searchText, int pageNumber = 1, int itemsPerPage = 10)
    {
        if (pageNumber <= 0 || itemsPerPage <= 0)
        {
            return this.BadRequest("Page number and items per page must be greater than zero.");
        }

        try
        {
            var paginatedTasks = await this.taskService.GetPaginatedSearchedTasksAsync(pageNumber, itemsPerPage, searchText);
            TaskControllerLoggerMessages.SearchedTasksRetrieved(this.logger);
            return this.Ok(paginatedTasks);
        }
        catch (InvalidOperationException ioe)
        {
            return this.StatusCode(StatusCodes.Status500InternalServerError, "An invalid operation occured" + ioe.Message);
        }
        catch (Exception ex)
        {
            TaskControllerLoggerMessages.UnexpectedErrorOccurredWhileSearchingTasks(this.logger, ex.Message, searchText, ex);
            throw;
        }
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
        catch (InvalidOperationException ioe)
        {
            return this.StatusCode(StatusCodes.Status500InternalServerError, "An invalid operation occured" + ioe.Message);
        }
        catch (Exception ex)
        {
            TaskControllerLoggerMessages.UnexpectedErrorOccurredWhileGettingTaskDetails(this.logger, ex.Message, ex);
            throw;
        }
    }

    [HttpGet("bytag/{tagId}")]
    public async Task<ActionResult<PaginatedListResult<TaskDto>>> GetTasksByTag(int tagId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        if (pageNumber < 1 || pageSize < 1)
        {
            return this.BadRequest("Page number and page size must be positive integers.");
        }

        try
        {
            var result = await this.taskService.GetTasksByTagIdAsync(tagId, pageNumber, pageSize);

            return this.Ok(result);
        }
        catch (InvalidOperationException ioe)
        {
            return this.StatusCode(StatusCodes.Status500InternalServerError, "An invalid operation occured" + ioe.Message);
        }
        catch (Exception ex)
        {
            TaskControllerLoggerMessages.UnexpectedErrorgGettingTasksbyTagId(this.logger, tagId, ex.Message, ex);
            throw;
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
            return this.Ok(createdTask.Id);
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
        catch (InvalidOperationException ioe)
        {
            return this.StatusCode(StatusCodes.Status500InternalServerError, "An invalid operation occured" + ioe.Message);
        }
        catch (Exception ex)
        {
            TaskControllerLoggerMessages.UnexpectedErrorCreatingTask(this.logger, ex);
            throw;
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
        catch (InvalidOperationException ioe)
        {
            return this.StatusCode(StatusCodes.Status500InternalServerError, "An invalid operation occured" + ioe.Message);
        }
        catch (Exception ex)
        {
            TaskControllerLoggerMessages.UnexpectedErrorOccurredWhileDeletingTask(this.logger, ex.Message, taskId, ex);
            throw;
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
        catch (InvalidOperationException ioe)
        {
            return this.StatusCode(StatusCodes.Status500InternalServerError, "An invalid operation occured" + ioe.Message);
        }
        catch (Exception ex)
        {
            TaskControllerLoggerMessages.UnexpectedErrorOccurredWhileUpdatingTask(this.logger, ex.Message, taskId, ex);
            throw;
        }
    }

    [HttpPost("{taskId}/comments")]
    public async Task<ActionResult<CommentDto>> AddComment(int taskId, [FromBody] AddCommentDto addCommentDto)
    {
        if (taskId != addCommentDto.TaskId)
        {
            return this.BadRequest("Task ID in the URL does not match the task ID in the request body.");
        }

        if (!this.ModelState.IsValid)
        {
            return this.BadRequest(this.ModelState);
        }

        try
        {
            var comment = await this.taskService.AddCommentAsync(addCommentDto);
            return this.CreatedAtAction(nameof(this.GetTaskDetails), new { taskId = comment.Id }, comment);
        }
        catch (TaskException ex)
        {
            return this.NotFound(ex.Message);
        }
        catch (InvalidOperationException ioe)
        {
            TaskControllerLoggerMessages.IOExceptionWhileAddingComment(this.logger, ioe.Message, ioe);
            return this.StatusCode(StatusCodes.Status500InternalServerError, "An invalid operation occured");
        }
        catch (Exception ex)
        {
            TaskControllerLoggerMessages.UnexpectedErrorWhileAddingComment(this.logger, ex.Message, ex);
            throw;
        }
    }
}
