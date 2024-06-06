using Microsoft.AspNetCore.Mvc;
using TodoListApp.Services.Interfaces;
using TodoListApp.WebApi.Models.Tasks;

namespace TodoListApp.WebApi.Controllers;
[Route("api/[controller]")]
[ApiController]
public class TasksController : ControllerBase
{
    private readonly ITaskService taskService;

    public TasksController(ITaskService taskService)
    {
        this.taskService = taskService;
    }

    [HttpGet("{todoListId}/tasks/{taskId}")]
    public async Task<IActionResult> GetTaskDetails(int todoListId, int taskId)
    {
        var taskDetails = await this.taskService.GetTaskDetailsAsync(todoListId, taskId);
        if (taskDetails == null)
        {
            return this.NotFound();
        }

        return this.Ok(taskDetails);
    }

    [HttpPost]
    public async Task<IActionResult> CreateTask(CreateTaskDto createTaskDto)
    {
        var createdTask = await this.taskService.CreateTaskAsync(createTaskDto);

        return this.CreatedAtAction(
        nameof(this.GetTaskDetails),
        new { todoListId = createdTask.TodoListId, taskId = createdTask.Id },
        createdTask);
    }

    [HttpDelete("{todoListId}/tasks/{taskId}")]
    public async Task<IActionResult> DeleteTask(int todoListId, int taskId)
    {
        var result = await this.taskService.DeleteTaskAsync(todoListId, taskId);
        if (!result)
        {
            return this.NotFound();
        }

        return this.NoContent();
    }

    [HttpPut("{todoListId}/tasks/{taskId}")]
    public async Task<IActionResult> UpdateTask(int todoListId, int taskId, UpdateTaskDto updateTaskDto)
    {
        var result = await this.taskService.UpdateTaskAsync(todoListId, taskId, updateTaskDto);
        if (!result)
        {
            return this.NotFound();
        }

        return this.Ok();
    }
}
