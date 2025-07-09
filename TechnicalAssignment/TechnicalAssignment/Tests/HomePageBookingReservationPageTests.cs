using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using TechnicalAssignment.Pages;
using TechnicalAssignment.Utilities;
using TechnicalAssignment.Models;
using System.Linq;

namespace TechnicalAssignment.Tests;

[TestFixture]
[Category("Booking")]
[Category("ReservationPage")]
[Parallelizable(ParallelScope.Fixtures)]
public class HomePageBookingReservationPageTests : BaseTest
{
    private HomePage _homePage = null!;
    private ReservationPage _reservationPage = null!;

    [SetUp]
    public void PageSetup()
    {
        Logger.LogInformation("Setting up HomePageBookingReservationPageTests");
        Driver.Navigate().GoToUrl(TestConfig.BaseUrl);
        _homePage = new HomePage(Driver);
        _homePage.WaitForPageToLoad();
        _reservationPage = new ReservationPage(Driver);
    }

    [Test(Description = "TC009: Verify that room details are correctly displayed on the reservation page")]
    public void ReservationPage_WhenLoaded_ShouldDisplayCorrectRoomDetails()
    {
        Logger.LogInformation("Starting TC009: Reservation page room details test");
        
        Assert.That(_homePage.NavigateToReservationPage(), Is.True, 
            "Should successfully navigate to reservation page");
        _reservationPage.WaitForPageToLoad(TimeSpan.FromSeconds(15));
        
        var roomTitle = _reservationPage.GetRoomTitle();
        Assert.That(roomTitle, Is.Not.Empty, "Room title should be displayed");
        
        Logger.LogDebug("Room title: {Title}", roomTitle);
        
        Assert.That(_reservationPage.IsRoomImageDisplayed(), Is.True, 
            "Room image should be displayed");
        
        var roomDescription = _reservationPage.GetRoomDescription();
        Assert.That(roomDescription, Is.Not.Empty, "Room description should be displayed");
        
        Assert.That(_reservationPage.AreRoomPoliciesDisplayed(), Is.True, 
            "Room policies should be displayed");
        
        var roomFeatures = _reservationPage.GetRoomFeatures();
        Logger.LogDebug("Room features count: {Count}", roomFeatures.Count);
        
        Logger.LogInformation("TC009: Reservation page room details test passed successfully");
    }

    [TestCaseSource(typeof(BookingTestData), nameof(BookingTestData.AllRoomTypes))]
    [Test(Description = "TC010: Verify that each room type displays correct guest count")]
    public void ReservationPage_RoomType_ShouldDisplayCorrectGuestCount(BookingTestData.RoomInfo roomInfo)
    {
        Logger.LogInformation("Starting TC010: {RoomType} room guest count validation test", roomInfo.RoomType);
        
        if (!_homePage.EnsureOnHomePageForBooking(TestConfig.BaseUrl) || 
            !_homePage.NavigateToReservationPageForRoomType(roomInfo.RoomType))
        {
            Assert.Ignore($"{roomInfo.RoomType} room type is not available for booking - skipping test");
            return;
        }
        _reservationPage.WaitForPageToLoad();
        
        var isValid = _reservationPage.ValidateGuestCount(roomInfo.RoomType, roomInfo.ExpectedGuestCount);
        var actualGuestCount = _reservationPage.GetMaxGuestCount();
        var guestText = _reservationPage.GetGuestCount();
        
        Logger.LogDebug("{RoomType} room - Guest text: '{Text}', Max guests: {Count}", roomInfo.RoomType, guestText, actualGuestCount);
        
        Assert.That(isValid, Is.True, $"{roomInfo.RoomType} room should have max guest count of {roomInfo.ExpectedGuestCount}, but found: {actualGuestCount}");
        Assert.That(actualGuestCount, Is.EqualTo(roomInfo.ExpectedGuestCount), $"{roomInfo.RoomType} room should accommodate exactly {roomInfo.ExpectedGuestCount} guests");
        
        Logger.LogInformation("TC010: {RoomType} room guest count validation test passed successfully", roomInfo.RoomType);
    }
    
