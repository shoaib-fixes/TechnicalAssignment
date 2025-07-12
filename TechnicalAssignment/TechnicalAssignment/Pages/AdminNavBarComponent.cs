using System;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using TechnicalAssignment.Configuration;
using TechnicalAssignment.Utilities;

namespace TechnicalAssignment.Pages;

public class AdminNavBarComponent : BasePage
{
    private static readonly By MessagesLink = By.CssSelector("a[href='/admin/message']");
    private static readonly By RoomsLink = By.CssSelector("a[href='/admin/rooms']");
    private static readonly By ReportLink = By.Id("reportLink");
    private static readonly By BrandingLink = By.Id("brandingLink");
    private static readonly By FrontPageLink = By.Id("frontPageLink");
    private static readonly By LogoutButton = By.CssSelector(".btn-outline-danger");
    private readonly AdminLoginPage _adminLoginPage;
    private readonly TestConfiguration _testConfig;

    public AdminNavBarComponent(IWebDriver driver, ILogger<AdminNavBarComponent> logger, AdminLoginPage adminLoginPage, TestConfiguration testConfig) : base(driver, logger)
    {
        _adminLoginPage = adminLoginPage;
        _testConfig = testConfig;
    }

    public override bool IsPageLoaded(TimeSpan? timeout = null)
    {
        Logger.LogDebug("Checking if admin nav bar component is loaded");
        return ElementHelper.IsElementVisible(Driver, MessagesLink, timeout);
    }

    public override void WaitForPageToLoad(TimeSpan? timeout = null)
    {
        Logger.LogDebug("Waiting for admin nav bar component to load");
        WaitHelper.WaitForElement(Driver, MessagesLink, timeout);
    }

    public AdminMessagesPage NavigateToMessages()
    {
        Logger.LogDebug("Navigating to Messages section");
        ElementHelper.SafeClick(Driver, MessagesLink);
        var messagesPage = new AdminMessagesPage(Driver, LoggingHelper.CreateLogger<AdminMessagesPage>());
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
        if (!Driver.Url.Contains("/admin"))
        {
            Driver.Navigate().GoToUrl(adminUrl);
        }

        try
        {
            WaitHelper.WaitForElement(Driver, LogoutButton, TimeSpan.FromSeconds(5)); 
            Logger.LogDebug("Logout button found. Clicking it.");
            ElementHelper.SafeClick(Driver, LogoutButton);

            // After logout, we are redirected to the home page.
            // So we explicitly navigate to the admin login page.
            var adminLoginPageUrl = new Uri(new Uri(_testConfig.BaseUrl), "admin").ToString();
            Driver.Navigate().GoToUrl(adminLoginPageUrl);

            Logger.LogDebug("Waiting for login page to confirm logout.");
            _adminLoginPage.WaitForPageToLoad(TimeSpan.FromSeconds(10));
        }
        catch (Exception ex)
        {
            Logger.LogWarning(ex, "An error occurred during logout. Current URL: {Url}", Driver.Url);
            // We will try to navigate to the admin page anyway to leave a clean state.
            try
            {
                var adminLoginPageUrl = new Uri(new Uri(_testConfig.BaseUrl), "admin").ToString();
                Driver.Navigate().GoToUrl(adminLoginPageUrl);
                _adminLoginPage.WaitForPageToLoad(TimeSpan.FromSeconds(5));
            }
            catch(Exception navEx)
            {
                Logger.LogError(navEx, "Could not navigate to admin login page after a logout error.");
                throw; 
            }
        }
    }

    public bool IsLoggedIn()
    {
        Logger.LogDebug("Checking if logged into admin panel");
        return ElementHelper.IsElementVisible(Driver, MessagesLink, TimeSpan.FromSeconds(2));
    }
} 