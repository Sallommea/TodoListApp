namespace TodoListApp.WebApp.Logging;

public static partial class AssignedTasksLoggerMessages
{
    [LoggerMessage(EventId = 1, Level = LogLevel.Error, Message = "Invalid Operation occurred while fetching assigned tasks: {Message}")]
    public static partial void IOErrorWhileFetchingAssignedTasks(this ILogger logger, string message, Exception ex);

    [LoggerMessage(EventId = 2, Level = LogLevel.Error, Message = "Error occurred while fetching assigned tasks: {Message}")]
    public static partial void ErrorWhileFetchingAssignedTasks(this ILogger logger, string message, Exception ex);

    [LoggerMessage(EventId = 3, Level = LogLevel.Error, Message = "Invalid Operation occured while changing status for task with Id: {Id}. {Message}")]
    public static partial void IOEWhileChangingStatus(this ILogger logger, int id, string message, Exception ex);

    [LoggerMessage(EventId = 4, Level = LogLevel.Error, Message = "Error occured while changing status for task with ID: {Id}. {Message}")]
    public static partial void ErrorWhileChangingStatus(this ILogger logger, int id, string message, Exception ex);
}
