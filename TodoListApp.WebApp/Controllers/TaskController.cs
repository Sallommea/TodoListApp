using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using TodoListApp.Services.WebApi.Services;
using TodoListApp.WebApi.Models.Comments;
using TodoListApp.WebApi.Models.Tasks;
using TodoListApp.WebApp.Logging;
using TodoListApp.WebApp.Models;

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
                var token = this.User.FindFirst(ClaimTypes.Name)?.Value;
                if (string.IsNullOrEmpty(token))
                {
                    return this.RedirectToAction("Login", "Account");
                }

                if (createTask.DueDate.HasValue)
                {
                    var dueDate = createTask.DueDate.Value;

                    if (dueDate.TimeOfDay == TimeSpan.Zero)
                    {
                        dueDate = dueDate.Date.AddHours(23).AddMinutes(59).AddSeconds(59).AddMilliseconds(999);
                    }

                    createTask.DueDate = DateTime.SpecifyKind(dueDate, DateTimeKind.Utc);
                }

                _ = await this.taskWebApiService.AddTaskAsync(createTask, token);
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
            var token = this.User.FindFirst(ClaimTypes.Name)?.Value;
            if (string.IsNullOrEmpty(token))
            {
                return this.RedirectToAction("Login", "Account");
            }

            await this.taskWebApiService.DeleteTaskAsync(id, token);

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
            var token = this.User.FindFirst(ClaimTypes.Name)?.Value;
            if (string.IsNullOrEmpty(token))
            {
                return this.RedirectToAction("Login", "Account");
            }

            var taskDetails = await this.taskWebApiService.GetTaskDetailsAsync(taskId, token);
            if (this.TempData["ErrorMessage"] != null)
            {
                this.ViewBag.ErrorMessage = this.TempData["ErrorMessage"];
            }

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
            var token = this.User.FindFirst(ClaimTypes.Name)?.Value;
            if (string.IsNullOrEmpty(token))
            {
                return this.RedirectToAction("Login", "Account");
            }

            var task = await this.taskWebApiService.GetTaskDetailsAsync(id, token);
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
            var token = this.User.FindFirst(ClaimTypes.Name)?.Value;
            if (string.IsNullOrEmpty(token))
            {
                return this.RedirectToAction("Login", "Account");
            }

            if (updateTaskDto.DueDate.HasValue)
            {
                var dueDate = updateTaskDto.DueDate.Value;

                if (dueDate.TimeOfDay == TimeSpan.Zero)
                {
                    dueDate = dueDate.Date.AddHours(23).AddMinutes(59).AddSeconds(59).AddMilliseconds(999);
                }

                updateTaskDto.DueDate = DateTime.SpecifyKind(dueDate, DateTimeKind.Utc);
            }

            await this.taskWebApiService.UpdateTaskAsync(id, updateTaskDto, token);
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

    [HttpGet]
    public async Task<IActionResult> Search(string searchText, int pageNumber = 1, int itemsPerPage = 4)
    {
        try
        {
            var token = this.User.FindFirst(ClaimTypes.Name)?.Value;
            if (string.IsNullOrEmpty(token))
            {
                return this.RedirectToAction("Login", "Account");
            }

            var viewModel = new TaskSearchViewModel
            {
                SearchText = searchText,
                CurrentPage = pageNumber,
                ItemsPerPage = itemsPerPage,
                SearchPerformed = !string.IsNullOrWhiteSpace(searchText),
            };
            if (!string.IsNullOrWhiteSpace(searchText))
            {
                var searchResult = await this.taskWebApiService.GetPaginatedSearchedTasksAsync(searchText, token, pageNumber, itemsPerPage);

                viewModel.Tasks = searchResult.ResultList!;
                viewModel.TotalPages = searchResult.TotalPages;
                viewModel.TotalRecords = searchResult.TotalRecords;
            }
            else
            {
                viewModel.Tasks = new List<TaskSearchResultDto>();
            }

            return this.View(viewModel);
        }
        catch (HttpRequestException ex)
        {
            TaskLoggerMessages.HTTPErrorGettingSearchedTasks(this.logger, searchText, ex.Message, ex);
            return this.StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while fetching the data. Please try again later." });
        }
        catch (JsonException ex)
        {
            TaskLoggerMessages.ParsingErrorGettingSearchedTasks(this.logger, searchText, ex.Message, ex);
            throw;
        }
        catch (InvalidOperationException ioe)
        {
            TaskLoggerMessages.IOEGettingSearchedTasks(this.logger, searchText, ioe.Message, ioe);
            return this.StatusCode(StatusCodes.Status500InternalServerError, new { message = "An invalid operation occurred." });
        }
        catch (Exception ex)
        {
            TaskLoggerMessages.ErrorGettingSearchedTasks(this.logger, searchText, ex.Message, ex);
            throw;
        }
    }

    [HttpPost]
    public async Task<IActionResult> AddComment(AddCommentDto addCommentDto)
    {
        try
        {
            var token = this.User.FindFirst(ClaimTypes.Name)?.Value;
            if (string.IsNullOrEmpty(token))
            {
                return this.RedirectToAction("Login", "Account");
            }

            _ = await this.taskWebApiService.AddCommentAsync(addCommentDto, token);
            return this.RedirectToAction("TaskDetails", new { taskId = addCommentDto.TaskId });
        }
        catch (HttpRequestException ex)
        {
            TaskLoggerMessages.HTTPErrorWhileAddingCommentToTask(this.logger, addCommentDto.TaskId, ex.Message, ex);
            if (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return this.NotFound("The task was not found.");
            }
            else if (ex.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                this.ModelState.AddModelError(string.Empty, "Invalid data submitted.");
                return this.View("TaskDetails", new { taskId = addCommentDto.TaskId });
            }
            else
            {
                return this.StatusCode(500, "An error occurred while adding the comment. Please try again later.");
            }
        }
        catch (InvalidOperationException ex)
        {
            TaskLoggerMessages.IOExceptionWhileAddingComment(this.logger, ex.Message, ex);
            return this.StatusCode(500, "An error occurred while processing the response. Please try again later.");
        }
        catch (Exception ex)
        {
            TaskLoggerMessages.ErrorGettingWhileAddingComment(this.logger, ex.Message, ex);
            throw;
        }
    }

    [HttpPost]
    public async Task<IActionResult> EditComment(EditCommentDto editCommentDto)
    {
        if (!this.ModelState.IsValid)
        {
            return this.View("TaskDetails", new { taskId = editCommentDto.TaskId });
        }

        try
        {
            var token = this.User.FindFirst(ClaimTypes.Name)?.Value;
            if (string.IsNullOrEmpty(token))
            {
                return this.RedirectToAction("Login", "Account");
            }

            _ = await this.taskWebApiService.EditCommentAsync(editCommentDto, token);
            return this.RedirectToAction("TaskDetails", new { taskId = editCommentDto.TaskId });
        }
        catch (HttpRequestException ex)
        {
            if (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return this.NotFound("The task or comment was not found.");
            }
            else if (ex.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                this.ModelState.AddModelError(string.Empty, "Invalid data submitted.");
                return this.View("TaskDetails", new { taskId = editCommentDto.TaskId });
            }
            else
            {
                this.TempData["ErrorMessage"] = "An error occurred while editing the comment. Please try again later.";
                return this.RedirectToAction("TaskDetails", new { taskId = editCommentDto.TaskId });
            }
        }
        catch (InvalidOperationException ex)
        {
            TaskLoggerMessages.IOExceptionWhileEditingComment(this.logger, ex.Message, ex);
            return this.StatusCode(500, "An error occurred while processing the response. Please try again later.");
        }
        catch (Exception ex)
        {
            TaskLoggerMessages.ErrorGettingWhileEditingComment(this.logger, ex.Message, ex);
            throw;
        }
    }

    [HttpPost]
    public async Task<IActionResult> DeleteComment(int taskId, int commentId)
    {
        try
        {
            var token = this.User.FindFirst(ClaimTypes.Name)?.Value;
            if (string.IsNullOrEmpty(token))
            {
                return this.RedirectToAction("Login", "Account");
            }

            await this.taskWebApiService.DeleteCommentAsync(taskId, commentId, token);
            return this.RedirectToAction("TaskDetails", new { taskId = taskId });
        }
        catch (HttpRequestException)
        {
            return this.StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred while deleting comment");
        }
        catch (InvalidOperationException ioe)
        {
            TaskLoggerMessages.IOEWhileDeletingComment(this.logger, ioe.Message, ioe);
            return this.StatusCode(StatusCodes.Status500InternalServerError, new { message = "An invalid operation occurred while deleting comment." });
        }
        catch (Exception ex)
        {
            TaskLoggerMessages.ErrorDeletingComment(this.logger, ex.Message, ex);
            throw;
        }
    }
}
