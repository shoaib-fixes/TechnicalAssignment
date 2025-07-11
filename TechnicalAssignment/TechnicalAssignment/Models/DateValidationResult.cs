using System;
using System.Collections.Generic;

namespace TechnicalAssignment.Models;

/// <summary>
/// Represents the result of a date validation operation
/// </summary>
public class DateValidationResult
{
    public bool IsValid { get; set; }
    public string CheckInDate { get; set; } = "";
    public string CheckOutDate { get; set; } = "";
    public DateTime? ParsedCheckInDate { get; set; }
    public DateTime? ParsedCheckOutDate { get; set; }
    public List<string> ValidationErrors { get; set; } = new List<string>();
    public bool HasPastCheckIn { get; set; }
    public bool HasInvalidDateOrder { get; set; }
    public bool HasSameDayBooking { get; set; }
} 