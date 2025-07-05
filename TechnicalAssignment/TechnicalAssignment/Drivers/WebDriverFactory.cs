using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Edge;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;
using NUnit.Framework;
using TechnicalAssignment.Configuration;
using TechnicalAssignment.Models;
using Microsoft.Extensions.Logging;
using TechnicalAssignment.Utilities;

namespace TechnicalAssignment.Drivers;

public static class WebDriverFactory
{
    private static readonly ILogger Logger = LoggingHelper.CreateLogger("WebDriverFactory");

    public static IWebDriver CreateDriver()
    {
        var browserString = TestContext.Parameters.Get("Browser", "Chrome");
        var browserType = BrowserTypeExtensions.ToBrowserType(browserString);
        return CreateDriver(browserType);
    }

    public static IWebDriver CreateDriver(string browserName)
    {
        var browserType = BrowserTypeExtensions.ToBrowserType(browserName);
        return CreateDriver(browserType);
    }

    public static IWebDriver CreateDriver(BrowserType browserType)
    {
        var config = ConfigurationManager.Instance;
        
        Logger.LogInformation("Creating {Browser} driver with configuration settings", browserType.ToStringValue());
        
        IWebDriver driver;

        switch (browserType)
        {
            case BrowserType.Chrome:
                new DriverManager().SetUpDriver(new ChromeConfig());
                var chromeOptions = CreateChromeOptions(config);
                driver = new ChromeDriver(chromeOptions);
                break;
            case BrowserType.Firefox:
                new DriverManager().SetUpDriver(new FirefoxConfig());
                var firefoxOptions = CreateFirefoxOptions(config);
                driver = new FirefoxDriver(firefoxOptions);
                break;
            case BrowserType.Edge:
                new DriverManager().SetUpDriver(new EdgeConfig());
                var edgeOptions = CreateEdgeOptions(config);
                driver = new EdgeDriver(edgeOptions);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(browserType), browserType, "Unsupported browser type");
        }

        ApplyDriverConfiguration(driver, config);
        
        Logger.LogInformation("{Browser} driver created successfully", browserType.ToStringValue());
        return driver;
    }

    private static ChromeOptions CreateChromeOptions(ConfigurationManager config)
    {
        var options = new ChromeOptions();
        
        if (config.Browser.Headless)
        {
            Logger.LogDebug("Enabling headless mode for Chrome");
            options.AddArgument("--headless");
        }
        
        options.AddArguments(
            "--no-sandbox",
            "--disable-dev-shm-usage",
            "--disable-gpu",
            "--disable-extensions",
            "--disable-infobars",
            "--disable-notifications"
        );
        
        Logger.LogDebug("Chrome options configured: Headless={Headless}", config.Browser.Headless);
        return options;
    }

    private static FirefoxOptions CreateFirefoxOptions(ConfigurationManager config)
    {
        var options = new FirefoxOptions();
        
        if (config.Browser.Headless)
        {
            Logger.LogDebug("Enabling headless mode for Firefox");
            options.AddArgument("--headless");
        }
        
        Logger.LogDebug("Firefox options configured: Headless={Headless}", config.Browser.Headless);
        return options;
    }

    private static EdgeOptions CreateEdgeOptions(ConfigurationManager config)
    {
        var options = new EdgeOptions();
        
        if (config.Browser.Headless)
        {
            Logger.LogDebug("Enabling headless mode for Edge");
            options.AddArgument("--headless");
        }
        
        options.AddArguments(
            "--no-sandbox",
            "--disable-dev-shm-usage",
            "--disable-gpu",
            "--disable-extensions"
        );
        
        Logger.LogDebug("Edge options configured: Headless={Headless}", config.Browser.Headless);
        return options;
    }

    private static void ApplyDriverConfiguration(IWebDriver driver, ConfigurationManager config)
    {
        Logger.LogDebug("Applying driver configuration: WindowSize={Width}x{Height}, Headless={Headless}", 
            config.Browser.WindowSize.Width, config.Browser.WindowSize.Height, config.Browser.Headless);
        
        driver.Manage().Timeouts().PageLoad = config.Timeouts.DefaultTimeout;
        
        driver.Manage().Window.Size = new System.Drawing.Size(
            config.Browser.WindowSize.Width, 
            config.Browser.WindowSize.Height
        );
        Logger.LogDebug("Window size set to {Width}x{Height} (Headless: {Headless})", 
            config.Browser.WindowSize.Width, config.Browser.WindowSize.Height, config.Browser.Headless);
    }
} 