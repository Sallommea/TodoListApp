using System.Net.Http;
using System.Net.Http.Json;
using TodoListApp.Services.Models;
using TodoListApp.WebApi.Models;

namespace TodoListApp.Services.WebApi.Services
{
    public class TodoListWebApiService
    {
        private readonly HttpClient httpClient;

        public TodoListWebApiService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<PaginatedListResult<TodoListDto>> GetPaginatedTodoListsAsync(int pageNumber, int itemsPerPage)
        {
            var response = await this.httpClient.GetAsync($"api/TodoList?pageNumber={pageNumber}&itemsPerPage={itemsPerPage}");
            _ = response.EnsureSuccessStatusCode();

            var paginatedTodoLists = await response.Content.ReadFromJsonAsync<PaginatedListResult<TodoListDto>>();
            return paginatedTodoLists ?? new PaginatedListResult<TodoListDto>();
        }

        public async Task DeleteTodoListAsync(int id)
        {
            var response = await this.httpClient.DeleteAsync($"api/TodoList/{id}");
            _ = response.EnsureSuccessStatusCode();
        }

        public async Task<int> AddTodoListAsync(CreateTodoList createTodoList)
        {
            var response = await this.httpClient.PostAsJsonAsync("api/TodoList", createTodoList);
            _ = response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<int>();
        }

        public async Task<TodoDetailsDto> GetTodoListAsync(int id, int pageNumber = 1, int itemsPerPage = 10)
        {
            var response = await this.httpClient.GetAsync($"api/TodoList/{id}?pageNumber={pageNumber}&itemsPerPage={itemsPerPage}");

            var todoDetailsDto = await response.Content.ReadFromJsonAsync<TodoDetailsDto>();
            return todoDetailsDto ?? new TodoDetailsDto();
        }

        public async Task UpdateTodoListAsync(UpdateTodo updateTodo)
        {
            var response = await this.httpClient.PutAsJsonAsync($"api/TodoList/{updateTodo.Id}", updateTodo);
            _ = response.EnsureSuccessStatusCode();
        }
    }
}
