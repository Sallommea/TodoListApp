using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoListApp.Services.WebApi.Services;
using TodoListApp.WebApi.Models;
using TodoListApp.WebApp.Logging;
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

#pragma warning disable S6967 // ModelState.IsValid should be called in controller actions
    [Authorize]
    public async Task<IActionResult> Index(int pageNumber = 1)
    {
        try
        {
            var token = this.User.FindFirst(ClaimTypes.Name)?.Value;
            if (string.IsNullOrEmpty(token))
            {
                return this.RedirectToAction("Login", "Account");
            }

            var paginatedTodoLists = await this.todoListWebApiService.GetPaginatedTodoListsAsync(pageNumber, 9, token);

            var viewModel = new TodoListViewModel
            {
                TodoLists = paginatedTodoLists.ResultList,
                TotalPages = paginatedTodoLists.TotalPages ?? 0,
                TotalRecord = paginatedTodoLists.TotalRecords ?? 0,
                CurrentPage = pageNumber,
            };

            return this.View(viewModel);
        }
        catch (HttpRequestException ex)
        {
            TodoListLoggerMessages.HTTPErrorWhileGettingLists(this.logger, ex.Message, ex);
            return this.StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while fetching the data. Please try again later." });
        }
        catch (InvalidOperationException ioe)
        {
            TodoListLoggerMessages.IOEGettingTodoLists(this.logger, ioe.Message, ioe);
            return this.StatusCode(StatusCodes.Status500InternalServerError, new { message = "An invalid operation occurred." });
        }
        catch (Exception ex)
        {
            TodoListLoggerMessages.ErrorGettingTodoLists(this.logger, ex.Message, ex);
            throw;
        }
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var token = this.User.FindFirst(ClaimTypes.Name)?.Value;
            if (string.IsNullOrEmpty(token))
            {
                return this.RedirectToAction("Login", "Account");
            }

            await this.todoListWebApiService.DeleteTodoListAsync(id, token);
            return this.RedirectToAction("Index");
        }
        catch (KeyNotFoundException)
        {
            return this.NotFound();
        }
        catch (HttpRequestException)
        {
            return this.StatusCode(StatusCodes.Status500InternalServerError);
        }
        catch (InvalidOperationException ioe)
        {
            TodoListLoggerMessages.IOEWhileDeleting(this.logger, id, ioe.Message, ioe);
            return this.StatusCode(StatusCodes.Status500InternalServerError, new { message = "An invalid operation occurred." });
        }
        catch (Exception ex)
        {
            TodoListLoggerMessages.ErrorDeletingTodoList(this.logger, id, ex.Message, ex);
            throw;
        }
    }

    [HttpGet]
    [Authorize]
    public IActionResult CreateTodoList()
    {
        return this.View();
    }

    [Authorize]
    public async Task<IActionResult> Edit(int id)
    {
        try
        {
            var token = this.User.FindFirst(ClaimTypes.Name)?.Value;
            if (string.IsNullOrEmpty(token))
            {
                return this.RedirectToAction("Login", "Account");
            }

            var todoDetails = await this.todoListWebApiService.GetTodoListAsync(id, token);

            if (todoDetails == null)
            {
                return this.NotFound(new { message = "Todo list not found." });
            }

            var updateTodo = new UpdateTodo
            {
                Id = todoDetails.Id,
                Name = todoDetails.Name,
                Description = todoDetails.Description,
            };

            return this.View(updateTodo);
        }
        catch (HttpRequestException)
        {
            return this.StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while fetching the todo list. Please try again later." });
        }
        catch (InvalidOperationException ioe)
        {
            TodoListLoggerMessages.IOErrorFetchingTodoForUpdate(this.logger, id, ioe.Message, ioe);
            return this.StatusCode(StatusCodes.Status500InternalServerError, new { message = "An invalid operation occurred: " + ioe.Message });
        }
        catch (Exception ex)
        {
            TodoListLoggerMessages.ErrorFetchingTodoForUpdate(this.logger, id, ex.Message, ex);
            throw;
        }
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create(CreateTodoList createTodoList)
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

                _ = await this.todoListWebApiService.AddTodoListAsync(createTodoList, token);
                return this.RedirectToAction(nameof(this.Index));
            }
            catch (HttpRequestException)
            {
                this.ModelState.AddModelError(string.Empty, "An error occurred while creating the todo list.");
            }
            catch (InvalidOperationException ioe)
            {
                TodoListLoggerMessages.IOErrorWhileAddingList(this.logger, ioe.Message, ioe);
                return this.StatusCode(StatusCodes.Status500InternalServerError, new { message = "An invalid operation occurred: " + ioe.Message });
            }
            catch (Exception ex)
            {
                TodoListLoggerMessages.ErrorAddingTodoList(this.logger, ex.Message, ex);
                throw;
            }
        }

        return this.View("CreateTodoList", createTodoList);
    }

    [Authorize]
    public async Task<IActionResult> Details(int id, int pageNumber = 1, int itemsPerPage = 2)
    {
        try
        {
            var token = this.User.FindFirst(ClaimTypes.Name)?.Value;
            if (string.IsNullOrEmpty(token))
            {
                return this.RedirectToAction("Login", "Account");
            }

            var todoDetails = await this.todoListWebApiService.GetTodoListAsync(id, token, pageNumber, itemsPerPage);
            if (todoDetails == null)
            {
                return this.NotFound();
            }

            return this.View(todoDetails);
        }
        catch (HttpRequestException)
        {
            return this.StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while fetching the data. Please try again later." });
        }
        catch (InvalidOperationException ioe)
        {
            TodoListLoggerMessages.IOErrorGettingTodo(this.logger, id, ioe.Message, ioe);
            return this.StatusCode(StatusCodes.Status500InternalServerError, new { message = "An invalid operation occurred: " + ioe.Message });
        }
        catch (Exception ex)
        {
            TodoListLoggerMessages.ErrorGettingTodoList(this.logger, id, ex.Message, ex);
            throw;
        }
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> UpdateTodo(UpdateTodo updateTodo)
    {
        if (!this.ModelState.IsValid)
        {
            return this.View("Edit", updateTodo);
        }

        try
        {
            var token = this.User.FindFirst(ClaimTypes.Name)?.Value;
            if (string.IsNullOrEmpty(token))
            {
                return this.RedirectToAction("Login", "Account");
            }

            await this.todoListWebApiService.UpdateTodoListAsync(updateTodo, token);
            return this.RedirectToAction("Details", new { id = updateTodo.Id });
        }
        catch (HttpRequestException)
        {
            this.ModelState.AddModelError(string.Empty, "An error occurred while updating the todo list.");
            return this.View("Edit", updateTodo);
        }
        catch (InvalidOperationException ioe)
        {
            TodoListLoggerMessages.IOErrorUpdatingTodo(this.logger, updateTodo.Id, ioe.Message, ioe);
            return this.StatusCode(StatusCodes.Status500InternalServerError, new { message = "An invalid operation occurred: " + ioe.Message });
        }
        catch (Exception ex)
        {
            TodoListLoggerMessages.ErrorUpdatingTodoList(this.logger, updateTodo.Id, ex.Message, ex);
            throw;
        }
    }
}
