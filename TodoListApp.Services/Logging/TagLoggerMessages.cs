using Microsoft.Extensions.Logging;

namespace TodoListApp.Services.Logging;
public static partial class TagLoggerMessages
{
    [LoggerMessage(EventId = 2, Level = LogLevel.Information, Message = "Successfully retrieved all tags.")]
    public static partial void TagsRetrieved(this ILogger logger);

    [LoggerMessage(EventId = 3, Level = LogLevel.Information, Message = "No Tags were found")]
    public static partial void TagNotFoundWhileGettingAllTags(this ILogger logger);

    [LoggerMessage(EventId = 6, Level = LogLevel.Warning, Message = "AddTagToTask failed: Tag name is empty.")]
    public static partial void AddTagToTaskLogWarningTagEmpty(this ILogger logger);

    [LoggerMessage(EventId = 7, Level = LogLevel.Warning, Message = "AddTagToTask failed: Tag name '{TagName}'")]
    public static partial void AddTagToTaskLogWarningFailed(this ILogger logger, string tagName);

    [LoggerMessage(EventId = 8, Level = LogLevel.Information, Message = "Tag '{TagName}' successfully added to task {TaskId}.")]
    public static partial void TagAddedToTask(this ILogger logger, string tagName, int taskId);

    [LoggerMessage(EventId = 9, Level = LogLevel.Error, Message = "An error occurred while checking if tag name '{TagName}. {Message}")]
    public static partial void ErrorOccurredWhileCheckingTagName(this ILogger logger, string tagName, string message, Exception ex);

    [LoggerMessage(EventId = 10, Level = LogLevel.Error, Message = "An unexpected error occurred in AddTagToTask: {Message}")]
    public static partial void UnexpectedErrorOccurredWhileAddingTag(this ILogger logger, string message, Exception ex);

    [LoggerMessage(EventId = 11, Level = LogLevel.Error, Message = "An unexpected error occurred while deleting tag: {Message}")]
    public static partial void UnexpectedErrorOccurredWhileDeletingTag(this ILogger logger, string message, Exception ex);
}
