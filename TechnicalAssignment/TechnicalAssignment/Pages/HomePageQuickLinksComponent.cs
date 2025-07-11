using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using TechnicalAssignment.Utilities;

namespace TechnicalAssignment.Pages;

public class HomePageQuickLinksComponent
{
    protected readonly IWebDriver Driver;
    protected readonly ILogger<HomePageQuickLinksComponent> Logger;
    
    private static readonly By QuickLinksSection = By.XPath("//h5[text()='Quick Links']/parent::div");
    private static readonly By QuickLinksHeader = By.XPath("//h5[text()='Quick Links']");
    private static readonly By QuickLinksList = By.XPath("//h5[text()='Quick Links']/following-sibling::ul");
    private static readonly By AllQuickLinks = By.XPath("//h5[text()='Quick Links']/following-sibling::ul//a");

    public HomePageQuickLinksComponent(IWebDriver driver, ILogger<HomePageQuickLinksComponent> logger)
    {
        Driver = driver;
        Logger = logger;
    }

    private static By GetLinkByText(string linkText) => By.XPath($"//h5[text()='Quick Links']/following-sibling::ul//a[text()='{linkText}']");

    public void ScrollToQuickLinks()
    {
        Logger.LogDebug("Scrolling to Quick Links section");
        ScrollHelper.ScrollToElement(Driver, QuickLinksSection, TimeSpan.FromSeconds(10));
    }

    public bool IsQuickLinksSectionVisible(TimeSpan? timeout = null)
    {
        return ElementHelper.IsElementVisible(Driver, QuickLinksSection, timeout);
    }

    public bool IsQuickLinksHeaderVisible(TimeSpan? timeout = null)
    {
        return ElementHelper.IsElementVisible(Driver, QuickLinksHeader, timeout);
    }

    public string GetQuickLinksHeaderText()
    {
        return ElementHelper.GetElementText(Driver, QuickLinksHeader);
    }

    public IWebElement GetQuickLinksHeaderElement()
    {
        return WaitHelper.WaitForElement(Driver, QuickLinksHeader);
    }

    public IWebElement GetQuickLinksSectionElement()
    {
        return WaitHelper.WaitForElement(Driver, QuickLinksSection);
    }

    public IWebElement GetQuickLinksListElement()
    {
        return WaitHelper.WaitForElement(Driver, QuickLinksList);
    }

    public bool IsLinkVisible(string linkText, TimeSpan? timeout = null)
    {
        return ElementHelper.IsElementVisible(Driver, GetLinkByText(linkText), timeout);
    }

    public string GetLinkText(string linkText)
    {
        return ElementHelper.GetElementText(Driver, GetLinkByText(linkText));
    }

    public void ClickLink(string linkText)
    {
        ElementHelper.SafeClick(Driver, GetLinkByText(linkText));
    }
    
    public string GetLinkHref(string linkText)
    {
        return ElementHelper.GetElementAttribute(Driver, GetLinkByText(linkText), "href");
    }

    public bool IsLinkEnabled(string linkText)
    {
        return WaitHelper.WaitForElement(Driver, GetLinkByText(linkText)).Enabled;
    }
    
    public string GetLinkCssValue(string linkText, string cssProperty)
    {
        var element = WaitHelper.WaitForElement(Driver, GetLinkByText(linkText));
        return element.GetCssValue(cssProperty);
    }

    public IWebElement GetLinkElement(string linkText)
    {
        return WaitHelper.WaitForElement(Driver, GetLinkByText(linkText));
    }

    public void HoverOverLink(string linkText)
    {
        Logger.LogDebug("Hovering over Quick Link: {LinkText}", linkText);
        ElementHelper.HoverOverElement(Driver, GetLinkByText(linkText));
    }




    public bool ValidateQuickLinksListStructure()
    {
        Logger.LogDebug("Validating Quick Links list structure");
        try
        {
            var listElement = GetQuickLinksListElement();
            var listItems = listElement.FindElements(By.TagName("li"));
            
            if (listItems.Count != 4)
            {
                Logger.LogWarning("Quick Links list should have 4 list items, found: {Count}", listItems.Count);
                return false;
            }
            
            return true;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error validating Quick Links list structure");
            return false;
        }
    }

    public bool AreAllQuickLinksVisible(TimeSpan? timeout = null)
    {
        return IsLinkVisible("Home", timeout) && 
               IsLinkVisible("Rooms", timeout) && 
               IsLinkVisible("Booking", timeout) && 
               IsLinkVisible("Contact", timeout);
    }

    public List<string> GetAllQuickLinksText()
    {
        Logger.LogDebug("Getting all Quick Links text");
        var linkElements = Driver.FindElements(AllQuickLinks);
        return linkElements.Select(element => element.Text).ToList();
    }
} 