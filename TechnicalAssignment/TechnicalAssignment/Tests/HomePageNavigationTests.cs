using System;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using OpenQA.Selenium;
using TechnicalAssignment.Pages;
using TechnicalAssignment.Utilities;
using TechnicalAssignment.Models;

namespace TechnicalAssignment.Tests;

[TestFixture]
[Category("Navigation")]
[Category("HomePage")]
[Parallelizable(ParallelScope.Fixtures)]
public class HomePageNavigationTests : BaseTest
{
    private HomePage _homePage = null!;

    [SetUp]
    public void HomePageSetUp()
    {
        Logger.LogInformation("Starting HomePage Navigation test setup");
        Driver.Navigate().GoToUrl(TestConfig.BaseUrl);
        Logger.LogDebug("Navigated to URL: {Url}", TestConfig.BaseUrl);
        
        _homePage = new HomePage(Driver);
        _homePage.WaitForPageToLoad();
        Logger.LogDebug("HomePage loaded successfully for Navigation tests");
    }

    [Test]
    [Description("TC001: Verify the brand link is clickable and maintains homepage state")]
    public void BrandLink_WhenClicked_ShouldMaintainHomepageState()
    {
        Logger.LogInformation("Starting TC001: Brand link functionality test");
        
        Assert.That(_homePage.Navigation.IsOnHomePage(), Is.True, 
            "Should be on home page initially");
        
        Assert.That(_homePage.Navigation.IsBrandLinkVisible(), Is.True, 
            "Brand link should be visible");
        
        _homePage.Navigation.ClickBrandLink();
        
        Assert.That(_homePage.Navigation.IsOnHomePage(), Is.True, 
            "Brand link should maintain homepage state");
        
        Assert.That(_homePage.Navigation.IsBrandLinkVisible(), Is.True, 
            "Brand link should still be visible after clicking");
        
        Assert.That(_homePage.Navigation.IsRoomsLinkVisible(), Is.True, 
            "Navigation links should still be visible after clicking brand link");
        
        Logger.LogInformation("TC001: Brand link functionality test passed successfully");
    }

    [Test]
    [Description("TC002: Verify rooms link scrolls to rooms section")]
    public void RoomsLink_WhenClicked_ShouldScrollToRoomsSection()
    {
        Logger.LogInformation("Starting TC002: Rooms link scroll functionality test");
        
        _homePage.Navigation.ClickRoomsLink();
        
        Assert.That(_homePage.Navigation.IsRoomsSectionVisible(TimeSpan.FromSeconds(5)), Is.True, 
            "Rooms section should be visible after clicking rooms link");
        
        Logger.LogInformation("TC002: Rooms link scroll functionality test passed successfully");
    }

    [Test]
    [Description("TC003: Verify booking link scrolls to booking section")]
    public void BookingLink_WhenClicked_ShouldScrollToBookingSection()
    {
        Logger.LogInformation("Starting TC003: Booking link scroll functionality test");
        
        _homePage.Navigation.ClickBookingLink();
        
        Assert.That(_homePage.Navigation.IsBookingSectionVisible(TimeSpan.FromSeconds(5)), Is.True, 
            "Booking section should be visible after clicking booking link");
        
        Logger.LogInformation("TC003: Booking link scroll functionality test passed successfully");
    }

    [Test]
    [Description("TC004: Verify amenities link scrolls to amenities section")]
    public void AmenitiesLink_WhenClicked_ShouldScrollToAmenitiesSection()
    {
        Logger.LogInformation("Starting TC004: Amenities link scroll functionality test");
        
        _homePage.Navigation.ClickAmenitiesLink();
        
        Assert.That(_homePage.Navigation.IsAmenitiesSectionVisible(TimeSpan.FromSeconds(5)), Is.True, 
            "Amenities section should be visible after clicking amenities link");
        
        Logger.LogInformation("TC004: Amenities link scroll functionality test passed successfully");
    }

    [Test]
    [Description("TC005: Verify location link scrolls to location section")]
    public void LocationLink_WhenClicked_ShouldScrollToLocationSection()
    {
        Logger.LogInformation("Starting TC005: Location link scroll functionality test");
        
        _homePage.Navigation.ClickLocationLink();
        
        Assert.That(_homePage.Navigation.IsLocationSectionVisible(TimeSpan.FromSeconds(5)), Is.True, 
            "Location section should be visible after clicking location link");
        
        Logger.LogInformation("TC005: Location link scroll functionality test passed successfully");
    }

    [Test]
    [Description("TC006: Verify contact link scrolls to contact section")]
    public void ContactLink_WhenClicked_ShouldScrollToContactSection()
    {
        Logger.LogInformation("Starting TC006: Contact link scroll functionality test");
        
        _homePage.Navigation.ClickContactLink();
        
        Assert.That(_homePage.Navigation.IsContactSectionVisible(TimeSpan.FromSeconds(5)), Is.True, 
            "Contact section should be visible after clicking contact link");
        
        Logger.LogInformation("TC006: Contact link scroll functionality test passed successfully");
    }

