using System;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System.Linq; // Added for .Any()

namespace TechnicalAssignment.Utilities;

/// <summary>
/// Provides robust waiting utilities for Selenium WebDriver operations.
/// 
/// Key Features:
/// - Automatic loading spinner detection and handling
/// - Comprehensive timeout and polling interval configuration
/// - Detailed logging for debugging test failures
/// - Multiple wait condition types (visibility, clickability, text presence, etc.)
/// 
/// Loading Spinner Handling:
/// The WaitHelper automatically detects and waits for loading spinners to disappear
/// before proceeding with element interactions. This prevents the common "Node with given id 
/// does not belong to the document" errors that occur when elements are modified while 
/// spinners are active. The spinner detection supports:
/// - Bootstrap spinner-border class (primary pattern used by the application)
/// - Generic spinner patterns (.loading, .spinner, .fa-spinner, etc.) as fallback
/// 
/// Usage:
/// All wait methods automatically handle spinners. For explicit spinner waiting, use:
/// WaitForLoadingSpinnerToDisappear(driver);
/// </summary>
public static class WaitHelper
{
    private static readonly ILogger Logger = LoggingHelper.CreateLogger("WaitHelper");
    private static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(30);
    private static readonly TimeSpan DefaultPollingInterval = TimeSpan.FromMilliseconds(500);
    
    // Global spinner locators - common patterns for loading spinners
    private static readonly By[] LoadingSpinnerLocators = new[]
    {
        By.ClassName("spinner-border"),
        By.CssSelector(".spinner-border[role='status']"),
        By.CssSelector("div.spinner-border"),
        By.CssSelector(".container-fluid .spinner-border"),
        By.ClassName("loading"),
        By.ClassName("spinner"),
        By.ClassName("fa-spinner"),
        By.ClassName("loading-overlay"),
        By.CssSelector(".loading, .spinner, .fa-spinner, .loading-overlay"),
        By.CssSelector("[class*='loading'], [class*='spinner']"),
        By.XPath("//*[contains(@class, 'loading') or contains(@class, 'spinner')]")
    };
    private static readonly TimeSpan SpinnerTimeout = TimeSpan.FromSeconds(10);

    /// <summary>
    /// Creates a WebDriverWait instance with specified timeout and polling interval
    /// </summary>
    public static WebDriverWait CreateWait(IWebDriver driver, TimeSpan? timeout = null, TimeSpan? pollingInterval = null)
    {
        var actualTimeout = timeout ?? DefaultTimeout;
        var actualPollingInterval = pollingInterval ?? DefaultPollingInterval;
        
        Logger.LogDebug("Creating WebDriverWait with timeout: {Timeout}s, polling: {Polling}ms", 
            actualTimeout.TotalSeconds, actualPollingInterval.TotalMilliseconds);
        
        return new WebDriverWait(driver, actualTimeout)
        {
            PollingInterval = actualPollingInterval
        };
    }

