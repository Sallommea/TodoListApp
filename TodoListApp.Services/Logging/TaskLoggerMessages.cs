using Microsoft.Extensions.Logging;

namespace TodoListApp.Services.Logging;
public static partial class TaskLoggerMessages
{
    [LoggerMessage(EventId = 1, Level = LogLevel.Information, Message = "Task with ID {TaskId} created.")]
    public static partial void TaskCreated(ILogger logger, int taskId);

    [LoggerMessage(EventId = 2, Level = LogLevel.Error, Message = "An unexpected error occurred while creating a task.")]
    public static partial void UnexpectedErrorCreatingTask(ILogger logger, Exception ex);

    [LoggerMessage(EventId = 3, Level = LogLevel.Error, Message = "An error occurred while retrieving the task details. {Message}")]
    public static partial void UnexpectedErrorOccurredWhileGettingTaskDetails(this ILogger logger, string message, Exception ex);

    [LoggerMessage(EventId = 4, Level = LogLevel.Information, Message = "Task with ID {TaskId} not found to get task details")]
    public static partial void TaskIdNotFoundToGetTaskDetails(this ILogger logger, int taskId);

    [LoggerMessage(EventId = 5, Level = LogLevel.Error, Message = "Todo List not found: {Message}")]
    public static partial void ToDoListNotFound(this ILogger logger, string message);

    [LoggerMessage(EventId = 6, Level = LogLevel.Information, Message = "Task with ID {TaskId} not found to delete task.")]
    public static partial void TaskIdNotFoundToDelete(this ILogger logger, int taskId);

    [LoggerMessage(EventId = 7, Level = LogLevel.Information, Message = "Task with ID {TaskId} deleted successfully.")]
    public static partial void TaskDeletedSuccessfully(this ILogger logger, int taskId);

    [LoggerMessage(EventId = 8, Level = LogLevel.Error, Message = "An unexpected error occurred while deleting task with ID {TaskId}. Error: {Message}")]
    public static partial void UnexpectedErrorOccurredWhileDeletingTask(this ILogger logger, string message, int taskId, Exception ex);

    [LoggerMessage(EventId = 9, Level = LogLevel.Warning, Message = "Task ID not found for update: {TaskId}")]
    public static partial void TaskIdNotFoundForUpdate(this ILogger logger, int taskId);

    [LoggerMessage(EventId = 10, Level = LogLevel.Error, Message = "An unexpected error occurred while updating task with ID {TaskId}. Error: {Message}")]
    public static partial void UnexpectedErrorOccurredWhileUpdatingTask(this ILogger logger, string message, int taskId, Exception ex);
}
