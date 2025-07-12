using System.ComponentModel.DataAnnotations;
using TechnicalAssignment.Models;

namespace TechnicalAssignment.Configuration;

/// <summary>
/// Main configuration class for test settings
/// </summary>
public class TestConfiguration
{
    public const string SectionName = "TestConfiguration";

    [Required]
    public string BaseUrl { get; set; } = string.Empty;

    [Required]
    public AdminCredentials AdminCredentials { get; set; } = new();

    public TimeoutSettings Timeouts { get; set; } = new();

    public BrowserSettings Browser { get; set; } = new();

    public AccessibilitySettings Accessibility { get; set; } = new();
}

/// <summary>
/// Admin credentials configuration
/// </summary>
public class AdminCredentials
{
    [Required]
    public string Username { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}

/// <summary>
/// Timeout settings configuration
/// </summary>
public class TimeoutSettings
{
    public int DefaultTimeoutSeconds { get; set; } = 10;
    public int PollingIntervalMilliseconds { get; set; } = 500;

    public TimeSpan DefaultTimeout => TimeSpan.FromSeconds(DefaultTimeoutSeconds);
    public TimeSpan PollingInterval => TimeSpan.FromMilliseconds(PollingIntervalMilliseconds);
}

/// <summary>
/// Browser settings configuration
/// </summary>
public class BrowserSettings
{
    private string _defaultBrowser = "Chrome";
    
    public string DefaultBrowser 
    { 
        get => _defaultBrowser;
        set => _defaultBrowser = value;
    }
    
    public BrowserType DefaultBrowserType
    {
        get => BrowserTypeExtensions.TryParseBrowserType(DefaultBrowser, out var browserType) 
            ? browserType 
            : BrowserType.Chrome;
    }
    
    public bool Headless { get; set; } = false;
    public WindowSize WindowSize { get; set; } = new();
    public int RemoteDebuggingPort { get; set; } = 9222;
}

/// <summary>
/// Window size configuration
/// </summary>
public class WindowSize
{
    public int Width { get; set; } = 1920;
    public int Height { get; set; } = 1080;
}

/// <summary>
/// Accessibility testing configuration
/// </summary>
public class AccessibilitySettings
{
    public bool EnableAccessibilityTesting { get; set; } = true;
    public string[] AccessibilityTags { get; set; } = { "wcag2aa" };
    public bool CaptureScreenshotsOnViolations { get; set; } = true;
    public bool HighlightViolatingElements { get; set; } = true;
    public bool FailOnViolations { get; set; } = true;
} 