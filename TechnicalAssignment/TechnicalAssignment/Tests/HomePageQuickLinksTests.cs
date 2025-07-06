using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using OpenQA.Selenium;
using TechnicalAssignment.Pages;
using TechnicalAssignment.Utilities;

namespace TechnicalAssignment.Tests;

[TestFixture]
[Category("QuickLinks")]
[Category("HomePage")]
[Parallelizable(ParallelScope.Fixtures)]
public class HomePageQuickLinksTests : BaseTest
{
    private HomePage _homePage = null!;

    // Data source for Quick Links tests
    private static readonly object[] _quickLinkCases =
    {
        // LinkText, HrefContains
        new object[] { "Home", "#" },
        new object[] { "Rooms", "#rooms" },
        new object[] { "Booking", "#booking" },
        new object[] { "Contact", "#contact" }
    };

    [SetUp]
    public void HomePageSetUp()
    {
        Logger.LogInformation("Starting HomePage Quick Links test setup");
        Driver.Navigate().GoToUrl(TestConfig.BaseUrl);
        Logger.LogDebug("Navigated to URL: {Url}", TestConfig.BaseUrl);
        
        _homePage = new HomePage(Driver);
        _homePage.WaitForPageToLoad();
        Logger.LogDebug("HomePage loaded successfully for Quick Links tests");
    }

    [Test]
    [Description("TC001: Verify Quick Links section is present and visible")]
    public void QuickLinksSection_WhenPageLoads_ShouldBeVisible()
    {
        Logger.LogInformation("Starting TC001: Quick Links section visibility test");
        
        Logger.LogDebug("Scrolling to Quick Links section");
        _homePage.QuickLinks.ScrollToQuickLinks();
        
        Logger.LogDebug("Checking Quick Links section visibility");
        Assert.That(_homePage.QuickLinks.IsQuickLinksSectionVisible(TimeSpan.FromSeconds(5)), Is.True, 
            "Quick Links section should be visible");
        
        Logger.LogDebug("Checking Quick Links header visibility");
        Assert.That(_homePage.QuickLinks.IsQuickLinksHeaderVisible(TimeSpan.FromSeconds(5)), Is.True, 
            "Quick Links header should be visible");
        
        Logger.LogInformation("TC001: Quick Links section visibility test passed successfully");
    }

    [Test]
    [Description("TC002: Verify Quick Links header text is correct")]
    public void QuickLinksHeader_WhenChecked_ShouldHaveCorrectText()
    {
        Logger.LogInformation("Starting TC002: Quick Links header text test");
        
        Logger.LogDebug("Scrolling to Quick Links section for header test");
        _homePage.QuickLinks.ScrollToQuickLinks();
        
        Logger.LogDebug("Getting Quick Links header text");
        var headerText = _homePage.QuickLinks.GetQuickLinksHeaderText();
        Logger.LogDebug("Header text retrieved: {HeaderText}", headerText);
        Assert.That(headerText, Is.EqualTo("Quick Links"), 
            "Quick Links header should display 'Quick Links'");
        
        Logger.LogInformation("TC002: Quick Links header text test passed successfully");
    }

