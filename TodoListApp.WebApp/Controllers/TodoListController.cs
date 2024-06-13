using Microsoft.AspNetCore.Mvc;
using TodoListApp.Services.WebApi.Services;
using TodoListApp.WebApi.Models;
using TodoListApp.WebApp.Models;

namespace TodoListApp.WebApp.Controllers;
public class TodoListController : Controller
{
    private readonly ILogger<TodoListController> logger;
    private readonly TodoListWebApiService todoListWebApiService;

    public TodoListController(ILogger<TodoListController> logger, TodoListWebApiService todoListWebApiService)
    {
        this.logger = logger;
        this.todoListWebApiService = todoListWebApiService;
    }

    public async Task<IActionResult> Index(int pageNumber = 1)
    {
        var paginatedTodoLists = await this.todoListWebApiService.GetPaginatedTodoListsAsync(pageNumber, 9);

        var viewModel = new TodoListViewModel
        {
            TodoLists = paginatedTodoLists.ResultList,
            TotalPages = paginatedTodoLists.TotalPages ?? 0,
            TotalRecord = paginatedTodoLists.TotalRecords ?? 0,
            CurrentPage = pageNumber,
        };

        return this.View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await this.todoListWebApiService.DeleteTodoListAsync(id);
            this.logger.LogInformation($"Todo list with ID {id} deleted.");
            return this.RedirectToAction("Index");
        }
        catch (KeyNotFoundException ex)
        {
            this.logger.LogWarning($"Todo list with ID {id} not found: {ex.Message}");
            return this.NotFound();
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, $"Error deleting todo list with ID {id}");
            return this.StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [HttpGet]
    public IActionResult CreateTodoList()
    {
        return this.View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateTodoList createTodoList)
    {
        if (this.ModelState.IsValid)
        {
            try
            {
                _ = await this.todoListWebApiService.AddTodoListAsync(createTodoList);
                return this.RedirectToAction(nameof(this.Index));
            }
            catch (Exception ex)
            {
                this.ModelState.AddModelError(string.Empty, "An error occurred while creating the todo list.");
            }
        }

        return this.View("CreateTodoList", createTodoList);
    }
}
