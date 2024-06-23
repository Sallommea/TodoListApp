namespace TodoListApp.WebApp.Logging;

public static partial class TaskLoggerMessages
{
    [LoggerMessage(EventId = 1, Level = LogLevel.Error, Message = "Invalid Operation occurred while adding task: {Message}")]
    public static partial void IOErrorWhileAddingTask(this ILogger logger, string message, Exception ex);

    [LoggerMessage(EventId = 2, Level = LogLevel.Error, Message = "Error adding task: {Message}")]
    public static partial void ErrorAddingTask(this ILogger logger, string message, Exception ex);

    [LoggerMessage(EventId = 3, Level = LogLevel.Error, Message = "Invalid Operation Exception while deleting task with ID: {Id}. {Message}")]
    public static partial void IOEWhileDeleting(this ILogger logger, int id, string message, Exception ex);

    [LoggerMessage(EventId = 4, Level = LogLevel.Error, Message = "Error deleting task with ID: {Id}. {Message}")]
    public static partial void ErrorDeletingTask(this ILogger logger, int id, string message, Exception ex);

    [LoggerMessage(EventId = 5, Level = LogLevel.Error, Message = "Invalid Operation error occurred while getting task details for ID : {Id}. {Message}")]
    public static partial void IOErrorGettingTask(this ILogger logger, int id, string message, Exception ex);

    [LoggerMessage(EventId = 6, Level = LogLevel.Error, Message = "Error getting task details for ID: {Id}. {Message}")]
    public static partial void ErrorGettingTask(this ILogger logger, int id, string message, Exception ex);

    [LoggerMessage(EventId = 7, Level = LogLevel.Error, Message = "Invalid Operation occurred while fetching task with ID for an update: {Id}. {Message}")]
    public static partial void IOErrorFetchingTaskUpdate(this ILogger logger, int id, string message, Exception ex);

    [LoggerMessage(EventId = 8, Level = LogLevel.Error, Message = "Error fetching task with ID for an update: {Id}. {Message}")]
    public static partial void ErrorFetchingTaskForUpdate(this ILogger logger, int id, string message, Exception ex);

    [LoggerMessage(EventId = 9, Level = LogLevel.Error, Message = "Invalid Operation occurred while updating task with ID: {Id}. {Message}")]
    public static partial void IOErrorUpdatingTask(this ILogger logger, int id, string message, Exception ex);

    [LoggerMessage(EventId = 10, Level = LogLevel.Error, Message = "Error while updating task with ID: {Id}. {Message}")]
    public static partial void ErrorUpdatingTask(this ILogger logger, int id, string message, Exception ex);
}