    [Test]
    [TestCaseSource(nameof(_quickLinkCases))]
    [Description("TC003: Verify Quick Links have correct properties, styling and are clickable")]
    public void QuickLink_WhenChecked_ShouldHaveCorrectPropertiesAndBehavior(string linkText, string expectedHref)
    {
        Logger.LogInformation("Starting TC003: Verifying Quick Link: {LinkText}", linkText);
        
        Logger.LogDebug("Scrolling to Quick Links section for {LinkText} test", linkText);
        _homePage.QuickLinks.ScrollToQuickLinks();

        // 1. Check Visibility
        Logger.LogDebug("Checking {LinkText} link visibility", linkText);
        Assert.That(_homePage.QuickLinks.IsLinkVisible(linkText, TimeSpan.FromSeconds(5)), Is.True, 
            $"{linkText} link should be visible");

        // 2. Check Text
        Logger.LogDebug("Validating {LinkText} link text content", linkText);
        Assert.That(_homePage.QuickLinks.GetLinkText(linkText), Is.EqualTo(linkText), 
            $"{linkText} link text should be correct");

        // 3. Check Behavior/Attribute (better than CSS class)
        Logger.LogDebug("Validating {LinkText} link href attribute", linkText);
        Assert.That(_homePage.QuickLinks.GetLinkHref(linkText), Does.EndWith(expectedHref), 
            $"{linkText} link href should end with '{expectedHref}'");

        // 4. Check Clickability
        Logger.LogDebug("Checking {LinkText} link clickability", linkText);
        Assert.That(_homePage.QuickLinks.IsLinkEnabled(linkText), Is.True, 
            $"{linkText} link should be enabled");

        // 5. Check user-facing style, not class names
        Logger.LogDebug("Validating {LinkText} link text decoration style", linkText);
        var textDecoration = _homePage.QuickLinks.GetLinkCssValue(linkText, "text-decoration");
        Assert.That(textDecoration, Does.Contain("none"), 
            $"{linkText} link should not be underlined by default");

        Logger.LogInformation("TC003: Quick Link {LinkText} validation test passed successfully", linkText);
    }

    [Test]
    [Description("TC004: Verify Quick Links list structure has correct number of items")]
    public void QuickLinksListStructure_WhenChecked_ShouldHaveCorrectStructure()
    {
        Logger.LogInformation("Starting TC004: Quick Links list structure validation test");
        
        Logger.LogDebug("Scrolling to Quick Links section for list structure test");
        _homePage.QuickLinks.ScrollToQuickLinks();
        
        Logger.LogDebug("Validating Quick Links list structure");
        Assert.That(_homePage.QuickLinks.ValidateQuickLinksListStructure(), Is.True, 
            "Quick Links list structure should have correct number of items");
        
        Logger.LogDebug("Getting Quick Links list element and counting items");
        var listElement = _homePage.QuickLinks.GetQuickLinksListElement();
        var listItems = listElement.FindElements(By.TagName("li"));
        Logger.LogDebug("Found {ItemCount} list items", listItems.Count);
        Assert.That(listItems.Count, Is.EqualTo(4), 
            "Quick Links list should contain exactly 4 list items");
        
        Logger.LogInformation("TC004: Quick Links list structure validation test passed successfully");
    }

    [Test]
    [Description("TC005: Verify all 4 Quick Links are present and visible")]
    public void AllQuickLinks_ShouldBePresentVisibleAndInCorrectOrder()
    {
        Logger.LogInformation("Starting TC005: All Quick Links presence and order test");
        
        Logger.LogDebug("Scrolling to Quick Links section for all links test");
        _homePage.QuickLinks.ScrollToQuickLinks();
        
        Logger.LogDebug("Checking if all Quick Links are visible");
        Assert.That(_homePage.QuickLinks.AreAllQuickLinksVisible(TimeSpan.FromSeconds(5)), Is.True, 
            "All 4 Quick Links should be present and visible");
        
        Logger.LogDebug("Getting all Quick Links text for order validation");
        var allLinksText = _homePage.QuickLinks.GetAllQuickLinksText();
        var expectedLinks = new List<string> { "Home", "Rooms", "Booking", "Contact" };
        Logger.LogDebug("Found Quick Links: {ActualLinks}", string.Join(", ", allLinksText));
        Logger.LogDebug("Expected Quick Links: {ExpectedLinks}", string.Join(", ", expectedLinks));
        
        Logger.LogDebug("Validating Quick Links count");
        Assert.That(allLinksText.Count, Is.EqualTo(4), 
            "Should have exactly 4 Quick Links");
        
        Logger.LogDebug("Validating Quick Links order");
        Assert.That(allLinksText, Is.EqualTo(expectedLinks), 
            "Quick Links should be in correct order: Home, Rooms, Booking, Contact");
        
        Logger.LogInformation("TC005: All Quick Links presence and order test passed successfully");
    }