    [Test]
    [Description("TC007: Verify admin link navigates to admin page")]
    public void AdminLink_WhenClicked_ShouldNavigateToAdminPage()
    {
        Logger.LogInformation("Starting TC007: Admin link navigation functionality test");
        
        var initialUrl = Driver.Url;
        Logger.LogDebug("Initial URL: {InitialUrl}", initialUrl);
        
        _homePage.Navigation.ClickAdminLink();
        
        WaitHelper.WaitForUrlChange(Driver, initialUrl);
        
        Assert.That(_homePage.Navigation.IsOnAdminPage(), Is.True, 
            "Admin link should navigate to admin page");
        
        Logger.LogInformation("TC007: Admin link navigation functionality test passed successfully");
    }

    [TestCaseSource(typeof(ViewportTestData), nameof(ViewportTestData.GetMobileViewportTestCases))]
    [Description("TC008: Verify navbar toggle works on all mobile viewports")]
    public void NavbarToggle_OnMobileViewport_ShouldToggleNavbarVisibility(int width, int height, string deviceName)
    {
        Logger.LogInformation("Starting TC008: Navbar toggle test on {DeviceName} ({Width}x{Height})", 
            deviceName, width, height);
        
        BrowserHelper.SetViewportSize(Driver, width, height);
        
        Assert.That(_homePage.Navigation.IsNavbarTogglerVisible(TimeSpan.FromSeconds(5)), Is.True, 
            $"Navbar toggler should be visible on {deviceName} ({width}x{height})");
        
        Assert.That(_homePage.Navigation.IsRoomsLinkVisible(TimeSpan.FromSeconds(2)), Is.False, 
            $"Navigation links should be hidden on {deviceName} initially");
        
        var expandSuccessful = _homePage.Navigation.ToggleNavbarWithRetry(shouldBeCollapsed: false, TimeSpan.FromSeconds(10));
        
        Assert.That(expandSuccessful, Is.True, 
            $"Navbar should be successfully expanded after clicking toggler on {deviceName}");
        
        Assert.That(_homePage.Navigation.IsRoomsLinkVisible(TimeSpan.FromSeconds(5)), Is.True, 
            $"Navigation links should be visible after expanding navbar on {deviceName}");
        Assert.That(_homePage.Navigation.IsContactLinkVisible(TimeSpan.FromSeconds(5)), Is.True, 
            $"All navigation links should be accessible after expansion on {deviceName}");
        
        var toggleSuccessful = _homePage.Navigation.ToggleNavbarWithRetry(shouldBeCollapsed: true, TimeSpan.FromSeconds(10));
        
        Assert.That(toggleSuccessful, Is.True, 
            $"Navbar should be successfully collapsed after clicking toggler again on {deviceName}");
        
        Logger.LogInformation("TC008: Navbar toggle test passed successfully on {DeviceName} ({Width}x{Height})", 
            deviceName, width, height);
        
        BrowserHelper.SetViewportSize(Driver, 1920, 1080);
    }

    [Test]
    [Description("TC009: Verify all navigation links are present and visible")]
    public void AllNavigationLinks_ShouldBeVisibleAndPresent()
    {
        Logger.LogInformation("Starting TC009: All navigation links visibility test");
        
        Assert.Multiple(() =>
        {
            Assert.That(_homePage.Navigation.IsBrandLinkVisible(), Is.True, 
                "Brand link should be visible");
            
            Assert.That(_homePage.Navigation.IsRoomsLinkVisible(), Is.True, 
                "Rooms link should be visible");
            
            Assert.That(_homePage.Navigation.IsBookingLinkVisible(), Is.True, 
                "Booking link should be visible");
            
            Assert.That(_homePage.Navigation.IsAmenitiesLinkVisible(), Is.True, 
                "Amenities link should be visible");
            
            Assert.That(_homePage.Navigation.IsLocationLinkVisible(), Is.True, 
                "Location link should be visible");
            
            Assert.That(_homePage.Navigation.IsContactLinkVisible(), Is.True, 
                "Contact link should be visible");
            
            Assert.That(_homePage.Navigation.IsAdminLinkVisible(), Is.True, 
                "Admin link should be visible");
        });
        
        Logger.LogInformation("TC009: All navigation links visibility test passed successfully");
    }

    [Test]
    [Description("TC010: Verify all navigation links have correct text content")]
    public void NavigationLinks_ShouldHaveCorrectTextContent()
    {
        Logger.LogInformation("Starting TC010: Navigation links text content validation test");
        
        Assert.Multiple(() =>
        {
            Assert.That(_homePage.Navigation.GetBrandLinkText(), Does.Contain("Shady Meadows"), 
                "Brand link should contain 'Shady Meadows'");
            
            Assert.That(_homePage.Navigation.GetRoomsLinkText(), Is.EqualTo("Rooms"), 
                "Rooms link should have text 'Rooms'");
            
            Assert.That(_homePage.Navigation.GetBookingLinkText(), Is.EqualTo("Booking"), 
                "Booking link should have text 'Booking'");
            
            Assert.That(_homePage.Navigation.GetAmenitiesLinkText(), Is.EqualTo("Amenities"), 
                "Amenities link should have text 'Amenities'");
            
            Assert.That(_homePage.Navigation.GetLocationLinkText(), Is.EqualTo("Location"), 
                "Location link should have text 'Location'");
            
            Assert.That(_homePage.Navigation.GetContactLinkText(), Is.EqualTo("Contact"), 
                "Contact link should have text 'Contact'");
            
            Assert.That(_homePage.Navigation.GetAdminLinkText(), Is.EqualTo("Admin"), 
                "Admin link should have text 'Admin'");
        });
        
        Logger.LogInformation("TC010: Navigation links text content validation test passed successfully");
    }

