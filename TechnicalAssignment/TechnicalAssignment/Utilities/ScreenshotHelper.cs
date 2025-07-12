using System;
using System.IO;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using OpenQA.Selenium;
using Selenium.Axe;
using TechnicalAssignment.Configuration;
using System.Linq;

namespace TechnicalAssignment.Utilities;

public static class ScreenshotHelper
{
    public static void CaptureScreenshot(IWebDriver driver, string testName, ILogger logger)
    {
        try
        {
            logger.LogInformation("Starting screenshot capture for test: {TestName}", testName);
            
            // Validate driver state
            if (driver == null)
            {
                throw new ArgumentNullException(nameof(driver), "WebDriver is null");
            }
            
            if (!(driver is ITakesScreenshot))
            {
                throw new InvalidOperationException("WebDriver does not support taking screenshots");
            }
            
            logger.LogDebug("WebDriver validation passed, capturing screenshot");
            var screenshot = ((ITakesScreenshot)driver).GetScreenshot();
            logger.LogDebug("Screenshot captured from driver");

            var screenshotPath = TestResultsHelper.GetScreenshotPath(testName);
            logger.LogDebug("Screenshot will be saved to: {ScreenshotPath}", screenshotPath);
            
            screenshot.SaveAsFile(screenshotPath);
            
            logger.LogInformation("Screenshot saved successfully to: {ScreenshotPath}", screenshotPath);
            TestContext.AddTestAttachment(screenshotPath, "Screenshot of test execution");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to capture screenshot for test: {TestName}", testName);
        }
    }

