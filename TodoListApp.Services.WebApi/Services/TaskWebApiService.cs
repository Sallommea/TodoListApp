using System.Net.Http.Json;
using TodoListApp.WebApi.Models;
using TodoListApp.WebApi.Models.Tasks;

namespace TodoListApp.Services.WebApi.Services;
public class TaskWebApiService
{
    private readonly HttpClient httpClient;

    public TaskWebApiService(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task<int> AddTaskAsync(CreateTaskDto createTask)
    {
        var response = await this.httpClient.PostAsJsonAsync("api/Tasks", createTask);
        _ = response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<int>();
    }

    public async Task DeleteTaskAsync(int taskId)
    {
        var response = await this.httpClient.DeleteAsync($"api/Tasks/{taskId}");

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException($"Error deleting task: {response.StatusCode}, {errorContent}");
        }
    }
}
