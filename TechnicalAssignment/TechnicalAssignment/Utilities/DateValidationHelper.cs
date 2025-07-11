using System;
using System.Collections.Generic;
using TechnicalAssignment.Models;

namespace TechnicalAssignment.Utilities;

/// <summary>
/// Helper class for date validation business logic
/// </summary>
public static class DateValidationHelper
{
    /// <summary>
    /// Validates if check-out date is on or after check-in date
    /// </summary>
    public static bool IsCheckOutDateOnOrAfterCheckIn(DateTime checkIn, DateTime checkOut)
    {
        return checkOut.Date >= checkIn.Date;
    }

    /// <summary>
    /// Validates if a date is in the past
    /// </summary>
    public static bool IsDateInThePast(DateTime date)
    {
        return date.Date < DateTime.Today;
    }

    /// <summary>
    /// Validates if check-in and check-out are on the same day
    /// </summary>
    public static bool IsSameDayBooking(DateTime checkIn, DateTime checkOut)
    {
        return checkIn.Date == checkOut.Date;
    }

    /// <summary>
    /// Validates date input and returns validation result
    /// </summary>
    public static DateValidationResult ValidateDateInput(string checkInDate, string checkOutDate)
    {
        var result = new DateValidationResult
        {
            IsValid = true,
            CheckInDate = checkInDate,
            CheckOutDate = checkOutDate,
            ValidationErrors = new List<string>()
        };
        
        var dateFormats = new[] { "dd/MM/yyyy", "MM/dd/yyyy", "yyyy-MM-dd" };
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
            
            if (IsDateInThePast(checkIn))
            {
                result.IsValid = false;
                result.ValidationErrors.Add($"Check-in date '{checkInDate}' is in the past");
                result.HasPastCheckIn = true;
            }
            
            if (!IsCheckOutDateOnOrAfterCheckIn(checkIn, checkOut))
            {
                result.IsValid = false;
                result.ValidationErrors.Add($"Check-out date '{checkOutDate}' is before check-in date '{checkInDate}'");
                result.HasInvalidDateOrder = true;
            }
            
            if (IsSameDayBooking(checkIn, checkOut))
            {
                result.IsValid = false;
                result.ValidationErrors.Add($"Check-in and check-out dates cannot be the same day");
                result.HasSameDayBooking = true;
            }
        }
        
        return result;
    }

    /// <summary>
    /// Tries to parse a date string using multiple formats
    /// </summary>
    public static bool TryParseDate(string dateString, out DateTime date)
    {
        var dateFormats = new[] { "dd/MM/yyyy", "MM/dd/yyyy", "yyyy-MM-dd", "dd-MM-yyyy", "MM-dd-yyyy" };
        
        var parsed = DateTime.TryParseExact(dateString, dateFormats, System.Globalization.CultureInfo.InvariantCulture, 
            System.Globalization.DateTimeStyles.None, out date);
        
        if (!parsed)
        {
            parsed = DateTime.TryParse(dateString, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out date);
        }
        
        return parsed;
    }
} 