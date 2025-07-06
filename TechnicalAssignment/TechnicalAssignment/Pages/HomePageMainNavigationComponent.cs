using System;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using TechnicalAssignment.Utilities;

namespace TechnicalAssignment.Pages;

public class HomePageMainNavigationComponent
{
    protected readonly IWebDriver Driver;
    protected readonly ILogger Logger;
    
    private static readonly By NavbarContainer = By.CssSelector(".container");
    private static readonly By BrandLink = By.CssSelector(".navbar-brand");
    private static readonly By NavbarToggler = By.CssSelector(".navbar-toggler");
    private static readonly By NavbarCollapse = By.CssSelector("#navbarNav");
    private static readonly By RoomsLink = By.CssSelector("a[href='/#rooms']");
    private static readonly By BookingLink = By.CssSelector("a[href='/#booking']");
    private static readonly By AmenitiesLink = By.CssSelector("a[href='/#amenities']");
    private static readonly By LocationLink = By.CssSelector("a[href='/#location']");
    private static readonly By ContactLink = By.CssSelector("a[href='/#contact']");
    private static readonly By AdminLink = By.CssSelector("a[href='/admin']");
    
    private static readonly By RoomsSection = By.Id("rooms");
    private static readonly By BookingSection = By.Id("booking");
    private static readonly By AmenitiesSection = By.Id("amenities");
    private static readonly By LocationSection = By.Id("location");
    private static readonly By ContactSection = By.Id("contact");

    public HomePageMainNavigationComponent(IWebDriver driver, ILogger logger)
    {
        Driver = driver;
        Logger = logger;
    }

    public void ClickBrandLink()
    {
        ElementHelper.SafeClick(Driver, BrandLink);
    }

    public void ClickRoomsLink()
    {
        ElementHelper.SafeClick(Driver, RoomsLink);
    }

    public void ClickBookingLink()
    {
        ElementHelper.SafeClick(Driver, BookingLink);
    }

    public void ClickAmenitiesLink()
    {
        ElementHelper.SafeClick(Driver, AmenitiesLink);
    }

    public void ClickLocationLink()
    {
        ElementHelper.SafeClick(Driver, LocationLink);
    }

    public void ClickContactLink()
    {
        ElementHelper.SafeClick(Driver, ContactLink);
    }

    public void ClickAdminLink()
    {
        ElementHelper.SafeClick(Driver, AdminLink);
    }

    public void ClickNavbarToggler()
    {
        ElementHelper.SafeClick(Driver, NavbarToggler);
    }

