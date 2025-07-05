using System;
using System.IO;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using OpenQA.Selenium;

namespace TechnicalAssignment.Utilities;

public static class ScreenshotHelper
{
    private static readonly ILogger Logger = LoggingHelper.CreateLogger("ScreenshotHelper");

    public static void CaptureScreenshot(IWebDriver driver, string testName)
    {
        try
        {
            Logger.LogInformation("Starting screenshot capture for test: {TestName}", testName);
            
            // Validate driver state
            if (driver == null)
            {
                throw new ArgumentNullException(nameof(driver), "WebDriver is null");
            }
            
            if (!(driver is ITakesScreenshot))
            {
                throw new InvalidOperationException("WebDriver does not support taking screenshots");
            }
            
            Logger.LogDebug("WebDriver validation passed, capturing screenshot");
            var screenshot = ((ITakesScreenshot)driver).GetScreenshot();
            Logger.LogDebug("Screenshot captured from driver");

            var screenshotPath = TestResultsHelper.GetScreenshotPath(testName);
            Logger.LogDebug("Screenshot will be saved to: {ScreenshotPath}", screenshotPath);
            
            screenshot.SaveAsFile(screenshotPath);
            
            Logger.LogInformation("Screenshot saved successfully to: {ScreenshotPath}", screenshotPath);
            TestContext.AddTestAttachment(screenshotPath, "Screenshot of test execution");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to capture screenshot for test: {TestName}", testName);
        }
    }

    public static void CaptureScreenshotOnFailure(IWebDriver driver, string testName)
    {
        try
        {
            if (TestContext.CurrentContext.Result.Outcome.Status == TestStatus.Failed)
            {
                Logger.LogWarning("Test failed, capturing screenshot for: {TestName}", testName);
                CaptureScreenshot(driver, $"{testName}_FAILURE");
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to capture failure screenshot for test: {TestName}", testName);
        }
    }

    /// <summary>
    /// Takes a screenshot of the current page with optional custom name
    /// </summary>
    public static void TakePageScreenshot(IWebDriver driver, string screenshotName = "")
    {
        try
        {
            var name = string.IsNullOrEmpty(screenshotName) ? $"PageScreenshot_{DateTime.Now:yyyyMMdd_HHmmss}" : screenshotName;
            Logger.LogDebug("Taking page screenshot: {Name}", name);
            CaptureScreenshot(driver, name);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to take page screenshot: {ScreenshotName}", screenshotName);
        }
    }

    /// <summary>
    /// Captures a screenshot of a specific element using Selenium 4's element screenshot feature
    /// </summary>
    public static void CaptureElementScreenshot(IWebDriver driver, IWebElement element, string screenshotName)
    {
        try
        {
            Logger.LogInformation("Starting element screenshot capture: {ScreenshotName}", screenshotName);
            
            // Validate inputs
            if (driver == null)
            {
                throw new ArgumentNullException(nameof(driver), "WebDriver is null");
            }
            
            if (element == null)
            {
                throw new ArgumentNullException(nameof(element), "Element is null");
            }
            
            Logger.LogDebug("Element screenshot validation passed, capturing screenshot");
            
            // Use Selenium 4's element screenshot feature
            var screenshot = ((ITakesScreenshot)element).GetScreenshot();
            Logger.LogDebug("Element screenshot captured");

            var screenshotPath = TestResultsHelper.GetScreenshotPath(screenshotName);
            Logger.LogDebug("Element screenshot will be saved to: {ScreenshotPath}", screenshotPath);
            
            screenshot.SaveAsFile(screenshotPath);
            
            Logger.LogInformation("Element screenshot saved successfully to: {ScreenshotPath}", screenshotPath);
            TestContext.AddTestAttachment(screenshotPath, $"Element screenshot: {screenshotName}");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to capture element screenshot: {ScreenshotName}", screenshotName);
            
            try
            {
                Logger.LogWarning("Falling back to full page screenshot for: {ScreenshotName}", screenshotName);
                CaptureScreenshot(driver, $"{screenshotName}_FALLBACK");
            }
            catch (Exception fallbackEx)
            {
                Logger.LogError(fallbackEx, "Fallback screenshot also failed for: {ScreenshotName}", screenshotName);
            }
        }
    }
} 