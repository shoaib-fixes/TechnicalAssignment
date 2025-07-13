using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using OpenQA.Selenium;
using Selenium.Axe;
using TechnicalAssignment.Configuration;
using TechnicalAssignment.Pages;
using TechnicalAssignment.Utilities;

namespace TechnicalAssignment.Tests;

/// <summary>
/// Accessibility tests using axe-core to validate WCAG compliance
/// </summary>
[TestFixture]
[Category("AccessibilityTests")]
[Parallelizable(ParallelScope.Fixtures)]
public class AccessibilityTests : BaseTest
{
    private HomePage _homePage = null!;
    private AdminLoginPage _adminLoginPage = null!;
    private AdminRoomsPage _adminRoomsPage = null!;

    [SetUp]
    public void AccessibilityTestSetup()
    {
        _homePage = GetService<HomePage>();
        _adminLoginPage = GetService<AdminLoginPage>();
        _adminRoomsPage = GetService<AdminRoomsPage>();
    }

    /// <summary>
    /// Tests accessibility compliance on the home page with different WCAG levels
    /// </summary>
    [Test(Description = "TC001: Verify home page accessibility compliance with different WCAG levels")]
    [TestCaseSource(nameof(HomePageWcagLevels))]
    public void HomePage_Accessibility_WithDifferentWCAGLevels(string[] wcagTags)
    {
        RunAccessibilityTest("HomePage", wcagTags, () =>
        {
            Driver.Navigate().GoToUrl(TestConfig.BaseUrl);
            _homePage.WaitForPageToLoad();
        });
    }

    /// <summary>
    /// Tests accessibility compliance on the admin login page
    /// </summary>
    [Test(Description = "TC002: Verify admin login page accessibility compliance with different WCAG levels")]
    [TestCaseSource(nameof(AdminLoginPageWcagLevels))]
    public void AdminLoginPage_Accessibility_WithDifferentWCAGLevels(string[] wcagTags)
    {
        RunAccessibilityTest("AdminLoginPage", wcagTags, () =>
        {
            var adminLoginUrl = new Uri(new Uri(TestConfig.BaseUrl), "admin").ToString();
            Driver.Navigate().GoToUrl(adminLoginUrl);
            _adminLoginPage.WaitForPageToLoad();
        });
    }

    /// <summary>
    /// Tests accessibility compliance on the admin rooms page after login
    /// </summary>
    [Test(Description = "TC003: Verify admin rooms page accessibility compliance with different WCAG levels")]
    [TestCaseSource(nameof(AdminRoomsPageWcagLevels))]
    public void AdminRoomsPage_Accessibility_WithDifferentWCAGLevels(string[] wcagTags)
    {
        RunAccessibilityTest("AdminRoomsPage", wcagTags, () =>
        {
            var adminLoginUrl = new Uri(new Uri(TestConfig.BaseUrl), "admin").ToString();
            Driver.Navigate().GoToUrl(adminLoginUrl);
            _adminLoginPage.WaitForPageToLoad();

            Logger.LogInformation("Logging in as admin user");
            _adminLoginPage.Login(TestConfig.AdminCredentials.Username, TestConfig.AdminCredentials.Password);

            _adminRoomsPage.WaitForPageToLoad();
            Logger.LogInformation("Admin rooms page loaded successfully");
        });
    }

    private void RunAccessibilityTest(string pageName, string[] wcagTags, Action setupAction)
    {
        if (!TestConfig.Accessibility.EnableAccessibilityTesting)
        {
            Assert.Ignore("Accessibility testing is disabled in configuration");
            return;
        }
        
        var configuredTags = TestConfig.Accessibility.AccessibilityTags;
        if (!wcagTags.Any(tag => configuredTags.Contains(tag, StringComparer.OrdinalIgnoreCase)))
        {
            Assert.Ignore($"WCAG level '{string.Join(", ", wcagTags)}' is not enabled in appsettings.json. Skipping test.");
            return;
        }

        var wcagLevel = string.Join(", ", wcagTags);
        Logger.LogInformation("Starting accessibility test for {PageName} with WCAG level: {WCAGLevel}", pageName, wcagLevel);
        
        setupAction();

        var axeBuilder = new AxeBuilder(Driver)
            .WithTags(wcagTags);
        var results = axeBuilder.Analyze();

        ScreenshotHelper.LogAccessibilityViolations(results, TestContext.CurrentContext.Test.Name, Logger);
        ScreenshotHelper.CaptureAccessibilityViolationScreenshots(
            Driver, results, TestContext.CurrentContext.Test.Name, TestConfig.Accessibility, Logger);

        if (TestConfig.Accessibility.FailOnViolations)
        {
            if (results.Violations.Any())
            {
                var violationsSummary = string.Join(Environment.NewLine, results.Violations.Select(v =>
                    $"Violation: {v.Id} - {v.Description}{Environment.NewLine}" +
                    $"Impact: {v.Impact}{Environment.NewLine}" +
                    $"Tags: {string.Join(", ", v.Tags)}{Environment.NewLine}" +
                    $"Help: {v.HelpUrl}{Environment.NewLine}" +
                    $"Elements: {string.Join(", ", v.Nodes.SelectMany(n => n.Target.Select(t => t.ToString())))}"));
                
                Assert.Fail(
                    $"{pageName} should have no accessibility violations for {wcagLevel}. Found {results.Violations.Length} violations:{Environment.NewLine}{violationsSummary}");
            }
            else
            {
                Assert.Pass($"{pageName} has no accessibility violations for {wcagLevel}.");
            }
        }
        else
        {
            Logger.LogInformation("Found {ViolationCount} accessibility violations on {PageName} for {wcagLevel} (not failing test due to configuration)", 
                results.Violations.Count(), pageName, wcagLevel);
        }
    }

    private static TestCaseData[] CreateWcagTestCases(string pageName) =>
        new[]
        {
            new TestCaseData((object)new[] { "wcag2a" }).SetName($"{pageName}_Accessibility_WCAG2A"),
            new TestCaseData((object)new[] { "wcag2aa" }).SetName($"{pageName}_Accessibility_WCAG2AA"),
            new TestCaseData((object)new[] { "wcag21aa" }).SetName($"{pageName}_Accessibility_WCAG21AA"),
            new TestCaseData((object)new[] { "wcag2aaa" }).SetName($"{pageName}_Accessibility_WCAG2AAA")
        };

    private static readonly object[] HomePageWcagLevels = CreateWcagTestCases("HomePage");

    private static readonly object[] AdminLoginPageWcagLevels = CreateWcagTestCases("AdminLoginPage");

    private static readonly object[] AdminRoomsPageWcagLevels = CreateWcagTestCases("AdminRoomsPage");
} 