    public bool ToggleNavbarWithRetry(bool shouldBeCollapsed, TimeSpan timeout)
    {
        Logger.LogDebug("Attempting to toggle navbar to {State} state", shouldBeCollapsed ? "collapsed" : "expanded");
        
        var endTime = DateTime.Now.Add(timeout);
        var attempts = 0;
        const int maxAttempts = 3;
        
        while (DateTime.Now < endTime && attempts < maxAttempts)
        {
            attempts++;
            Logger.LogDebug("Toggle attempt {Attempt}/{MaxAttempts}", attempts, maxAttempts);
            
            try
            {
                var currentlyCollapsed = IsNavbarCollapsed(TimeSpan.FromSeconds(1));
                Logger.LogDebug("Current navbar state - collapsed: {IsCollapsed}", currentlyCollapsed);
                
                if (currentlyCollapsed == shouldBeCollapsed)
                {
                    Logger.LogDebug("Navbar already in desired state");
                    return true;
                }
                
                if (attempts == 1)
                {
                    Logger.LogDebug("Attempting standard click");
                    ElementHelper.SafeClick(Driver, NavbarToggler);
                }
                else if (attempts == 2)
                {
                    Logger.LogDebug("Attempting JavaScript click");
                    ElementHelper.JavaScriptClick(Driver, NavbarToggler);
                }
                else
                {
                    Logger.LogDebug("Attempting delayed standard click");
                    WaitHelper.WaitForElementToBeClickable(Driver, NavbarToggler, TimeSpan.FromSeconds(2));
                    ElementHelper.SafeClick(Driver, NavbarToggler);
                }
                
                WaitHelper.WaitForCondition(Driver, d => 
                {
                    try 
                    {
                        var navElement = d.FindElement(NavbarCollapse);
                        var classes = navElement.GetAttribute("class") ?? "";
                        return !classes.Contains("collapsing");
                    }
                    catch 
                    {
                        return true;
                    }
                }, TimeSpan.FromSeconds(3));
                
                var newState = IsNavbarCollapsed(TimeSpan.FromSeconds(2));
                Logger.LogDebug("New navbar state after toggle - collapsed: {IsCollapsed}", newState);
                
                if (newState == shouldBeCollapsed)
                {
                    Logger.LogDebug("Navbar toggle successful on attempt {Attempt}", attempts);
                    return true;
                }
                
                Logger.LogDebug("Toggle attempt {Attempt} failed, retrying", attempts);
            }
            catch (Exception ex)
            {
                Logger.LogWarning(ex, "Error during toggle attempt {Attempt}", attempts);
            }
        }
        
        Logger.LogError("Failed to toggle navbar to {State} state after {Attempts} attempts", 
            shouldBeCollapsed ? "collapsed" : "expanded", attempts);
        return false;
    }

    public bool IsNavbarCollapsed(TimeSpan? timeout = null)
    {
        try
        {
            if (!ElementHelper.IsElementPresent(Driver, NavbarCollapse, timeout ?? TimeSpan.FromSeconds(5)))
            {
                return true;
            }
            
            var navbarElement = Driver.FindElement(NavbarCollapse);
            var classAttribute = navbarElement.GetAttribute("class") ?? "";
            
            Logger.LogDebug("Navbar collapse classes: {Classes}", classAttribute);
            
            var isExpanded = classAttribute.Contains("show");
            var isCollapsing = classAttribute.Contains("collapsing");
            
            if (isCollapsing)
            {
                Logger.LogDebug("Navbar is in collapsing state, waiting for animation to complete");
                
                WaitHelper.WaitForCondition(Driver, d => 
                {
                    try 
                    {
                        var element = d.FindElement(NavbarCollapse);
                        var classes = element.GetAttribute("class") ?? "";
                        return !classes.Contains("collapsing");
                    }
                    catch 
                    {
                        return true;
                    }
                }, TimeSpan.FromSeconds(2));
                
                classAttribute = navbarElement.GetAttribute("class") ?? "";
                isExpanded = classAttribute.Contains("show");
            }
            
            Logger.LogDebug("Navbar collapsed state: {IsCollapsed} (classes: {Classes})", !isExpanded, classAttribute);
            return !isExpanded;
        }
        catch (Exception ex)
        {
            Logger.LogWarning(ex, "Error checking navbar collapsed state, assuming collapsed");
            return true;
        }
    }

    public bool IsBrandLinkVisible(TimeSpan? timeout = null)
    {
        return ElementHelper.IsElementVisible(Driver, BrandLink, timeout);
    }

    public bool IsRoomsLinkVisible(TimeSpan? timeout = null)
    {
        return ElementHelper.IsElementVisible(Driver, RoomsLink, timeout);
    }

    public bool IsBookingLinkVisible(TimeSpan? timeout = null)
    {
        return ElementHelper.IsElementVisible(Driver, BookingLink, timeout);
    }

    public bool IsAmenitiesLinkVisible(TimeSpan? timeout = null)
    {
        return ElementHelper.IsElementVisible(Driver, AmenitiesLink, timeout);
    }

    public bool IsLocationLinkVisible(TimeSpan? timeout = null)
    {
        return ElementHelper.IsElementVisible(Driver, LocationLink, timeout);
    }

