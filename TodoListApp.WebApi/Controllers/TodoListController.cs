using System.Data.Common;
using Microsoft.AspNetCore.Mvc;
using TodoListApp.Services.Exceptions;
using TodoListApp.Services.Interfaces;
using TodoListApp.Services.Models;
using TodoListApp.WebApi.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860
namespace TodoListApp.WebApi.Controllers;
[Route("api/[controller]")]
[ApiController]
public class TodoListController : ControllerBase
{
    private readonly ITodoListService todoListService;

    public TodoListController(ITodoListService todoListService)
    {
        this.todoListService = todoListService;
    }

    // GET: api/<TodoListController>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TodoListDto>>> GetAllTodoLists()
    {
        try
        {
            var todoLists = await this.todoListService.GetAllTodoListsAsync();
            var todoListDtos = todoLists.Select(todoList => new TodoListDto
            {
                Id = todoList.Id,
                Name = todoList.Name,
                Description = todoList.Description,
                CreatedDate = todoList.CreatedDate,
                DueDate = todoList.DueDate,
                TaskCount = todoList.TaskCount,
                IsShared = todoList.IsShared,
            });
            return this.Ok(todoListDtos);
        }
        catch (DbException)
        {
            return this.StatusCode(StatusCodes.Status503ServiceUnavailable, "The database is currently unavailable.");
        }
        catch (Exception)
        {
            return this.StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
        }
    }

    // POST api/<TodoListController>
    [HttpPost]
    public async Task<ActionResult<TodoListDto>> PostTodoList(TodoListDto todoListDto)
    {
        if (!this.ModelState.IsValid)
        {
            return this.BadRequest(this.ModelState);
        }

        var todoList = new TodoList
        {
            Name = todoListDto.Name,
            Description = todoListDto.Description,
            CreatedDate = DateTime.UtcNow,
            DueDate = todoListDto.DueDate,
            TaskCount = 0,
            IsShared = todoListDto.IsShared,
        };

        try
        {
            await this.todoListService.AddTodoListAsync(todoList);
            return this.CreatedAtAction(nameof(this.PostTodoList), todoListDto);
        }
        catch (TodoListException ex)
        {
            return this.StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    // PUT api/<TodoListController>/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutTodoList(int id, TodoListDto todoListDto)
    {
        if (id != todoListDto.Id)
        {
            return this.BadRequest();
        }

        var todoList = new TodoList
        {
            Id = todoListDto.Id,
            Name = todoListDto.Name,
            Description = todoListDto.Description,
            CreatedDate = todoListDto.CreatedDate,
            DueDate = todoListDto.DueDate,
            TaskCount = todoListDto.TaskCount,
            IsShared = todoListDto.IsShared,
        };

        try
        {
            await this.todoListService.UpdateTodoListAsync(todoList);
            return this.NoContent();
        }
        catch (TodoListException ex)
        {
            return this.StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
        catch (DbException)
        {
            return this.StatusCode(StatusCodes.Status503ServiceUnavailable, "The database is currently unavailable.");
        }
    }

    // DELETE api/<TodoListController>/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTodoList(int id)
    {
        try
        {
            await this.todoListService.DeleteTodoListAsync(id);
            return this.NoContent();
        }
        catch (TodoListException ex)
        {
            return this.StatusCode(StatusCodes.Status404NotFound, ex.Message);
        }
        catch (DbException)
        {
            return this.StatusCode(StatusCodes.Status503ServiceUnavailable, "The database is currently unavailable.");
        }
    }
}
