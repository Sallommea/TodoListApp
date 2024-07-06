namespace TodoListApp.WebApi.Logging;

public static partial class AuthLoggerMessages
{
    [LoggerMessage(EventId = 1, Level = LogLevel.Error, Message = "Invalid Operation occurred while registering an user. {Message}")]
    public static partial void InvalidOperationWhileRegistering(this ILogger logger, string message, Exception ex);

    [LoggerMessage(EventId = 2, Level = LogLevel.Error, Message = "An unexpected error occurred while registering an user: {Message}")]
    public static partial void UnexpectedErrorOccurredWhileRegistering(this ILogger logger, string message, Exception ex);

    [LoggerMessage(EventId = 3, Level = LogLevel.Error, Message = "Invalid Operation occurred while signing in: {Message}")]
    public static partial void InvalidOperationWhileSigningIn(this ILogger logger, string message, Exception ex);

    [LoggerMessage(EventId = 4, Level = LogLevel.Error, Message = "An unexpected error occurred while signing in: {Message}")]
    public static partial void UnexpectedErrorOccurredWhileSigningIn(this ILogger logger, string message, Exception ex);
}
