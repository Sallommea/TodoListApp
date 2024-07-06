namespace TodoListApp.WebApp.Logging;

public static partial class AuthLoggingMessages
{
    [LoggerMessage(EventId = 1, Level = LogLevel.Error, Message = "An unexpected error occurred while signing in: {Message}")]
    public static partial void ErrorSigninIn(this ILogger logger, string message, Exception ex);

    [LoggerMessage(EventId = 2, Level = LogLevel.Error, Message = "An unexpected error occurred while registering an user: {Message}")]
    public static partial void ErrorWhileRegistering(this ILogger logger, string message, Exception ex);

    [LoggerMessage(EventId = 3, Level = LogLevel.Error, Message = "An unexpected error occurred while signing in: {Message}")]
    public static partial void ErrorWhileSigningInUser(this ILogger logger, string message, Exception ex);
}
