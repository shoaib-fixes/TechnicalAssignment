using System;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;

namespace TechnicalAssignment.Utilities;

public static class ScrollHelper
{
    private static readonly ILogger Logger = LoggingHelper.CreateLogger("ScrollHelper");

    public static void ScrollToTop(IWebDriver driver)
    {
        Logger.LogDebug("Scrolling to top of page");
        var jsExecutor = (IJavaScriptExecutor)driver;
        jsExecutor.ExecuteScript("window.scrollTo(0, 0);");
        Logger.LogDebug("Scrolled to top of page");
    }

    public static void ScrollToBottom(IWebDriver driver)
    {
        Logger.LogDebug("Scrolling to bottom of page");
        var jsExecutor = (IJavaScriptExecutor)driver;
        jsExecutor.ExecuteScript("window.scrollTo(0, document.body.scrollHeight);");
        Logger.LogDebug("Scrolled to bottom of page");
    }

    public static void ScrollToElement(IWebDriver driver, By locator, TimeSpan? timeout = null)
    {
        Logger.LogDebug("Scrolling to element: {Locator}", locator);
        
        try
        {
            var element = WaitHelper.WaitForElementPresent(driver, locator, timeout);
            var jsExecutor = (IJavaScriptExecutor)driver;
            jsExecutor.ExecuteScript("arguments[0].scrollIntoView({behavior: 'instant', block: 'center'});", element);
            Logger.LogDebug("Successfully scrolled to element: {Locator}", locator);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to scroll to element: {Locator}", locator);
            throw;
        }
    }

    public static void ScrollToElement(IWebDriver driver, By locator, ScrollBehavior behavior, ScrollAlignment alignment = ScrollAlignment.Center, TimeSpan? timeout = null)
    {
        Logger.LogDebug("Scrolling to element with behavior {Behavior} and alignment {Alignment}: {Locator}", behavior, alignment, locator);
        
        try
        {
            var element = WaitHelper.WaitForElementPresent(driver, locator, timeout);
            var jsExecutor = (IJavaScriptExecutor)driver;
            var behaviorStr = behavior == ScrollBehavior.Instant ? "auto" : behavior.ToString().ToLowerInvariant();
            var alignmentStr = alignment.ToString().ToLowerInvariant();
            jsExecutor.ExecuteScript($"arguments[0].scrollIntoView({{behavior: '{behaviorStr}', block: '{alignmentStr}'}});", element);
            Logger.LogDebug("Successfully scrolled to element with custom behavior: {Locator}", locator);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to scroll to element with custom behavior: {Locator}", locator);
            throw;
        }
    }

    public static void ScrollBy(IWebDriver driver, int horizontalPixels, int verticalPixels)
    {
        Logger.LogDebug("Scrolling by {Horizontal}px horizontally and {Vertical}px vertically", horizontalPixels, verticalPixels);
        var jsExecutor = (IJavaScriptExecutor)driver;
        jsExecutor.ExecuteScript($"window.scrollBy({horizontalPixels}, {verticalPixels});");
        Logger.LogDebug("Scrolled by specified pixels");
    }

    public static void ScrollToPosition(IWebDriver driver, int x, int y)
    {
        Logger.LogDebug("Scrolling to position ({X}, {Y})", x, y);
        var jsExecutor = (IJavaScriptExecutor)driver;
        jsExecutor.ExecuteScript($"window.scrollTo({x}, {y});");
        Logger.LogDebug("Scrolled to position ({X}, {Y})", x, y);
    }
}

public enum ScrollBehavior
{
    Auto,
    Smooth,
    Instant
}

public enum ScrollAlignment
{
    Start,
    Center,
    End,
    Nearest
} 