    [Test]
    [Description("TC006: Verify all Quick Links have correct text content")]
    public void QuickLinksTextContent_WhenChecked_ShouldBeCorrect()
    {
        Logger.LogInformation("Starting TC006: Quick Links text content validation test");
        
        Logger.LogDebug("Scrolling to Quick Links section for text content test");
        _homePage.QuickLinks.ScrollToQuickLinks();
        
        Logger.LogDebug("Validating all Quick Links text content");
        Assert.Multiple(() =>
        {
            Logger.LogDebug("Checking Home link text content");
            Assert.That(_homePage.QuickLinks.GetLinkText("Home"), Is.EqualTo("Home"), 
                "Home link should have text 'Home'");
            
            Logger.LogDebug("Checking Rooms link text content");
            Assert.That(_homePage.QuickLinks.GetLinkText("Rooms"), Is.EqualTo("Rooms"), 
                "Rooms link should have text 'Rooms'");
            
            Logger.LogDebug("Checking Booking link text content");
            Assert.That(_homePage.QuickLinks.GetLinkText("Booking"), Is.EqualTo("Booking"), 
                "Booking link should have text 'Booking'");
            
            Logger.LogDebug("Checking Contact link text content");
            Assert.That(_homePage.QuickLinks.GetLinkText("Contact"), Is.EqualTo("Contact"), 
                "Contact link should have text 'Contact'");
        });
        
        Logger.LogInformation("TC006: Quick Links text content validation test passed successfully");
    }

    [TestCaseSource(nameof(MobileViewports))]
    [Description("TC008: Verify Quick Links section displays correctly on mobile viewports")]
    public void QuickLinks_OnMobileViewport_ShouldDisplayCorrectly(int width, int height, string deviceName)
    {
        Logger.LogInformation("Starting TC008: Quick Links mobile viewport test on {DeviceName} ({Width}x{Height})", 
            deviceName, width, height);
        
        try
        {
            Logger.LogDebug("Setting viewport size to {Width}x{Height} for {DeviceName}", width, height, deviceName);
            BrowserHelper.SetViewportSize(Driver, TestConfig, width, height);
            
            Logger.LogDebug("Scrolling to Quick Links section on {DeviceName}", deviceName);
            _homePage.QuickLinks.ScrollToQuickLinks();
            
            Logger.LogDebug("Verifying Quick Links section visibility on {DeviceName}", deviceName);
            Assert.That(_homePage.QuickLinks.IsQuickLinksSectionVisible(TimeSpan.FromSeconds(5)), Is.True, 
                $"Quick Links section should be visible on {deviceName} ({width}x{height})");
            
            Logger.LogDebug("Verifying all Quick Links are accessible on {DeviceName}", deviceName);
            Assert.That(_homePage.QuickLinks.AreAllQuickLinksVisible(TimeSpan.FromSeconds(5)), Is.True, 
                $"All 4 Quick Links should be accessible on {deviceName}");
            
            Logger.LogDebug("Verifying Quick Links text is readable on {DeviceName}", deviceName);
            Assert.That(_homePage.QuickLinks.GetAllQuickLinksText().All(text => !string.IsNullOrEmpty(text)), Is.True, 
                $"All Quick Links text should be readable on {deviceName}");
            
            Logger.LogDebug("Simulating touch interaction on {DeviceName}", deviceName);
            _homePage.QuickLinks.ClickLink("Home");
            
            Logger.LogDebug("Verifying link functionality after touch on {DeviceName}", deviceName);
            Assert.That(_homePage.QuickLinks.IsLinkVisible("Home", TimeSpan.FromSeconds(2)), Is.True, 
                $"Quick Links should remain functional after touch interaction on {deviceName}");
            
            Logger.LogInformation("TC008: Quick Links mobile viewport test passed successfully on {DeviceName} ({Width}x{Height})", 
                deviceName, width, height);
        }
        finally
        {
            Logger.LogDebug("Resetting viewport size to default after {DeviceName} test", deviceName);
            BrowserHelper.ResetToDefaultSize(Driver, TestConfig);
        }
    }

