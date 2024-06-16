using System.Net.Http.Json;
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

    public async Task<TaskDetailsDto> GetTaskDetailsAsync(int taskId)
    {
        var response = await this.httpClient.GetAsync($"api/Tasks/{taskId}");

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException($"Error getting task details: {response.StatusCode}, {errorContent}");
        }

        var taskDetails = await response.Content.ReadFromJsonAsync<TaskDetailsDto>();

        if (taskDetails == null)
        {
            throw new InvalidOperationException("Failed to deserialize task details.");
        }

        return taskDetails;
    }
}