    /// <summary>
    /// Waits for any loading spinner to disappear before proceeding
    /// </summary>
    public static bool WaitForLoadingSpinnerToDisappear(IWebDriver driver, TimeSpan? timeout = null)
    {
        var spinnerTimeout = timeout ?? SpinnerTimeout;
        Logger.LogDebug("Checking for loading spinner presence");
        
        try
        {
            foreach (var spinnerLocator in LoadingSpinnerLocators)
            {
                try
                {
                    var spinnerElements = driver.FindElements(spinnerLocator);
                    if (spinnerElements.Count > 0 && spinnerElements.Any(el => el.Displayed))
                    {
                        Logger.LogDebug("Loading spinner detected with locator: {Locator} - waiting for it to disappear", spinnerLocator);
                        var wait = CreateWait(driver, spinnerTimeout);
                        var result = wait.Until(ExpectedConditions.InvisibilityOfElementLocated(spinnerLocator));
                        Logger.LogDebug("Loading spinner disappeared");
                        return result;
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogDebug(ex, "Error checking spinner with locator: {Locator}", spinnerLocator);
                    continue;
                }
            }
            
            Logger.LogDebug("No loading spinner found - proceeding");
            return true;
        }
        catch (WebDriverTimeoutException ex)
        {
            Logger.LogWarning(ex, "Loading spinner still visible after timeout of {Timeout}s - proceeding anyway", spinnerTimeout.TotalSeconds);
            return false;
        }
        catch (Exception ex)
        {
            Logger.LogDebug(ex, "Error checking for loading spinner - proceeding anyway");
            return true;
        }
    }

    /// <summary>
    /// Waits for an element to be present and visible
    /// </summary>
    public static IWebElement WaitForElement(IWebDriver driver, By locator, TimeSpan? timeout = null)
    {
        Logger.LogDebug("Waiting for element: {Locator}", locator);
        
        WaitForLoadingSpinnerToDisappear(driver);
        
        try
        {
            var wait = CreateWait(driver, timeout);
            var element = wait.Until(ExpectedConditions.ElementIsVisible(locator));
            Logger.LogDebug("Element found and visible: {Locator}", locator);
            return element;
        }
        catch (WebDriverTimeoutException ex)
        {
            Logger.LogError("Element not found within timeout: {Locator}", locator);
            throw new TimeoutException($"Element not found within {timeout ?? DefaultTimeout}: {locator}", ex);
        }
    }

    /// <summary>
    /// Waits for an element to be clickable
    /// </summary>
    public static IWebElement WaitForElementToBeClickable(IWebDriver driver, By locator, TimeSpan? timeout = null)
    {
        Logger.LogDebug("Waiting for element to be clickable: {Locator}", locator);
        
        WaitForLoadingSpinnerToDisappear(driver);
        
        try
        {
            var wait = CreateWait(driver, timeout);
            var element = wait.Until(ExpectedConditions.ElementToBeClickable(locator));
            Logger.LogDebug("Element is clickable: {Locator}", locator);
            return element;
        }
        catch (WebDriverTimeoutException ex)
        {
            Logger.LogError("Element not clickable within timeout: {Locator}", locator);
            throw new TimeoutException($"Element not clickable within {timeout ?? DefaultTimeout}: {locator}", ex);
        }
    }

    /// <summary>
    /// Waits for specific text to be present in an element
    /// </summary>
    public static bool WaitForTextToBePresentInElement(IWebDriver driver, By locator, string text, TimeSpan? timeout = null)
    {
        Logger.LogDebug("Waiting for text '{Text}' in element: {Locator}", text, locator);
        
        try
        {
            var wait = CreateWait(driver, timeout);
            var result = wait.Until(ExpectedConditions.TextToBePresentInElementLocated(locator, text));
            Logger.LogDebug("Text '{Text}' found in element: {Locator}", text, locator);
            return result;
        }
        catch (WebDriverTimeoutException ex)
        {
            Logger.LogError("Text '{Text}' not found in element within timeout: {Locator}", text, locator);
            throw new TimeoutException($"Text '{text}' not found in element within {timeout ?? DefaultTimeout}: {locator}", ex);
        }
    }

    /// <summary>
    /// Waits for an element to be present (but not necessarily visible)
    /// </summary>
    public static IWebElement WaitForElementPresent(IWebDriver driver, By locator, TimeSpan? timeout = null)
    {
        Logger.LogDebug("Waiting for element to be present: {Locator}", locator);
        
        try
        {
            var wait = CreateWait(driver, timeout);
            var element = wait.Until(ExpectedConditions.ElementExists(locator));
            Logger.LogDebug("Element present: {Locator}", locator);
            return element;
        }
        catch (WebDriverTimeoutException ex)
        {
            Logger.LogError("Element not present within timeout: {Locator}", locator);
            throw new TimeoutException($"Element not present within {timeout ?? DefaultTimeout}: {locator}", ex);
        }
    }

    /// <summary>
    /// Waits for an element to disappear/become invisible
    /// </summary>
    public static bool WaitForElementToDisappear(IWebDriver driver, By locator, TimeSpan? timeout = null)
    {
        Logger.LogDebug("Waiting for element to disappear: {Locator}", locator);
        
        try
        {
            var wait = CreateWait(driver, timeout);
            var result = wait.Until(ExpectedConditions.InvisibilityOfElementLocated(locator));
            Logger.LogDebug("Element disappeared: {Locator}", locator);
            return result;
        }
        catch (WebDriverTimeoutException ex)
        {
            Logger.LogError("Element still visible after timeout: {Locator}", locator);
            throw new TimeoutException($"Element still visible after {timeout ?? DefaultTimeout}: {locator}", ex);
        }
    }

    /// <summary>
    /// Waits for page title to contain specific text
    /// </summary>
    public static bool WaitForTitleContains(IWebDriver driver, string titleText, TimeSpan? timeout = null)
    {
        Logger.LogDebug("Waiting for page title to contain: '{Title}'", titleText);
        
        try
        {
            var wait = CreateWait(driver, timeout);
            var result = wait.Until(ExpectedConditions.TitleContains(titleText));
            Logger.LogDebug("Page title contains: '{Title}'", titleText);
            return result;
        }
        catch (WebDriverTimeoutException ex)
        {
            Logger.LogError("Page title does not contain '{Title}' within timeout", titleText);
            throw new TimeoutException($"Page title does not contain '{titleText}' within {timeout ?? DefaultTimeout}", ex);
        }
    }

    /// <summary>
    /// Waits for URL to contain specific text
    /// </summary>
    public static bool WaitForUrlContains(IWebDriver driver, string urlText, TimeSpan? timeout = null)
    {
        Logger.LogDebug("Waiting for URL to contain: '{Url}'", urlText);
        
        try
        {
            var wait = CreateWait(driver, timeout);
            var result = wait.Until(ExpectedConditions.UrlContains(urlText));
            Logger.LogDebug("URL contains: '{Url}'", urlText);
            return result;
        }
        catch (WebDriverTimeoutException ex)
        {
            Logger.LogError("URL does not contain '{Url}' within timeout", urlText);
            throw new TimeoutException($"URL does not contain '{urlText}' within {timeout ?? DefaultTimeout}", ex);
        }
    }

    /// <summary>
    /// Waits for a custom condition with retry logic, ensuring loading spinner is gone first
    /// </summary>
    public static T WaitForCondition<T>(IWebDriver driver, Func<IWebDriver, T> condition, TimeSpan? timeout = null)
    {
        Logger.LogDebug("Waiting for custom condition");
        
        WaitForLoadingSpinnerToDisappear(driver);
        
        try
        {
            var wait = CreateWait(driver, timeout);
            var result = wait.Until(condition);
            Logger.LogDebug("Custom condition met");
            return result;
        }
        catch (WebDriverTimeoutException ex)
        {
            Logger.LogError("Custom condition not met within timeout");
            throw new TimeoutException($"Custom condition not met within {timeout ?? DefaultTimeout}", ex);
        }
    }

    /// <summary>
    /// Waits for URL to change from the initial URL
    /// </summary>
    public static bool WaitForUrlChange(IWebDriver driver, string initialUrl, TimeSpan? timeout = null)
    {
        Logger.LogDebug("Waiting for URL to change from: '{InitialUrl}'", initialUrl);
        
        try
        {
            var wait = CreateWait(driver, timeout);
            var result = wait.Until(d => !d.Url.Equals(initialUrl, StringComparison.OrdinalIgnoreCase));
            Logger.LogDebug("URL changed from: '{InitialUrl}' to: '{NewUrl}'", initialUrl, driver.Url);
            return result;
        }
        catch (WebDriverTimeoutException ex)
        {
            Logger.LogError("URL did not change from '{InitialUrl}' within timeout", initialUrl);
            throw new TimeoutException($"URL did not change from '{initialUrl}' within {timeout ?? DefaultTimeout}", ex);
        }
    }


} 