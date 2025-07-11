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
    }

    [SetUp]
    public void SetUp()
    {
        _scope = _serviceProvider.CreateScope();
        Driver = _scope.ServiceProvider.GetRequiredService<IWebDriver>();
        TestConfig = _scope.ServiceProvider.GetRequiredService<TestConfiguration>();
        Logger = _scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger(GetType().Name);
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
            
            if (testStatus == TestStatus.Failed && Driver != null)
            {
                var testName = TestContext.CurrentContext.Test.Name;
                Logger.LogInformation("Test failed: {TestName}. Capturing screenshot.", testName);
                ScreenshotHelper.CaptureScreenshot(Driver, testName);
            }
        }
        catch (Exception ex)
        {
            Logger.LogWarning(ex, "Error during screenshot capture on failure.");
        }
        finally
        {
            Driver?.Quit();
            _scope?.Dispose();
            Logger.LogInformation("Driver quit and scope disposed.");
        }
    }
} 