using System;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using TechnicalAssignment.Utilities;

namespace TechnicalAssignment.Pages;

public class AdminDashboardPage : BasePage
{
    private static readonly By MessagesLink = By.CssSelector("a[href='/admin/message']");
    private static readonly By RoomsLink = By.CssSelector("a[href='/admin/rooms']");
    private static readonly By ReportLink = By.Id("reportLink");
    private static readonly By BrandingLink = By.Id("brandingLink");
    private static readonly By FrontPageLink = By.Id("frontPageLink");
    private static readonly By LogoutButton = By.CssSelector(".btn-outline-danger");

    public AdminDashboardPage(IWebDriver driver) : base(driver)
    {
    }

    public override bool IsPageLoaded(TimeSpan? timeout = null)
    {
        Logger.LogDebug("Checking if admin dashboard page is loaded");
        return ElementHelper.IsElementVisible(Driver, MessagesLink, timeout);
    }

    public override void WaitForPageToLoad(TimeSpan? timeout = null)
    {
        Logger.LogDebug("Waiting for admin dashboard page to load");
        WaitHelper.WaitForElement(Driver, MessagesLink, timeout);
    }

    public AdminMessagesPage NavigateToMessages()
    {
        Logger.LogDebug("Navigating to Messages section");
        ElementHelper.SafeClick(Driver, MessagesLink);
        var messagesPage = new AdminMessagesPage(Driver);
        messagesPage.WaitForPageToLoad(TimeSpan.FromSeconds(10));
        return messagesPage;
    }

    public void NavigateToRooms()
    {
        Logger.LogDebug("Navigating to Rooms section");
        ElementHelper.SafeClick(Driver, RoomsLink);
    }

    public void NavigateToReport()
    {
        Logger.LogDebug("Navigating to Report section");
        ElementHelper.SafeClick(Driver, ReportLink);
    }

    public void NavigateToBranding()
    {
        Logger.LogDebug("Navigating to Branding section");
        ElementHelper.SafeClick(Driver, BrandingLink);
    }

    public void NavigateToFrontPage()
    {
        Logger.LogDebug("Navigating to Front Page section");
        ElementHelper.SafeClick(Driver, FrontPageLink);
    }

    public void Logout()
    {
        var currentUrl = new Uri(Driver.Url);
        var adminUrl = $"{currentUrl.Scheme}://{currentUrl.Authority}/admin";

        Logger.LogInformation("Ensuring navigation to main admin page before logout from {currentUrl}", Driver.Url);
        Driver.Navigate().GoToUrl(adminUrl);

        try
        {
            WaitHelper.WaitForElement(Driver, LogoutButton, TimeSpan.FromSeconds(5)); 
            Logger.LogDebug("Logout button found. Clicking it.");
            ElementHelper.SafeClick(Driver, LogoutButton);
        }
        catch (WebDriverTimeoutException)
        {
            Logger.LogWarning("Logout button not found, assuming already logged out.");
        }
        Logger.LogDebug("Waiting for login page to confirm logout.");
        try
        {
            var loginPage = new AdminLoginPage(Driver);
            loginPage.WaitForPageToLoad(TimeSpan.FromSeconds(10));
        }
        catch (TimeoutException ex)
        {
            Logger.LogWarning(ex, "Login page not reached after logout; current URL: {Url}", Driver.Url);
        }
    }

    public bool IsLoggedIn()
    {
        Logger.LogDebug("Checking if logged into admin panel");
        return ElementHelper.IsElementVisible(Driver, MessagesLink, TimeSpan.FromSeconds(2));
    }
} 