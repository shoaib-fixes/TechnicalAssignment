using System;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using TechnicalAssignment.Utilities;

namespace TechnicalAssignment.Pages;

public class AdminLoginPage : BasePage
{
    private static readonly By UsernameField = By.Id("username");
    private static readonly By PasswordField = By.Id("password");
    private static readonly By LoginButton = By.Id("doLogin");
    private static readonly By LoginForm = By.CssSelector(".card");
    private static readonly By MessagesLink = By.CssSelector("a[href='/admin/message']");

    public AdminLoginPage(IWebDriver driver) : base(driver)
    {
    }

    public override bool IsPageLoaded(TimeSpan? timeout = null)
    {
        Logger.LogDebug("Checking if admin login page is loaded");
        return ElementHelper.IsElementVisible(Driver, UsernameField, timeout) &&
               ElementHelper.IsElementVisible(Driver, PasswordField, timeout) &&
               ElementHelper.IsElementVisible(Driver, LoginButton, timeout);
    }

    public override void WaitForPageToLoad(TimeSpan? timeout = null)
    {
        Logger.LogDebug("Waiting for admin login page to load");
        WaitHelper.WaitForElement(Driver, UsernameField, timeout);
        WaitHelper.WaitForElement(Driver, PasswordField, timeout);
        WaitHelper.WaitForElement(Driver, LoginButton, timeout);
    }

    public AdminDashboardPage Login(string username, string password)
    {
        Logger.LogDebug("Logging into admin panel with username: {Username}", username);
        
        ElementHelper.SafeSendKeys(Driver, UsernameField, username);
        ElementHelper.SafeSendKeys(Driver, PasswordField, password);
        ElementHelper.SafeClick(Driver, LoginButton);
        
        WaitHelper.WaitForElement(Driver, MessagesLink, TimeSpan.FromSeconds(10));
        Logger.LogDebug("Login successful - admin dashboard loaded");
        
        return new AdminDashboardPage(Driver);
    }

    public bool IsOnLoginPage()
    {
        Logger.LogDebug("Checking if on admin login page");
        return IsPageLoaded(TimeSpan.FromSeconds(2));
    }

    public IWebElement GetLoginFormElement()
    {
        Logger.LogDebug("Getting login form element");
        return WaitHelper.WaitForElement(Driver, LoginForm);
    }
} 