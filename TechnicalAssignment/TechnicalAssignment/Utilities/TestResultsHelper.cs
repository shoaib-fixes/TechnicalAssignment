using System;
using System.IO;
using NUnit.Framework;

namespace TechnicalAssignment.Utilities;

public static class TestResultsHelper
{
    private static readonly string RunTimestamp = DateTime.Now.ToString("yyyy-MM-dd_HHmmss");
    
    public static string GetTestResultsDirectory()
    {
        var browser = TestContext.Parameters.Get("Browser", "Chrome");
        
        var workDir = TestContext.CurrentContext.WorkDirectory;
        var solutionDir = Directory.GetParent(workDir)?.Parent?.Parent?.Parent?.FullName;
        if (string.IsNullOrEmpty(solutionDir))
        {
            throw new DirectoryNotFoundException("Could not determine solution directory");
        }

        var testResultsDir = Path.Combine(solutionDir, "TestResults");
        var browserDir = Path.Combine(testResultsDir, browser);
        var dateTimeDir = Path.Combine(browserDir, RunTimestamp);
        
        Directory.CreateDirectory(dateTimeDir);
        
        return dateTimeDir;
    }

    public static string GetLogFilePath()
    {
        var directory = GetTestResultsDirectory();
        var logFileName = $"TestLog_{DateTime.Now:yyyyMMdd}.log";
        return Path.Combine(directory, logFileName);
    }

    public static string GetScreenshotPath(string testName)
    {
        var directory = GetTestResultsDirectory();
        var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        var sanitizedTestName = SanitizeFileName(testName);
        return Path.Combine(directory, $"{sanitizedTestName}_{timestamp}.png");
    }

    private static string SanitizeFileName(string fileName)
    {
        var invalidChars = Path.GetInvalidFileNameChars();
        var sanitized = fileName;
        
        foreach (var invalidChar in invalidChars)
        {
            sanitized = sanitized.Replace(invalidChar, '_');
        }
        
        sanitized = sanitized.Replace("(", "_")
                             .Replace(")", "_")
                             .Replace("\"", "_")
                             .Replace("'", "_")
                             .Replace(" ", "_");
        
        return sanitized;
    }
} 