    public bool IsContactLinkVisible(TimeSpan? timeout = null)
    {
        return ElementHelper.IsElementVisible(Driver, ContactLink, timeout);
    }

    public bool IsAdminLinkVisible(TimeSpan? timeout = null)
    {
        return ElementHelper.IsElementVisible(Driver, AdminLink, timeout);
    }

    public bool IsNavbarTogglerVisible(TimeSpan? timeout = null)
    {
        return ElementHelper.IsElementVisible(Driver, NavbarToggler, timeout);
    }

    public bool IsRoomsSectionVisible(TimeSpan? timeout = null)
    {
        return ElementHelper.IsElementVisible(Driver, RoomsSection, timeout);
    }

    public bool IsBookingSectionVisible(TimeSpan? timeout = null)
    {
        return ElementHelper.IsElementVisible(Driver, BookingSection, timeout);
    }

    public bool IsAmenitiesSectionVisible(TimeSpan? timeout = null)
    {
        return ElementHelper.IsElementVisible(Driver, AmenitiesSection, timeout);
    }

    public bool IsLocationSectionVisible(TimeSpan? timeout = null)
    {
        return ElementHelper.IsElementVisible(Driver, LocationSection, timeout);
    }

    public bool IsContactSectionVisible(TimeSpan? timeout = null)
    {
        return ElementHelper.IsElementVisible(Driver, ContactSection, timeout);
    }

    public IWebElement GetBrandLinkElement()
    {
        return WaitHelper.WaitForElement(Driver, BrandLink);
    }

    public IWebElement GetRoomsLinkElement()
    {
        return WaitHelper.WaitForElement(Driver, RoomsLink);
    }

    public IWebElement GetBookingLinkElement()
    {
        return WaitHelper.WaitForElement(Driver, BookingLink);
    }

    public IWebElement GetAmenitiesLinkElement()
    {
        return WaitHelper.WaitForElement(Driver, AmenitiesLink);
    }

    public IWebElement GetLocationLinkElement()
    {
        return WaitHelper.WaitForElement(Driver, LocationLink);
    }

    public IWebElement GetContactLinkElement()
    {
        return WaitHelper.WaitForElement(Driver, ContactLink);
    }

    public IWebElement GetAdminLinkElement()
    {
        return WaitHelper.WaitForElement(Driver, AdminLink);
    }

    public IWebElement GetNavbarElement()
    {
        return WaitHelper.WaitForElement(Driver, NavbarContainer);
    }

    public string GetBrandLinkText()
    {
        return ElementHelper.GetElementText(Driver, BrandLink);
    }

    public string GetRoomsLinkText()
    {
        return ElementHelper.GetElementText(Driver, RoomsLink);
    }

    public string GetBookingLinkText()
    {
        return ElementHelper.GetElementText(Driver, BookingLink);
    }

    public string GetAmenitiesLinkText()
    {
        return ElementHelper.GetElementText(Driver, AmenitiesLink);
    }

    public string GetLocationLinkText()
    {
        return ElementHelper.GetElementText(Driver, LocationLink);
    }

    public string GetContactLinkText()
    {
        return ElementHelper.GetElementText(Driver, ContactLink);
    }

    public string GetAdminLinkText()
    {
        return ElementHelper.GetElementText(Driver, AdminLink);
    }

    public bool IsOnHomePage()
    {
        var path = new Uri(Driver.Url).AbsolutePath;
        var onHomePage = path == "/";
        Logger.LogDebug("IsOnHomePage check: URL '{Url}', Path: '{Path}', result: {Result}", Driver.Url, path, onHomePage);
        return onHomePage;
    }
    
    public bool IsOnAdminPage()
    {
        var onAdminPage = new Uri(Driver.Url).AbsolutePath.StartsWith("/admin");
        Logger.LogDebug("IsOnAdminPage check: URL '{Url}', result: {Result}", Driver.Url, onAdminPage);
        return onAdminPage;
    }
} 