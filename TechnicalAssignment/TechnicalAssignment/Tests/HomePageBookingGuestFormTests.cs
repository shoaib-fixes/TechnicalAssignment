using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using OpenQA.Selenium;
using TechnicalAssignment.Pages;
using TechnicalAssignment.Utilities;
using TechnicalAssignment.Models;

namespace TechnicalAssignment.Tests;

[TestFixture]
[Category("Booking")]
[Category("GuestForm")]
[Parallelizable(ParallelScope.Fixtures)]
public class HomePageBookingGuestFormTests : BaseTest
{
    private HomePage _homePage = null!;
    private ReservationPage _reservationPage = null!;

    [SetUp]
    public void PageSetup()
    {
        Logger.LogInformation("Setting up HomePageBookingGuestFormTests");
        Driver.Navigate().GoToUrl(TestConfig.BaseUrl);
        _homePage = GetService<HomePage>();
        _homePage.WaitForPageToLoad();
        _reservationPage = GetService<ReservationPage>();
    }
    
    [Test(Description = "TC014: Verify that submitting empty form shows appropriate validation errors")]
    public void GuestForm_WithEmptyFields_ShouldShowValidationErrors()
    {
        Logger.LogInformation("Starting TC014: Empty form validation test");
        
        var (checkIn, checkOut) = DateHelper.GenerateRandomTestDates();
        _homePage.BookingForm.CheckAvailability(checkIn, checkOut);
        Assert.That(_homePage.RoomList.WaitForRoomsToUpdate(TimeSpan.FromSeconds(15)), Is.True,
            "Rooms should be available for the selected dates");
        
        Assert.That(_homePage.RoomList.NavigateToReservationPage(), Is.True, 
            "Should successfully navigate to reservation page");
        _reservationPage.WaitForPageToLoad(TimeSpan.FromSeconds(15));
        _reservationPage.NavigateToGuestForm();
        
        Logger.LogDebug("Submitting empty form");
        _reservationPage.SubmitGuestForm();
        
        Logger.LogDebug("Waiting for validation errors");
        WaitHelper.WaitForCondition(Driver, _ => _reservationPage.AreValidationErrorsVisible(), TimeSpan.FromSeconds(5));
        
        Logger.LogDebug("Verifying validation errors are displayed");
        Assert.That(_reservationPage.AreValidationErrorsVisible(), Is.True, 
            "Validation errors should be displayed for empty form");
        
        var errors = _reservationPage.GetValidationErrors();
        Logger.LogDebug("Validation errors count: {Count}", errors.Count);
        
        Assert.That(errors.Count, Is.GreaterThan(0), 
            "At least one validation error should be displayed");
        
        Logger.LogInformation("TC014: Empty form validation test passed successfully");
    }

    [TestCaseSource(typeof(BookingTestData), nameof(BookingTestData.FieldValidationTestCases))]
    [Test(Description = "TC015: Verify validation for individual fields with specific invalid data")]
    public void GuestForm_WithInvalidFieldData_ShouldShowValidationError(string fieldName, string firstName, string lastName, string email, string phone)
    {
        Logger.LogInformation("Starting TC015: {FieldName} validation test", fieldName);
        
        var (checkIn, checkOut) = DateHelper.GenerateRandomTestDates();
        _homePage.BookingForm.CheckAvailability(checkIn, checkOut);
        Assert.That(_homePage.RoomList.WaitForRoomsToUpdate(TimeSpan.FromSeconds(15)), Is.True,
            "Rooms should be available for the selected dates");
        
        Assert.That(_homePage.RoomList.NavigateToReservationPage(), Is.True, 
            "Should successfully navigate to reservation page");
        _reservationPage.WaitForPageToLoad(TimeSpan.FromSeconds(15));
        _reservationPage.NavigateToGuestForm();
        
        _reservationPage.FillGuestInformation(firstName, lastName, email, phone);
        _reservationPage.SubmitGuestForm();
        
        var result = _reservationPage.WaitForValidationOrConfirmation($"TC015: {fieldName}");

        Assert.Multiple(() =>
        {
            Assert.That(result.HasValidationError, Is.True, 
                $"Validation errors should be displayed for invalid {fieldName}.");
            Assert.That(result.HasBookingConfirmation, Is.False, 
                $"Booking should NOT be confirmed with invalid {fieldName}.");
            Assert.That(result.ValidationErrors, Is.Not.Empty, 
                $"Validation error list should not be empty for invalid {fieldName}.");
        });
        
        Logger.LogInformation("TC015: {FieldName} validation test passed", fieldName);
    }

