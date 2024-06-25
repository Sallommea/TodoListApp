using Microsoft.Extensions.Logging;

namespace TodoListApp.Services.WebApi.Logging;
public static partial class TagServiceLoggerMessages
{
    [LoggerMessage(EventId = 1, Level = LogLevel.Error, Message = "HTTP An error occurred while adding tag to task. {Message}")]
    public static partial void HTTPErrorAddingTagToTask(this ILogger logger, string message, Exception ex);

    [LoggerMessage(EventId = 2, Level = LogLevel.Error, Message = "An unexpected error occurred while adding tag to task. {Message}")]
    public static partial void ErrorAddingTagToTask(this ILogger logger, string message, Exception ex);

    [LoggerMessage(EventId = 3, Level = LogLevel.Error, Message = "HTTP An error occurred while getting tags. {Message}")]
    public static partial void HTTPErrorWhileGettingTags(this ILogger logger, string message, Exception ex);

    [LoggerMessage(EventId = 4, Level = LogLevel.Error, Message = "JSON parsing error occurred while getting tags. {Message}")]
    public static partial void JSONParsingErrorWhileGettingTags(this ILogger logger, string message, Exception ex);

    [LoggerMessage(EventId = 5, Level = LogLevel.Error, Message = "An unexpected error occurred while getting tags. {Message}")]
    public static partial void UnexpectedErrorWhileGettingTags(this ILogger logger, string message, Exception ex);
}
