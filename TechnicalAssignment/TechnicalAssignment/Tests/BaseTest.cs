using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using OpenQA.Selenium;
using TechnicalAssignment.Drivers;
using TechnicalAssignment.Utilities;
using TechnicalAssignment.Configuration;
using TechnicalAssignment.Pages;
using TechnicalAssignment.Models;
using Microsoft.Extensions.Logging;

namespace TechnicalAssignment.Tests;

[TestFixture]
public abstract class BaseTest
{
    private IWebDriver? _driver;
    
    protected IWebDriver Driver 
    { 
        get
        {
            if (_driver == null)
                throw new InvalidOperationException("Driver is not initialized. It should be initialized in a SetUp method.");
            return _driver;
        }
        private set => _driver = value;
    }
    
    protected string CurrentBrowser { get; private set; } = string.Empty;
    protected ILogger Logger { get; private set; }
    protected ConfigurationManager Config { get; private set; }

    public static BrowserType[] BrowserTypeTestCases => new[] { BrowserType.Chrome, BrowserType.Firefox, BrowserType.Edge };
    
    public static object[] MobileViewports => ViewportTestData.GetMobileViewportTestCases();
    public static object[] TabletViewports => ViewportTestData.GetTabletViewportTestCases();
    public static object[] DesktopViewports => ViewportTestData.GetDesktopViewportTestCases();

    /// <summary>
    /// Gets all viewport sizes for comprehensive responsive testing
    /// </summary>
    protected static ViewportSize[] GetAllViewportSizes()
    {
        return ViewportTestData.GetAllViewportSizes();
    }

    /// <summary>
    /// Gets mobile viewport sizes only
    /// </summary>
    protected static ViewportSize[] GetMobileViewportSizes()
    {
        return ViewportTestData.GetMobileViewportSizes();
    }

    /// <summary>
    /// Gets tablet viewport sizes only
    /// </summary>
    protected static ViewportSize[] GetTabletViewportSizes()
    {
        return ViewportTestData.GetTabletViewportSizes();
    }

    /// <summary>
    /// Gets desktop viewport sizes only
    /// </summary>
    protected static ViewportSize[] GetDesktopViewportSizes()
    {
        return ViewportTestData.GetDesktopViewportSizes();
    }

    protected BaseTest()
    {
        Logger = LoggingHelper.CreateLogger(GetType().Name);
        Config = ConfigurationManager.Instance;
        Logger.LogInformation("Test instance {FixtureName} initialized", GetType().Name);
    }

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        CurrentBrowser = GetTargetBrowser();
        Logger.LogInformation("Starting test setup for browser: {Browser}", CurrentBrowser);

        try
        {
            Driver = WebDriverFactory.CreateDriver(CurrentBrowser);
            
            Logger.LogInformation("WebDriver initialized successfully for {Browser} with window size: {Width}x{Height}", 
                CurrentBrowser, Driver.Manage().Window.Size.Width, Driver.Manage().Window.Size.Height);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to initialize WebDriver for {Browser}", CurrentBrowser);
            throw;
        }
    }

    [TearDown]
    public void TearDown()
    {
        try
        {
            var testStatus = TestContext.CurrentContext.Result.Outcome.Status;
            
            if (testStatus == TestStatus.Failed && _driver != null)
            {
                var testName = TestContext.CurrentContext.Test.Name;
                var errorMessage = TestContext.CurrentContext.Result.Message;
                Logger.LogInformation("Test failed: {TestName}", testName);
                Logger.LogInformation("Error message: {ErrorMessage}", errorMessage);
                
                Logger.LogInformation("Capturing screenshot for failed test");
                try 
                {
                    ScreenshotHelper.CaptureScreenshot(_driver, testName);
                }
                catch (Exception ex)
                {
                    Logger.LogWarning(ex, "Screenshot capture failed");
                }
            }
            
            if (_driver != null)
            {
                try
                {
                    ResetBrowserState();
                }
                catch (Exception ex)
                {
                    Logger.LogWarning(ex, "Failed to reset browser state after test");
                }
            }
        }
        catch (Exception ex)
        {
            Logger.LogWarning(ex, "Error in test teardown for {Browser}", CurrentBrowser);
        }
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        Logger.LogInformation("Starting test fixture teardown for browser: {Browser}", CurrentBrowser);
        
        try
        {
            CleanupDriver();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error in driver cleanup for {Browser}", CurrentBrowser);
            throw;
        }
    }

    private void CleanupDriver()
    {
        if (_driver != null)
        {
            try
            {
                _driver.Quit();
                _driver.Dispose();
                Logger.LogInformation("WebDriver disposed successfully for {Browser}", CurrentBrowser);
            }
            catch (Exception ex)
            {
                Logger.LogWarning(ex, "Exception during driver cleanup for {Browser}", CurrentBrowser);
            }
            finally
            {
                _driver = null;
            }
        }
    }

    /// <summary>
    /// Resets browser state to ensure test isolation
    /// This prevents browser state leakage between tests while maintaining performance
    /// </summary>
    private void ResetBrowserState()
    {
        if (_driver == null) return;
        
        Logger.LogDebug("Resetting browser state for test isolation");
        
        try
        {
            _driver.Manage().Cookies.DeleteAllCookies();
            
            var jsExecutor = (IJavaScriptExecutor)_driver;
            jsExecutor.ExecuteScript("if (window.sessionStorage) { window.sessionStorage.clear(); }");
            jsExecutor.ExecuteScript("if (window.localStorage) { window.localStorage.clear(); }");
            
            BrowserHelper.ResetToDefaultSize(_driver);
            
            var windowHandles = _driver.WindowHandles;
            if (windowHandles.Count > 1)
            {
                foreach (var handle in windowHandles.Skip(1))
                {
                    _driver.SwitchTo().Window(handle);
                    _driver.Close();
                }
                _driver.SwitchTo().Window(windowHandles[0]);
            }
            
            Logger.LogDebug("Browser state reset completed");
        }
        catch (Exception ex)
        {
            Logger.LogWarning(ex, "Partial failure in browser state reset");
        }
    }

    private string GetTargetBrowser()
    {
        var browserFromRunSettings = TestContext.Parameters.Get("Browser");
        if (!string.IsNullOrEmpty(browserFromRunSettings))
        {
            return browserFromRunSettings;
        }

        return Config?.Browser?.DefaultBrowser ?? BrowserType.Chrome.ToStringValue();
    }
} 