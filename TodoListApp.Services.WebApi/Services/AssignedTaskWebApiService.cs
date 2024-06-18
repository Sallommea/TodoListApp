using System.Net;
using System.Net.Http.Json;
using TodoListApp.Services.Models;
using TodoListApp.WebApi.Models.Tasks;

namespace TodoListApp.Services.WebApi.Services;
public class AssignedTaskWebApiService
{
    private readonly HttpClient httpClient;

    public AssignedTaskWebApiService(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task<PaginatedListResult<AssignedTasksdto>> GetTasksAssignedToMeAsync(
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
            return await response.Content.ReadFromJsonAsync<PaginatedListResult<AssignedTasksdto>>()
                ?? throw new InvalidOperationException("Failed to deserialize the response.");
        }

        throw new HttpRequestException($"Error getting assigned tasks: {response.StatusCode}");
    }

    public async Task UpdateTaskStatusAsync(UpdateTaskStatus updateTaskStatusDto)
    {
        try
        {
            var response = await this.httpClient.PutAsJsonAsync("api/AssignedTasks/update-status", updateTaskStatusDto);

            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    throw new InvalidOperationException($"Task with ID {updateTaskStatusDto.TaskId} not found.");
                }

                _ = response.EnsureSuccessStatusCode();
            }
        }
        catch (HttpRequestException ex)
        {
            throw new InvalidOperationException("An error occurred while communicating with the server.", ex);
        }
    }
}
