using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using TodoListApp.Services.Models;
using TodoListApp.Services.WebApi.Logging;
using TodoListApp.WebApi.Models.Tasks;

namespace TodoListApp.Services.WebApi.Services;
public class AssignedTaskWebApiService
{
    private readonly HttpClient httpClient;
    private readonly ILogger<AssignedTaskWebApiService> logger;

    public AssignedTaskWebApiService(HttpClient httpClient, ILogger<AssignedTaskWebApiService> logger)
    {
        this.httpClient = httpClient;
        this.logger = logger;
    }

    public async Task<PaginatedListResult<AssignedTasksdto>> GetTasksAssignedToMeAsync(
        string token,
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

        try
        {
            if (!string.IsNullOrEmpty(token))
            {
                this.httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }

            var response = await this.httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<PaginatedListResult<AssignedTasksdto>>()
                    ?? throw new InvalidOperationException("Failed to deserialize the response.");
            }

            throw new HttpRequestException($"Error getting assigned tasks: {response.StatusCode}");
        }
        catch (HttpRequestException ex)
        {
            AssignedTasksServiceLoggerMessages.HTTPErrorGettingTasksAssigned(this.logger, url, ex.Message, ex);
            throw new InvalidOperationException("An error occurred while communicating with the server.", ex);
        }
        catch (Exception ex)
        {
            AssignedTasksServiceLoggerMessages.ErrorGettingTasksAssigned(this.logger, url, ex.Message, ex);
            throw;
        }
    }

    public async Task UpdateTaskStatusAsync(UpdateTaskStatus updateTaskStatusDto, string token)
    {
        try
        {
            if (!string.IsNullOrEmpty(token))
            {
                this.httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }

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
            AssignedTasksServiceLoggerMessages.HTTPErrorUpdatingTaskStatus(this.logger, updateTaskStatusDto.TaskId, ex.Message, ex);
            throw new InvalidOperationException("An error occurred while communicating with the server.", ex);
        }
        catch (Exception ex)
        {
            AssignedTasksServiceLoggerMessages.UnexpectedErrorUpdatingTaskStatus(this.logger, updateTaskStatusDto.TaskId, ex.Message, ex);
            throw;
        }
    }
}
