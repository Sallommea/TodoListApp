using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using TodoListApp.Services.Exceptions;
using TodoListApp.Services.Interfaces;
using TodoListApp.Services.Models;

namespace TodoListApp.Services.Database;
public class TodoListDatabaseService : ITodoListService
{
    private readonly TodoListDbContext dbContext;

    public TodoListDatabaseService(TodoListDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<IEnumerable<TodoList>> GetAllTodoListsAsync()
    {
        return await this.dbContext.TodoLists
            .Select(todoList => new TodoList
            {
                Id = todoList.Id,
                Name = todoList.Name,
                Description = todoList.Description,
                CreatedDate = todoList.CreatedDate,
                DueDate = todoList.DueDate,

                // TaskCount = todoList.Tasks.Count(), // not yet implemented
                IsShared = todoList.IsShared,
            })
        .ToListAsync();
    }

    public async Task AddTodoListAsync(TodoList todoList)
    {
        try
        {
            var entity = new TodoListEntity
            {
                Name = todoList.Name,
                Description = todoList.Description,
                CreatedDate = todoList.CreatedDate,
                DueDate = todoList.DueDate,
                TaskCount = todoList.TaskCount,
                IsShared = todoList.IsShared,
            };

            _ = await this.dbContext.TodoLists.AddAsync(entity);
            _ = await this.dbContext.SaveChangesAsync();
        }
        catch (DbException ex)
        {
            throw new TodoListException("An error occurred while adding the todo list.", ex);
        }
    }

    public async Task DeleteTodoListAsync(int id)
    {
        try
        {
            var entity = await this.dbContext.TodoLists.FindAsync(id);

            if (entity != null)
            {
                _ = this.dbContext.TodoLists.Remove(entity);
                _ = await this.dbContext.SaveChangesAsync();
            }
            else
            {
                throw new TodoListException($"Todo list with ID {id} not found.");
            }
        }
        catch (DbException ex)
        {
            throw new TodoListException("An error occurred while deleting the todo list.", ex);
        }
    }

    public async Task UpdateTodoListAsync(TodoList todoList)
    {
        try
        {
            var entity = await this.dbContext.TodoLists.FindAsync(todoList.Id);

            if (entity != null)
            {
                entity.Name = todoList.Name;
                entity.Description = todoList.Description;
                entity.DueDate = todoList.DueDate;
                entity.IsShared = todoList.IsShared;

                _ = this.dbContext.TodoLists.Update(entity);
                _ = await this.dbContext.SaveChangesAsync();
            }
            else
            {
                throw new TodoListException($"Todo list with ID {todoList.Id} not found.");
            }
        }
        catch (DbException ex)
        {
            throw new TodoListException("An error occurred while updating the todo list.", ex);
        }
    }
}
