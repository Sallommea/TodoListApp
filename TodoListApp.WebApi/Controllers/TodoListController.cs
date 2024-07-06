using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoListApp.Services.Exceptions;
using TodoListApp.Services.Interfaces;
using TodoListApp.WebApi.Logging;
using TodoListApp.WebApi.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860
namespace TodoListApp.WebApi.Controllers;
[Route("api/[controller]")]
[ApiController]
[Authorize]
public class TodoListController : ControllerBase
{
    private readonly ITodoListService todoListService;
    private readonly ILogger<TodoListController> logger;

    public TodoListController(ITodoListService todoListService, ILogger<TodoListController> logger)
    {
        this.todoListService = todoListService;
        this.logger = logger;
    }

    // GET: api/<TodoListController>
    [HttpGet]
    public async Task<IActionResult> GetPaginatedTodoLists([FromQuery] int pageNumber = 1, [FromQuery] int itemsPerPage = 10)
    {
        if (pageNumber <= 0)
        {
            LoggerMessages.InvalidPageNumber(this.logger, pageNumber);
            return this.BadRequest(new { message = "Page number must be greater than zero." });
        }

        if (itemsPerPage <= 0)
        {
            LoggerMessages.InvalidItemsPerPage(this.logger, itemsPerPage);
            return this.BadRequest(new { message = "Items per page must be greater than zero." });
        }

        string? userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return this.Unauthorized();
        }

        this.logger.FetchingPaginatedTodoLists(pageNumber, itemsPerPage);

        try
        {
            var paginatedTodoLists = await this.todoListService.GetPaginatedTodoListsAsync(userId, pageNumber, itemsPerPage);

            this.logger.SuccessfullyFetchedPaginatedTodoLists();

            return this.Ok(paginatedTodoLists);
        }
        catch (TodoListException ex)
        {
            this.logger.TodoListExceptionOccurred(ex);
            return this.StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
        catch (InvalidOperationException ioe)
        {
            LoggerMessages.InvalidOperationOccurredWhileGettingTodoLists(this.logger, ioe.Message, ioe);
            return this.StatusCode(StatusCodes.Status500InternalServerError, "An invalid operation occured");
        }
        catch (Exception ex)
        {
            this.logger.UnexpectedErrorOccurred(ex);
            throw;
        }
    }

    // GET: api/<TodoListController>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetTodoList(int id, int pageNumber = 1, int itemsPerPage = 10)
    {
        if (pageNumber <= 0)
        {
            LoggerMessages.InvalidPageNumber(this.logger, pageNumber);
            return this.BadRequest(new { message = "Page number must be greater than zero." });
        }

        if (itemsPerPage <= 0)
        {
            LoggerMessages.InvalidItemsPerPage(this.logger, itemsPerPage);
            return this.BadRequest(new { message = "Tasks per page must be greater than zero." });
        }

        string? userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return this.Unauthorized();
        }

        try
        {
            var todoListDto = await this.todoListService.GetTodoListWithTasksAsync(id, userId, pageNumber, itemsPerPage);
            if (todoListDto == null)
            {
                this.logger.TodoListNotFound(id);
                return this.NotFound(new { message = "Todo list not found." });
            }

            return this.Ok(todoListDto);
        }
        catch (TodoListException ex)
        {
            this.logger.TodoListExceptionOccurredWhileGettingTodoDetails(ex);
            return this.NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ioe)
        {
            return this.StatusCode(StatusCodes.Status500InternalServerError, "An invalid operation occured" + ioe.Message);
        }
        catch (Exception ex)
        {
            this.logger.UnexpectedErrorOccurredWhileGettingTodoDetails(ex);
            throw;
        }
    }

    // POST api/<TodoListController>
    [HttpPost]
    public async Task<ActionResult<int>> PostTodoList(CreateTodoList createTodoList)
    {
        if (!this.ModelState.IsValid)
        {
            return this.BadRequest(this.ModelState);
        }

        string? userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return this.Unauthorized();
        }

        try
        {
            var createdTodoList = await this.todoListService.AddTodoListAsync(createTodoList, userId);
            this.logger.TodoListAdded(createTodoList.Name);
            return this.CreatedAtAction(nameof(this.PostTodoList), new { id = createdTodoList.Id }, createdTodoList.Id);
        }
        catch (TodoListException ex)
        {
            if (ex.InnerException is DbUpdateException)
            {
                this.logger.DatabaseErrorOccurred(ex);
                return this.StatusCode(StatusCodes.Status500InternalServerError, "A database error occurred while adding the todo list.");
            }
            else
            {
                this.logger.TodoListExceptionOccurredWhileAddingTodoList(ex.Message, ex);
                return this.StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        catch (InvalidOperationException ioe)
        {
            return this.StatusCode(StatusCodes.Status500InternalServerError, "An invalid operation occured" + ioe.Message);
        }
        catch (Exception ex)
        {
            this.logger.UnexpectedErrorOccurredWhileAddingTodoList(ex);
            throw;
        }
    }

    // PUT api/<TodoListController>/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutTodoList(int id, UpdateTodo updateTodo)
    {
        if (id != updateTodo.Id)
        {
            return this.BadRequest("ID mismatch between route parameter and request body.");
        }

        if (!this.ModelState.IsValid)
        {
            return this.BadRequest(this.ModelState);
        }

        string? userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return this.Unauthorized();
        }

        try
        {
            await this.todoListService.UpdateTodoListAsync(updateTodo, userId);
            this.logger.TodoListUpdated(updateTodo.Id, updateTodo.Name);
            return this.NoContent();
        }
        catch (TodoListException ex) when (ex.InnerException is KeyNotFoundException)
        {
            return this.NotFound(ex.Message);
        }
        catch (TodoListException ex)
        {
            this.logger.TodoListExceptionOccurredWhileUpdatingTodoList(ex.Message, ex);
            return this.StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
        catch (InvalidOperationException ioe)
        {
            return this.StatusCode(StatusCodes.Status500InternalServerError, "An invalid operation occured" + ioe.Message);
        }
        catch (Exception ex)
        {
            this.logger.UnexpectedErrorOccurredWhileUpdatingTodoList(ex);
            throw;
        }
    }

    // DELETE api/<TodoListController>/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTodoList(int id)
    {
        string? userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return this.Unauthorized();
        }

        try
        {
            await this.todoListService.DeleteTodoListAsync(id, userId);
            this.logger.TodoListDeleted(id);
            return this.NoContent();
        }
        catch (TodoListException ex) when (ex.InnerException is KeyNotFoundException)
        {
            return this.NotFound(ex.Message);
        }
        catch (TodoListException ex)
        {
            this.logger.TodoListExceptionOccurredWhileDeletingTodoList(ex.Message, ex);
            return this.StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
        catch (InvalidOperationException ioe)
        {
            return this.StatusCode(StatusCodes.Status500InternalServerError, "An invalid operation occured" + ioe.Message);
        }
        catch (Exception ex)
        {
            this.logger.UnexpectedErrorOccurredWhileDeletingTodoList(ex);
            throw;
        }
    }
}
