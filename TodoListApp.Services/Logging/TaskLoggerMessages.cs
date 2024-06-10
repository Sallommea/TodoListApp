using Microsoft.Extensions.Logging;

namespace TodoListApp.Services.Logging;
public static partial class TaskLoggerMessages
{
    [LoggerMessage(EventId = 1, Level = LogLevel.Information, Message = "Task with ID {TaskId} created.")]
    public static partial void TaskCreated(ILogger logger, int taskId);

    [LoggerMessage(EventId = 2, Level = LogLevel.Error, Message = "An unexpected error occurred while creating a task.")]
    public static partial void UnexpectedErrorCreatingTask(ILogger logger, Exception ex);
}
