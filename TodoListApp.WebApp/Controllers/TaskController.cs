using Microsoft.AspNetCore.Mvc;
using TodoListApp.Services.WebApi.Services;
using TodoListApp.WebApi.Models;
using TodoListApp.WebApi.Models.Tasks;

namespace TodoListApp.WebApp.Controllers;
public class TaskController : Controller
{
    private readonly TaskWebApiService taskWebApiService;
    private readonly ILogger<TaskController> logger;

    public TaskController(TaskWebApiService taskWebApiService, ILogger<TaskController> logger)
    {
        this.taskWebApiService = taskWebApiService;
        this.logger = logger;
    }

    public IActionResult Create(int todoListId)
    {
        var createTaskDto = new CreateTaskDto { TodoListId = todoListId };
        return this.View(createTaskDto);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateTaskDto createTask)
    {
        if (this.ModelState.IsValid)
        {
            try
            {
                if (createTask.DueDate.HasValue)
                {
                    var dueDate = createTask.DueDate.Value;

                    if (dueDate.TimeOfDay == TimeSpan.Zero)
                    {
                        dueDate = dueDate.Date.AddHours(23).AddMinutes(59).AddSeconds(59).AddMilliseconds(999);
                    }

                    createTask.DueDate = DateTime.SpecifyKind(dueDate, DateTimeKind.Utc);
                }

                int id = await this.taskWebApiService.AddTaskAsync(createTask);
                return this.RedirectToAction("Details", "TodoList", new { id = createTask.TodoListId });
            }
            catch (HttpRequestException)
            {
                this.ModelState.AddModelError(string.Empty, "An error occurred while creating the task.");
            }
        }

        return this.View(createTask);
    }

    [HttpPost]
    public async Task<IActionResult> DeleteTask(int id, int todoListId)
    {
        this.logger.LogInformation("Attempting to delete task with ID {Id} from todo list {TodoListId}", id, todoListId);

        try
        {
            await this.taskWebApiService.DeleteTaskAsync(id);
            this.logger.LogInformation("Successfully deleted task with ID {Id}", id);
            return this.RedirectToAction("Details", "TodoList", new { id = todoListId });
        }
        catch (HttpRequestException ex)
        {
            this.logger.LogError(ex, "Error deleting task with ID {Id}", id);
            return this.StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred: " + ex.Message);
        }
    }

    public async Task<IActionResult> TaskDetails(int taskId)
    {
        if (taskId <= 0)
        {
            this.logger.LogInformation(taskId.ToString());
            return this.BadRequest(new { message = "Invalid task ID." });
        }

        try
        {
            var taskDetails = await this.taskWebApiService.GetTaskDetailsAsync(taskId);
            return this.View(taskDetails);
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "Error occurred while fetching task details");
            return this.NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ioe)
        {
            logger.LogError(ioe, "Invalid operation error occurred");
            return this.StatusCode(StatusCodes.Status500InternalServerError, "An error occurred: " + ioe.Message);
        }
    }

    public async Task<IActionResult> EditTask(int id)
    {
        var task = await this.taskWebApiService.GetTaskDetailsAsync(id);
        if (task == null)
        {
            return this.NotFound();
        }

        var updateTaskDto = new UpdateTaskDto
        {
            Title = task.Title,
            Description = task.Description,
            DueDate = task.DueDate,
            Status = task.Status,
        };

        this.ViewData["TaskId"] = id;
        return this.View(updateTaskDto);
    }

    [HttpPost]
    public async Task<IActionResult> UpdateTask(int id, UpdateTaskDto updateTaskDto)
    {
        if (!this.ModelState.IsValid)
        {
            this.ViewData["TaskId"] = id;
            return this.View("EditTask", updateTaskDto);
        }

        try
        {
            if (updateTaskDto.DueDate.HasValue)
            {
                var dueDate = updateTaskDto.DueDate.Value;

                if (dueDate.TimeOfDay == TimeSpan.Zero)
                {
                    dueDate = dueDate.Date.AddHours(23).AddMinutes(59).AddSeconds(59).AddMilliseconds(999);
                }

                updateTaskDto.DueDate = DateTime.SpecifyKind(dueDate, DateTimeKind.Utc);
            }

            await this.taskWebApiService.UpdateTaskAsync(id, updateTaskDto);
            return this.RedirectToAction("TaskDetails", new { taskId = id });
        }
        catch (HttpRequestException)
        {
            return this.NotFound();
        }
    }
}