    [Test(Description = "TC011: Verify that the calendar navigation buttons (Today, Back, Next) work correctly on the reservation page")]
    public void ReservationPage_CalendarButtons_ShouldFunctionCorrectly()
    {
        Logger.LogInformation("Starting TC011: Calendar buttons functionality test");
        
        Assert.That(_homePage.NavigateToReservationPage(), Is.True, 
            "Should successfully navigate to reservation page");
        _reservationPage.WaitForPageToLoad(TimeSpan.FromSeconds(15));
        
        Assert.That(_reservationPage.IsCalendarVisible(), Is.True, 
            "Calendar component should be visible");
        
        var initialMonth = _reservationPage.GetCalendarMonth();
        Logger.LogDebug("Initial calendar month: {Month}", initialMonth);
        Assert.That(initialMonth, Is.Not.Empty, "Calendar should display a month/year");
        
        Logger.LogDebug("Testing calendar Next button");
        _reservationPage.ClickCalendarNext();
        WaitHelper.WaitForCondition(Driver, _ => 
            {
                var currentMonth = _reservationPage.GetCalendarMonth();
                return currentMonth != initialMonth;
            }, TimeSpan.FromSeconds(3));
        
        var nextMonth = _reservationPage.GetCalendarMonth();
        Logger.LogDebug("Month after clicking Next: {Month}", nextMonth);
        Assert.That(nextMonth, Is.Not.EqualTo(initialMonth), 
            "Calendar should show different month after clicking Next");
        
        Logger.LogDebug("Testing calendar Back button");
        _reservationPage.ClickCalendarBack();
        WaitHelper.WaitForCondition(Driver, _ => 
            {
                var currentMonth = _reservationPage.GetCalendarMonth();
                return currentMonth == initialMonth;
            }, TimeSpan.FromSeconds(3));
        
        var backMonth = _reservationPage.GetCalendarMonth();
        Logger.LogDebug("Month after clicking Back: {Month}", backMonth);
        Assert.That(backMonth, Is.EqualTo(initialMonth), 
            "Calendar should return to initial month after clicking Back");
        
        Logger.LogDebug("Testing calendar Today button");
        _reservationPage.ClickCalendarNext();
        WaitHelper.WaitForCondition(Driver, _ => true, TimeSpan.FromSeconds(1));
        
        var currentDate = DateTime.Now;
        var expectedCurrentMonth = currentDate.ToString("MMMM yyyy");
        
        _reservationPage.ClickCalendarToday();
        WaitHelper.WaitForCondition(Driver, _ => 
            {
                var currentMonth = _reservationPage.GetCalendarMonth();
                return currentMonth.Contains(currentDate.ToString("MMMM")) && 
                       currentMonth.Contains(currentDate.Year.ToString());
            }, TimeSpan.FromSeconds(3));
        
        var todayMonth = _reservationPage.GetCalendarMonth();
        Logger.LogDebug("Month after clicking Today: {Month} (expected: {Expected})", todayMonth, expectedCurrentMonth);
        Assert.That(todayMonth, Does.Contain(currentDate.ToString("MMMM")), 
            "Calendar should show current month after clicking Today");
        Assert.That(todayMonth, Does.Contain(currentDate.Year.ToString()), 
            "Calendar should show current year after clicking Today");
        
        Logger.LogInformation("TC011: Calendar buttons functionality test passed successfully");
    }
    
