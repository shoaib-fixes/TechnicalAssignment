using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;

namespace TechnicalAssignment.Utilities;

public static class ElementHelper
{
    private static readonly ILogger Logger = LoggingHelper.CreateLogger("ElementHelper");

    /// <summary>
    /// Safely clicks an element after waiting for it to be clickable
    /// </summary>
    public static void SafeClick(IWebDriver driver, By locator, TimeSpan? timeout = null)
    {
        Logger.LogDebug("Attempting safe click on element: {Locator}", locator);
        var clickTimeout = timeout ?? TimeSpan.FromSeconds(30);

        try
        {
            WaitHelper.WaitForCondition(driver, d =>
            {
                try
                {
                    var element = WaitHelper.WaitForElementToBeClickable(d, locator, timeout);
                    element.Click();
                    return true;
                }
                catch (StaleElementReferenceException ex)
                {
                    Logger.LogWarning(ex, "Stale element reference when clicking {Locator}, retrying", locator);
                    return false;
                }
            }, clickTimeout);
            Logger.LogDebug("Successfully clicked element: {Locator}", locator);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to click element: {Locator}", locator);
            throw;
        }
    }

    /// <summary>
    /// Safely clicks an element using JavaScript executor
    /// </summary>
    public static void JavaScriptClick(IWebDriver driver, By locator, TimeSpan? timeout = null)
    {
        Logger.LogDebug("Attempting JavaScript click on element: {Locator}", locator);
        
        try
        {
            var element = WaitHelper.WaitForElement(driver, locator, timeout);
            var jsExecutor = (IJavaScriptExecutor)driver;
            jsExecutor.ExecuteScript("arguments[0].click();", element);
            Logger.LogDebug("Successfully JavaScript clicked element: {Locator}", locator);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to JavaScript click element: {Locator}", locator);
            throw;
        }
    }

    /// <summary>
    /// Safely sends keys to an element after clearing it
    /// </summary>
    public static void SafeSendKeys(IWebDriver driver, By locator, string text, TimeSpan? timeout = null)
    {
        Logger.LogDebug("Attempting to send keys '{Text}' to element: {Locator}", text, locator);
        
        try
        {
            var element = WaitHelper.WaitForElement(driver, locator, timeout);
            element.Clear();
            element.SendKeys(text);
            Logger.LogDebug("Successfully sent keys to element: {Locator}", locator);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to send keys to element: {Locator}", locator);
            throw;
        }
    }

    /// <summary>
    /// Safely gets text from an element
    /// </summary>
    public static string GetElementText(IWebDriver driver, By locator, TimeSpan? timeout = null)
    {
        Logger.LogDebug("Attempting to get text from element: {Locator}", locator);
        
        try
        {
            var element = WaitHelper.WaitForElement(driver, locator, timeout);
            var text = element.Text;
            Logger.LogDebug("Successfully got text '{Text}' from element: {Locator}", text, locator);
            return text;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to get text from element: {Locator}", locator);
            throw;
        }
    }

    /// <summary>
    /// Safely gets an attribute value from an element
    /// </summary>
    public static string GetElementAttribute(IWebDriver driver, By locator, string attributeName, TimeSpan? timeout = null)
    {
        Logger.LogDebug("Attempting to get attribute '{Attribute}' from element: {Locator}", attributeName, locator);
        
        try
        {
            var element = WaitHelper.WaitForElement(driver, locator, timeout);
            var attributeValue = element.GetAttribute(attributeName);
            Logger.LogDebug("Successfully got attribute '{Attribute}' = '{Value}' from element: {Locator}", 
                attributeName, attributeValue, locator);
            return attributeValue ?? string.Empty;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to get attribute '{Attribute}' from element: {Locator}", attributeName, locator);
            throw;
        }
    }

    /// <summary>
    /// Checks if an element is visible on the page
    /// </summary>
    public static bool IsElementVisible(IWebDriver driver, By locator, TimeSpan? timeout = null)
    {
        Logger.LogDebug("Checking if element is visible: {Locator}", locator);
        
        try
        {
            WaitHelper.WaitForElement(driver, locator, timeout ?? TimeSpan.FromSeconds(5));
            Logger.LogDebug("Element is visible: {Locator}", locator);
            return true;
        }
        catch (TimeoutException)
        {
            Logger.LogDebug("Element is not visible: {Locator}", locator);
            return false;
        }
    }

    /// <summary>
    /// Checks if an element exists in the DOM (but may not be visible)
    /// </summary>
    public static bool IsElementPresent(IWebDriver driver, By locator, TimeSpan? timeout = null)
    {
        Logger.LogDebug("Checking if element is present: {Locator}", locator);
        
        try
        {
            WaitHelper.WaitForElementPresent(driver, locator, timeout ?? TimeSpan.FromSeconds(5));
            Logger.LogDebug("Element is present: {Locator}", locator);
            return true;
        }
        catch (TimeoutException)
        {
            Logger.LogDebug("Element is not present: {Locator}", locator);
            return false;
        }
    }



