using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using TechnicalAssignment.Utilities;

namespace TechnicalAssignment.Configuration;

/// <summary>
/// Configuration manager for test settings with support for multiple environments
/// and environment variable overrides
/// </summary>
public sealed class ConfigurationManager
{
    private readonly IConfiguration _configuration;
    private readonly TestConfiguration _testConfiguration;
    private readonly ILogger _logger;

    public ConfigurationManager(ILogger<ConfigurationManager> logger)
    {
        _logger = logger;
        _configuration = BuildConfiguration();
        _testConfiguration = LoadTestConfiguration();
        ValidateConfiguration();
    }

    /// <summary>
    /// Gets the test configuration settings
    /// </summary>
    public TestConfiguration TestConfig => _testConfiguration;

    /// <summary>
    /// Gets the base URL for the application under test
    /// </summary>
    public string BaseUrl => _testConfiguration.BaseUrl;

    /// <summary>
    /// Gets the admin credentials
    /// </summary>
    public AdminCredentials AdminCredentials => _testConfiguration.AdminCredentials;

    /// <summary>
    /// Gets the timeout settings
    /// </summary>
    public TimeoutSettings Timeouts => _testConfiguration.Timeouts;

    /// <summary>
    /// Gets the browser settings
    /// </summary>
    public BrowserSettings Browser => _testConfiguration.Browser;

    private IConfiguration BuildConfiguration()
    {
        _logger.LogDebug("Building configuration from multiple sources");

        var builder = new ConfigurationBuilder()
            .SetBasePath(GetConfigurationBasePath())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
            .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: false)
            .AddEnvironmentVariables(prefix: "TEST_");

        var configuration = builder.Build();
        _logger.LogDebug("Configuration built successfully");
        return configuration;
    }

    private string GetConfigurationBasePath()
    {
        var currentDirectory = Directory.GetCurrentDirectory();
        if (File.Exists(Path.Combine(currentDirectory, "appsettings.json")))
        {
            _logger.LogDebug("Using current directory for configuration: {Path}", currentDirectory);
            return currentDirectory;
        }

        var executableDirectory = AppDomain.CurrentDomain.BaseDirectory;
        if (File.Exists(Path.Combine(executableDirectory, "appsettings.json")))
        {
            _logger.LogDebug("Using executable directory for configuration: {Path}", executableDirectory);
            return executableDirectory;
        }

        _logger.LogWarning("Configuration file not found in expected locations, using current directory: {Path}", currentDirectory);
        return currentDirectory;
    }

    private TestConfiguration LoadTestConfiguration()
    {
        _logger.LogDebug("Loading test configuration from section: {SectionName}", TestConfiguration.SectionName);

        var testConfig = new TestConfiguration();
        var section = _configuration.GetSection(TestConfiguration.SectionName);
        
        if (!section.Exists())
        {
            throw new InvalidOperationException($"Configuration section '{TestConfiguration.SectionName}' not found");
        }

        section.Bind(testConfig);
        _logger.LogDebug("Test configuration loaded successfully");
        return testConfig;
    }

    private void ValidateConfiguration()
    {
        _logger.LogDebug("Validating configuration");

        var context = new ValidationContext(_testConfiguration);
        var results = new List<ValidationResult>();

        if (!Validator.TryValidateObject(_testConfiguration, context, results, true))
        {
            var errors = string.Join(", ", results.Select(r => r.ErrorMessage));
            throw new InvalidOperationException($"Configuration validation failed: {errors}");
        }

        if (!Uri.TryCreate(_testConfiguration.BaseUrl, UriKind.Absolute, out _))
        {
            throw new InvalidOperationException($"Invalid BaseUrl: {_testConfiguration.BaseUrl}");
        }

        if (string.IsNullOrWhiteSpace(_testConfiguration.AdminCredentials.Username))
        {
            throw new InvalidOperationException("Admin username cannot be empty");
        }

        if (string.IsNullOrWhiteSpace(_testConfiguration.AdminCredentials.Password))
        {
            throw new InvalidOperationException("Admin password cannot be empty");
        }

        _logger.LogInformation("Configuration validation passed successfully");
        _logger.LogDebug("Base URL: {BaseUrl}", _testConfiguration.BaseUrl);
        _logger.LogDebug("Admin Username: {Username}", _testConfiguration.AdminCredentials.Username);
        _logger.LogDebug("Default Timeout: {Timeout}s", _testConfiguration.Timeouts.DefaultTimeoutSeconds);
        _logger.LogDebug("Default Browser: {Browser}", _testConfiguration.Browser.DefaultBrowser);
    }

    /// <summary>
    /// Gets a configuration value by key with optional default value
    /// </summary>
    public T GetValue<T>(string key, T defaultValue = default!)
    {
        return _configuration.GetValue<T>(key, defaultValue)!;
    }

    /// <summary>
    /// Gets a configuration section
    /// </summary>
    public IConfigurationSection GetSection(string key)
    {
        return _configuration.GetSection(key);
    }

    /// <summary>
    /// Checks if a configuration section exists
    /// </summary>
    public bool SectionExists(string key)
    {
        return _configuration.GetSection(key).Exists();
    }
} 