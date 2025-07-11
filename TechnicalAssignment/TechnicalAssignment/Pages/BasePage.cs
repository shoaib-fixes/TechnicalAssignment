using System;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using TechnicalAssignment.Utilities;

namespace TechnicalAssignment.Pages;

/// <summary>
/// Base class for all page objects following the Page Object Model pattern.
/// This class provides only essential page object abstractions and infrastructure.
/// For browser operations, use utility classes directly (BrowserHelper, ScrollHelper, etc.)
/// </summary>

public abstract class BasePage
{
    protected readonly IWebDriver Driver;
    protected readonly ILogger Logger;

    protected BasePage(IWebDriver driver, ILogger logger)
    {
        Driver = driver;
        Logger = logger;
    }

    protected BasePage(IWebDriver driver)
    {
        Driver = driver;
        // This is a fallback for pages that don't get a specific logger.
        // It's better to provide one, but this prevents crashes.
        var loggerFactory = new LoggerFactory();
        Logger = loggerFactory.CreateLogger(GetType());
    }


    public abstract bool IsPageLoaded(TimeSpan? timeout = null);

    public abstract void WaitForPageToLoad(TimeSpan? timeout = null);

    protected bool IsElementVisible(By locator, TimeSpan? timeout = null)
    {
        return ElementHelper.IsElementVisible(Driver, locator, timeout);
    }

    protected IWebElement FindElement(By locator, TimeSpan? timeout = null)
    {
        return WaitHelper.WaitForElement(Driver, locator, timeout);
    }

    protected string GetElementText(By locator, TimeSpan? timeout = null)
    {
        return ElementHelper.GetElementText(Driver, locator, timeout);
    }

    protected void ClickElement(By locator, TimeSpan? timeout = null)
    {
        ElementHelper.SafeClick(Driver, locator, timeout);
    }

    protected void SendKeysToElement(By locator, string text, TimeSpan? timeout = null)
    {
        ElementHelper.SafeSendKeys(Driver, locator, text, timeout);
    }

    protected void WaitForElement(By locator, TimeSpan? timeout = null)
    {
        WaitHelper.WaitForElement(Driver, locator, timeout);
    }
} 