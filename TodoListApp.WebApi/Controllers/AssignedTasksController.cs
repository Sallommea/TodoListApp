using Microsoft.AspNetCore.Mvc;
using TodoListApp.Services.Interfaces;
using TodoListApp.WebApi.Models.Tasks;

namespace TodoListApp.WebApi.Controllers;
[Route("api/[controller]")]
[ApiController]
public class AssignedTasksController : ControllerBase
{
    private readonly IAssignedTasksService assignedTasksService;

    public AssignedTasksController(IAssignedTasksService assignedTasksService)
    {
        this.assignedTasksService = assignedTasksService;
    }

    [HttpGet("assigned-to-me")]
    public async Task<ActionResult<List<TaskDetailsDto>>> GetTasksAssignedToMe([FromQuery] Status? status = null, [FromQuery] string? sortCriteria = null)
    {
        var assignee = "DefaultUser"; // to be replaced by the actual user's identity after implementing authorization
        var tasks = await this.assignedTasksService.GetTasksByAssigneeAsync(assignee, status, sortCriteria);
        return this.Ok(tasks);
    }
}