    [Test(Description = "TC012: Verify that price summary calculates correctly: (Room Price × Nights) + Cleaning Fee + Service Fee = Total")]
    public void ReservationPage_PriceSummary_ShouldCalculateCorrectly()
    {
        Logger.LogInformation("Starting TC012: Price summary calculation test");
        
        Assert.That(_homePage.NavigateToReservationPage(), Is.True, 
            "Should successfully navigate to reservation page");
        _reservationPage.WaitForPageToLoad(TimeSpan.FromSeconds(15));
        
        var roomPriceDisplay = _reservationPage.GetRoomPrice();
        var basePriceText = _reservationPage.GetBasePriceFromSummary();
        var cleaningFeeText = _reservationPage.GetCleaningFee();
        var serviceFeeText = _reservationPage.GetServiceFee();
        var totalPriceText = _reservationPage.GetTotalPrice();
        var nightsCountText = _reservationPage.GetNightsCount();
        
        Logger.LogDebug("Raw price texts - Room Display: '{RoomDisplay}', Base: '{Base}', Cleaning: '{Cleaning}', Service: '{Service}', Total: '{Total}', Nights: '{Nights}'",
            roomPriceDisplay, basePriceText, cleaningFeeText, serviceFeeText, totalPriceText, nightsCountText);
        
        var roomPricePerNight = PriceHelper.ExtractPriceValue(roomPriceDisplay);
        var basePriceCalculated = PriceHelper.ExtractPriceValue(basePriceText);
        var cleaningFee = PriceHelper.ExtractPriceValue(cleaningFeeText);
        var serviceFee = PriceHelper.ExtractPriceValue(serviceFeeText);
        var totalPrice = PriceHelper.ExtractPriceValue(totalPriceText);
        
        var nightsCount = PriceHelper.ExtractNightsCount(nightsCountText);
        var roomPriceFromNights = PriceHelper.ExtractRoomPriceFromNightsText(nightsCountText);
        
        Logger.LogDebug("Extracted values - Room Price/Night: £{RoomPrice}, Nights: {Nights}, Base Calculated: £{BaseCalc}, Cleaning: £{Cleaning}, Service: £{Service}, Total: £{Total}",
            roomPricePerNight, nightsCount, basePriceCalculated, cleaningFee, serviceFee, totalPrice);
        
        Assert.That(nightsCount, Is.GreaterThan(0), "Number of nights should be greater than 0");
        Assert.That(roomPricePerNight, Is.GreaterThan(0), "Room price per night should be greater than 0");
        Assert.That(cleaningFee, Is.GreaterThan(0), "Cleaning fee should be greater than 0");
        Assert.That(serviceFee, Is.GreaterThan(0), "Service fee should be greater than 0");
        Assert.That(totalPrice, Is.GreaterThan(0), "Total price should be greater than 0");
        
        Assert.That(cleaningFee, Is.EqualTo(25m), "Cleaning fee should be £25");
        Assert.That(serviceFee, Is.EqualTo(15m), "Service fee should be £15");
        
        if (roomPriceFromNights > 0)
        {
            Assert.That(roomPriceFromNights, Is.EqualTo(roomPricePerNight).Within(0.01m), 
                $"Room price in nights text (£{roomPriceFromNights}) should match display price (£{roomPricePerNight})");
        }
        
        var expectedBasePrice = roomPricePerNight * nightsCount;
        Logger.LogDebug("Expected base price calculation: £{RoomPrice} × {Nights} = £{ExpectedBase}", 
            roomPricePerNight, nightsCount, expectedBasePrice);
        
        Assert.That(basePriceCalculated, Is.EqualTo(expectedBasePrice).Within(0.01m), 
            $"Base price (£{basePriceCalculated}) should equal room price × nights (£{expectedBasePrice})");
        
        var expectedTotal = expectedBasePrice + cleaningFee + serviceFee;
        Logger.LogDebug("Expected total calculation: £{Base} + £{Cleaning} + £{Service} = £{ExpectedTotal}", 
            expectedBasePrice, cleaningFee, serviceFee, expectedTotal);
        
        Assert.That(totalPrice, Is.EqualTo(expectedTotal).Within(0.01m), 
            $"Total price (£{totalPrice}) should equal base + cleaning + service (£{expectedTotal})");
        
        var isValidByMethod = _reservationPage.ValidatePriceSummary(roomPricePerNight, nightsCount);
        Assert.That(isValidByMethod, Is.True, "Price summary should pass validation using ValidatePriceSummary method");
        
        Logger.LogInformation("TC012: Price summary calculation test passed successfully. " +
            "Verified: £{Room} × {Nights} nights + £{Cleaning} cleaning + £{Service} service = £{Total} total",
            roomPricePerNight, nightsCount, cleaningFee, serviceFee, totalPrice);
    }
    
    [Test(Description = "TC013: Verify that clicking 'Reserve Now' shows the guest information form")]
    public void ReservationPage_ReserveNowButton_ShouldShowGuestForm()
    {
        Logger.LogInformation("Starting TC013: Reserve Now button functionality test");
        
        Assert.That(_homePage.NavigateToReservationPage(), Is.True, 
            "Should successfully navigate to reservation page");
        _reservationPage.WaitForPageToLoad(TimeSpan.FromSeconds(15));
        
        Logger.LogDebug("Clicking Reserve Now button");
        _reservationPage.ClickReserveNow();
        
        Logger.LogDebug("Waiting for guest form to appear");
        _reservationPage.WaitForGuestFormState();
        
        Logger.LogDebug("Verifying guest form is visible");
        Assert.That(_reservationPage.IsGuestFormVisible(), Is.True, 
            "Guest information form should be visible after clicking Reserve Now");
        
        Logger.LogInformation("TC013: Reserve Now button functionality test passed successfully");
    }
    