    /// <summary>
    /// Selects an option from a dropdown by visible text
    /// </summary>
    public static void SelectDropdownByText(IWebDriver driver, By locator, string text, TimeSpan? timeout = null)
    {
        Logger.LogDebug("Selecting dropdown option '{Text}' from element: {Locator}", text, locator);
        
        try
        {
            var element = WaitHelper.WaitForElement(driver, locator, timeout);
            var select = new SelectElement(element);
            select.SelectByText(text);
            Logger.LogDebug("Successfully selected dropdown option '{Text}' from element: {Locator}", text, locator);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to select dropdown option '{Text}' from element: {Locator}", text, locator);
            throw;
        }
    }

    /// <summary>
    /// Selects an option from a dropdown by value
    /// </summary>
    public static void SelectDropdownByValue(IWebDriver driver, By locator, string value, TimeSpan? timeout = null)
    {
        Logger.LogDebug("Selecting dropdown option by value '{Value}' from element: {Locator}", value, locator);
        
        try
        {
            var element = WaitHelper.WaitForElement(driver, locator, timeout);
            var select = new SelectElement(element);
            select.SelectByValue(value);
            Logger.LogDebug("Successfully selected dropdown option by value '{Value}' from element: {Locator}", value, locator);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to select dropdown option by value '{Value}' from element: {Locator}", value, locator);
            throw;
        }
    }

    /// <summary>
    /// Gets all selected options from a dropdown
    /// </summary>
    public static List<string> GetSelectedDropdownOptions(IWebDriver driver, By locator, TimeSpan? timeout = null)
    {
        Logger.LogDebug("Getting selected dropdown options from element: {Locator}", locator);
        
        try
        {
            var element = WaitHelper.WaitForElement(driver, locator, timeout);
            var select = new SelectElement(element);
            var selectedOptions = select.AllSelectedOptions.Select(option => option.Text).ToList();
            Logger.LogDebug("Successfully got {Count} selected options from element: {Locator}", selectedOptions.Count, locator);
            return selectedOptions;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to get selected dropdown options from element: {Locator}", locator);
            throw;
        }
    }

    /// <summary>
    /// Performs a hover action on an element
    /// </summary>
    public static void HoverOverElement(IWebDriver driver, By locator, TimeSpan? timeout = null)
    {
        Logger.LogDebug("Hovering over element: {Locator}", locator);
        
        try
        {
            var element = WaitHelper.WaitForElement(driver, locator, timeout);
            var actions = new Actions(driver);
            actions.MoveToElement(element).Perform();
            Logger.LogDebug("Successfully hovered over element: {Locator}", locator);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to hover over element: {Locator}", locator);
            throw;
        }
    }

    /// <summary>
    /// Double clicks an element
    /// </summary>
    public static void DoubleClick(IWebDriver driver, By locator, TimeSpan? timeout = null)
    {
        Logger.LogDebug("Double clicking element: {Locator}", locator);
        
        try
        {
            var element = WaitHelper.WaitForElement(driver, locator, timeout);
            var actions = new Actions(driver);
            actions.DoubleClick(element).Perform();
            Logger.LogDebug("Successfully double clicked element: {Locator}", locator);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to double click element: {Locator}", locator);
            throw;
        }
    }

    /// <summary>
    /// Right clicks an element
    /// </summary>
    public static void RightClick(IWebDriver driver, By locator, TimeSpan? timeout = null)
    {
        Logger.LogDebug("Right clicking element: {Locator}", locator);
        
        try
        {
            var element = WaitHelper.WaitForElement(driver, locator, timeout);
            var actions = new Actions(driver);
            actions.ContextClick(element).Perform();
            Logger.LogDebug("Successfully right clicked element: {Locator}", locator);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to right click element: {Locator}", locator);
            throw;
        }
    }

    /// <summary>
    /// Drags and drops an element to a target location
    /// </summary>
    public static void DragAndDrop(IWebDriver driver, By sourceLocator, By targetLocator, TimeSpan? timeout = null)
    {
        Logger.LogDebug("Dragging element from {Source} to {Target}", sourceLocator, targetLocator);
        
        try
        {
            var sourceElement = WaitHelper.WaitForElement(driver, sourceLocator, timeout);
            var targetElement = WaitHelper.WaitForElement(driver, targetLocator, timeout);
            
            var actions = new Actions(driver);
            actions.DragAndDrop(sourceElement, targetElement).Perform();
            Logger.LogDebug("Successfully dragged element from {Source} to {Target}", sourceLocator, targetLocator);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to drag element from {Source} to {Target}", sourceLocator, targetLocator);
            throw;
        }
    }
} 