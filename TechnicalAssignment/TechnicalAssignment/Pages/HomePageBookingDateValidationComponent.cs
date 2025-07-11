using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using TechnicalAssignment.Utilities;
using TechnicalAssignment.Models;

namespace TechnicalAssignment.Pages;

public class HomePageBookingDateValidationComponent
{
    protected readonly IWebDriver Driver;
    protected readonly ILogger Logger;
    
    private static readonly By CheckInDatePicker = By.CssSelector("#booking .col-md-6:nth-child(1) input.form-control");
    private static readonly By CheckOutDatePicker = By.CssSelector("#booking .col-md-6:nth-child(2) input.form-control");
    private static readonly By CheckAvailabilityButton = By.CssSelector("#booking button.btn-primary");
    private static readonly By RoomCards = By.CssSelector(".room-card");

    public HomePageBookingDateValidationComponent(IWebDriver driver, ILogger logger)
    {
        Driver = driver;
        Logger = logger;
    }

    public string GetCheckInDate()
    {
        Logger.LogDebug("Getting check-in date");
        try
        {
            var checkInElement = WaitHelper.WaitForElement(Driver, CheckInDatePicker, TimeSpan.FromSeconds(10));
            var checkInValue = checkInElement.GetAttribute("value") ?? "";
            Logger.LogDebug("Check-in date value: {CheckInValue}", checkInValue);
            return checkInValue;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error getting check-in date");
            return "";
        }
    }

    public string GetCheckOutDate()
    {
        Logger.LogDebug("Getting check-out date");
        try
        {
            var checkOutElement = WaitHelper.WaitForElement(Driver, CheckOutDatePicker, TimeSpan.FromSeconds(10));
            var checkOutValue = checkOutElement.GetAttribute("value") ?? "";
            Logger.LogDebug("Check-out date value: {CheckOutValue}", checkOutValue);
            return checkOutValue;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error getting check-out date");
            return "";
        }
    }

    public void SetCheckInDate(string date)
    {
        Logger.LogDebug("Setting check-in date to: {Date}", date);
        try
        {
            var checkInElement = WaitHelper.WaitForElement(Driver, CheckInDatePicker, TimeSpan.FromSeconds(10));
            checkInElement.Clear();
            checkInElement.SendKeys(date);
            Logger.LogDebug("Check-in date set successfully");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error setting check-in date to {Date}", date);
        }
    }

    public void SetCheckOutDate(string date)
    {
        Logger.LogDebug("Setting check-out date to: {Date}", date);
        try
        {
            var checkOutElement = WaitHelper.WaitForElement(Driver, CheckOutDatePicker, TimeSpan.FromSeconds(10));
            checkOutElement.Clear();
            checkOutElement.SendKeys(date);
            Logger.LogDebug("Check-out date set successfully");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error setting check-out date to {Date}", date);
        }
    }

    public void ClickCheckAvailability()
    {
        Logger.LogDebug("Clicking check availability button");
        ElementHelper.SafeClick(Driver, CheckAvailabilityButton);
    }

    public int GetAvailableRoomsCount()
    {
        Logger.LogDebug("Getting available rooms count");
        var roomCards = Driver.FindElements(RoomCards);
        var count = roomCards.Count;
        Logger.LogDebug("Found {Count} available rooms", count);
        return count;
    }

    public bool WaitForRoomsToUpdate(TimeSpan? timeout = null)
    {
        Logger.LogDebug("Waiting for rooms to update");
        try
        {
            WaitHelper.WaitForCondition(Driver, 
                d => d.FindElements(RoomCards).Count >= 0, 
                timeout ?? TimeSpan.FromSeconds(10));
            return true;
        }
        catch (Exception ex)
        {
            Logger.LogWarning(ex, "Timeout waiting for rooms to update");
            return false;
        }
    }

    public bool IsCheckOutDateAfterCheckIn()
    {
        Logger.LogDebug("Checking if check-out date is after check-in date");
        
        var checkInDate = GetCheckInDate();
        var checkOutDate = GetCheckOutDate();
        
        return IsCheckOutDateAfterCheckIn(checkInDate, checkOutDate);
    }

