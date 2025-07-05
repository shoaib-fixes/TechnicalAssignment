using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using TechnicalAssignment.Utilities;

namespace TechnicalAssignment.Pages;

public class HomePageNavigationComponent
{
    protected readonly IWebDriver Driver;
    protected readonly ILogger Logger;
    
    private static readonly By SocialMediaIconsContainer = By.CssSelector(".d-flex.gap-2");
    private static readonly By FacebookIcon = By.CssSelector(".d-flex.gap-2 a:has(.bi-facebook)");
    private static readonly By InstagramIcon = By.CssSelector(".d-flex.gap-2 a:has(.bi-instagram)");
    private static readonly By TwitterIcon = By.CssSelector(".d-flex.gap-2 a:has(.bi-twitter)");
    private static readonly By AllSocialIcons = By.CssSelector(".d-flex.gap-2 a");
    private static readonly By FacebookIconElement = By.CssSelector(".bi-facebook");
    private static readonly By InstagramIconElement = By.CssSelector(".bi-instagram");
    private static readonly By TwitterIconElement = By.CssSelector(".bi-twitter");

    public HomePageNavigationComponent(IWebDriver driver, ILogger logger)
    {
        Driver = driver;
        Logger = logger;
    }

    public void ScrollToSocialMediaIcons()
    {
        Logger.LogDebug("Scrolling to Social Media Icons section");
        ScrollHelper.ScrollToElement(Driver, SocialMediaIconsContainer, TimeSpan.FromSeconds(10));
    }

    public void ClickFacebookIcon()
    {
        ElementHelper.SafeClick(Driver, FacebookIcon);
    }

    public void ClickInstagramIcon()
    {
        ElementHelper.SafeClick(Driver, InstagramIcon);
    }

    public void ClickTwitterIcon()
    {
        ElementHelper.SafeClick(Driver, TwitterIcon);
    }

    public void HoverOverFacebookIcon()
    {
        ElementHelper.HoverOverElement(Driver, FacebookIcon);
    }

    public void HoverOverInstagramIcon()
    {
        ElementHelper.HoverOverElement(Driver, InstagramIcon);
    }

    public void HoverOverTwitterIcon()
    {
        ElementHelper.HoverOverElement(Driver, TwitterIcon);
    }

    public bool IsSocialMediaIconsContainerVisible(TimeSpan? timeout = null)
    {
        return ElementHelper.IsElementVisible(Driver, SocialMediaIconsContainer, timeout);
    }

    public bool IsFacebookIconVisible(TimeSpan? timeout = null)
    {
        return ElementHelper.IsElementVisible(Driver, FacebookIcon, timeout);
    }

    public bool IsInstagramIconVisible(TimeSpan? timeout = null)
    {
        return ElementHelper.IsElementVisible(Driver, InstagramIcon, timeout);
    }

    public bool IsTwitterIconVisible(TimeSpan? timeout = null)
    {
        return ElementHelper.IsElementVisible(Driver, TwitterIcon, timeout);
    }

    public bool AreAllSocialMediaIconsVisible(TimeSpan? timeout = null)
    {
        return IsFacebookIconVisible(timeout) && IsInstagramIconVisible(timeout) && IsTwitterIconVisible(timeout);
    }

    public IWebElement GetSocialMediaIconsContainerElement()
    {
        return WaitHelper.WaitForElement(Driver, SocialMediaIconsContainer);
    }

    public IWebElement GetFacebookIconElement()
    {
        return WaitHelper.WaitForElement(Driver, FacebookIcon);
    }

    public IWebElement GetInstagramIconElement()
    {
        return WaitHelper.WaitForElement(Driver, InstagramIcon);
    }

    public IWebElement GetTwitterIconElement()
    {
        return WaitHelper.WaitForElement(Driver, TwitterIcon);
    }

    public bool ValidateSocialMediaIconStyling(string iconType)
    {
        Logger.LogDebug("Validating Social Media Icon styling for: {IconType}", iconType);
        
        By iconLocator = iconType.ToLower() switch
        {
            "facebook" => FacebookIcon,
            "instagram" => InstagramIcon,
            "twitter" => TwitterIcon,
            _ => throw new ArgumentException($"Unknown icon type: {iconType}")
        };
        
        var classes = ElementHelper.GetElementAttribute(Driver, iconLocator, "class");
        Logger.LogDebug("Social Media Icon {IconType} classes: {Classes}", iconType, classes);
        
        return classes.Contains("btn") && 
               classes.Contains("btn-sm") && 
               classes.Contains("btn-outline-light") && 
               classes.Contains("rounded-circle");
    }

