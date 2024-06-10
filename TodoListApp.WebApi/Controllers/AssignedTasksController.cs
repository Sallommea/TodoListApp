using System.Net;
using Microsoft.AspNetCore.Mvc;
using TodoListApp.Services.Interfaces;
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
    public async Task<ActionResult<List<TaskDetailsDto>>> GetTasksAssignedToMe(int pageNumber = 1, int tasksPerPage = 10, [FromQuery] Status? status = null, [FromQuery] string? sortCriteria = null)
    {
        if (pageNumber <= 0)
        {
            this.logger.LogWarning("Invalid page number: {PageNumber}", pageNumber);
            return this.BadRequest(new { message = "Page number must be greater than zero." });
        }

        if (tasksPerPage <= 0)
        {
            this.logger.LogWarning("Invalid tasks per page: {TasksPerPage}", tasksPerPage);
            return this.BadRequest(new { message = "Tasks per page must be greater than zero." });
        }

        try
        {
            var assignee = "DefaultUser"; // to be replaced by the actual user's identity after implementing authorization
            var tasks = await this.assignedTasksService.GetTasksByAssigneeAsync(assignee, pageNumber, tasksPerPage, status, sortCriteria);
            return this.Ok(tasks);
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "An error occurred while retrieving tasks assigned to the user: {ExceptionMessage}", ex.Message);
            return this.StatusCode((int)HttpStatusCode.InternalServerError, new { message = "An unexpected error occurred." + ex.Message });
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
                this.logger.LogWarning("Task with ID {TaskId} not found.", updateTaskStatusDto.TaskId);
                return this.NotFound(new { message = $"Task with ID {updateTaskStatusDto.TaskId} not found." });
            }

            this.logger.LogInformation("Task status with ID {TaskId} updated successfully.", updateTaskStatusDto.TaskId);
            return this.Ok();
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "An error occurred while updating the status of task with ID {TaskId}.", updateTaskStatusDto.TaskId);
            return this.StatusCode((int)HttpStatusCode.InternalServerError, new { message = "An unexpected error occurred." + ex.Message });
        }
    }
}
