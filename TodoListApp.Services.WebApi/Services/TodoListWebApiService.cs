using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using TodoListApp.Services.Models;
using TodoListApp.Services.WebApi.Logging;
using TodoListApp.WebApi.Models;

namespace TodoListApp.Services.WebApi.Services
{
    public class TodoListWebApiService
    {
        private readonly HttpClient httpClient;
        private readonly ILogger<TodoListWebApiService> logger;

        public TodoListWebApiService(HttpClient httpClient, ILogger<TodoListWebApiService> logger)
        {
            this.httpClient = httpClient;
            this.logger = logger;
        }

        public async Task<PaginatedListResult<TodoListDto>> GetPaginatedTodoListsAsync(int pageNumber, int itemsPerPage)
        {
            try
            {
                var response = await this.httpClient.GetAsync($"api/TodoList?pageNumber={pageNumber}&itemsPerPage={itemsPerPage}");
                _ = response.EnsureSuccessStatusCode();

                var paginatedTodoLists = await response.Content.ReadFromJsonAsync<PaginatedListResult<TodoListDto>>();
                return paginatedTodoLists ?? new PaginatedListResult<TodoListDto>();
            }
            catch (HttpRequestException ex)
            {
                TodoListServiceLoggerMessages.HTTPErrorWhileGettingLists(this.logger, ex.Message, ex);
                throw;
            }
            catch (Exception ex)
            {
                TodoListServiceLoggerMessages.ErrorGettingTodoLists(this.logger, ex.Message, ex);
                throw;
            }
        }

        public async Task DeleteTodoListAsync(int id)
        {
            try
            {
                var response = await this.httpClient.DeleteAsync($"api/TodoList/{id}");
                _ = response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException ex)
            {
                TodoListServiceLoggerMessages.HTTPErrorWhileDeleting(this.logger, id, ex.Message, ex);
                throw;
            }
            catch (Exception ex)
            {
                TodoListServiceLoggerMessages.ErrorDeletingTodoList(this.logger, id, ex.Message, ex);
                throw;
            }
        }

        public async Task<int> AddTodoListAsync(CreateTodoList createTodoList)
        {
            try
            {
                var response = await this.httpClient.PostAsJsonAsync("api/TodoList", createTodoList);
                _ = response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<int>();
            }
            catch (HttpRequestException ex)
            {
                TodoListServiceLoggerMessages.HTTPErrorWhileAddingList(this.logger, ex.Message, ex);
                throw;
            }
            catch (Exception ex)
            {
                TodoListServiceLoggerMessages.ErrorAddingTodoList(this.logger, ex.Message, ex);
                throw;
            }
        }

        public async Task<TodoDetailsDto> GetTodoListAsync(int id, int pageNumber = 1, int itemsPerPage = 10)
        {
            try
            {
                var response = await this.httpClient.GetAsync($"api/TodoList/{id}?pageNumber={pageNumber}&itemsPerPage={itemsPerPage}");

                var todoDetails = await response.Content.ReadFromJsonAsync<TodoDetailsDto>();

                if (todoDetails == null)
                {
                    throw new InvalidOperationException("Failed to deserialize todo list details.");
                }

                return todoDetails;
            }
            catch (HttpRequestException ex)
            {
                TodoListServiceLoggerMessages.HTTPErrorGettingTodo(this.logger, id, ex.Message, ex);
                throw;
            }
            catch (Exception ex)
            {
                TodoListServiceLoggerMessages.ErrorGettingTodoList(this.logger, id, ex.Message, ex);
                throw;
            }
        }

        public async Task UpdateTodoListAsync(UpdateTodo updateTodo)
        {
            try
            {
                var response = await this.httpClient.PutAsJsonAsync($"api/TodoList/{updateTodo.Id}", updateTodo);
                _ = response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException ex)
            {
                TodoListServiceLoggerMessages.HTTPErrorUpdatingTodo(this.logger, updateTodo.Id, ex.Message, ex);
                throw;
            }
            catch (Exception ex)
            {
                TodoListServiceLoggerMessages.ErrorUpdatingTodoList(this.logger, updateTodo.Id, ex.Message, ex);
                throw;
            }
        }
    }
}
