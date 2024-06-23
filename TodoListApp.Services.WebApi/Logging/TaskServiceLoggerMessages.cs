using Microsoft.Extensions.Logging;

namespace TodoListApp.Services.WebApi.Logging;
public static partial class TaskServiceLoggerMessages
{
    [LoggerMessage(EventId = 1, Level = LogLevel.Error, Message = "HTTP error occurred while adding new task: {Message}")]
    public static partial void HTTPErrorWhileAddingTask(this ILogger logger, string message, Exception ex);

    [LoggerMessage(EventId = 2, Level = LogLevel.Error, Message = "Error adding new task: {Message}")]
    public static partial void ErrorAddingTask(this ILogger logger, string message, Exception ex);

    [LoggerMessage(EventId = 3, Level = LogLevel.Error, Message = "HTTP error occurred while deleting task with ID: {Id}. {Message}")]
    public static partial void HTTPErrorWhileDeletingTask(this ILogger logger, int id, string message, Exception ex);

    [LoggerMessage(EventId = 4, Level = LogLevel.Error, Message = "Error deleting task with ID: {Id}. {Message}")]
    public static partial void ErrorDeletingTask(this ILogger logger, int id, string message, Exception ex);

    [LoggerMessage(EventId = 5, Level = LogLevel.Error, Message = "HTTP error occurred while getting task details for ID : {Id}. {Message}")]
    public static partial void HTTPErrorGettingTask(this ILogger logger, int id, string message, Exception ex);

    [LoggerMessage(EventId = 6, Level = LogLevel.Error, Message = "Error getting task details for ID: {Id}. {Message}")]
    public static partial void ErrorGettingTask(this ILogger logger, int id, string message, Exception ex);

    [LoggerMessage(EventId = 7, Level = LogLevel.Error, Message = "HTTP error occurred while updating task with ID: {Id}. {Message}")]
    public static partial void HTTPErrorUpdatingTask(this ILogger logger, int id, string message, Exception ex);

    [LoggerMessage(EventId = 8, Level = LogLevel.Error, Message = "Error updating task with ID: {Id}. {Message}")]
    public static partial void ErrorUpdatingTask(this ILogger logger, int id, string message, Exception ex);
}
