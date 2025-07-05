using System;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using TechnicalAssignment.Configuration;

namespace TechnicalAssignment.Utilities;

public static class BrowserHelper
{
    private static readonly ILogger Logger = LoggingHelper.CreateLogger("BrowserHelper");
    private static ConfigurationManager Config => ConfigurationManager.Instance;

    public static void SwitchToWindow(IWebDriver driver, int windowIndex)
    {
        Logger.LogDebug("Switching to window index: {Index}", windowIndex);
        var windowHandles = driver.WindowHandles;
        
        if (windowIndex >= 0 && windowIndex < windowHandles.Count)
        {
            driver.SwitchTo().Window(windowHandles[windowIndex]);
            Logger.LogDebug("Switched to window index: {Index}", windowIndex);
        }
        else
        {
            Logger.LogError("Invalid window index: {Index}. Available windows: {Count}", windowIndex, windowHandles.Count);
            throw new ArgumentOutOfRangeException(nameof(windowIndex), $"Window index {windowIndex} is out of range. Available windows: {windowHandles.Count}");
        }
    }

    public static void SwitchToMainWindow(IWebDriver driver)
    {
        Logger.LogDebug("Switching to main window");
        var windowHandles = driver.WindowHandles;
        if (windowHandles.Count > 0)
        {
            driver.SwitchTo().Window(windowHandles[0]);
            Logger.LogDebug("Switched to main window");
        }
    }

    public static void CloseCurrentWindow(IWebDriver driver)
    {
        Logger.LogDebug("Closing current window");
        driver.Close();
        SwitchToMainWindow(driver);
        Logger.LogDebug("Closed current window and switched to main window");
    }

    public static int GetWindowCount(IWebDriver driver)
    {
        Logger.LogDebug("Getting window count");
        var count = driver.WindowHandles.Count;
        Logger.LogDebug("Current window count: {Count}", count);
        return count;
    }

    public static void SwitchToLastWindow(IWebDriver driver)
    {
        Logger.LogDebug("Attempting to switch to the last window.");
        var windowCount = driver.WindowHandles.Count;
        if (windowCount > 0)
        {
            SwitchToWindow(driver, windowCount - 1);
        }
        else
        {
            Logger.LogError("Cannot switch to last window as no windows are open.");
            throw new NoSuchWindowException("No windows available to switch to.");
        }
    }

    public static object? ExecuteJavaScript(IWebDriver driver, string script, params object[] args)
    {
        Logger.LogDebug("Executing JavaScript: {Script}", script);
        var jsExecutor = (IJavaScriptExecutor)driver;
        var result = jsExecutor.ExecuteScript(script, args);
        Logger.LogDebug("JavaScript executed successfully");
        return result;
    }

    public static void WaitForJavaScriptToComplete(IWebDriver driver, TimeSpan? timeout = null)
    {
        Logger.LogDebug("Waiting for JavaScript to complete");
        var actualTimeout = timeout ?? Config.Timeouts.DefaultTimeout;
        
        WaitHelper.WaitForCondition(driver, driver =>
        {
            var jsExecutor = (IJavaScriptExecutor)driver;
            return jsExecutor.ExecuteScript("return jQuery.active == 0 && document.readyState == 'complete'");
        }, actualTimeout);
        
        Logger.LogDebug("JavaScript completed");
    }

    public static string GetPageTitle(IWebDriver driver)
    {
        Logger.LogDebug("Getting page title");
        var title = driver.Title;
        Logger.LogDebug("Page title: '{Title}'", title);
        return title;
    }

    public static string GetCurrentUrl(IWebDriver driver)
    {
        Logger.LogDebug("Getting current URL");
        var url = driver.Url;
        Logger.LogDebug("Current URL: '{Url}'", url);
        return url;
    }

    public static void NavigateTo(IWebDriver driver, string url)
    {
        Logger.LogInformation("Navigating to URL: {Url}", url);
        driver.Navigate().GoToUrl(url);
        Logger.LogDebug("Navigation completed to: {Url}", url);
    }

    public static void RefreshPage(IWebDriver driver)
    {
        Logger.LogDebug("Refreshing page");
        driver.Navigate().Refresh();
        Logger.LogDebug("Page refreshed");
    }

    public static void GoBack(IWebDriver driver)
    {
        Logger.LogDebug("Navigating back");
        driver.Navigate().Back();
        Logger.LogDebug("Navigated back");
    }

    public static void GoForward(IWebDriver driver)
    {
        Logger.LogDebug("Navigating forward");
        driver.Navigate().Forward();
        Logger.LogDebug("Navigated forward");
    }

    /// <summary>
    /// Sets the browser viewport size using configuration defaults if not specified
    /// </summary>
    public static void SetViewportSize(IWebDriver driver, int? width = null, int? height = null)
    {
        var actualWidth = width ?? Config.Browser.WindowSize.Width;
        var actualHeight = height ?? Config.Browser.WindowSize.Height;
        
        Logger.LogDebug("Setting viewport size to: {Width}x{Height}", actualWidth, actualHeight);
        driver.Manage().Window.Size = new System.Drawing.Size(actualWidth, actualHeight);
        Logger.LogDebug("Viewport size set to: {Width}x{Height}", actualWidth, actualHeight);
    }

    /// <summary>
    /// Maximizes the browser window
    /// </summary>
    public static void MaximizeWindow(IWebDriver driver)
    {
        Logger.LogDebug("Maximizing browser window");
        driver.Manage().Window.Maximize();
        Logger.LogDebug("Browser window maximized");
    }

    /// <summary>
    /// Gets the current viewport size
    /// </summary>
    public static (int Width, int Height) GetViewportSize(IWebDriver driver)
    {
        Logger.LogDebug("Getting viewport size");
        var size = driver.Manage().Window.Size;
        Logger.LogDebug("Current viewport size: {Width}x{Height}", size.Width, size.Height);
        return (size.Width, size.Height);
    }

    /// <summary>
    /// Resets the browser window to the configured default size
    /// </summary>
    public static void ResetToDefaultSize(IWebDriver driver)
    {
        Logger.LogDebug("Resetting browser window to default size from configuration");
        SetViewportSize(driver);
    }

    /// <summary>
    /// Configures browser timeouts using configuration settings
    /// Note: Only configures page load timeout - no implicit wait to avoid mixing wait types
    /// </summary>
    public static void ConfigureTimeouts(IWebDriver driver)
    {
        Logger.LogDebug("Configuring browser timeouts from configuration");
        
        var timeouts = driver.Manage().Timeouts();
        timeouts.PageLoad = Config.Timeouts.DefaultTimeout;
        
        Logger.LogDebug("Browser timeouts configured: PageLoad={PageLoad}s", 
            Config.Timeouts.DefaultTimeoutSeconds);
    }

    /// <summary>
    /// Checks if the browser is running in headless mode (best effort detection)
    /// </summary>
    public static bool IsHeadlessMode()
    {
        return Config.Browser.Headless;
    }
} 