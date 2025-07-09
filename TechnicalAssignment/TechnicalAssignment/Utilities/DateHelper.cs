using System;
using Microsoft.Extensions.Logging;
using TechnicalAssignment.Utilities;

namespace TechnicalAssignment.Utilities;

public static class DateHelper
{
    private static readonly ILogger Logger = LoggingHelper.CreateLogger(nameof(DateHelper));

    public static (string CheckIn, string CheckOut) GenerateRandomTestDates(int stayDuration = 1)
    {
        var random = new Random();
        
        // Start from 1 day from now to avoid same-day issues
        var daysFromNow = random.Next(1, 365);
        
        var checkIn = DateTime.Now.AddDays(daysFromNow);
        var checkOut = checkIn.AddDays(stayDuration);
        
        var checkInString = checkIn.ToString("dd/MM/yyyy");
        var checkOutString = checkOut.ToString("dd/MM/yyyy");
        
        Logger.LogDebug("Generated random test dates: CheckIn={CheckIn}, CheckOut={CheckOut} (Days from now: {DaysFromNow})", 
            checkInString, checkOutString, daysFromNow);
        
        return (checkInString, checkOutString);
    }

    public static (string CheckIn, string CheckOut) GenerateRandomTestDates(int minDaysFromNow, int maxDaysFromNow, int stayDuration = 1)
    {
        var random = new Random();
        var daysFromNow = random.Next(minDaysFromNow, maxDaysFromNow);
        
        var checkIn = DateTime.Now.AddDays(daysFromNow);
        var checkOut = checkIn.AddDays(stayDuration);
        
        var checkInString = checkIn.ToString("dd/MM/yyyy");
        var checkOutString = checkOut.ToString("dd/MM/yyyy");
        
        Logger.LogDebug("Generated random test dates: CheckIn={CheckIn}, CheckOut={CheckOut} (Days from now: {DaysFromNow})", 
            checkInString, checkOutString, daysFromNow);
        
        return (checkInString, checkOutString);
    }

    public static (string CheckIn, string CheckOut) GenerateTestDates(int daysFromNow = 1, int stayDuration = 1)
    {
        var checkIn = DateTime.Now.AddDays(daysFromNow);
        var checkOut = checkIn.AddDays(stayDuration);
        
        var checkInString = checkIn.ToString("dd/MM/yyyy");
        var checkOutString = checkOut.ToString("dd/MM/yyyy");
        
        Logger.LogDebug("Generated test dates: CheckIn={CheckIn}, CheckOut={CheckOut} (Days from now: {DaysFromNow})", 
            checkInString, checkOutString, daysFromNow);
        
        return (checkInString, checkOutString);
    }
} 