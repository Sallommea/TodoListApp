using Microsoft.AspNetCore.Mvc;
using TodoListApp.Services.Database.Models;
using TodoListApp.Services.Interfaces;
using TodoListApp.WebApi.Logging;
using TodoListApp.WebApi.Models.Tasks;

namespace TodoListApp.WebApi.Controllers;
[Route("api/[controller]")]
[ApiController]
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
    public async Task<ActionResult<PaginatedListResult<TaskDetailsDto>>> GetTasksAssignedToMeAsync(int pageNumber = 1, int tasksPerPage = 10, [FromQuery] Status? status = null, [FromQuery] string? sortCriteria = null)
    {
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
            var assignee = "DefaultUser"; // to be replaced by the actual user's identity after implementing authorization
            var tasks = await this.assignedTasksService.GetTasksByAssigneeAsync(assignee, pageNumber, tasksPerPage, status, sortCriteria);
            return this.Ok(tasks);
        }
        catch (InvalidOperationException ioe)
        {
            return this.StatusCode(StatusCodes.Status500InternalServerError, "An invalid operation occured" + ioe.Message);
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
        if (!this.ModelState.IsValid)
        {
            return this.BadRequest(this.ModelState);
        }

        try
        {
            var result = await this.assignedTasksService.UpdateTaskStatusAsync(updateTaskStatusDto);
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
            return this.StatusCode(StatusCodes.Status500InternalServerError, "An invalid operation occured" + ioe.Message);
        }
        catch (Exception ex)
        {
            AssignedTasksControllerLoggerMessages.UnexpectedErrorOccurredWhileUpdatingTaskStatus(this.logger, ex.Message, ex);
            throw;
        }
    }
}