    public bool IsCheckOutDateAfterCheckIn(string checkInDate, string checkOutDate)
    {
        Logger.LogDebug("Validating date order - CheckIn: {CheckIn}, CheckOut: {CheckOut}", checkInDate, checkOutDate);
        
        if (string.IsNullOrEmpty(checkInDate) || string.IsNullOrEmpty(checkOutDate))
        {
            Logger.LogDebug("One or both dates are empty");
            return false;
        }
        
        var dateFormats = new[] { "dd/MM/yyyy", "MM/dd/yyyy", "yyyy-MM-dd" };
        DateTime checkIn, checkOut;
        bool checkInParsed = false, checkOutParsed = false;
        
        checkInParsed = DateTime.TryParseExact(checkInDate, dateFormats, System.Globalization.CultureInfo.InvariantCulture, 
            System.Globalization.DateTimeStyles.None, out checkIn);
        
        if (!checkInParsed)
        {
            checkInParsed = DateTime.TryParse(checkInDate, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out checkIn);
        }
        
        checkOutParsed = DateTime.TryParseExact(checkOutDate, dateFormats, System.Globalization.CultureInfo.InvariantCulture, 
            System.Globalization.DateTimeStyles.None, out checkOut);
        
        if (!checkOutParsed)
        {
            checkOutParsed = DateTime.TryParse(checkOutDate, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out checkOut);
        }
        
        if (checkInParsed && checkOutParsed)
        {
            var isValid = checkOut >= checkIn;
            Logger.LogDebug("Date order validation result: {IsValid} (CheckIn: {ParsedCheckIn}, CheckOut: {ParsedCheckOut})", 
                isValid, checkIn.ToString("yyyy-MM-dd"), checkOut.ToString("yyyy-MM-dd"));
            return isValid;
        }
        
        Logger.LogWarning("Could not parse dates for order validation - CheckIn: '{CheckIn}' (Parsed: {CheckInParsed}), CheckOut: '{CheckOut}' (Parsed: {CheckOutParsed})", 
            checkInDate, checkInParsed, checkOutDate, checkOutParsed);
        return false;
    }

    public DateValidationResult ValidateDateInput(string checkInDate, string checkOutDate)
    {
        var result = new DateValidationResult
        {
            IsValid = true,
            CheckInDate = checkInDate,
            CheckOutDate = checkOutDate,
            ValidationErrors = new List<string>()
        };
        
        var dateFormats = new[] { "dd/MM/yyyy" };
        var checkInParsed = DateTime.TryParseExact(checkInDate, dateFormats, System.Globalization.CultureInfo.InvariantCulture, 
            System.Globalization.DateTimeStyles.None, out var checkIn) ||
            DateTime.TryParse(checkInDate, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out checkIn);
        
        var checkOutParsed = DateTime.TryParseExact(checkOutDate, dateFormats, System.Globalization.CultureInfo.InvariantCulture, 
            System.Globalization.DateTimeStyles.None, out var checkOut) ||
            DateTime.TryParse(checkOutDate, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out checkOut);
        
        if (!checkInParsed)
        {
            result.IsValid = false;
            result.ValidationErrors.Add($"Check-in date '{checkInDate}' is not in a valid format");
        }
        
        if (!checkOutParsed)
        {
            result.IsValid = false;
            result.ValidationErrors.Add($"Check-out date '{checkOutDate}' is not in a valid format");
        }
        
        if (checkInParsed && checkOutParsed)
        {
            result.ParsedCheckInDate = checkIn;
            result.ParsedCheckOutDate = checkOut;
            
            if (checkIn < DateTime.Today)
            {
                result.IsValid = false;
                result.ValidationErrors.Add($"Check-in date '{checkInDate}' is in the past");
                result.HasPastCheckIn = true;
            }
            
            if (checkOut < checkIn)
            {
                result.IsValid = false;
                result.ValidationErrors.Add($"Check-out date '{checkOutDate}' is before check-in date '{checkInDate}'");
                result.HasInvalidDateOrder = true;
            }
            
            if (checkOut.Date == checkIn.Date)
            {
                result.IsValid = false;
                result.ValidationErrors.Add($"Check-in and check-out dates cannot be the same day");
                result.HasSameDayBooking = true;
            }
        }
        
        return result;
    }

