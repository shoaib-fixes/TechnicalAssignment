using System;
using System.Collections.Generic;

namespace TechnicalAssignment.Models;

/// <summary>
/// Contains test data for date validation tests
/// </summary>
public static class DateValidationTestData
{
    /// <summary>
    /// Represents a date validation test case
    /// </summary>
    public record DateValidationCase(string CheckIn, string CheckOut, bool ExpectedValid, string Description, string[] ExpectedErrorKeywords);

    /// <summary>
    /// Gets test cases for various date input validation scenarios
    /// </summary>
    public static object[] GetDateInputValidationTestCases()
    {
        return new object[]
        {
            new object[] { "01/01/2025", "02/01/2025", true, "Valid future dates", new string[0] },
            new object[] { "invalid", "02/01/2025", false, "Invalid check-in format", new[] { "invalid", "format" } },
            new object[] { "01/01/2025", "invalid", false, "Invalid check-out format", new[] { "invalid", "format" } },
            new object[] { "02/01/2025", "01/01/2025", false, "Check-out before check-in", new[] { "invalid", "order", "before" } },
            new object[] { "01/01/2025", "01/01/2025", false, "Same day booking", new[] { "same", "day" } }
        };
    }

    /// <summary>
    /// Gets test cases for past date validation scenarios
    /// </summary>
    public static object[] GetPastDateValidationTestCases()
    {
        var yesterday = DateTime.Now.AddDays(-1).ToString("dd/MM/yyyy");
        var tomorrow = DateTime.Now.AddDays(1).ToString("dd/MM/yyyy");
        
        return new object[]
        {
            new object[] { yesterday, tomorrow, false, "Past check-in date", new[] { "past" } },
            new object[] { tomorrow, yesterday, false, "Past check-out date", new[] { "past" } },
            new object[] { yesterday, yesterday, false, "Both dates in past", new[] { "past" } }
        };
    }

    /// <summary>
    /// Gets test cases for date order validation scenarios
    /// </summary>
    public static object[] GetDateOrderValidationTestCases()
    {
        var today = DateTime.Now.ToString("dd/MM/yyyy");
        var tomorrow = DateTime.Now.AddDays(1).ToString("dd/MM/yyyy");
        var dayAfterTomorrow = DateTime.Now.AddDays(2).ToString("dd/MM/yyyy");
        
        return new object[]
        {
            new object[] { tomorrow, dayAfterTomorrow, true, "Valid date order", new string[0] },
            new object[] { dayAfterTomorrow, tomorrow, false, "Invalid date order", new[] { "invalid", "order", "before" } },
            new object[] { today, today, false, "Same day booking", new[] { "same", "day" } }
        };
    }

    /// <summary>
    /// Gets test cases for boundary date validation scenarios
    /// </summary>
    public static object[] GetBoundaryDateValidationTestCases()
    {
        var today = DateTime.Now.ToString("dd/MM/yyyy");
        var tomorrow = DateTime.Now.AddDays(1).ToString("dd/MM/yyyy");
        var farFuture = DateTime.Now.AddYears(1).ToString("dd/MM/yyyy");
        
        return new object[]
        {
            new object[] { today, tomorrow, true, "Today to tomorrow", new string[0] },
            new object[] { tomorrow, farFuture, true, "Near to far future", new string[0] },
            new object[] { "01/01/1900", "02/01/1900", false, "Very old dates", new[] { "past", "invalid" } },
            new object[] { "01/01/2100", "02/01/2100", true, "Very future dates", new string[0] }
        };
    }

    /// <summary>
    /// Gets test cases for date format validation scenarios
    /// </summary>
    public static object[] GetDateFormatValidationTestCases()
    {
        var tomorrow = DateTime.Now.AddDays(1).ToString("dd/MM/yyyy");
        var dayAfterTomorrow = DateTime.Now.AddDays(2).ToString("dd/MM/yyyy");
        
        return new object[]
        {
            new object[] { tomorrow, dayAfterTomorrow, true, "Valid DD/MM/YYYY format", new string[0] },
            new object[] { "", dayAfterTomorrow, false, "Empty check-in date", new[] { "empty", "required" } },
            new object[] { tomorrow, "", false, "Empty check-out date", new[] { "empty", "required" } },
            new object[] { "invalid-date", dayAfterTomorrow, false, "Invalid date format", new[] { "invalid", "format" } },
            new object[] { "32/13/2025", dayAfterTomorrow, false, "Invalid date values", new[] { "invalid", "date" } },
            new object[] { "2025-01-01", dayAfterTomorrow, false, "Wrong date format", new[] { "format" } }
        };
    }

    /// <summary>
    /// Gets comprehensive date validation test cases combining multiple scenarios
    /// </summary>
    public static IEnumerable<object[]> GetComprehensiveDateValidationTestCases()
    {
        foreach (var testCase in GetDateInputValidationTestCases())
            yield return (object[])testCase;
            
        foreach (var testCase in GetPastDateValidationTestCases())
            yield return (object[])testCase;
            
        foreach (var testCase in GetDateOrderValidationTestCases())
            yield return (object[])testCase;
            
        foreach (var testCase in GetBoundaryDateValidationTestCases())
            yield return (object[])testCase;
            
        foreach (var testCase in GetDateFormatValidationTestCases())
            yield return (object[])testCase;
    }

    /// <summary>
    /// Gets valid date ranges for positive testing
    /// </summary>
    public static object[] GetValidDateRangeTestCases()
    {
        return new object[]
        {
            new object[] { DateTime.Now.AddDays(1).ToString("dd/MM/yyyy"), DateTime.Now.AddDays(2).ToString("dd/MM/yyyy"), "1 day stay" },
            new object[] { DateTime.Now.AddDays(7).ToString("dd/MM/yyyy"), DateTime.Now.AddDays(14).ToString("dd/MM/yyyy"), "1 week stay" },
            new object[] { DateTime.Now.AddDays(30).ToString("dd/MM/yyyy"), DateTime.Now.AddDays(60).ToString("dd/MM/yyyy"), "1 month stay" }
        };
    }
} 