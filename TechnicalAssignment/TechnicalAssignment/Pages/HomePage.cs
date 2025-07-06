using System;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using TechnicalAssignment.Utilities;

namespace TechnicalAssignment.Pages;

public class HomePage : BasePage
{
    private static readonly By PageLoadIndicator = By.CssSelector("body");
    private static readonly By PageTitle = By.TagName("title");
    
    public HomePageMainNavigationComponent Navigation { get; }
    public HomePageNavigationComponent SocialMedia { get; }
    public HomePageQuickLinksComponent QuickLinks { get; }
    public HomePageContactComponent Contact { get; }

    public HomePage(IWebDriver driver) : base(driver)
    {
        Navigation = new HomePageMainNavigationComponent(driver, Logger);
        SocialMedia = new HomePageNavigationComponent(driver, Logger);
        QuickLinks = new HomePageQuickLinksComponent(driver, Logger);
        Contact = new HomePageContactComponent(driver, Logger);
    }

    public override bool IsPageLoaded(TimeSpan? timeout = null)
    {
        try
        {
            return ElementHelper.IsElementVisible(Driver, PageLoadIndicator, timeout ?? TimeSpan.FromSeconds(10));
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error checking if HomePage is loaded");
            return false;
        }
    }

    public override void WaitForPageToLoad(TimeSpan? timeout = null)
    {
        Logger.LogDebug("Waiting for HomePage to load");
        WaitHelper.WaitForElement(Driver, PageLoadIndicator, timeout ?? TimeSpan.FromSeconds(30));
        Logger.LogDebug("HomePage loaded successfully");
    }
} 