using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using TechnicalAssignment.Utilities;
using TechnicalAssignment.Models;

namespace TechnicalAssignment.Pages;

public class HomePageBookingDateValidationComponent
{
    private readonly IWebDriver _driver;
    private readonly ILogger<HomePageBookingDateValidationComponent> _logger;
    private readonly HomePageBookingFormComponent _bookingForm;
    private readonly HomePageBookingRoomListComponent _roomList;
    
    public HomePageBookingDateValidationComponent(IWebDriver driver, ILogger<HomePageBookingDateValidationComponent> logger,
        HomePageBookingFormComponent bookingForm, HomePageBookingRoomListComponent roomList)
    {
        _driver = driver;
        _logger = logger;
        _bookingForm = bookingForm;
        _roomList = roomList;
    }

    public DateValidationResult ValidatePastDatesHandling()
    {
        return GetPastDatesHandlingResult();
    }

    public DateValidationResult ValidateInvalidDateOrderHandling()
    {
        return GetInvalidDateOrderHandlingResult();
    }

    public DateValidationResult ValidateSameDayBookingHandling()
    {
        return GetSameDayBookingHandlingResult();
    }

    private DateValidationResult GetPastDatesHandlingResult()
    {
        var result = new DateValidationResult();
        
        try
        {
            var yesterday = DateTime.Now.AddDays(-1).ToString("dd/MM/yyyy");
            var dayBeforeYesterday = DateTime.Now.AddDays(-2).ToString("dd/MM/yyyy");
            
            _logger.LogDebug("Testing past dates - Setting CheckIn: {CheckIn}, CheckOut: {CheckOut}", dayBeforeYesterday, yesterday);
            
            _bookingForm.SetCheckInDate(dayBeforeYesterday);
            _bookingForm.SetCheckOutDate(yesterday);
            _bookingForm.ClickCheckAvailability();
            
            _roomList.WaitForRoomsToUpdate(TimeSpan.FromSeconds(10));
            
            var currentCheckIn = _bookingForm.GetCheckInDate();
            var currentCheckOut = _bookingForm.GetCheckOutDate();
            
            result.CheckInDate = currentCheckIn;
            result.CheckOutDate = currentCheckOut;
            
            _logger.LogDebug("After setting past dates - Current CheckIn: {CheckIn}, CheckOut: {CheckOut}", currentCheckIn, currentCheckOut);
            
            if (DateValidationHelper.TryParseDate(currentCheckIn, out var checkInParsed) &&
                DateValidationHelper.TryParseDate(currentCheckOut, out var checkOutParsed))
            {
                result.ParsedCheckInDate = checkInParsed;
                result.ParsedCheckOutDate = checkOutParsed;
                
                var roomsCount = _roomList.GetAvailableRoomsCount();
                bool pastDatesWereAccepted = DateValidationHelper.IsDateInThePast(checkInParsed);
                result.HasPastCheckIn = pastDatesWereAccepted;
                
                if (roomsCount > 0)
                {
                    result.ValidationErrors.Add($"{roomsCount} rooms are available after attempting past dates");
                    result.ValidationErrors.Add($"Original invalid input: CheckIn={dayBeforeYesterday}, CheckOut={yesterday}");
                    result.ValidationErrors.Add($"Current form values: CheckIn={currentCheckIn}, CheckOut={currentCheckOut}");
                    
                    if (pastDatesWereAccepted)
                    {
                        result.ValidationErrors.Add($"Past dates were literally accepted in form fields");
                    }
                    else
                    {
                        result.ValidationErrors.Add($"Application auto-corrected dates but still shows availability (should show error/no availability)");
                    }
                    
                    result.IsValid = false;
                }
                else
                {
                    _logger.LogDebug("Application correctly handled past dates - no rooms available");
                    result.IsValid = true;
                }
            }
            else
            {
                result.ValidationErrors.Add($"Could not parse dates after past dates test. CheckIn: {currentCheckIn}, CheckOut: {currentCheckOut}");
                result.IsValid = false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting past dates handling result");
            result.ValidationErrors.Add($"Error during past dates validation: {ex.Message}");
            result.IsValid = false;
        }
        
        return result;
    }

    private DateValidationResult GetInvalidDateOrderHandlingResult()
    {
        var result = new DateValidationResult();
        
        try
        {
            var nextWeek = DateTime.Now.AddDays(7).ToString("dd/MM/yyyy");
            var tomorrow = DateTime.Now.AddDays(1).ToString("dd/MM/yyyy");
            
            _logger.LogDebug("Testing invalid date order - Setting CheckIn: {CheckIn}, CheckOut: {CheckOut}", nextWeek, tomorrow);
            
            _bookingForm.SetCheckInDate(nextWeek);
            _bookingForm.SetCheckOutDate(tomorrow);
            _bookingForm.ClickCheckAvailability();
            
            _roomList.WaitForRoomsToUpdate(TimeSpan.FromSeconds(10));
            
            var currentCheckIn = _bookingForm.GetCheckInDate();
            var currentCheckOut = _bookingForm.GetCheckOutDate();
            
            result.CheckInDate = currentCheckIn;
            result.CheckOutDate = currentCheckOut;
            
            _logger.LogDebug("After setting invalid dates - Current CheckIn: {CheckIn}, CheckOut: {CheckOut}", currentCheckIn, currentCheckOut);
            
            if (DateValidationHelper.TryParseDate(currentCheckIn, out var checkInParsed) &&
                DateValidationHelper.TryParseDate(currentCheckOut, out var checkOutParsed))
            {
                result.ParsedCheckInDate = checkInParsed;
                result.ParsedCheckOutDate = checkOutParsed;
                
                var roomsCount = _roomList.GetAvailableRoomsCount();
                bool invalidDatesWereAccepted = !DateValidationHelper.IsCheckOutDateOnOrAfterCheckIn(checkInParsed, checkOutParsed);
                result.HasInvalidDateOrder = invalidDatesWereAccepted;
                
                if (roomsCount > 0)
                {
                    result.ValidationErrors.Add($"{roomsCount} rooms are available after attempting invalid date order");
                    result.ValidationErrors.Add($"Original invalid input: CheckIn={nextWeek}, CheckOut={tomorrow}");
                    result.ValidationErrors.Add($"Current form values: CheckIn={currentCheckIn}, CheckOut={currentCheckOut}");
                    
                    if (invalidDatesWereAccepted)
                    {
                        result.ValidationErrors.Add($"Invalid date order was literally accepted in form fields");
                    }
                    else
                    {
                        result.ValidationErrors.Add($"Application auto-corrected dates but still shows availability (should show error/no availability)");
                    }
                    
                    result.IsValid = false;
                }
                else
                {
                    _logger.LogDebug("Application correctly handled invalid date order - no rooms available");
                    result.IsValid = true;
                }
            }
            else
            {
                result.ValidationErrors.Add($"Could not parse dates after invalid date order test. CheckIn: {currentCheckIn}, CheckOut: {currentCheckOut}");
                result.IsValid = false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting invalid date order handling result");
            result.ValidationErrors.Add($"Error during invalid date order validation: {ex.Message}");
            result.IsValid = false;
        }
        
        return result;
    }

    private DateValidationResult GetSameDayBookingHandlingResult()
    {
        var result = new DateValidationResult();
        
        try
        {
            var today = DateTime.Now.ToString("dd/MM/yyyy");
            
            _logger.LogDebug("Testing same-day booking - Setting CheckIn: {CheckIn}, CheckOut: {CheckOut}", today, today);
            
            _bookingForm.SetCheckInDate(today);
            _bookingForm.SetCheckOutDate(today);
            _bookingForm.ClickCheckAvailability();
            
            _roomList.WaitForRoomsToUpdate(TimeSpan.FromSeconds(10));
            
            var currentCheckIn = _bookingForm.GetCheckInDate();
            var currentCheckOut = _bookingForm.GetCheckOutDate();
            
            result.CheckInDate = currentCheckIn;
            result.CheckOutDate = currentCheckOut;
            
            _logger.LogDebug("After setting same-day dates - Current CheckIn: {CheckIn}, CheckOut: {CheckOut}", currentCheckIn, currentCheckOut);
            
            if (DateValidationHelper.TryParseDate(currentCheckIn, out var checkInParsed) &&
                DateValidationHelper.TryParseDate(currentCheckOut, out var checkOutParsed))
            {
                result.ParsedCheckInDate = checkInParsed;
                result.ParsedCheckOutDate = checkOutParsed;
                
                var roomsCount = _roomList.GetAvailableRoomsCount();
                bool sameDayWasAccepted = DateValidationHelper.IsSameDayBooking(checkInParsed, checkOutParsed);
                result.HasSameDayBooking = sameDayWasAccepted;
                
                if (roomsCount > 0)
                {
                    result.ValidationErrors.Add($"{roomsCount} rooms are available after attempting same-day booking");
                    result.ValidationErrors.Add($"Original invalid input: CheckIn={today}, CheckOut={today}");
                    result.ValidationErrors.Add($"Current form values: CheckIn={currentCheckIn}, CheckOut={currentCheckOut}");
                    
                    if (sameDayWasAccepted)
                    {
                        result.ValidationErrors.Add($"Same-day booking was literally accepted in form fields");
                    }
                    else
                    {
                        result.ValidationErrors.Add($"Application auto-corrected dates but still shows availability (should show error/no availability)");
                    }
                    
                    result.IsValid = false;
                }
                else
                {
                    _logger.LogDebug("Application correctly handled same-day booking - no rooms available");
                    result.IsValid = true;
                }
            }
            else
            {
                result.ValidationErrors.Add($"Could not parse dates after same-day booking test. CheckIn: {currentCheckIn}, CheckOut: {currentCheckOut}");
                result.IsValid = false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting same-day booking handling result");
            result.ValidationErrors.Add($"Error during same-day booking validation: {ex.Message}");
            result.IsValid = false;
        }
        
        return result;
    }
} 