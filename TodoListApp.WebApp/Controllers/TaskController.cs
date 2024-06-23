using Microsoft.AspNetCore.Mvc;
using TodoListApp.Services.WebApi.Services;
using TodoListApp.WebApi.Models.Tasks;
using TodoListApp.WebApp.Logging;

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

#pragma warning disable S6967 // ModelState.IsValid should be called in controller actions

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

                _ = await this.taskWebApiService.AddTaskAsync(createTask);
                return this.RedirectToAction("Details", "TodoList", new { id = createTask.TodoListId });
            }
            catch (HttpRequestException)
            {
                this.ModelState.AddModelError(string.Empty, "An error occurred while creating the task.");
            }
            catch (InvalidOperationException ioe)
            {
                TaskLoggerMessages.IOErrorWhileAddingTask(this.logger, ioe.Message, ioe);
                return this.StatusCode(StatusCodes.Status500InternalServerError, new { message = "An invalid operation occurred: while creating the task." });
            }
            catch (Exception ex)
            {
                TaskLoggerMessages.ErrorAddingTask(this.logger, ex.Message, ex);
                throw;
            }
        }

        return this.View(createTask);
    }

    [HttpPost]
    public async Task<IActionResult> DeleteTask(int id, int todoListId)
    {
        try
        {
            await this.taskWebApiService.DeleteTaskAsync(id);
            return this.RedirectToAction("Details", "TodoList", new { id = todoListId });
        }
        catch (HttpRequestException)
        {
            return this.StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred while deleting task.");
        }
        catch (InvalidOperationException ioe)
        {
            TaskLoggerMessages.IOEWhileDeleting(this.logger, id, ioe.Message, ioe);
            return this.StatusCode(StatusCodes.Status500InternalServerError, new { message = "An invalid operation occurred while deleting task." });
        }
        catch (Exception ex)
        {
            TaskLoggerMessages.ErrorDeletingTask(this.logger, id, ex.Message, ex);
            throw;
        }
    }

    public async Task<IActionResult> TaskDetails(int taskId)
    {
        if (taskId <= 0)
        {
            return this.BadRequest(new { message = "Invalid task ID." });
        }

        try
        {
            var taskDetails = await this.taskWebApiService.GetTaskDetailsAsync(taskId);
            return this.View(taskDetails);
        }
        catch (HttpRequestException ex)
        {
            return this.NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ioe)
        {
            TaskLoggerMessages.IOErrorGettingTask(this.logger, taskId, ioe.Message, ioe);
            return this.StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while fetching task.");
        }
        catch (Exception ex)
        {
            TaskLoggerMessages.ErrorGettingTask(this.logger, taskId, ex.Message, ex);
            throw;
        }
    }

    public async Task<IActionResult> EditTask(int id)
    {
        try
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
        catch (HttpRequestException)
        {
            return this.StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while fetching the task. Please try again later." });
        }
        catch (InvalidOperationException ioe)
        {
            TaskLoggerMessages.IOErrorFetchingTaskUpdate(this.logger, id, ioe.Message, ioe);
            return this.StatusCode(StatusCodes.Status500InternalServerError, new { message = "An invalid operation occurred: " + ioe.Message });
        }
        catch (Exception ex)
        {
            TaskLoggerMessages.ErrorFetchingTaskForUpdate(this.logger, id, ex.Message, ex);
            throw;
        }
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
            this.ModelState.AddModelError(string.Empty, "An error occurred while updating the task.");
            return this.View("Edit", updateTaskDto);
        }
        catch (InvalidOperationException ioe)
        {
            TaskLoggerMessages.IOErrorUpdatingTask(this.logger, id, ioe.Message, ioe);
            return this.StatusCode(StatusCodes.Status500InternalServerError, new { message = "An invalid operation occurred: " + ioe.Message });
        }
        catch (Exception ex)
        {
            TaskLoggerMessages.ErrorUpdatingTask(this.logger, id, ex.Message, ex);
            throw;
        }
    }
}
