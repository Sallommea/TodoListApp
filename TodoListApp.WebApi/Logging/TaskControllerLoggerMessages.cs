namespace TodoListApp.WebApi.Logging;

public static partial class TaskControllerLoggerMessages
{
    [LoggerMessage(EventId = 1, Level = LogLevel.Information, Message = "Task created successfully. TaskId: {TaskId}, TodoListId: {TodoListId}")]
    public static partial void TaskCreatedSuccessfully(ILogger logger, int taskId, int todoListId);

    [LoggerMessage(EventId = 2, Level = LogLevel.Error, Message = "Todo list not found. Unable to create task.")]
    public static partial void TodoListNotFound(ILogger logger, Exception ex);

    [LoggerMessage(EventId = 3, Level = LogLevel.Error, Message = "An unexpected error occurred while creating the task.")]
    public static partial void UnexpectedErrorCreatingTask(ILogger logger, Exception ex);
}
