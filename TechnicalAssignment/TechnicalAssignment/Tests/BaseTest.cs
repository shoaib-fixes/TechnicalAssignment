using NUnit.Framework;
using OpenQA.Selenium;
using TechnicalAssignment.Drivers;
using TechnicalAssignment.Utilities;
using TechnicalAssignment.Configuration;
using TechnicalAssignment.Models;
using TechnicalAssignment.Pages;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using System;
using NUnit.Framework.Interfaces;

namespace TechnicalAssignment.Tests;

[TestFixture]
public abstract class BaseTest
{
    private IServiceProvider _serviceProvider = null!;
    private IServiceScope _scope = null!;
    
    protected IWebDriver Driver { get; private set; } = null!;
    protected TestConfiguration TestConfig { get; private set; } = null!;
    protected ILogger Logger { get; private set; } = null!;
    protected bool ScreenshotCaptured { get; set; } = false;

    public static BrowserType[] BrowserTypeTestCases => new[] { BrowserType.Chrome, BrowserType.Firefox, BrowserType.Edge };

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        var services = new ServiceCollection();

        services.AddLogging(builder =>
        {
            builder.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
            builder.AddConsole();
        });

        services.AddSingleton<ConfigurationManager>();
        
        services.AddSingleton(provider => provider.GetRequiredService<ConfigurationManager>().TestConfig);
        
        services.AddTransient<WebDriverFactory>();

        // Register page components
        services.AddTransient<HomePageMainNavigationComponent>();
        services.AddTransient<HomePageNavigationComponent>();
        services.AddTransient<HomePageQuickLinksComponent>();
        services.AddTransient<HomePageContactComponent>();
        services.AddTransient<HomePageBookingFormComponent>();
        services.AddTransient<HomePageBookingRoomListComponent>();
        services.AddTransient<HomePageBookingDateValidationComponent>();
        
        // Register reservation page components
        services.AddTransient<ReservationPagePriceSummaryComponent>();
        services.AddTransient<ReservationPageGuestFormComponent>();
        services.AddTransient<ReservationPageCalendarComponent>();

        // Register admin page components
        services.AddTransient<AdminNavBarComponent>();
        
        // Register pages
        services.AddTransient<HomePage>();
        services.AddTransient<ReservationPage>();
        services.AddTransient<AdminLoginPage>();
        services.AddTransient<AdminRoomPage>();
        services.AddTransient<AdminRoomsPage>();
        services.AddTransient<AdminMessagesPage>();

        // For each test, a new driver will be created. The scope is managed by NUnit.
        services.AddScoped<IWebDriver>(provider =>
        {
            var factory = provider.GetRequiredService<WebDriverFactory>();
            var config = provider.GetRequiredService<TestConfiguration>();
            var browserFromRunSettings = TestContext.Parameters.Get("Browser");
            var browserType = !string.IsNullOrEmpty(browserFromRunSettings)
                ? BrowserTypeExtensions.ToBrowserType(browserFromRunSettings)
                : config.Browser.DefaultBrowserType;
            return factory.CreateDriver(browserType);
        });

        _serviceProvider = services.BuildServiceProvider();
        // Force early configuration validation
        _serviceProvider.GetRequiredService<ConfigurationManager>();
        
        ExtentReportHelper.Instance.ToString();
    }

    [SetUp]
    public void SetUp()
    {
        _scope = _serviceProvider.CreateScope();
        Driver = _scope.ServiceProvider.GetRequiredService<IWebDriver>();
        TestConfig = _scope.ServiceProvider.GetRequiredService<TestConfiguration>();
        Logger = _scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger(GetType().Name);
        
        var testName = TestContext.CurrentContext.Test.Name;
        var testDescription = TestContext.CurrentContext.Test.Properties.Get("Description") as string;
        ExtentReportHelper.CreateTest(testName, testDescription);
        
        Logger.LogInformation("Starting test: {TestName}", TestContext.CurrentContext.Test.Name);
    }

    protected T GetService<T>() where T : notnull
    {
        return _scope.ServiceProvider.GetRequiredService<T>();
    }

    [TearDown]
    public void TearDown()
    {
        try
        {
            var testStatus = TestContext.CurrentContext.Result.Outcome.Status;
            var stackTrace = TestContext.CurrentContext.Result.StackTrace;
            var errorMessage = TestContext.CurrentContext.Result.Message;
            var failureCount = TestContext.CurrentContext.Result.FailCount;

            if (testStatus == TestStatus.Failed || failureCount > 0)
            {
                var finalMessage = string.IsNullOrEmpty(errorMessage) ? "Test failed with one or more assertions." : errorMessage;
                ExtentReportHelper.LogToReport(Microsoft.Extensions.Logging.LogLevel.Error, $"Test failed: {finalMessage}", new Exception(stackTrace));
                if (Driver != null && !ScreenshotCaptured)
                {
                    var testName = TestContext.CurrentContext.Test.Name;
                    Logger.LogInformation("Test failed: {TestName}. Capturing screenshot.", testName);
                    ScreenshotHelper.CaptureScreenshot(Driver, testName, Logger);
                    ScreenshotCaptured = true;
                }
            }
            else if (testStatus == TestStatus.Skipped)
            {
                ExtentReportHelper.LogToReport(Microsoft.Extensions.Logging.LogLevel.Warning, "Test skipped");
            }
            else
            {
                ExtentReportHelper.LogToReport(Microsoft.Extensions.Logging.LogLevel.Information, "Test passed");
            }
        }
        catch (Exception ex)
        {
            Logger.LogWarning(ex, "Error during teardown.");
            ExtentReportHelper.LogToReport(Microsoft.Extensions.Logging.LogLevel.Error, "Error in TearDown.", ex);
        }
        finally
        {
            Driver?.Quit();
            _scope?.Dispose();
            Logger.LogInformation("Driver quit and scope disposed.");
        }
    }
    
    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        ExtentReportHelper.Instance.Flush();
    }
} 