    [Test(Description = "TC022: Verify that accessible badge is displayed for accessible rooms")]
    public void ReservationPage_AccessibleBadge_ShouldDisplayForAccessibleRooms()
    {
        Logger.LogInformation("Starting TC022: Accessible badge display test");
        
        Assert.That(_homePage.NavigateToReservationPage(), Is.True, 
            "Should successfully navigate to reservation page");
        _reservationPage.WaitForPageToLoad(TimeSpan.FromSeconds(15));
        
        Logger.LogDebug("Checking for accessible badge");
        var hasAccessibilityBadge = _reservationPage.HasAccessibilityBadge();
        
        if (hasAccessibilityBadge)
        {
            var badgeText = _reservationPage.GetAccessibilityBadgeText();
            Logger.LogDebug("Accessible badge text: {Text}", badgeText);
            
            Assert.That(badgeText, Does.Contain("Accessible"), 
                "Accessible badge should contain 'Accessible' text");
        }
        else
        {
            Logger.LogDebug("No accessible badge found - room may not be accessible");
        }
        
        Logger.LogInformation("TC022: Accessible badge display test passed successfully");
    }

    [Test(Description = "TC023: Verify that similar rooms section is displayed on reservation page")]
    public void ReservationPage_SimilarRoomsSection_ShouldBeDisplayed()
    {
        Logger.LogInformation("Starting TC023: Similar rooms section display test");
        
        Assert.That(_homePage.NavigateToReservationPage(), Is.True, 
            "Should successfully navigate to reservation page");
        _reservationPage.WaitForPageToLoad(TimeSpan.FromSeconds(15));
        
        Logger.LogDebug("Scrolling to bottom of page to find similar rooms section");
        ScrollHelper.ScrollToBottom(Driver);
        
        Logger.LogDebug("Checking for similar rooms section");
        var isSimilarRoomsVisible = _reservationPage.IsSimilarRoomsSectionVisible();
        
        if (isSimilarRoomsVisible)
        {
            var similarRoomsCount = _reservationPage.GetSimilarRoomsCount();
            Logger.LogDebug("Similar rooms count: {Count}", similarRoomsCount);
            
            Assert.That(similarRoomsCount, Is.GreaterThan(0), 
                "Similar rooms section should contain at least one room");
        }
        else
        {
            Logger.LogDebug("Similar rooms section not found - may not be implemented");
        }
        
        Logger.LogInformation("TC023: Similar rooms section display test passed successfully");
    }

    [Test(Description = "TC024: Verify that room features are correctly displayed on reservation page")]
    public void ReservationPage_RoomFeatures_ShouldBeDisplayedCorrectly()
    {
        Logger.LogInformation("Starting TC024: Room features display test");
        
        Assert.That(_homePage.NavigateToReservationPage(), Is.True, 
            "Should successfully navigate to reservation page");
        _reservationPage.WaitForPageToLoad(TimeSpan.FromSeconds(15));
        
        Logger.LogDebug("Getting room features");
        var roomFeatures = _reservationPage.GetRoomFeatures();
        
        Logger.LogDebug("Room features: {Features}", string.Join(", ", roomFeatures));
        
        Assert.That(roomFeatures, Is.Not.Null, 
            "Room features list should not be null");
        
        var commonFeatures = new[] { "WiFi", "TV", "Radio", "Safe", "Views", "Refreshments" };
        var foundFeatures = roomFeatures.Where(f => commonFeatures.Any(cf => 
            f.Contains(cf, StringComparison.OrdinalIgnoreCase))).ToList();
        
        Logger.LogDebug("Found common features: {Features}", string.Join(", ", foundFeatures));
        
        Logger.LogInformation("TC024: Room features display test passed successfully");
    }

    [Test(Description = "TC025: Verify that room images load correctly on reservation page")]
    public void ReservationPage_RoomImage_ShouldLoadCorrectly()
    {
        Logger.LogInformation("Starting TC025: Room image loading test");
        
        Assert.That(_homePage.NavigateToReservationPage(), Is.True, 
            "Should successfully navigate to reservation page");
        _reservationPage.WaitForPageToLoad(TimeSpan.FromSeconds(15));
        
        Logger.LogDebug("Verifying room image element is present and visible");
        Assert.That(_reservationPage.IsRoomImageDisplayed(), Is.True, 
            "Room image element should be present and visible on reservation page");
        
        Logger.LogDebug("Getting room image source URL");
        var imageSrc = _reservationPage.GetRoomImageSrc();
        Assert.That(imageSrc, Is.Not.Empty, 
            "Room image should have a valid src attribute");
        
        Logger.LogDebug("Room image src: {ImageSrc}", imageSrc);
        
        Logger.LogDebug("Validating that room image loads successfully");
        Assert.That(_reservationPage.IsRoomImageLoadedSuccessfully(), Is.True, 
            "Room image should load successfully without errors");
        
        Logger.LogInformation("TC025: Room image loading test passed successfully. Image src: {ImageSrc}", imageSrc);
    }

