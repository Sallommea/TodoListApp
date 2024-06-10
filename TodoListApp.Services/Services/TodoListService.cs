using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TodoListApp.Services.Database;
using TodoListApp.Services.Database.Repositories;
using TodoListApp.Services.Exceptions;
using TodoListApp.Services.Interfaces;
using TodoListApp.Services.Models;
using TodoListApp.WebApi.Models;
using TodoListApp.WebApi.Models.Tasks;

namespace TodoListApp.Services.Services;
public class TodoListService : ITodoListService
{
    private readonly ITodoListRepository todoListRepo;
    private readonly ILogger<TodoListService> logger;

    public TodoListService(ITodoListRepository todoListRepo, ILogger<TodoListService> logger)
    {
        this.todoListRepo = todoListRepo;
        this.logger = logger;
    }

    public async Task<PaginatedListResult<TodoListDto>> GetPaginatedTodoListsAsync(int pageNumber, int itemsPerPage)
    {
        try
        {
            var todoLists = await this.todoListRepo.GetAllTodoListsAsync();
            var totalRecords = todoLists.Count();
            var paginatedTodoLists = todoLists
            .Skip((pageNumber - 1) * itemsPerPage)
            .Take(itemsPerPage)
            .ToList();

            var totalPages = (int)Math.Ceiling((double)totalRecords / itemsPerPage);
            var result = new PaginatedListResult<TodoListDto>
            {
                TotalRecords = totalRecords,
                TotalPages = totalPages,
                ResultList = paginatedTodoLists.Select(e => new TodoListDto
                {
                    Id = e.Id,
                    Name = e.Name,
                    TaskCount = e.TaskCount,
                }).ToList(),
            };
            return result;
        }
        catch (Exception ex)
        {
            throw new TodoListException("An unexpected error occurred while retrieving the todo lists.", ex);
        }
    }

    public async Task<TodoDetailsDto?> GetTodoListWithTasksAsync(int todoListId, int taskPageNumber, int tasksPerPage)
    {
        try
        {
            var result = await this.todoListRepo.GetTodoListWithTasksAsync(todoListId);

            if (result is null)
            {
                throw new TodoListException($"TodoList with ID {todoListId} not found.");
            }

            var tasks = result.Tasks?.OrderBy(t => t.Id).ToList() ?? new List<TaskEntity>();

            var totalTaskPages = (int)Math.Ceiling((double)tasks.Count / tasksPerPage);
            var paginatedTasks = tasks
                .Skip((taskPageNumber - 1) * tasksPerPage)
                .Take(tasksPerPage)
                .ToList();

            var currentDate = DateTime.UtcNow;

            foreach (var task in paginatedTasks)
            {
                if (task.DueDate.HasValue && task.DueDate.Value < currentDate)
                {
                    task.IsExpired = true;
                }
                else
                {
                    task.IsExpired = false;
                }
            }

            await this.todoListRepo.SaveChangesAsync();

            var todoListDetailsDto = new TodoDetailsDto
            {
                Id = result.Id!,
                Name = result.Name!,
                Description = result.Description!,
                Tasks = paginatedTasks.Select(task => new TaskDto
                {
                    Id = task.Id,
                    Title = task.Title,
                    DueDate = task.DueDate,
                    Status = (WebApi.Models.Tasks.Status)task.Status,
                    IsExpired = task.IsExpired,
                }).ToList(),
                TotalTasks = tasks.Count,
                TotalTaskPages = totalTaskPages,
                CurrentTaskPage = taskPageNumber,
            };

            return todoListDetailsDto;
        }
        catch (TodoListException ex)
        {
            this.logger.LogWarning(ex, "TodoList not found: {TodoListId}", todoListId);
            throw;
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "An unexpected error occurred while retrieving the todo list with ID {TodoListId}.", todoListId);
            throw new TodoListException("An unexpected error occurred while retrieving the todo list.", ex);
        }
    }

    public async Task<TodoListEntity> AddTodoListAsync(CreateTodoList todoList)
    {
        try
        {
            var entity = new TodoListEntity
            {
                Name = todoList.Name,
                Description = todoList.Description,
                TaskCount = 0,
            };

            await this.todoListRepo.AddTodoListAsync(entity);
            return entity;
        }
        catch (DbUpdateException ex)
        {
            throw new TodoListException("An error occurred while adding the todo list.", ex);
        }
        catch (Exception ex)
        {
            throw new TodoListException("An unexpected error occurred while adding the todo list.", ex);
        }
    }

    public async Task DeleteTodoListAsync(int id)
    {
        try
        {
            await this.todoListRepo.DeleteTodoListAsync(id);
        }
        catch (KeyNotFoundException ex)
        {
            throw new TodoListException($"TodoList with ID {id} not found.", ex);
        }
        catch (Exception ex)
        {
            throw new TodoListException("An unexpected error occurred while deleting the todo list.", ex);
        }
    }

    public async Task UpdateTodoListAsync(UpdateTodo updateTodo)
    {
        try
        {
            var entity = new TodoListEntity
            {
                Id = updateTodo.Id,
                Name = updateTodo.Name,
                Description = updateTodo.Description,
            };

            await this.todoListRepo.UpdateTodoListAsync(entity);
        }
        catch (KeyNotFoundException ex)
        {
            throw new TodoListException($"TodoList with ID {updateTodo.Id} not found.", ex);
        }
        catch (Exception ex)
        {
            throw new TodoListException("An unexpected error occurred while updating the todo list.", ex);
        }
    }
}
