using NUnit.Framework;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using System;
using TechnicalAssignment.Pages;
using TechnicalAssignment.Utilities;

namespace TechnicalAssignment.Tests;

[TestFixture, Order(1)]
[Category("SmokeTests")]
[Parallelizable(ParallelScope.Fixtures)]
public class SmokeTests : BaseTest
{
    private HomePage _homePage = null!;

    [SetUp]
    public void PageSetup()
    {
        Logger.LogInformation("Navigating to base URL for smoke test.");
        Driver.Navigate().GoToUrl(TestConfig.BaseUrl);
        _homePage = new HomePage(Driver);
        _homePage.WaitForPageToLoad();
    }

    [Test(Description = "Verify that the home page loads without errors")]
    public void HomePage_ShouldLoadSuccessfully()
    {
        Logger.LogInformation("Verifying home page title.");
        Assert.That(Driver.Title, Does.Contain("Restful-booker-platform demo"), "Page title should be correct.");
        
        Logger.LogInformation("Verifying home page main content is visible.");
        Assert.That(_homePage.IsPageLoaded(TimeSpan.FromSeconds(10)), Is.True, "Home page content should be loaded and visible.");
    }
    
    [Test(Description = "Verify that a core UI component, Quick Links, is visible")]
    public void QuickLinks_ShouldBeVisibleOnHomePage()
    {
        Logger.LogInformation("Verifying Quick Links component visibility.");
        Assert.That(_homePage.QuickLinks.IsQuickLinksSectionVisible(TimeSpan.FromSeconds(5)), Is.True, "Quick Links section should be visible.");
    }
    
    [Test(Description = "Verify that a core UI component, Social Media, is visible")]
    public void SocialMedia_ShouldBeVisibleOnHomePage()
    {
        Logger.LogInformation("Verifying Social Media component visibility.");
        Assert.That(_homePage.SocialMedia.IsSocialMediaIconsContainerVisible(TimeSpan.FromSeconds(5)), Is.True, "Social Media section should be visible.");
    }

    [Test(Description = "Verify basic navigation to a different section of the page works")]
    public void Navigation_ClickingRoomsLink_ShouldNavigateToRoomsSection()
    {
        Logger.LogInformation("Verifying navigation to Rooms section via main navigation bar.");
        _homePage.Navigation.ClickRoomsLink();
        WaitHelper.WaitForCondition(Driver, d => d.Url.Contains("#rooms"), TimeSpan.FromSeconds(5));
        Assert.That(Driver.Url, Does.Contain("#rooms"), "Clicking 'Rooms' link from the main navigation should navigate to the rooms section.");
    }
    
    [Test(Description = "Verify the site is responsive on a mobile viewport")]
    [TestCase(375, 667, "iPhone 8")]
    public void HomePage_ShouldBeResponsiveOnMobile(int width, int height, string deviceName)
    {
        Logger.LogInformation("Verifying responsiveness on mobile device: {DeviceName}", deviceName);
        BrowserHelper.SetViewportSize(Driver, TestConfig, width, height);
        Assert.That(_homePage.IsPageLoaded(), Is.True, $"Page should remain loaded on {deviceName}.");
    }

    [Test(Description = "Verify the site is responsive on a tablet viewport")]
    [TestCase(768, 1024, "iPad")]
    public void HomePage_ShouldBeResponsiveOnTablet(int width, int height, string deviceName)
    {
        Logger.LogInformation("Verifying responsiveness on tablet device: {DeviceName}", deviceName);
        BrowserHelper.SetViewportSize(Driver, TestConfig, width, height);
        Assert.That(_homePage.IsPageLoaded(), Is.True, $"Page should remain loaded on {deviceName}.");
    }
} 