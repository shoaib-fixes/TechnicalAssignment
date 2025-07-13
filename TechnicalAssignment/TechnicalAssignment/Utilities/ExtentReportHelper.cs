using AventStack.ExtentReports;
using AventStack.ExtentReports.Reporter;
using System.Threading;
using TechnicalAssignment.Utilities;
using Microsoft.Extensions.Logging;
using System;
using NUnit.Framework;
using System.IO;
using AventStack.ExtentReports.Reporter.Config;

namespace TechnicalAssignment.Utilities;

public static class ExtentReportHelper
{
    private static readonly Lazy<ExtentReports> _lazyReport = new(InitializeReport);
    private static readonly AsyncLocal<ExtentTest?> _lazyTest = new();
    private static readonly string _reportPath = Path.Combine(TestResultsHelper.GetTestResultsDirectory(), "ExtentReport.html");

    public static ExtentReports Instance => _lazyReport.Value;
    public static ExtentTest? CurrentTest
    {
        get => _lazyTest.Value;
        set => _lazyTest.Value = value;
    }

    private static ExtentReports InitializeReport()
    {
        var reporter = new ExtentSparkReporter(_reportPath);
        reporter.Config.DocumentTitle = "Test Automation Report";
        reporter.Config.ReportName = "Test Results";
        reporter.Config.Theme = Theme.Dark;

        var extent = new ExtentReports();
        extent.AttachReporter(reporter);

        extent.AddSystemInfo("OS", Environment.OSVersion.ToString());
        extent.AddSystemInfo("HostName", Environment.MachineName);
        extent.AddSystemInfo("Browser", TestContext.Parameters.Get("Browser", "Chrome"));

        return extent;
    }

    public static ExtentTest CreateTest(string testName, string? description = null)
    {
        CurrentTest = Instance.CreateTest(testName, description);
        return CurrentTest;
    }

    public static void LogToReport(LogLevel logLevel, string message, Exception? ex = null)
    {
        var status = ConvertLogLevel(logLevel);
        if (ex != null)
        {
            CurrentTest?.Log(status, $"<pre>{message}<br>{ex}</pre>");
        }
        else
        {
            CurrentTest?.Log(status, message);
        }
    }

    public static void AddScreenshotToReport(string screenshotPath, string title = "Screenshot")
    {
        if (CurrentTest == null || string.IsNullOrEmpty(screenshotPath)) return;

        try
        {
            var relativePath = Path.GetFileName(screenshotPath);
            CurrentTest.AddScreenCaptureFromPath(relativePath, title);
        }
        catch (Exception ex)
        {
            LogToReport(LogLevel.Error, "Failed to attach screenshot to report.", ex);
        }
    }

    private static Status ConvertLogLevel(LogLevel logLevel) => logLevel switch
    {
        LogLevel.Trace => Status.Info,
        LogLevel.Debug => Status.Info,
        LogLevel.Information => Status.Info,
        LogLevel.Warning => Status.Warning,
        LogLevel.Error => Status.Fail,
        LogLevel.Critical => Status.Fail,
        LogLevel.None => Status.Pass,
        _ => Status.Info
    };

    public sealed class ExtentLoggerProvider : ILoggerProvider
    {
        public ILogger CreateLogger(string categoryName) => new ExtentLogger();
        public void Dispose() { /* Nothing to dispose */ }
    }

    private sealed class ExtentLogger : ILogger
    {
        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => NullScope.Instance;
        public bool IsEnabled(LogLevel logLevel) => CurrentTest != null;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            if (!IsEnabled(logLevel)) return;

            var message = formatter(state, exception);
            LogToReport(logLevel, message, exception);
        }
    }

    private sealed class NullScope : IDisposable
    {
        public static NullScope Instance { get; } = new NullScope();
        public void Dispose() { /* Nothing to dispose */ }
    }
} 