    [TestCaseSource(typeof(BookingTestData), nameof(BookingTestData.BoundaryValidationTestCases))]
    [Test(Description = "TC015b: Verify validation for boundary cases (empty, whitespace, too long)")]
    public void GuestForm_WithBoundaryData_ShouldShowValidationError(string fieldName, string firstName, string lastName, string email, string phone)
    {
        Logger.LogInformation("Starting TC015b: {FieldName} boundary validation test", fieldName);
        
        var (checkIn, checkOut) = DateHelper.GenerateRandomTestDates();
        _homePage.BookingForm.CheckAvailability(checkIn, checkOut);
        Assert.That(_homePage.RoomList.WaitForRoomsToUpdate(TimeSpan.FromSeconds(15)), Is.True,
            "Rooms should be available for the selected dates");

        Assert.That(_homePage.RoomList.NavigateToReservationPage(), Is.True, 
            "Should successfully navigate to reservation page");
        _reservationPage.WaitForPageToLoad(TimeSpan.FromSeconds(15));
        _reservationPage.NavigateToGuestForm();
        
        _reservationPage.FillGuestInformation(firstName, lastName, email, phone);
        _reservationPage.SubmitGuestForm();
        
        var result = _reservationPage.WaitForValidationOrConfirmation($"TC015b: {fieldName}");
        
        Assert.Multiple(() =>
        {
            Assert.That(result.HasValidationError, Is.True, 
                $"Validation errors should be displayed for boundary case {fieldName}.");
            Assert.That(result.HasBookingConfirmation, Is.False, 
                $"Booking should NOT be confirmed with boundary case {fieldName}.");
            Assert.That(result.ValidationErrors, Is.Not.Empty, 
                $"Validation error list should not be empty for boundary case {fieldName}.");
        });
        
        Logger.LogInformation("TC015b: {FieldName} boundary validation test passed", fieldName);
    }

    [Test(Description = "TC016: Verify complete end-to-end booking workflow with valid guest information")]
    public void EndToEndBooking_WithValidGuestData_ShouldCompleteSuccessfully()
    {
        Logger.LogInformation("Starting TC016: End-to-end booking workflow test");
        
        var (checkIn, checkOut) = DateHelper.GenerateRandomTestDates();
        _homePage.BookingForm.CheckAvailability(checkIn, checkOut);
        Assert.That(_homePage.RoomList.WaitForRoomsToUpdate(TimeSpan.FromSeconds(15)), Is.True,
            "Rooms should be available for the selected dates");

        Assert.That(_homePage.RoomList.NavigateToReservationPage(), Is.True, 
            "Should successfully navigate to reservation page");
        _reservationPage.WaitForPageToLoad(TimeSpan.FromSeconds(15));
        _reservationPage.NavigateToGuestForm();
        
        Logger.LogDebug("Filling form with valid data");
        var guestInfo = BookingTestData.GenerateRandomGuest();
        
        _reservationPage.FillGuestInformation(guestInfo.FirstName, guestInfo.LastName, guestInfo.Email, guestInfo.Phone);
        _reservationPage.SubmitGuestForm();
        
        Logger.LogDebug("Waiting for booking confirmation");
        _reservationPage.WaitForConfirmationState();
        
        Logger.LogDebug("Verifying booking confirmation");
        Assert.That(_reservationPage.IsBookingConfirmed(), Is.True, 
            "Booking should be confirmed with valid guest information");
        
        Logger.LogInformation("TC016: End-to-end booking workflow test passed successfully");
    }

    [Test(Description = "TC018: Verify that Cancel button returns to previous state")]
    public void GuestForm_CancelButton_ShouldReturnToPreviousState()
    {
        Logger.LogInformation("Starting TC018: Cancel button functionality test");
        
        var (checkIn, checkOut) = DateHelper.GenerateRandomTestDates();
        _homePage.BookingForm.CheckAvailability(checkIn, checkOut);
        Assert.That(_homePage.RoomList.WaitForRoomsToUpdate(TimeSpan.FromSeconds(15)), Is.True,
            "Rooms should be available for the selected dates");

        Assert.That(_homePage.RoomList.NavigateToReservationPage(), Is.True, 
            "Should successfully navigate to reservation page");
        _reservationPage.WaitForPageToLoad(TimeSpan.FromSeconds(15));
        _reservationPage.NavigateToGuestForm();
        
        Logger.LogDebug("Filling some information in the form");
        _reservationPage.FillGuestInformation("Test", "User", "test@example.com", "07123456789");
        
        Logger.LogDebug("Clicking Cancel button");
        _reservationPage.ClickCancel();
        
        Logger.LogDebug("Waiting for return to booking state");
        _reservationPage.WaitForBookingState();
        
        Logger.LogDebug("Verifying return to previous state");
        Assert.That(_reservationPage.IsGuestFormVisible(), Is.False, 
            "Guest form should be closed after clicking Cancel");
        
        Logger.LogInformation("TC018: Cancel button functionality test passed successfully");
    }

