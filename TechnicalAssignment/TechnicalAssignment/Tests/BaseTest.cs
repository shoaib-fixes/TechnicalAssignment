using NUnit.Framework;
using OpenQA.Selenium;
using TechnicalAssignment.Drivers;
using TechnicalAssignment.Utilities;
using TechnicalAssignment.Configuration;
using TechnicalAssignment.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using System;
using NUnit.Framework.Interfaces;

namespace TechnicalAssignment.Tests;

[TestFixture]
public abstract class BaseTest
{
    private IServiceProvider _serviceProvider = null!;
    
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
            // You can customize logging here, e.g., add console, debug, or file logging
            builder.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
            builder.AddConsole();
        });

        services.AddSingleton(provider =>
        {
            var logger = provider.GetRequiredService<ILogger<ConfigurationManager>>();
            return new ConfigurationManager(logger);
        });
        
        services.AddSingleton(provider => provider.GetRequiredService<ConfigurationManager>().TestConfig);
        
        services.AddTransient<WebDriverFactory>();

        // For each test, a new driver will be created. The scope is managed by NUnit.
        services.AddTransient<IWebDriver>(provider =>
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
        Driver = _serviceProvider.GetRequiredService<IWebDriver>();
        TestConfig = _serviceProvider.GetRequiredService<TestConfiguration>();
        Logger = _serviceProvider.GetRequiredService<ILoggerFactory>().CreateLogger(GetType().Name);
        Logger.LogInformation("Starting test: {TestName}", TestContext.CurrentContext.Test.Name);
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
            Logger.LogInformation("Driver quit and disposed.");
        }
    }
} 