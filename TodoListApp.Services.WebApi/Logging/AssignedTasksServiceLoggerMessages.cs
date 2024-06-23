using Microsoft.Extensions.Logging;

namespace TodoListApp.Services.WebApi.Logging;
public static partial class AssignedTasksServiceLoggerMessages
{
    [LoggerMessage(EventId = 1, Level = LogLevel.Error, Message = "HTTP An error occurred while getting tasks assigned to me from URL:{Url}. {Message}")]
    public static partial void HTTPErrorGettingTasksAssigned(this ILogger logger, string url, string message, Exception ex);

    [LoggerMessage(EventId = 2, Level = LogLevel.Error, Message = "An unexpected error occurred while getting tasks assigned to me from URL: {Url}. {Message}")]
    public static partial void ErrorGettingTasksAssigned(this ILogger logger, string url, string message, Exception ex);

    [LoggerMessage(EventId = 3, Level = LogLevel.Error, Message = "HTTP error occurred while updating task status for task ID : {Id}. {Message}")]
    public static partial void HTTPErrorUpdatingTaskStatus(this ILogger logger, int id, string message, Exception ex);

    [LoggerMessage(EventId = 4, Level = LogLevel.Error, Message = "An unexpected error occurred while updating task status for Task ID: {Id}. {Message}")]
    public static partial void UnexpectedErrorUpdatingTaskStatus(this ILogger logger, int id, string message, Exception ex);
}
