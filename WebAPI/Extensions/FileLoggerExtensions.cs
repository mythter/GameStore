using WebAPI.Loggers;

namespace WebAPI.Extensions;

public static class FileLoggerExtensions
{
    public static ILoggingBuilder AddFile(this ILoggingBuilder builder, string logDirectory)
    {
        Directory.CreateDirectory(logDirectory);
        builder.AddProvider(new FileLoggerProvider(logDirectory));
        return builder;
    }
}
