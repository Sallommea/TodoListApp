using System.Net.Http;
using System.Net.Http.Json;
using TodoListApp.Services.Models;
using TodoListApp.WebApi.Models;
using TodoListApp.WebApi.Models.Tasks;

namespace TodoListApp.Services.WebApi.Services;
public class AssignedTaskWebApiService
{
    private readonly HttpClient httpClient;

    public AssignedTaskWebApiService(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task<PaginatedListResult<TaskDetailsDto>> GetTasksAssignedToMeAsync(
        int pageNumber = 1,
        int tasksPerPage = 10,
        Status? status = null,
        string? sortCriteria = null)
    {
        var query = new List<string>
        {
            $"pageNumber={pageNumber}",
            $"tasksPerPage={tasksPerPage}",
        };

        if (status.HasValue)
        {
            query.Add($"status={status.Value}");
        }

        if (!string.IsNullOrEmpty(sortCriteria))
        {
            query.Add($"sortCriteria={Uri.EscapeDataString(sortCriteria)}");
        }

        var url = $"api/AssignedTasks/assigned-to-me?{string.Join("&", query)}";

        var response = await this.httpClient.GetAsync(url);

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<PaginatedListResult<TaskDetailsDto>>()
                ?? throw new InvalidOperationException("Failed to deserialize the response.");
        }

        throw new HttpRequestException($"Error getting assigned tasks: {response.StatusCode}");
    }
}
