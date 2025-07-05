using NUnit.Framework;
using Microsoft.Extensions.Logging;

namespace TechnicalAssignment.Tests;

[TestFixture, Order(1)]
[Category("SmokeTests")]
[Parallelizable(ParallelScope.Fixtures)]
public class SmokeTests : BaseTest
{

    [Test]
    [Order(1)]
    [Retry(3)]
    public void Framework_WithBasicAssertion_ShouldPass()
    {
        Logger.LogInformation("Starting basic framework validation test for browser: {Browser}", CurrentBrowser);
        
        var expected = "Smoke Test";
        var actual = "Smoke Test";

        Logger.LogDebug("Comparing expected: '{Expected}' with actual: '{Actual}'", expected, actual);
        
        Assert.That(actual, Is.EqualTo(expected), "Basic string comparison should pass to validate test framework.");
        
        Logger.LogInformation("Basic framework validation test passed successfully for browser: {Browser}", CurrentBrowser);
    }

    [Test]
    [Order(2)]
    [Retry(2)]
    public void RetryMechanism_WithIntentionalFailure_ShouldPassOnSecondAttempt()
    {
        var currentAttempt = TestContext.CurrentContext.CurrentRepeatCount + 1;
        Logger.LogInformation("Retry mechanism test attempt {AttemptCount} for browser: {Browser}", currentAttempt, CurrentBrowser);
        
        if (currentAttempt < 2)
        {
            Logger.LogInformation("Failing test intentionally on attempt {AttemptCount}", currentAttempt);
            Assert.Fail($"Intentionally failing on attempt {currentAttempt} to test retry mechanism");
        }
        
        Logger.LogInformation("Retry mechanism test passed successfully on attempt {AttemptCount} for browser: {Browser}", currentAttempt, CurrentBrowser);
    }

    [Test]
    [Order(3)]
    [Retry(3)]
    public void Website_WithValidUrl_ShouldBeReachable()
    {
        Logger.LogInformation("Starting website connectivity test for browser: {Browser}", CurrentBrowser);
        
        var url = Config.BaseUrl;
        Logger.LogInformation("Navigating to URL: {Url}", url);
        
        Driver.Navigate().GoToUrl(url);
        
        var title = Driver.Title;
        Logger.LogInformation("Page title retrieved: '{Title}' for browser: {Browser}", title, CurrentBrowser);
        
        Assert.That(title, Is.Not.Null.Or.Empty, $"Page title should not be empty when website is reachable in {CurrentBrowser}.");
        
        Logger.LogInformation("Website connectivity test passed successfully for browser: {Browser}", CurrentBrowser);
    }

    [Test]
    [Order(4)]
    [Retry(1)]
    public void ScreenshotHelper_WithManualCapture_ShouldSaveFile()
    {
        Logger.LogInformation("Starting manual screenshot capture test for browser: {Browser}", CurrentBrowser);
        
        var url = Config.BaseUrl;
        Logger.LogInformation("Navigating to URL: {Url}", url);
        
        Driver.Navigate().GoToUrl(url);
        
        Logger.LogInformation("Manually capturing screenshot to validate helper functionality");
        Utilities.ScreenshotHelper.CaptureScreenshot(Driver, "Manual_Screenshot_Test");
        
        var testResultsDir = Utilities.TestResultsHelper.GetTestResultsDirectory();
        var expectedLogFile = Utilities.TestResultsHelper.GetLogFilePath();
        
        Logger.LogInformation("Test results directory: {Directory}", testResultsDir);
        Logger.LogInformation("Expected log file: {LogFile}", expectedLogFile);
        
        Logger.LogInformation("Manual screenshot capture test completed successfully for browser: {Browser}", CurrentBrowser);
        Assert.Pass("Screenshot helper validation completed successfully");
    }

    [Test]
    [Order(5)]
    [Retry(1)]
    [Explicit("This test intentionally fails to verify screenshot capture - run explicitly when needed")]
    public void ScreenshotHelper_WithTestFailure_ShouldCaptureAutomatically()
    {
        Logger.LogInformation("Starting automatic screenshot capture test for browser: {Browser}", CurrentBrowser);
        
        var url = Config.BaseUrl;
        Logger.LogInformation("Navigating to URL: {Url}", url);
        
        Driver.Navigate().GoToUrl(url);
        
        Logger.LogInformation("Intentionally failing test to verify automatic screenshot capture");
        Assert.Fail("Intentionally failing test to verify automatic screenshot capture functionality");
    }
} 