    [Test(Description = "TC019: Verify that booking a room removes those dates from availability for subsequent bookings")]
    public void DoubleBooking_WhenRoomBooked_ShouldRemoveDatesFromAvailability()
    {
        Logger.LogInformation("Starting TC019: Double booking prevention - room availability test");
        
        Logger.LogDebug("Step 1: Completing initial booking");
        
        Driver.Navigate().GoToUrl(TestConfig.BaseUrl);
        _homePage.WaitForPageToLoad(TimeSpan.FromSeconds(10));
        _homePage.BookingForm.ScrollToBookingSection();
        
        var testDates = DateHelper.GenerateRandomTestDates(1, 30, 1);
        
        Logger.LogDebug("Using test dates: Check-in {CheckIn}, Check-out {CheckOut}", testDates.CheckIn, testDates.CheckOut);
        
        _homePage.BookingForm.CheckAvailability(testDates.CheckIn, testDates.CheckOut);
        
        var checkInDate = DateTime.ParseExact(testDates.CheckIn, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
        var checkOutDate = DateTime.ParseExact(testDates.CheckOut, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
        Assert.That(_homePage.RoomList.WaitForRoomsToUpdate(TimeSpan.FromSeconds(15)), Is.True, 
            "Rooms should be available for the selected dates");
        
        var availableRoomsBefore = _homePage.RoomList.GetAvailableRoomTypesWithRetry("before booking");
        
        Assert.That(availableRoomsBefore.Count, Is.GreaterThan(0), "Should have available rooms before booking");
        
        var roomTypeToBook = availableRoomsBefore[0];
        Logger.LogDebug("Booking room type: {RoomType}", roomTypeToBook);
        
        _homePage.RoomList.ClickRoomByType(roomTypeToBook);
        _reservationPage.WaitForPageToLoad(TimeSpan.FromSeconds(10));
        
        var bookedRoomTitle = _reservationPage.GetRoomTitle();
        var bookedRoomPriceText = _reservationPage.GetRoomPrice();
        var bookedRoomFeatures = _reservationPage.GetRoomFeatures();
        
        var bookedRoomPrice = PriceHelper.ExtractPriceValueAsInt(bookedRoomPriceText);
        
        Logger.LogDebug("Booking specific room - Title: {Title}, Price: £{Price}, Features: {Features}", 
            bookedRoomTitle, bookedRoomPrice, string.Join(", ", bookedRoomFeatures));
        
        var guestInfo = BookingTestData.GenerateRandomGuest();
        _reservationPage.CompleteBooking(guestInfo.FirstName, guestInfo.LastName, guestInfo.Email, guestInfo.Phone);
        
        _reservationPage.WaitForConfirmationState();
        Assert.That(_reservationPage.IsBookingConfirmed(), Is.True, 
            "First booking should be confirmed successfully");
        
        Logger.LogDebug("Step 1 completed: Successfully booked {Title} (£{Price}) for {CheckIn} to {CheckOut}", 
            bookedRoomTitle, bookedRoomPrice, checkInDate, checkOutDate);
        
        Logger.LogDebug("Step 2: Checking availability for the same dates after booking");
        
        Driver.Navigate().GoToUrl(TestConfig.BaseUrl);
        _homePage.WaitForPageToLoad(TimeSpan.FromSeconds(10));
        _homePage.BookingForm.ScrollToBookingSection();
        
        _homePage.BookingForm.SetCheckInDate(testDates.CheckIn);
        _homePage.BookingForm.SetCheckOutDate(testDates.CheckOut);
        _homePage.BookingForm.ClickCheckAvailability();

        try
        {
            _homePage.RoomList.WaitForRoomsToUpdate(TimeSpan.FromSeconds(10));
        }
        catch (System.TimeoutException)
        {
            Logger.LogDebug("No rooms available after booking - this is expected behavior");
        }
        
        var availableRoomsAfter = _homePage.RoomList.GetAvailableRoomTypesWithRetry("after booking");
        
        var specificRoomStillAvailable = _homePage.RoomList.FindRoomCard(bookedRoomTitle, bookedRoomPrice, bookedRoomFeatures);
        
        if (specificRoomStillAvailable == null)
        {
            Logger.LogInformation("SUCCESS: The specific room that was booked ({Title}, £{Price}) is no longer available for the same dates", 
                bookedRoomTitle, bookedRoomPrice);
            
            Assert.That(specificRoomStillAvailable, Is.Null, 
                $"The specific room ({bookedRoomTitle}, £{bookedRoomPrice}) should not be available for double booking");
        }
        else
        {
            Logger.LogError("The specific room that was already booked is still available for booking on the same dates!");
            Logger.LogError("Booked room details - Title: {Title}, Price: £{Price}, Features: {Features}", 
                bookedRoomTitle, bookedRoomPrice, string.Join(", ", bookedRoomFeatures));
            Logger.LogError("Available rooms after booking: {AvailableRooms}", string.Join(", ", availableRoomsAfter));
            
            ScreenshotHelper.CaptureScreenshot(Driver, "DoubleBooking_CriticalBug_SameRoomAvailable");
            
            Assert.Fail($"The specific room ({bookedRoomTitle}, £{bookedRoomPrice}) that was already booked " +
                $"is still available for booking on the same dates ({testDates.CheckIn} to {testDates.CheckOut}). " +
                "This indicates a serious double booking vulnerability in the system.");
        }
        
        Logger.LogInformation("TC019: Double booking prevention test passed successfully");
    }

    [Test(Description = "TC020: Verify that attempting to force a double booking by URL manipulation shows validation error")]
    public void AttemptForcedDoubleBooking_ShouldShowValidationError()
    {
        Logger.LogInformation("Starting TC020: Forced double booking attempt test");
        
        string bookingUrl = "";
        string roomId = "";
        string checkInDate = "";
        string checkOutDate = "";
        
        Logger.LogDebug("Step 1: Completing initial booking to get URL pattern");
        
        Driver.Navigate().GoToUrl(TestConfig.BaseUrl);
        _homePage.WaitForPageToLoad(TimeSpan.FromSeconds(10));
        _homePage.BookingForm.ScrollToBookingSection();
        
        var testDates = DateHelper.GenerateRandomTestDates(1, 30, 1);
        
        Logger.LogDebug("Using test dates: Check-in {CheckIn}, Check-out {CheckOut}", testDates.CheckIn, testDates.CheckOut);
        
        _homePage.BookingForm.CheckAvailability(testDates.CheckIn, testDates.CheckOut);
        
        checkInDate = DateTime.ParseExact(testDates.CheckIn, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
        checkOutDate = DateTime.ParseExact(testDates.CheckOut, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
        Assert.That(_homePage.RoomList.WaitForRoomsToUpdate(TimeSpan.FromSeconds(15)), Is.True, 
            "Rooms should be available for the selected dates");
        
        var availableRooms = _homePage.RoomList.GetAvailableRoomTypesWithRetry("for forced double booking");
        Assert.That(availableRooms.Count, Is.GreaterThan(0), "Should have available rooms for booking");
        
        var roomTypeToBook = availableRooms[0];
        Logger.LogDebug("Booking room type: {RoomType}", roomTypeToBook);
        
        _homePage.RoomList.ClickRoomByType(roomTypeToBook);
        _reservationPage.WaitForPageToLoad(TimeSpan.FromSeconds(10));
        
        bookingUrl = Driver.Url;
        roomId = _reservationPage.GetRoomIdFromUrl();
        Logger.LogDebug("Captured booking URL: {Url}", bookingUrl);
        Logger.LogDebug("Room ID: {RoomId}", roomId);
        
        var guestInfo1 = BookingTestData.GenerateRandomGuest();
        _reservationPage.CompleteBooking(guestInfo1.FirstName, guestInfo1.LastName, guestInfo1.Email, guestInfo1.Phone);
        
        _reservationPage.WaitForConfirmationState();
        Assert.That(_reservationPage.IsBookingConfirmed(), Is.True, 
            "First booking should be confirmed successfully");
        
        Logger.LogDebug("Step 1 completed: Successfully booked room {RoomId} for {CheckIn} to {CheckOut}", 
            roomId, checkInDate, checkOutDate);
        
        Logger.LogDebug("Step 2: Attempting forced double booking via URL manipulation");
        
        var forcedBookingUrl = $"{TestConfig.BaseUrl.TrimEnd('/')}/reservation/{roomId}?checkin={checkInDate}&checkout={checkOutDate}";
        Logger.LogDebug("Navigating to forced booking URL: {Url}", forcedBookingUrl);
        
        Driver.Navigate().GoToUrl(forcedBookingUrl);
        _reservationPage.WaitForPageToLoad(TimeSpan.FromSeconds(10));
        
        Assert.That(_reservationPage.IsOnReservationPage(), Is.True, 
            "Should be on reservation page after URL navigation");
        
        Logger.LogDebug("Step 3: Attempting to complete the double booking");
        
        _reservationPage.ClickReserveNow();
        _reservationPage.WaitForGuestFormState();
        
        var guestInfo2 = BookingTestData.GenerateRandomGuest();
        _reservationPage.FillGuestInformation(guestInfo2.FirstName, guestInfo2.LastName, guestInfo2.Email, guestInfo2.Phone);
        
        Logger.LogDebug("Submitting guest form for double booking attempt");
        _reservationPage.SubmitGuestForm();
        
        Logger.LogDebug("Step 4: Checking for validation error or unexpected booking confirmation");
        
        var validationResult = _reservationPage.WaitForValidationOrConfirmation("Forced double booking attempt");
        
        if (validationResult.HasValidationError && !validationResult.HasBookingConfirmation)
        {
            Logger.LogInformation("SUCCESS: Double booking correctly prevented with validation error");
            Logger.LogDebug("Validation errors: {Errors}", string.Join(", ", validationResult.ValidationErrors));
            
            Assert.That(validationResult.HasValidationError, Is.True, 
                "Should show validation error when attempting double booking");
            Assert.That(validationResult.ValidationErrors.Count, Is.GreaterThan(0), 
                "Should have specific validation error messages");
        }
        else if (validationResult.HasBookingConfirmation)
        {
            Logger.LogError("BUG DETECTED: Double booking was allowed! The system incorrectly accepted a booking for already booked dates.");
            ScreenshotHelper.CaptureScreenshot(Driver, "DoubleBooking_Bug_Detected");
            
            Assert.Fail("CRITICAL BUG: Double booking was allowed. The system should prevent booking the same room for the same dates twice. " +
                $"Room {roomId} was successfully booked twice for {checkInDate} to {checkOutDate}.");
        }
        else
        {
            Logger.LogWarning("Unexpected state: Neither validation error nor booking confirmation detected");
            ScreenshotHelper.CaptureScreenshot(Driver, "DoubleBooking_Unexpected_State");
            
            Assert.Fail("Unexpected result when attempting double booking. Expected validation error but got neither error nor confirmation.");
        }
        
        Logger.LogInformation("TC020: Forced double booking attempt test passed successfully");
    }

    [Test(Description = "TC021: Verify that Return Home button navigates back to homepage")]
    public void BookingConfirmation_ReturnHomeButton_ShouldNavigateToHomepage()
    {
        Logger.LogInformation("Starting TC021: Return Home button functionality test");
        
        var (checkIn, checkOut) = DateHelper.GenerateRandomTestDates();
        _homePage.BookingForm.CheckAvailability(checkIn, checkOut);
        Assert.That(_homePage.RoomList.WaitForRoomsToUpdate(TimeSpan.FromSeconds(15)), Is.True,
            "Rooms should be available for the selected dates");

        Assert.That(_homePage.RoomList.NavigateToReservationPage(), Is.True, 
            "Should successfully navigate to reservation page");
        _reservationPage.WaitForPageToLoad(TimeSpan.FromSeconds(15));
        
        var guestInfo = BookingTestData.GenerateRandomGuest();
        Assert.That(_reservationPage.CompleteBookingWorkflow(guestInfo.FirstName, guestInfo.LastName, guestInfo.Email, guestInfo.Phone), Is.True, 
            "Should successfully complete booking workflow");

        Assert.That(_reservationPage.IsBookingConfirmed(), Is.True,
            "Booking must be confirmed before testing the Return Home button");
        
        Logger.LogDebug("Clicking Return Home button");
        _reservationPage.ClickReturnHome();
        
        Logger.LogDebug("Waiting for homepage to load");
        _homePage.WaitForPageToLoad(TimeSpan.FromSeconds(10));
        
        Logger.LogDebug("Verifying navigation to homepage");
        Assert.That(_homePage.IsOnHomePage(), Is.True, 
            "Should navigate back to homepage after clicking Return Home");
        
        Logger.LogInformation("TC021: Return Home button functionality test passed successfully");
    }
} 