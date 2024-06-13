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
            response.EnsureSuccessStatusCode();
        }

        public async Task<int> AddTodoListAsync(CreateTodoList createTodoList)
        {
            var response = await this.httpClient.PostAsJsonAsync("api/TodoList", createTodoList);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<int>();
        }
    }
}
