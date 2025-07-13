using Microsoft.Extensions.Logging;
using System;
using System.IO;
using NUnit.Framework;

namespace TechnicalAssignment.Utilities;

public static class LoggingHelper
{
    private static ILoggerFactory? _loggerFactory;
    
    public static ILoggerFactory LoggerFactory
    {
        get
        {
            if (_loggerFactory == null)
            {
                InitializeLogging();
            }
            return _loggerFactory!;
        }
    }

    private static void InitializeLogging()
    {
        var logLevel = GetLogLevelFromEnvironment();
        
        _loggerFactory = Microsoft.Extensions.Logging.LoggerFactory.Create(builder =>
        {
            builder
                .AddSimpleConsole(options =>
                {
                    options.IncludeScopes = true;
                    options.TimestampFormat = "[yyyy-MM-dd HH:mm:ss] ";
                })
                .AddProvider(new TestContextLoggerProvider())
                .AddProvider(new FileLoggerProvider())
                .AddProvider(new ExtentReportHelper.ExtentLoggerProvider())
                .SetMinimumLevel(logLevel);
        });
    }

    private static LogLevel GetLogLevelFromEnvironment()
    {
        var logLevelString = Environment.GetEnvironmentVariable("LOG_LEVEL") ?? "Information";
        
        if (Enum.TryParse<LogLevel>(logLevelString, true, out var logLevel))
        {
            return logLevel;
        }
        
        return LogLevel.Information;
    }

    public static ILogger<T> CreateLogger<T>()
    {
        return LoggerFactory.CreateLogger<T>();
    }

    public static ILogger CreateLogger(string categoryName)
    {
        return LoggerFactory.CreateLogger(categoryName);
    }

    public static void Dispose()
    {
        _loggerFactory?.Dispose();
        _loggerFactory = null;
    }

    private sealed class TestContextLoggerProvider : Microsoft.Extensions.Logging.ILoggerProvider
    {
        Microsoft.Extensions.Logging.ILogger Microsoft.Extensions.Logging.ILoggerProvider.CreateLogger(string categoryName) => new TestContextLogger(categoryName);
        void IDisposable.Dispose() 
        { 
            // Nothing to dispose
        }
    }

    private sealed class TestContextLogger : Microsoft.Extensions.Logging.ILogger
    {
        private readonly string _category;
        public TestContextLogger(string category) => _category = category;

        IDisposable Microsoft.Extensions.Logging.ILogger.BeginScope<TState>(TState state)
            => NullScope.Instance;

        public bool IsEnabled(LogLevel logLevel) => true;

        void Microsoft.Extensions.Logging.ILogger.Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            var message = formatter(state, exception);
            var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            NUnit.Framework.TestContext.Progress.WriteLine($"[{timestamp}] [{logLevel}] {_category}: {message}");
        }
    }

    private sealed class FileLoggerProvider : Microsoft.Extensions.Logging.ILoggerProvider
    {
        Microsoft.Extensions.Logging.ILogger Microsoft.Extensions.Logging.ILoggerProvider.CreateLogger(string categoryName) => new FileLogger(categoryName);
        void IDisposable.Dispose() 
        { 
            // Nothing to dispose
        }
    }

    private sealed class FileLogger : Microsoft.Extensions.Logging.ILogger
    {
        private readonly string _category;
        private static readonly object _fileLock = new object();
        
        public FileLogger(string category) => _category = category;

        IDisposable Microsoft.Extensions.Logging.ILogger.BeginScope<TState>(TState state)
            => NullScope.Instance;

        public bool IsEnabled(LogLevel logLevel) => true;

        void Microsoft.Extensions.Logging.ILogger.Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            try
            {
                var message = formatter(state, exception);
                var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                var logEntry = $"[{timestamp}] [{logLevel}] {_category}: {message}";
                
                if (exception != null)
                {
                    logEntry += Environment.NewLine + $"Exception: {exception}";
                }
                
                WriteToFile(logEntry);
            }
            catch
            {
                // Ignore file logging errors
            }
        }

        private static void WriteToFile(string logEntry)
        {
            try
            {
                var logFilePath = TestResultsHelper.GetLogFilePath();
                
                lock (_fileLock)
                {
                    File.AppendAllText(logFilePath, logEntry + Environment.NewLine);
                }
            }
            catch
            {
                // Ignore file writing errors
            }
        }
    }

    private sealed class NullScope : IDisposable
    {
        public static NullScope Instance { get; } = new NullScope();
        void IDisposable.Dispose() 
        { 
            // Nothing to dispose
        }
    }
} 