    [TestCaseSource(nameof(TabletViewports))]
    [Description("TC009: Verify Quick Links section displays correctly on tablet viewports")]
    public void QuickLinks_OnTabletViewport_ShouldDisplayCorrectly(int width, int height, string deviceName)
    {
        Logger.LogInformation("Starting TC009: Quick Links tablet viewport test on {DeviceName} ({Width}x{Height})", 
            deviceName, width, height);

        try
        {
            Logger.LogDebug("Setting viewport size to {Width}x{Height} for {DeviceName}", width, height, deviceName);
            BrowserHelper.SetViewportSize(Driver, TestConfig, width, height);

            Logger.LogDebug("Scrolling to Quick Links section on {DeviceName}", deviceName);
            _homePage.QuickLinks.ScrollToQuickLinks();

            Logger.LogDebug("Verifying Quick Links section structure on {DeviceName}", deviceName);
            Assert.That(_homePage.QuickLinks.ValidateQuickLinksListStructure(), Is.True, 
                $"Quick Links list structure should be maintained on {deviceName}");
            
            Logger.LogDebug("Simulating interaction on {DeviceName}", deviceName);
            _homePage.QuickLinks.ClickLink("Home");
            
            Logger.LogDebug("Verifying link functionality after interaction on {DeviceName}", deviceName);
            Assert.That(_homePage.QuickLinks.IsLinkVisible("Home", TimeSpan.FromSeconds(2)), Is.True, 
                $"Quick Links should remain functional after interaction on {deviceName}");
            
            Logger.LogInformation("TC009: Quick Links tablet viewport test passed successfully on {DeviceName} ({Width}x{Height})", 
                deviceName, width, height);
        }
        finally
        {
            Logger.LogDebug("Resetting viewport size to default after {DeviceName} test", deviceName);
            BrowserHelper.ResetToDefaultSize(Driver, TestConfig);
        }
    }

    [TestCaseSource(nameof(DesktopViewports))]
    [Description("TC010: Verify Quick Links section displays correctly on desktop viewports")]
    public void QuickLinks_OnDesktopViewport_ShouldDisplayCorrectly(int width, int height, string deviceName)
    {
        Logger.LogInformation("Starting TC010: Quick Links desktop viewport test on {DeviceName} ({Width}x{Height})", 
            deviceName, width, height);
        
        try
        {
            Logger.LogDebug("Setting viewport size to {Width}x{Height} for {DeviceName}", width, height, deviceName);
            BrowserHelper.SetViewportSize(Driver, TestConfig, width, height);
            
            Logger.LogDebug("Scrolling to Quick Links section on {DeviceName}", deviceName);
            _homePage.QuickLinks.ScrollToQuickLinks();

            Logger.LogDebug("Verifying Quick Links section layout on {DeviceName}", deviceName);
            Assert.That(_homePage.QuickLinks.ValidateQuickLinksListStructure(), Is.True, 
                $"Quick Links list structure should be optimal on {deviceName}");

            Logger.LogDebug("Simulating interaction on {DeviceName}", deviceName);
            _homePage.QuickLinks.ClickLink("Home");

            Logger.LogDebug("Verifying link functionality after interaction on {DeviceName}", deviceName);
            Assert.That(_homePage.QuickLinks.IsLinkVisible("Home", TimeSpan.FromSeconds(2)), Is.True,
                $"Quick Links should remain functional after interaction on {deviceName}");

            Logger.LogInformation("TC010: Quick Links desktop viewport test passed successfully on {DeviceName} ({Width}x{Height})", 
                deviceName, width, height);
        }
        finally
        {
            Logger.LogDebug("Resetting viewport size to default after {DeviceName} test", deviceName);
            BrowserHelper.ResetToDefaultSize(Driver, TestConfig);
        }
    }
} 