    public bool ValidateSocialMediaIconsContainerStructure()
    {
        Logger.LogDebug("Validating Social Media Icons container structure");
        try
        {
            var containerElement = GetSocialMediaIconsContainerElement();
            var containerClasses = containerElement.GetAttribute("class") ?? "";
            
            if (!containerClasses.Contains("d-flex") || !containerClasses.Contains("gap-2"))
            {
                Logger.LogWarning("Social Media Icons container does not have correct classes: {Classes}", containerClasses);
                return false;
            }
            
            var iconLinks = containerElement.FindElements(By.TagName("a"));
            if (iconLinks.Count != 3)
            {
                Logger.LogWarning("Social Media Icons container should have 3 icon links, found: {Count}", iconLinks.Count);
                return false;
            }
            
            return true;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error validating Social Media Icons container structure");
            return false;
        }
    }

    public List<string> GetAllSocialMediaIconsHref()
    {
        Logger.LogDebug("Getting all Social Media Icons href attributes");
        var iconElements = Driver.FindElements(AllSocialIcons);
        return iconElements.Select(element => element.GetAttribute("href") ?? "").ToList();
    }

    public bool ValidateAllSocialMediaIconsHref()
    {
        var allLinksValid = true;
        var socialMediaData = GetExpectedSocialMediaURLs();
        
        foreach (var (iconType, expectedUrl) in socialMediaData)
        {
            var locator = By.CssSelector($".d-flex.gap-2 a:has(.bi-{iconType.ToLower()})");
            var actualHref = ElementHelper.GetElementAttribute(Driver, locator, "href");
            
            if (!actualHref.EndsWith(expectedUrl, StringComparison.OrdinalIgnoreCase))
            {
                Logger.LogWarning("Social media link validation failed for {IconType}. Expected: '{ExpectedUrl}', Actual: '{ActualUrl}'", iconType, expectedUrl, actualHref);
                allLinksValid = false;
            }
        }
        
        return allLinksValid;
    }

    public bool ValidateAllSocialMediaIconsStyling()
    {
        Logger.LogDebug("Validating all Social Media Icons styling");
        try
        {
            var iconTypes = new[] { "Facebook", "Instagram", "Twitter" };
            
            foreach (var iconType in iconTypes)
            {
                if (!ValidateSocialMediaIconStyling(iconType))
                {
                    Logger.LogWarning("Social Media Icon {IconType} styling validation failed", iconType);
                    return false;
                }
            }
            
            return true;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error validating all Social Media Icons styling");
            return false;
        }
    }

    public bool ValidateBootstrapIconsLoading()
    {
        Logger.LogDebug("Validating Bootstrap icons loading");
        try
        {
            var facebookIcon = ElementHelper.IsElementVisible(Driver, FacebookIconElement, TimeSpan.FromSeconds(2));
            var instagramIcon = ElementHelper.IsElementVisible(Driver, InstagramIconElement, TimeSpan.FromSeconds(2));
            var twitterIcon = ElementHelper.IsElementVisible(Driver, TwitterIconElement, TimeSpan.FromSeconds(2));
            
            if (!facebookIcon || !instagramIcon || !twitterIcon)
            {
                Logger.LogWarning("Bootstrap icons loading validation failed. Expected all icons to be visible.");
                return false;
            }
            return true;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error validating Bootstrap icons loading");
            return false;
        }
    }

    public static Dictionary<string, string> GetExpectedSocialMediaURLs()
    {
        return new Dictionary<string, string>
        {
            { "Facebook", "https://www.facebook.com/[company-page]" },
            { "Instagram", "https://www.instagram.com/[company-page]" },
            { "Twitter", "https://twitter.com/[company-page]" }
        };
    }

    public static List<string> GetExpectedSocialMediaIconsOrder()
    {
        return new List<string> { "Facebook", "Twitter", "Instagram" };
    }
} 