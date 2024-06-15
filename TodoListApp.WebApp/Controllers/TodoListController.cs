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
        catch (HttpRequestException ex)
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

    public async Task<IActionResult> Edit(int id)
    {
        var todoDetails = await this.todoListWebApiService.GetTodoListAsync(id);
        var updateTodo = new UpdateTodo
        {
            Id = todoDetails.Id,
            Name = todoDetails.Name,
            Description = todoDetails.Description,
        };
        return this.View(updateTodo);
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
            catch (HttpRequestException)
            {
                this.ModelState.AddModelError(string.Empty, "An error occurred while creating the todo list.");
            }
        }

        return this.View("CreateTodoList", createTodoList);
    }

    public async Task<IActionResult> Details(int id, int pageNumber = 1, int itemsPerPage = 10)
    {
        try
        {
            var todoDetails = await this.todoListWebApiService.GetTodoListAsync(id, pageNumber, itemsPerPage);
            if (todoDetails == null)
            {
                return this.NotFound();
            }

            return this.View(todoDetails);
        }
        catch (HttpRequestException)
        {
            return this.StatusCode(500);
        }
    }

    [HttpPost]
    public async Task<IActionResult> UpdateTodo(UpdateTodo updateTodo)
    {
        if (!this.ModelState.IsValid)
        {
            return this.View("Edit", updateTodo);
        }

        try
        {
            await this.todoListWebApiService.UpdateTodoListAsync(updateTodo);
            return this.RedirectToAction("Details", new { id = updateTodo.Id });
        }
        catch (HttpRequestException)
        {
            this.ModelState.AddModelError(string.Empty, "An error occurred while updating the todo list.");
            return this.View("Edit", updateTodo);
        }
    }
}