    public DateValidationResult GetPastDatesHandlingResult()
    {
        var result = new DateValidationResult();
        
        try
        {
            var yesterday = DateTime.Now.AddDays(-1).ToString("dd/MM/yyyy");
            var dayBeforeYesterday = DateTime.Now.AddDays(-2).ToString("dd/MM/yyyy");
            
            Logger.LogDebug("Testing past dates - Setting CheckIn: {CheckIn}, CheckOut: {CheckOut}", dayBeforeYesterday, yesterday);
            
            SetCheckInDate(dayBeforeYesterday);
            SetCheckOutDate(yesterday);
            ClickCheckAvailability();
            
            WaitForRoomsToUpdate(TimeSpan.FromSeconds(10));
            
            var currentCheckIn = GetCheckInDate();
            var currentCheckOut = GetCheckOutDate();
            
            result.CheckInDate = currentCheckIn;
            result.CheckOutDate = currentCheckOut;
            
            Logger.LogDebug("After setting past dates - Current CheckIn: {CheckIn}, CheckOut: {CheckOut}", currentCheckIn, currentCheckOut);
            
            if (DateTime.TryParseExact(currentCheckIn, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture, 
                System.Globalization.DateTimeStyles.None, out var checkInParsed) &&
                DateTime.TryParseExact(currentCheckOut, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture, 
                System.Globalization.DateTimeStyles.None, out var checkOutParsed))
            {
                result.ParsedCheckInDate = checkInParsed;
                result.ParsedCheckOutDate = checkOutParsed;
                
                var roomsCount = GetAvailableRoomsCount();
                bool pastDatesWereAccepted = checkInParsed < DateTime.Today;
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
                    Logger.LogDebug("Application correctly handled past dates - no rooms available");
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
            Logger.LogError(ex, "Error getting past dates handling result");
            result.ValidationErrors.Add($"Error during past dates validation: {ex.Message}");
            result.IsValid = false;
        }
        
        return result;
    }

    public DateValidationResult GetInvalidDateOrderHandlingResult()
    {
        var result = new DateValidationResult();
        
        try
        {
            var nextWeek = DateTime.Now.AddDays(7).ToString("dd/MM/yyyy");
            var tomorrow = DateTime.Now.AddDays(1).ToString("dd/MM/yyyy");
            
            Logger.LogDebug("Testing invalid date order - Setting CheckIn: {CheckIn}, CheckOut: {CheckOut}", nextWeek, tomorrow);
            
            SetCheckInDate(nextWeek);
            SetCheckOutDate(tomorrow);
            ClickCheckAvailability();
            
            WaitForRoomsToUpdate(TimeSpan.FromSeconds(10));
            
            var currentCheckIn = GetCheckInDate();
            var currentCheckOut = GetCheckOutDate();
            
            result.CheckInDate = currentCheckIn;
            result.CheckOutDate = currentCheckOut;
            
            Logger.LogDebug("After setting invalid dates - Current CheckIn: {CheckIn}, CheckOut: {CheckOut}", currentCheckIn, currentCheckOut);
            
            if (DateTime.TryParseExact(currentCheckIn, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture, 
                System.Globalization.DateTimeStyles.None, out var checkInParsed) &&
                DateTime.TryParseExact(currentCheckOut, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture, 
                System.Globalization.DateTimeStyles.None, out var checkOutParsed))
            {
                result.ParsedCheckInDate = checkInParsed;
                result.ParsedCheckOutDate = checkOutParsed;
                
                var roomsCount = GetAvailableRoomsCount();
                bool invalidDatesWereAccepted = checkOutParsed < checkInParsed;
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
                    Logger.LogDebug("Application correctly handled invalid date order - no rooms available");
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
            Logger.LogError(ex, "Error getting invalid date order handling result");
            result.ValidationErrors.Add($"Error during invalid date order validation: {ex.Message}");
            result.IsValid = false;
        }
        
        return result;
    }

    public DateValidationResult GetSameDayBookingHandlingResult()
    {
        var result = new DateValidationResult();
        
        try
        {
            var today = DateTime.Now.ToString("dd/MM/yyyy");
            
            Logger.LogDebug("Testing same-day booking - Setting CheckIn: {CheckIn}, CheckOut: {CheckOut}", today, today);
            
            SetCheckInDate(today);
            SetCheckOutDate(today);
            ClickCheckAvailability();
            
            WaitForRoomsToUpdate(TimeSpan.FromSeconds(10));
            
            var currentCheckIn = GetCheckInDate();
            var currentCheckOut = GetCheckOutDate();
            
            result.CheckInDate = currentCheckIn;
            result.CheckOutDate = currentCheckOut;
            
            Logger.LogDebug("After setting same-day dates - Current CheckIn: {CheckIn}, CheckOut: {CheckOut}", currentCheckIn, currentCheckOut);
            
            if (DateTime.TryParseExact(currentCheckIn, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture, 
                System.Globalization.DateTimeStyles.None, out var checkInParsed) &&
                DateTime.TryParseExact(currentCheckOut, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture, 
                System.Globalization.DateTimeStyles.None, out var checkOutParsed))
            {
                result.ParsedCheckInDate = checkInParsed;
                result.ParsedCheckOutDate = checkOutParsed;
                
                var roomsCount = GetAvailableRoomsCount();
                bool sameDayWasAccepted = checkInParsed.Date == checkOutParsed.Date;
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
                    Logger.LogDebug("Application correctly handled same-day booking - no rooms available");
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
            Logger.LogError(ex, "Error getting same-day booking handling result");
            result.ValidationErrors.Add($"Error during same-day booking validation: {ex.Message}");
            result.IsValid = false;
        }
        
        return result;
    }
} 