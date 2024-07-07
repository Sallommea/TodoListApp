using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoListApp.Services.Database.Models;
using TodoListApp.Services.Interfaces;
using TodoListApp.WebApi.Logging;
using TodoListApp.WebApi.Models.Tasks;

namespace TodoListApp.WebApi.Controllers;
[Route("api/[controller]")]
[ApiController]
[Authorize]
public class AssignedTasksController : ControllerBase
{
    private readonly IAssignedTasksService assignedTasksService;
    private readonly ILogger<AssignedTasksController> logger;

    public AssignedTasksController(IAssignedTasksService assignedTasksService, ILogger<AssignedTasksController> logger)
    {
        this.assignedTasksService = assignedTasksService;
        this.logger = logger;
    }

    [HttpGet("assigned-to-me")]
    public async Task<ActionResult<PaginatedListResult<AssignedTasksdto>>> GetTasksAssignedToMeAsync(int pageNumber = 1, int tasksPerPage = 10, [FromQuery] Status? status = null, [FromQuery] string? sortCriteria = null)
    {
        string? userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return this.Unauthorized();
        }

        if (pageNumber <= 0)
        {
            AssignedTasksControllerLoggerMessages.InvalidPageNumber(this.logger, pageNumber);
            return this.BadRequest(new { message = "Page number must be greater than zero." });
        }

        if (tasksPerPage <= 0)
        {
            AssignedTasksControllerLoggerMessages.InvalidTasksPerPage(this.logger, tasksPerPage);
            return this.BadRequest(new { message = "Tasks per page must be greater than zero." });
        }

        try
        {
            var tasks = await this.assignedTasksService.GetTasksByAssigneeAsync(userId, pageNumber, tasksPerPage, status, sortCriteria);
            return this.Ok(tasks);
        }
        catch (InvalidOperationException ioe)
        {
            AssignedTasksControllerLoggerMessages.IOExceptionWhileGettingAssignedTasks(this.logger, ioe.Message, ioe);
            return this.StatusCode(StatusCodes.Status500InternalServerError, "An invalid operation occured while getting assigned tasks");
        }
        catch (Exception ex)
        {
            AssignedTasksControllerLoggerMessages.UnexpectedErrorOccurredWhileGettingUserTasks(this.logger, ex.Message, ex);
            throw;
        }
    }

    [HttpPut("update-status")]
    public async Task<IActionResult> UpdateTaskStatus([FromBody] UpdateTaskStatus updateTaskStatusDto)
    {
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
            var result = await this.assignedTasksService.UpdateTaskStatusAsync(updateTaskStatusDto, userId);
            if (!result)
            {
                AssignedTasksControllerLoggerMessages.InvalidTaskIdForTaskStatusUpdate(this.logger, updateTaskStatusDto.TaskId);
                return this.NotFound(new { message = $"Task with ID {updateTaskStatusDto.TaskId} not found." });
            }

            AssignedTasksControllerLoggerMessages.TaskStatusUpdatedSuccessfully(this.logger, updateTaskStatusDto.TaskId);
            return this.Ok();
        }
        catch (InvalidOperationException ioe)
        {
            AssignedTasksControllerLoggerMessages.IOExceptionWhileUpdatingTaskStatus(this.logger, ioe.Message, ioe);
            return this.StatusCode(StatusCodes.Status500InternalServerError, "An invalid operation occured while updating task status");
        }
        catch (Exception ex)
        {
            AssignedTasksControllerLoggerMessages.UnexpectedErrorOccurredWhileUpdatingTaskStatus(this.logger, ex.Message, ex);
            throw;
        }
    }
}
