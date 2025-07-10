using System;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using OpenQA.Selenium;
using TechnicalAssignment.Pages;
using TechnicalAssignment.Utilities;
using TechnicalAssignment.Models;

namespace TechnicalAssignment.Tests;

[TestFixture]
[Category("Booking")]
[Category("HomePage")]
[Parallelizable(ParallelScope.Fixtures)]
public class HomePageBookingTests : BaseTest
{
    private HomePage _homePage = null!;

    [SetUp]
    public void PageSetup()
    {
        Logger.LogInformation("Setting up HomePageBookingTests");
        Driver.Navigate().GoToUrl(TestConfig.BaseUrl);
        _homePage = new HomePage(Driver);
        _homePage.WaitForPageToLoad();
    }

    [Test(Description = "TC001: Verify that the booking section is visible and contains all required elements")]
    public void BookingSection_WhenLoaded_ShouldContainAllRequiredElements()
    {
        Logger.LogInformation("Starting TC001: Booking section visibility test");
        
        _homePage.ScrollToBookingSection();
        
        Assert.That(_homePage.ValidateBookingFormElements(), Is.True, 
            "All booking form elements should be present and functional");
        
        var formTitle = _homePage.GetBookingFormTitle();
        Assert.That(formTitle, Is.EqualTo("Check Availability & Book Your Stay"), 
            "Booking form should have correct title");
        
        Logger.LogInformation("TC001: Booking section visibility test passed successfully");
    }

    [Test(Description = "TC002: Verify that default check-in and check-out dates are set correctly")]
    public void BookingDates_WhenLoaded_ShouldHaveValidDefaultValues()
    {
        Logger.LogInformation("Starting TC002: Default date values test");
        
        _homePage.ScrollToBookingSection();
        
        var checkInDate = _homePage.GetCheckInDate();
        var checkOutDate = _homePage.GetCheckOutDate();
        
        Logger.LogDebug("Default dates - CheckIn: {CheckIn}, CheckOut: {CheckOut}", checkInDate, checkOutDate);
        
        Assert.That(checkInDate, Is.Not.Empty, "Check-in date should not be empty");
        Assert.That(checkOutDate, Is.Not.Empty, "Check-out date should not be empty");
        
        var dateFormats = new[] { "dd/MM/yyyy" };

        Assert.That(DateTime.TryParseExact(checkInDate, dateFormats, System.Globalization.CultureInfo.InvariantCulture,
            System.Globalization.DateTimeStyles.None, out var checkIn) ||
            DateTime.TryParse(checkInDate, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out checkIn), Is.True, "Check-in date should be parseable");
        
        Assert.That(DateTime.TryParseExact(checkOutDate, dateFormats, System.Globalization.CultureInfo.InvariantCulture,
            System.Globalization.DateTimeStyles.None, out var checkOut) ||
            DateTime.TryParse(checkOutDate, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out checkOut), Is.True, "Check-out date should be parseable");
        
        Assert.That(checkOut >= checkIn, Is.True, 
            "Check-out date should be on or after check-in date");
        
        Logger.LogInformation("TC002: Default date values test passed successfully");
    }

    [Test(Description = "TC003: Verify that clicking 'Check Availability' updates the rooms section")]
    public void CheckAvailability_WithDefaultDates_ShouldUpdateRoomsSection()
    {
        Logger.LogInformation("Starting TC003: Check availability with default dates test");
        
        _homePage.ScrollToBookingSection();
        _homePage.ClickCheckAvailability();
        
        Assert.That(_homePage.WaitForRoomsToUpdate(TimeSpan.FromSeconds(15)), Is.True, 
            "Rooms section should update after availability check");
        
        var roomsCount = _homePage.GetAvailableRoomsCount();
        Assert.That(roomsCount, Is.GreaterThan(0), 
            "At least one room should be available for default dates");
        
        Logger.LogInformation("TC003: Check availability test passed successfully");
    }

    [Test(Description = "TC004: Verify that custom future dates can be selected and used for availability checking")]
    public void DatePicker_WithValidFutureDates_ShouldAcceptAndFilterRooms()
    {
        Logger.LogInformation("Starting TC004: Date picker functionality test");
        
        _homePage.ScrollToBookingSection();
        
        var testDates = BookingTestData.GenerateRandomBookingDates(1, 30, 1);
        
        Logger.LogDebug("Setting custom dates: {CheckIn} to {CheckOut}", testDates.CheckIn, testDates.CheckOut);
        _homePage.CheckAvailability(testDates.CheckIn, testDates.CheckOut);
        
        Assert.That(_homePage.WaitForRoomsToUpdate(TimeSpan.FromSeconds(15)), Is.True, 
            "Rooms section should update with custom dates");
        
        var roomsCount = _homePage.GetAvailableRoomsCount();
        Assert.That(roomsCount, Is.GreaterThan(0), 
            "Rooms should be available for valid future dates");
        
        Logger.LogInformation("TC004: Date picker functionality test passed successfully");
    }

