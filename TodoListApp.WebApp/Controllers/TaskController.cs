using Microsoft.AspNetCore.Mvc;
using TodoListApp.Services.WebApi.Services;
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
}
