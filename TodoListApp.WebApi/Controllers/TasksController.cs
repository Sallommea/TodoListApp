using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
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
[Authorize]
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

        string? userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return this.Unauthorized();
        }

        try
        {
            var paginatedTasks = await this.taskService.GetPaginatedSearchedTasksAsync(pageNumber, itemsPerPage, searchText, userId);
            TaskControllerLoggerMessages.SearchedTasksRetrieved(this.logger);
            return this.Ok(paginatedTasks);
        }
        catch (InvalidOperationException ioe)
        {
            TaskControllerLoggerMessages.InvalidOperationOccurredGettingSearchedTasks(this.logger, ioe.Message, ioe);
            return this.StatusCode(StatusCodes.Status500InternalServerError, "An invalid operation occured while getting searched tasks");
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

        string? userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return this.Unauthorized();
        }

        try
        {
            var taskDetails = await this.taskService.GetTaskDetailsAsync(taskId, userId);

            return this.Ok(taskDetails);
        }
        catch (TaskException ex)
        {
            TaskControllerLoggerMessages.TaskExceptionOccurredWhileGettingTaskDetails(this.logger, ex.Message, ex);
            return this.NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ioe)
        {
            TaskControllerLoggerMessages.InvalidOperationOccurredGettingTaskDetails(this.logger, ioe.Message, ioe);
            return this.StatusCode(StatusCodes.Status500InternalServerError, "An invalid operation occured while getting task details");
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

        string? userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return this.Unauthorized();
        }

        try
        {
            var result = await this.taskService.GetTasksByTagIdAsync(tagId, userId, pageNumber, pageSize);

            return this.Ok(result);
        }
        catch (InvalidOperationException ioe)
        {
            TaskControllerLoggerMessages.InvalidOperationOccurredGettingTasksByTag(this.logger, ioe.Message, ioe);
            return this.StatusCode(StatusCodes.Status500InternalServerError, "An invalid operation occured while getting tasks by tag");
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

        string? userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return this.Unauthorized();
        }

        try
        {
            var createdTask = await this.taskService.CreateTaskAsync(createTaskDto, userId);
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
            TaskControllerLoggerMessages.InvalidOperationCreatingTask(this.logger, ioe.Message, ioe);
            return this.StatusCode(StatusCodes.Status500InternalServerError, "An invalid operation occured while creating task");
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

        string? userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return this.Unauthorized();
        }

        try
        {
            var result = await this.taskService.DeleteTaskAsync(taskId, userId);
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
            TaskControllerLoggerMessages.InvalidOperationDeletingTask(this.logger, ioe.Message, ioe);
            return this.StatusCode(StatusCodes.Status500InternalServerError, "An invalid operation occured while deleting task");
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

        string? userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return this.Unauthorized();
        }

        if (!this.ModelState.IsValid)
        {
            return this.BadRequest(this.ModelState);
        }

        try
        {
            var result = await this.taskService.UpdateTaskAsync(taskId, updateTaskDto, userId);
            if (!result)
            {
                TaskControllerLoggerMessages.TaskIdNotFoundToUpdate(this.logger, taskId);
                return this.NotFound(new { message = $"Task with ID {taskId} not found." });
            }

            return this.Ok();
        }
        catch (InvalidOperationException ioe)
        {
            TaskControllerLoggerMessages.InvalidOperationUpdatingTask(this.logger, ioe.Message, ioe);
            return this.StatusCode(StatusCodes.Status500InternalServerError, "An invalid operation occured while updating task");
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

        string? userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return this.Unauthorized();
        }

        if (!this.ModelState.IsValid)
        {
            return this.BadRequest(this.ModelState);
        }

        try
        {
            var comment = await this.taskService.AddCommentAsync(addCommentDto, userId);
            return this.CreatedAtAction(nameof(this.GetTaskDetails), new { taskId = comment.Id }, comment);
        }
        catch (TaskException ex)
        {
            return this.NotFound(ex.Message);
        }
        catch (InvalidOperationException ioe)
        {
            TaskControllerLoggerMessages.IOExceptionWhileAddingComment(this.logger, ioe.Message, ioe);
            return this.StatusCode(StatusCodes.Status500InternalServerError, "An invalid operation occured while adding comment");
        }
        catch (Exception ex)
        {
            TaskControllerLoggerMessages.UnexpectedErrorWhileAddingComment(this.logger, ex.Message, ex);
            throw;
        }
    }

    [HttpPut("{taskId}/comments/{commentId}")]
    public async Task<ActionResult<CommentDto>> EditComment(int taskId, int commentId, [FromBody] EditCommentDto editCommentDto)
    {
        string? userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return this.Unauthorized();
        }

        if (taskId != editCommentDto.TaskId || commentId != editCommentDto.CommentId)
        {
            return this.BadRequest("Task ID or Comment ID in the URL does not match the IDs in the request body.");
        }

        if (!this.ModelState.IsValid)
        {
            return this.BadRequest(this.ModelState);
        }

        try
        {
            var updatedComment = await this.taskService.EditCommentAsync(editCommentDto, userId);
            return this.Ok(updatedComment);
        }
        catch (TaskException ex)
        {
            return this.NotFound(ex.Message);
        }
        catch (InvalidOperationException ioe)
        {
            TaskControllerLoggerMessages.IOExceptionWhileEditingComment(this.logger, ioe.Message, ioe);
            return this.StatusCode(StatusCodes.Status500InternalServerError, "An invalid operation occured while updating comment");
        }
        catch (Exception ex)
        {
            TaskControllerLoggerMessages.UnexpectedErrorWhileEditingComment(this.logger, ex.Message, ex);
            throw;
        }
    }

    [HttpDelete("{taskId}/comments/{commentId}")]
    public async Task<ActionResult> DeleteComment(int taskId, int commentId)
    {
        string? userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return this.Unauthorized();
        }

        try
        {
            await this.taskService.DeleteCommentAsync(taskId, commentId, userId);
            return this.NoContent();
        }
        catch (TaskException ex)
        {
            return this.NotFound(ex.Message);
        }
        catch (InvalidOperationException ioe)
        {
            TaskControllerLoggerMessages.IOExceptionWhileDeletingComment(this.logger, ioe.Message, ioe);
            return this.StatusCode(StatusCodes.Status500InternalServerError, "An invalid operation occured while deleting comment");
        }
        catch (Exception ex)
        {
            TaskControllerLoggerMessages.UnexpectedErrorWhileDeletingComment(this.logger, ex.Message, ex);
            throw;
        }
    }
}
