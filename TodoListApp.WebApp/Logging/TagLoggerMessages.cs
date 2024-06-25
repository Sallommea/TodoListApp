namespace TodoListApp.WebApp.Logging;

public static partial class TagLoggerMessages
{
    [LoggerMessage(EventId = 1, Level = LogLevel.Error, Message = "HTTP An error occurred while adding tag to task. {Message}")]
    public static partial void HTTPErrorAddingTagToTask(this ILogger logger, string message, Exception ex);

    [LoggerMessage(EventId = 2, Level = LogLevel.Error, Message = "An unexpected error occurred while adding tag to task. {Message}")]
    public static partial void ErrorAddingTagToTask(this ILogger logger, string message, Exception ex);

    [LoggerMessage(EventId = 3, Level = LogLevel.Error, Message = "An Invalid Operation Exception error occurred while adding tag to task. {Message}")]
    public static partial void IOEAddingTagToTask(this ILogger logger, string message, Exception ex);

    [LoggerMessage(EventId = 4, Level = LogLevel.Error, Message = "HTTP An error occurred while deleting tag {TagId}. {Message}")]
    public static partial void HttpExceptionOccurredWhileDeletingTag(this ILogger logger, string message, int tagId, Exception ex);

    [LoggerMessage(EventId = 5, Level = LogLevel.Error, Message = "An unexpected error occurred while deleting tag {TagId}.. {Message}")]
    public static partial void ErrorDeletingTag(this ILogger logger, string message, int tagId, Exception ex);

    [LoggerMessage(EventId = 6, Level = LogLevel.Error, Message = "An Invalid Operation Exception error occurred while deleting tag {TagId}. {Message}")]
    public static partial void IOEDeletingTag(this ILogger logger, string message, int tagId, Exception ex);

    [LoggerMessage(EventId = 7, Level = LogLevel.Error, Message = "HTTP error occurred while while fetching the tags: {Message}")]
    public static partial void HTTPErrorWhileGettingTags(this ILogger logger, string message, Exception ex);

    [LoggerMessage(EventId = 8, Level = LogLevel.Error, Message = "An invalid operation occurred while fetching the tags: {Message}")]
    public static partial void IOEGettingTags(this ILogger logger, string message, Exception ex);

    [LoggerMessage(EventId = 9, Level = LogLevel.Error, Message = "An unexpected error occurred while fetching the tags: {Message}")]
    public static partial void ErrorGettingTags(this ILogger logger, string message, Exception ex);
}