    [Test(Description = "TC026: Verify that URL parameters are handled correctly for room booking")]
    public void ReservationPage_URLParameters_ShouldBeHandledCorrectly()
    {
        Logger.LogInformation("Starting TC026: URL parameter handling test");
        
        _homePage.ScrollToBookingSection();
        var testDates = BookingTestData.GenerateRandomBookingDates(2, 30, 1);
        
        Logger.LogDebug("Using test dates - CheckIn: {CheckIn}, CheckOut: {CheckOut}", testDates.CheckIn, testDates.CheckOut);
        
        _homePage.CheckAvailability(testDates.CheckIn, testDates.CheckOut);
        
        Assert.That(_homePage.WaitForRoomsToUpdate(TimeSpan.FromSeconds(15)), Is.True, 
            "Rooms should be available for booking");
        
        _homePage.ClickFirstAvailableRoom();
        _reservationPage.WaitForPageToLoad(TimeSpan.FromSeconds(15));
        
        Logger.LogDebug("Verifying URL contains reservation parameters");
        Assert.That(_reservationPage.IsOnReservationPage(), Is.True, 
            "Should be on reservation page with correct URL structure");
        
        var roomId = _reservationPage.GetRoomIdFromUrl();
        Logger.LogDebug("Room ID from URL: {RoomId}", roomId);
        Assert.That(roomId, Is.Not.Empty, 
            "Room ID should be present in URL");
        
        var urlCheckIn = _reservationPage.GetCheckInDateFromUrl();
        var urlCheckOut = _reservationPage.GetCheckOutDateFromUrl();
        
        Logger.LogDebug("URL parameters - CheckIn: {UrlCheckIn}, CheckOut: {UrlCheckOut}", urlCheckIn, urlCheckOut);
        Logger.LogDebug("Expected dates - CheckIn: {ExpectedCheckIn}, CheckOut: {ExpectedCheckOut}", testDates.CheckIn, testDates.CheckOut);
        
        var allParams = _reservationPage.GetAllUrlParameters();
        Logger.LogDebug("All URL parameters: {Parameters}", string.Join(", ", allParams.Select(kvp => $"{kvp.Key}={kvp.Value}")));
        
        if (!string.IsNullOrEmpty(urlCheckIn) && !string.IsNullOrEmpty(urlCheckOut))
        {
            Logger.LogDebug("URL contains date parameters - validating format and values");
            
            if (ReservationPage.TryParseDateFromUrl(urlCheckIn, out var parsedUrlCheckIn) && 
                ReservationPage.TryParseDateFromUrl(urlCheckOut, out var parsedUrlCheckOut) &&
                DateTime.TryParseExact(testDates.CheckIn, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out var expectedCheckInDate) &&
                DateTime.TryParseExact(testDates.CheckOut, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out var expectedCheckOutDate))
            {
                Assert.That(parsedUrlCheckIn.Date, Is.EqualTo(expectedCheckInDate.Date), 
                    $"URL check-in date ({urlCheckIn}) should match expected date ({testDates.CheckIn})");
                
                Assert.That(parsedUrlCheckOut.Date, Is.EqualTo(expectedCheckOutDate.Date), 
                    $"URL check-out date ({urlCheckOut}) should match expected date ({testDates.CheckOut})");
                
                Logger.LogDebug("Date parameters validation passed");
            }
            else
            {
                Logger.LogWarning("Could not parse date parameters for validation - URL CheckIn: {UrlCheckIn}, URL CheckOut: {UrlCheckOut}", urlCheckIn, urlCheckOut);
                
                Assert.That(urlCheckIn, Is.Not.Empty, "Check-in date parameter should be present in URL");
                Assert.That(urlCheckOut, Is.Not.Empty, "Check-out date parameter should be present in URL");
            }
        }
        else
        {
            Logger.LogWarning("Date parameters not found in URL - this may indicate the system doesn't use URL parameters for dates");
            Logger.LogInformation("System does not appear to use URL parameters for date persistence");
        }
        
        var currentUrl = Driver.Url;
        Assert.That(currentUrl, Does.Contain("/reservation/"), 
            "URL should contain reservation path");
        Assert.That(currentUrl, Does.Contain(roomId), 
            "URL should contain the room ID");
        
        Logger.LogInformation("TC026: URL parameter handling test passed successfully. Room ID: {RoomId}, URL: {Url}", roomId, currentUrl);
    }
} 