using Microsoft.Extensions.Logging;
using TodoListApp.Services.Database;

namespace TodoListApp.Services.Logging;
public static partial class AssignedTasksLoggerMessages
{
    [LoggerMessage(EventId = 1, Level = LogLevel.Error, Message = "An error occurred while retrieving tasks assigned to the user: {User}")]
    public static partial void UnexpectedErrorOccurredWhileGettingUserTasks(this ILogger logger, string user, Exception ex);

    [LoggerMessage(EventId = 2, Level = LogLevel.Warning, Message = "Task with ID {TaskId} not found.")]
    public static partial void InvalidTaskIdForTaskStatusUpdate(this ILogger logger, int taskId);

    [LoggerMessage(EventId = 3, Level = LogLevel.Information, Message = "Task status updated successfully. TaskId: {TaskId}.")]
    public static partial void TaskStatusUpdatedSuccessfully(ILogger logger, int taskId);

    [LoggerMessage(EventId = 4, Level = LogLevel.Error, Message = "An error occurred while updating task status: {Message}")]
    public static partial void UnexpectedErrorOccurredWhileUpdatingTaskStatus(this ILogger logger, string message, Exception ex);
}