    public static void CaptureScreenshotOnFailure(IWebDriver driver, string testName, ILogger logger)
    {
        try
        {
            if (TestContext.CurrentContext.Result.Outcome.Status == TestStatus.Failed)
            {
                logger.LogWarning("Test failed, capturing screenshot for: {TestName}", testName);
                CaptureScreenshot(driver, $"{testName}_FAILURE", logger);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to capture failure screenshot for test: {TestName}", testName);
        }
    }

    /// <summary>
    /// Takes a screenshot of the current page with optional custom name
    /// </summary>
    public static void TakePageScreenshot(IWebDriver driver, ILogger logger, string screenshotName = "")
    {
        try
        {
            var name = string.IsNullOrEmpty(screenshotName) ? $"PageScreenshot_{DateTime.Now:yyyyMMdd_HHmmss}" : screenshotName;
            logger.LogDebug("Taking page screenshot: {Name}", name);
            CaptureScreenshot(driver, name, logger);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to take page screenshot: {ScreenshotName}", screenshotName);
        }
    }

    /// <summary>
    /// Captures a screenshot of a specific element using Selenium 4's element screenshot feature
    /// </summary>
    public static void CaptureElementScreenshot(IWebDriver driver, IWebElement element, string screenshotName, ILogger logger)
    {
        try
        {
            logger.LogInformation("Starting element screenshot capture: {ScreenshotName}", screenshotName);
            
            // Validate inputs
            if (driver == null)
            {
                throw new ArgumentNullException(nameof(driver), "WebDriver is null");
            }
            
            if (element == null)
            {
                throw new ArgumentNullException(nameof(element), "Element is null");
            }
            
            logger.LogDebug("Element screenshot validation passed, capturing screenshot");
            
            // Use Selenium 4's element screenshot feature
            var screenshot = ((ITakesScreenshot)element).GetScreenshot();
            logger.LogDebug("Element screenshot captured");

            var screenshotPath = TestResultsHelper.GetScreenshotPath(screenshotName);
            logger.LogDebug("Element screenshot will be saved to: {ScreenshotPath}", screenshotPath);
            
            screenshot.SaveAsFile(screenshotPath);
            
            logger.LogInformation("Element screenshot saved successfully to: {ScreenshotPath}", screenshotPath);
            TestContext.AddTestAttachment(screenshotPath, $"Element screenshot: {screenshotName}");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to capture element screenshot: {ScreenshotName}", screenshotName);
            
            try
            {
                logger.LogWarning("Falling back to full page screenshot for: {ScreenshotName}", screenshotName);
                CaptureScreenshot(driver, $"{screenshotName}_FALLBACK", logger);
            }
            catch (Exception fallbackEx)
            {
                logger.LogError(fallbackEx, "Fallback screenshot also failed for: {ScreenshotName}", screenshotName);
            }
        }
    }

    /// <summary>
    /// Captures screenshots for accessibility violations with element highlighting
    /// </summary>
    /// <param name="driver">The WebDriver instance</param>
    /// <param name="results">The accessibility scan results</param>
    /// <param name="testName">The name of the test</param>
    /// <param name="accessibilitySettings">The accessibility configuration settings</param>
    /// <param name="logger">The logger instance</param>
    public static void CaptureAccessibilityViolationScreenshots(
        IWebDriver driver, 
        AxeResult results, 
        string testName, 
        AccessibilitySettings accessibilitySettings,
        ILogger logger)
    {
        if (!accessibilitySettings.CaptureScreenshotsOnViolations || !results.Violations.Any())
        {
            return;
        }

        try
        {
            logger.LogInformation("Capturing accessibility violation screenshots for test: {TestName}", testName);
            
            var violationIndex = 0;
            foreach (var violation in results.Violations)
            {
                violationIndex++;
                CaptureViolationScreenshot(driver, violation, testName, violationIndex, accessibilitySettings, logger);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to capture accessibility violation screenshots for test: {TestName}", testName);
        }
    }

    /// <summary>
    /// Logs accessibility violation details to the console
    /// </summary>
    /// <param name="results">The accessibility scan results</param>
    /// <param name="testName">The name of the test</param>
    /// <param name="logger">The logger instance</param>
    public static void LogAccessibilityViolations(AxeResult results, string testName, ILogger logger)
    {
        if (!results.Violations.Any())
        {
            logger.LogInformation("No accessibility violations found for test: {TestName}", testName);
            return;
        }

        logger.LogWarning("Accessibility violations found for test: {TestName}", testName);
        
        foreach (var violation in results.Violations)
        {
            var tagList = string.Join(", ", violation.Tags);
            logger.LogWarning("Violation: {ViolationId} [{Tags}]: {Description}", 
                violation.Id, tagList, violation.Description);
                
            foreach (var node in violation.Nodes)
            {
                var selector = node.Target.First().ToString();
                logger.LogWarning("  Element: {Selector}", selector);
            }
        }
    }
    
    /// <summary>
    /// Captures screenshots for a specific accessibility violation
    /// </summary>
    private static void CaptureViolationScreenshot(
        IWebDriver driver, 
        AxeResultItem violation, 
        string testName, 
        int violationIndex, 
        AccessibilitySettings accessibilitySettings,
        ILogger logger)
    {
        try
        {
            logger.LogDebug("Processing violation {Index}: {ViolationId}", violationIndex, violation.Id);

            foreach (var node in violation.Nodes)
            {
                var selector = node.Target.First().ToString();
                var elements = driver.FindElements(By.CssSelector(selector));
                
                foreach (var element in elements)
                {
                    if (accessibilitySettings.CaptureScreenshotsOnViolations)
                    {
                        CaptureElementScreenshotForAccessibility(driver, element, testName, violationIndex, violation.Id, logger);
                    }
                    
                    if (accessibilitySettings.HighlightViolatingElements)
                    {
                        HighlightElementAndCapturePageScreenshot(driver, element, testName, violationIndex, violation.Id, logger);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to capture screenshot for violation {ViolationId} in test {TestName}", 
                violation.Id, testName);
        }
    }

    /// <summary>
    /// Captures a screenshot of a specific element for an accessibility violation
    /// </summary>
    private static void CaptureElementScreenshotForAccessibility(
        IWebDriver driver,
        IWebElement element, 
        string testName, 
        int violationIndex, 
        string violationId, 
        ILogger logger)
    {
        try
        {
            var screenshotName = $"{testName}_{violationIndex}_{violationId}_element.png";
            CaptureElementScreenshot(driver, element, screenshotName, logger);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to capture element screenshot for violation {ViolationId} in test {TestName}", 
                violationId, testName);
        }
    }

    /// <summary>
    /// Highlights a violating element and captures a screenshot of the entire page
    /// </summary>
    private static void HighlightElementAndCapturePageScreenshot(
        IWebDriver driver, 
        IWebElement element, 
        string testName, 
        int violationIndex, 
        string violationId, 
        ILogger logger)
    {
        IJavaScriptExecutor? js = driver as IJavaScriptExecutor;
        if (js == null)
        {
            logger.LogWarning("Driver does not support JavaScript, cannot highlight elements.");
            return;
        }

        try
        {
            js.ExecuteScript("arguments[0].scrollIntoView(); arguments[0].style.outline = '3px solid red';", element);
            
            var screenshotName = $"{testName}_{violationIndex}_{violationId}_page.png";
            TakePageScreenshot(driver, logger, screenshotName);
            
            js.ExecuteScript("arguments[0].style.removeProperty('outline');", element);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to highlight element or capture page screenshot for violation {ViolationId} in test {TestName}", 
                violationId, testName);
        }
    }
} 