    [Test(Description = "TC005: Verify that past dates are rejected or no availability is shown for past dates")]
    public void DateValidation_WithPastDates_ShouldRejectOrShowNoAvailability()
    {
        Logger.LogInformation("Starting TC005: Past dates validation test");
        
        _homePage.ScrollToBookingSection();
        
        var validationResult = _homePage.ValidatePastDatesHandling();
        
        Assert.That(validationResult.IsValid, Is.True, 
            $"Past dates should be handled correctly. Validation errors: {string.Join(", ", validationResult.ValidationErrors)}");
        
        Logger.LogDebug("Past dates validation result - CheckIn: {CheckIn}, CheckOut: {CheckOut}, HasPastCheckIn: {HasPastCheckIn}", 
            validationResult.CheckInDate, validationResult.CheckOutDate, validationResult.HasPastCheckIn);
        
        if (validationResult.HasPastCheckIn)
        {
            Logger.LogInformation("Past dates were accepted but correctly showed no availability");
        }
        else
        {
            Logger.LogInformation("Past dates were correctly rejected/prevented from being set");
        }
        
        Logger.LogInformation("TC005: Past dates validation test passed successfully");
    }

    [Test(Description = "TC006: Verify that check-out date cannot be before check-in date")]
    public void DateValidation_WithCheckoutBeforeCheckin_ShouldPreventInvalidBooking()
    {
        Logger.LogInformation("Starting TC006: Check-out before check-in validation test");
        
        _homePage.ScrollToBookingSection();
        
        var validationResult = _homePage.ValidateInvalidDateOrderHandling();
        
        Assert.That(validationResult.IsValid, Is.True, 
            $"Invalid date order should be handled correctly. Validation errors: {string.Join(", ", validationResult.ValidationErrors)}");
            
        Logger.LogDebug("Invalid date order validation result - CheckIn: {CheckIn}, CheckOut: {CheckOut}, HasInvalidOrder: {HasInvalidOrder}", 
            validationResult.CheckInDate, validationResult.CheckOutDate, validationResult.HasInvalidDateOrder);

        if (validationResult.HasInvalidDateOrder)
        {
            Logger.LogInformation("Invalid date range was accepted but correctly showed no availability");
        }
        else
        {
            Logger.LogInformation("Invalid date range was correctly rejected/prevented from being set");
        }
        
        Logger.LogInformation("TC006: Check-out before check-in validation test passed successfully");
    }

    [Test(Description = "TC007: Verify that same-day check-in and check-out bookings are prevented")]
    public void DateValidation_WithSameDayCheckInOut_ShouldPreventBooking()
    {
        Logger.LogInformation("Starting TC007: Same day check-in/check-out validation test");
        
        _homePage.ScrollToBookingSection();
        
        var validationResult = _homePage.ValidateSameDayBookingHandling();
        
        Assert.That(validationResult.IsValid, Is.True, 
            $"Same-day booking should be handled correctly. Validation errors: {string.Join(", ", validationResult.ValidationErrors)}");
        
        Logger.LogDebug("Same-day booking validation result - CheckIn: {CheckIn}, CheckOut: {CheckOut}, HasSameDayBooking: {HasSameDayBooking}", 
            validationResult.CheckInDate, validationResult.CheckOutDate, validationResult.HasSameDayBooking);
        
        if (validationResult.HasSameDayBooking)
        {
            Logger.LogInformation("Same-day booking was accepted but correctly showed no availability");
        }
        else
        {
            Logger.LogInformation("Same-day booking was correctly rejected/prevented from being set");
        }
        
        Logger.LogInformation("TC007: Same day check-in/check-out validation test passed successfully");
    }

    [Test(Description = "TC008: Verify that clicking 'Book now' navigates to the reservation page")]
    public void RoomCard_WhenBookNowClicked_ShouldNavigateToReservationPage()
    {
        Logger.LogInformation("Starting TC008: Room card navigation test");
        
        _homePage.ScrollToBookingSection();
        _homePage.ClickCheckAvailability();
        
        Assert.That(_homePage.WaitForRoomsToUpdate(TimeSpan.FromSeconds(15)), Is.True, 
            "Rooms should be available for booking");
        
        _homePage.ClickFirstAvailableRoom();
        
        var reservationPage = new ReservationPage(Driver);
        reservationPage.WaitForPageToLoad(TimeSpan.FromSeconds(15));
        
        Assert.That(reservationPage.IsOnReservationPage(), Is.True, 
            "Should navigate to reservation page");
        
        Assert.That(reservationPage.IsPageLoaded(), Is.True, 
            "Reservation page should be loaded correctly");
        
        Logger.LogInformation("TC008: Room card navigation test passed successfully");
    }

