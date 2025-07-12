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

namespace TechnicalAssignment.Drivers;

public class WebDriverFactory
{
    private readonly TestConfiguration _config;
    private readonly ILogger<WebDriverFactory> _logger;

    public WebDriverFactory(TestConfiguration config, ILogger<WebDriverFactory> logger)
    {
        _config = config;
        _logger = logger;
    }

    public IWebDriver CreateDriver(BrowserType browserType)
    {
        _logger.LogInformation("Creating {Browser} driver with configuration settings", browserType.ToStringValue());
        
        IWebDriver driver;

        switch (browserType)
        {
            case BrowserType.Chrome:
                new DriverManager().SetUpDriver(new ChromeConfig());
                var chromeOptions = CreateChromeOptions();
                driver = new ChromeDriver(chromeOptions);
                break;
            case BrowserType.Firefox:
                new DriverManager().SetUpDriver(new FirefoxConfig());
                var firefoxOptions = CreateFirefoxOptions();
                driver = new FirefoxDriver(firefoxOptions);
                break;
            case BrowserType.Edge:
                new DriverManager().SetUpDriver(new EdgeConfig());
                var edgeOptions = CreateEdgeOptions();
                driver = new EdgeDriver(edgeOptions);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(browserType), browserType, "Unsupported browser type");
        }

        ApplyDriverConfiguration(driver);
        
        _logger.LogInformation("{Browser} driver created successfully", browserType.ToStringValue());
        return driver;
    }

    private ChromeOptions CreateChromeOptions()
    {
        var options = new ChromeOptions();
        
        if (_config.Browser.Headless)
        {
            _logger.LogDebug("Enabling headless mode for Chrome");
            options.AddArgument("--headless");
        }

        // Use dynamic port assignment for parallel test execution
        // Port 0 tells Chrome to find an available port automatically
        options.AddArgument("--remote-debugging-port=0");

        options.AddArguments(
            "--no-sandbox",
            "--disable-dev-shm-usage",
            "--disable-gpu",
            "--disable-extensions",
            "--disable-infobars",
            "--disable-notifications"
        );
        
        _logger.LogDebug("Chrome options configured: Headless={Headless}", _config.Browser.Headless);
        return options;
    }

    private FirefoxOptions CreateFirefoxOptions()
    {
        var options = new FirefoxOptions();
        
        if (_config.Browser.Headless)
        {
            _logger.LogDebug("Enabling headless mode for Firefox");
            options.AddArgument("--headless");
        }
        
        _logger.LogDebug("Firefox options configured: Headless={Headless}", _config.Browser.Headless);
        return options;
    }

    private EdgeOptions CreateEdgeOptions()
    {
        var options = new EdgeOptions();
        
        if (_config.Browser.Headless)
        {
            _logger.LogDebug("Enabling headless mode for Edge");
            options.AddArgument("--headless");
        }
        
        options.AddArguments(
            "--no-sandbox",
            "--disable-dev-shm-usage",
            "--disable-gpu",
            "--disable-extensions"
        );
        
        _logger.LogDebug("Edge options configured: Headless={Headless}", _config.Browser.Headless);
        return options;
    }

    private void ApplyDriverConfiguration(IWebDriver driver)
    {
        _logger.LogDebug("Applying driver configuration: WindowSize={Width}x{Height}, Headless={Headless}", 
            _config.Browser.WindowSize.Width, _config.Browser.WindowSize.Height, _config.Browser.Headless);
        
        driver.Manage().Timeouts().PageLoad = _config.Timeouts.DefaultTimeout;
        
        driver.Manage().Window.Size = new System.Drawing.Size(
            _config.Browser.WindowSize.Width, 
            _config.Browser.WindowSize.Height
        );
        _logger.LogDebug("Window size set to {Width}x{Height} (Headless: {Headless})", 
            _config.Browser.WindowSize.Width, _config.Browser.WindowSize.Height, _config.Browser.Headless);
    }
} 