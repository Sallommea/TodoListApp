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
        this.logger.FetchingPaginatedTodoLists(pageNumber, itemsPerPage);
        try
        {
            var paginatedTodoLists = await this.todoListService.GetPaginatedTodoListsAsync(pageNumber, itemsPerPage);

            this.logger.SuccessfullyFetchedPaginatedTodoLists();

            return this.Ok(paginatedTodoLists);
        }
        catch (TodoListException ex)
        {
            this.logger.TodoListExceptionOccurred(ex);
            return this.StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
        catch (Exception ex)
        {
            this.logger.UnexpectedErrorOccurred(ex);
            return this.StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
        }
    }

    // GET: api/<TodoListController>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetTodoList(int id)
    {
        var todoListDto = await this.todoListService.GetTodoListWithTasksAsync(id);
        if (todoListDto == null)
        {
            return this.NotFound();
        }

        return this.Ok(todoListDto);
    }

    // POST api/<TodoListController>
    [HttpPost]
    public async Task<ActionResult<int>> PostTodoList(CreateTodoList createTodoList)
    {
        if (!this.ModelState.IsValid)
        {
            return this.BadRequest(this.ModelState);
        }

        try
        {
            var createdTodoList = await this.todoListService.AddTodoListAsync(createTodoList);
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
        catch (Exception ex)
        {
            this.logger.UnexpectedErrorOccurredWhileAddingTodoList(ex);
            return this.StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
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

        try
        {
            await this.todoListService.UpdateTodoListAsync(updateTodo);
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
        catch (Exception ex)
        {
            this.logger.UnexpectedErrorOccurredWhileUpdatingTodoList(ex);
            return this.StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
        }
    }

    // DELETE api/<TodoListController>/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTodoList(int id)
    {
        try
        {
            await this.todoListService.DeleteTodoListAsync(id);
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
        catch (Exception ex)
        {
            this.logger.UnexpectedErrorOccurredWhileDeletingTodoList(ex);
            return this.StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
        }
    }
}
