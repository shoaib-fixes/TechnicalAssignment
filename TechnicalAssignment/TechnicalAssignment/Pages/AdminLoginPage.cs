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

    public AdminLoginPage(IWebDriver driver, ILogger<AdminLoginPage> logger) : base(driver, logger)
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

    public void Login(string username, string password)
    {
        ElementHelper.SafeSendKeys(Driver, UsernameField, username);
        ElementHelper.SafeSendKeys(Driver, PasswordField, password);

        Logger.LogDebug("Attempting to login with username: {Username}", username);
        ElementHelper.SafeClick(Driver, LoginButton);

        Logger.LogDebug("Login submitted. Waiting for dashboard to load.");
    }

    public bool IsOnLoginPage(TimeSpan? timeout = null)
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