using System;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using TechnicalAssignment.Utilities;

namespace TechnicalAssignment.Pages;

public class HomePage : BasePage
{
    private static readonly By PageLoadIndicator = By.CssSelector("body");
    private static readonly By PageTitle = By.TagName("title");
    
    public HomePageNavigationComponent SocialMedia { get; }

    public HomePage(IWebDriver driver) : base(driver)
    {
        SocialMedia = new HomePageNavigationComponent(driver, Logger);
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

    public void ScrollToSocialMediaIcons()
    {
        SocialMedia.ScrollToSocialMediaIcons();
    }

    public void ClickFacebookIcon()
    {
        SocialMedia.ClickFacebookIcon();
    }

    public void ClickInstagramIcon()
    {
        SocialMedia.ClickInstagramIcon();
    }

    public void ClickTwitterIcon()
    {
        SocialMedia.ClickTwitterIcon();
    }

    public bool IsSocialMediaIconsContainerVisible(TimeSpan? timeout = null)
    {
        return SocialMedia.IsSocialMediaIconsContainerVisible(timeout);
    }

    public bool AreAllSocialMediaIconsVisible(TimeSpan? timeout = null)
    {
        return SocialMedia.AreAllSocialMediaIconsVisible(timeout);
    }

    public IWebElement GetSocialMediaIconsContainerElement()
    {
        return SocialMedia.GetSocialMediaIconsContainerElement();
    }

    public IWebElement GetFacebookIconElement()
    {
        return SocialMedia.GetFacebookIconElement();
    }

    public IWebElement GetInstagramIconElement()
    {
        return SocialMedia.GetInstagramIconElement();
    }

    public IWebElement GetTwitterIconElement()
    {
        return SocialMedia.GetTwitterIconElement();
    }
} 