    [TestCaseSource(typeof(ViewportTestData), nameof(ViewportTestData.GetTabletViewportTestCases))]
    [Description("TC011: Verify navbar functionality works correctly on tablet viewports")]
    public void NavbarBehavior_OnTabletViewport_ShouldMaintainFunctionality(int width, int height, string deviceName)
    {
        Logger.LogInformation("Starting TC011: Navbar functionality test on {DeviceName} ({Width}x{Height})", 
            deviceName, width, height);
        
        BrowserHelper.SetViewportSize(Driver, width, height);
        
        Assert.That(_homePage.Navigation.IsBrandLinkVisible(TimeSpan.FromSeconds(5)), Is.True, 
            $"Brand link should be visible on {deviceName} ({width}x{height})");
        
        Assert.That(_homePage.Navigation.IsNavbarTogglerVisible(TimeSpan.FromSeconds(5)), Is.True, 
            $"Navbar toggler should be visible on {deviceName}");
        
        _homePage.Navigation.ClickNavbarToggler();
        
        Assert.That(_homePage.Navigation.IsRoomsLinkVisible(TimeSpan.FromSeconds(5)), Is.True, 
            $"Rooms link should be visible on {deviceName} after expanding navbar");
        
        _homePage.Navigation.ClickRoomsLink();
        
        Assert.That(_homePage.Navigation.IsRoomsSectionVisible(TimeSpan.FromSeconds(5)), Is.True, 
            $"Rooms section should be visible on {deviceName} after navigation");
        
        Logger.LogInformation("TC011: Navbar functionality test passed successfully on {DeviceName} ({Width}x{Height})", 
            deviceName, width, height);
        
        BrowserHelper.SetViewportSize(Driver, 1920, 1080);
    }

    [TestCaseSource(typeof(ViewportTestData), nameof(ViewportTestData.GetDesktopViewportTestCases))]
    [Description("TC012: Verify navbar shows all links directly on desktop viewports")]
    public void NavbarBehavior_OnDesktopViewport_ShouldShowAllLinksDirectly(int width, int height, string deviceName)
    {
        Logger.LogInformation("Starting TC012: Navbar behavior test on {DeviceName} ({Width}x{Height})", 
            deviceName, width, height);
        
        BrowserHelper.SetViewportSize(Driver, width, height);
        
        Assert.That(_homePage.Navigation.IsBrandLinkVisible(TimeSpan.FromSeconds(5)), Is.True, 
            $"Brand link should be visible on {deviceName} ({width}x{height})");
        
        Assert.That(_homePage.Navigation.IsNavbarTogglerVisible(TimeSpan.FromSeconds(2)), Is.False, 
            $"Navbar toggler should not be visible on {deviceName}");
        
        Assert.Multiple(() =>
        {
            Assert.That(_homePage.Navigation.IsRoomsLinkVisible(TimeSpan.FromSeconds(5)), Is.True, 
                $"Rooms link should be directly visible on {deviceName}");
            Assert.That(_homePage.Navigation.IsBookingLinkVisible(TimeSpan.FromSeconds(5)), Is.True, 
                $"Booking link should be directly visible on {deviceName}");
            Assert.That(_homePage.Navigation.IsAmenitiesLinkVisible(TimeSpan.FromSeconds(5)), Is.True, 
                $"Amenities link should be directly visible on {deviceName}");
            Assert.That(_homePage.Navigation.IsLocationLinkVisible(TimeSpan.FromSeconds(5)), Is.True, 
                $"Location link should be directly visible on {deviceName}");
            Assert.That(_homePage.Navigation.IsContactLinkVisible(TimeSpan.FromSeconds(5)), Is.True, 
                $"Contact link should be directly visible on {deviceName}");
            Assert.That(_homePage.Navigation.IsAdminLinkVisible(TimeSpan.FromSeconds(5)), Is.True, 
                $"Admin link should be directly visible on {deviceName}");
        });
        
        _homePage.Navigation.ClickRoomsLink();
        
        Assert.That(_homePage.Navigation.IsRoomsSectionVisible(TimeSpan.FromSeconds(5)), Is.True, 
            $"Rooms section should be visible on {deviceName} after navigation");
        
        Logger.LogInformation("TC012: Navbar behavior test passed successfully on {DeviceName} ({Width}x{Height})", 
            deviceName, width, height);
        
        BrowserHelper.SetViewportSize(Driver, 1920, 1080);
    }
} 