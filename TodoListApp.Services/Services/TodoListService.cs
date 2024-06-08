using Microsoft.EntityFrameworkCore;
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

    public TodoListService(ITodoListRepository todoListRepo)
    {
        this.todoListRepo = todoListRepo;
    }

    public async Task<PaginatedListResult<TodoListDto>> GetPaginatedTodoListsAsync(int pageNumber, int itemsPerPage)
    {
        try
        {
            var paginatedEntities = await this.todoListRepo.GetPaginatedTodoListsAsync(pageNumber, itemsPerPage);
            var result = new PaginatedListResult<TodoListDto>
            {
                TotalRecords = paginatedEntities.TotalRecords,
                TotalPages = paginatedEntities.TotalPages,
                ResultList = paginatedEntities.ResultList!.Select(e => new TodoListDto
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

    public async Task<TodoDetailsDto?> GetTodoListWithTasksAsync(int todoListId)
    {
        var todoListEntity = await this.todoListRepo.GetTodoListWithTasksAsync(todoListId);
        if (todoListEntity == null)
        {
            return null;
        }

        var currentDate = DateTime.UtcNow;

        foreach (var task in todoListEntity.Tasks!)
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

        var todoListDto = new TodoDetailsDto
        {
            Id = todoListEntity.Id,
            Name = todoListEntity.Name,
            Description = todoListEntity.Description,
            TaskCount = todoListEntity.TaskCount,
            Tasks = todoListEntity.Tasks?.Select(task => new TaskDto
            {
                Id = task.Id,
                Title = task.Title,
                DueDate = task.DueDate,
                Status = (WebApi.Models.Tasks.Status)task.Status,
                IsExpired = task.IsExpired,
            }).ToList() ?? new List<TaskDto>(),
        };

        return todoListDto;
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
            // Log exception (consider using a logging framework like Serilog or NLog)
            throw new TodoListException("An error occurred while adding the todo list.", ex);
        }
        catch (Exception ex)
        {
            // Log exception (consider using a logging framework like Serilog or NLog)
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