    [TestCaseSource(typeof(ViewportTestData), nameof(ViewportTestData.GetMobileViewportTestCases))]
    [Description("TC027a: Verify that booking form displays correctly on mobile viewports")]
    public void BookingForm_ResponsiveDisplay_ShouldWorkOnMobileViewports(int width, int height, string deviceName)
    {
        Logger.LogInformation("Starting TC027a: Booking form responsive display test on {DeviceName} ({Width}x{Height})", 
            deviceName, width, height);
        
        try
        {
            Logger.LogDebug("Setting viewport size to {Width}x{Height} for {DeviceName}", width, height, deviceName);
            BrowserHelper.SetViewportSize(Driver, width, height);
            
            Logger.LogDebug("Scrolling to booking section on {DeviceName}", deviceName);
            _homePage.ScrollToBookingSection();
            
            Logger.LogDebug("Validating booking form elements on {DeviceName}", deviceName);
            Assert.That(_homePage.TestBookingFormResponsiveDisplay(width, height, deviceName), Is.True, 
                $"Booking form should display correctly on {deviceName} ({width}x{height})");
            
            Logger.LogInformation("TC027a: Booking form responsive display test passed successfully on {DeviceName} ({Width}x{Height})", 
                deviceName, width, height);
        }
        finally
        {
            Logger.LogDebug("Resetting viewport size to default after {DeviceName} test", deviceName);
            BrowserHelper.SetViewportSize(Driver, 1920, 1080);
        }
    }

    [TestCaseSource(typeof(ViewportTestData), nameof(ViewportTestData.GetTabletViewportTestCases))]
    [Description("TC027b: Verify that booking form displays correctly on tablet viewports")]
    public void BookingForm_ResponsiveDisplay_ShouldWorkOnTabletViewports(int width, int height, string deviceName)
    {
        Logger.LogInformation("Starting TC027b: Booking form responsive display test on {DeviceName} ({Width}x{Height})", 
            deviceName, width, height);
        
        try
        {
            Logger.LogDebug("Setting viewport size to {Width}x{Height} for {DeviceName}", width, height, deviceName);
            BrowserHelper.SetViewportSize(Driver, width, height);
            
            Logger.LogDebug("Scrolling to booking section on {DeviceName}", deviceName);
            _homePage.ScrollToBookingSection();
            
            Logger.LogDebug("Validating booking form elements on {DeviceName}", deviceName);
            Assert.That(_homePage.TestBookingFormResponsiveDisplay(width, height, deviceName), Is.True, 
                $"Booking form should display correctly on {deviceName} ({width}x{height})");
            
            Logger.LogInformation("TC027b: Booking form responsive display test passed successfully on {DeviceName} ({Width}x{Height})", 
                deviceName, width, height);
        }
        finally
        {
            Logger.LogDebug("Resetting viewport size to default after {DeviceName} test", deviceName);
            BrowserHelper.SetViewportSize(Driver, 1920, 1080);
        }
    }

    [TestCaseSource(typeof(ViewportTestData), nameof(ViewportTestData.GetDesktopViewportTestCases))]
    [Description("TC027c: Verify that booking form displays correctly on desktop viewports")]
    public void BookingForm_ResponsiveDisplay_ShouldWorkOnDesktopViewports(int width, int height, string deviceName)
    {
        Logger.LogInformation("Starting TC027c: Booking form responsive display test on {DeviceName} ({Width}x{Height})", 
            deviceName, width, height);
        
        try
        {
            Logger.LogDebug("Setting viewport size to {Width}x{Height} for {DeviceName}", width, height, deviceName);
            BrowserHelper.SetViewportSize(Driver, width, height);
            
            Logger.LogDebug("Scrolling to booking section on {DeviceName}", deviceName);
            _homePage.ScrollToBookingSection();
            
            Logger.LogDebug("Validating booking form elements on {DeviceName}", deviceName);
            Assert.That(_homePage.TestBookingFormResponsiveDisplay(width, height, deviceName), Is.True, 
                $"Booking form should display correctly on {deviceName} ({width}x{height})");
            
            Logger.LogInformation("TC027c: Booking form responsive display test passed successfully on {DeviceName} ({Width}x{Height})", 
                deviceName, width, height);
        }
        finally
        {
            Logger.LogDebug("Resetting viewport size to default after {DeviceName} test", deviceName);
            BrowserHelper.SetViewportSize(Driver, 1920, 1080);
        }
    }
} 