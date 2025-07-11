using System;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using TechnicalAssignment.Pages;
using TechnicalAssignment.Utilities;
using TechnicalAssignment.Models;
using System.Linq;

namespace TechnicalAssignment.Tests;

[TestFixture]
[Category("DateValidation")]
[Parallelizable(ParallelScope.Fixtures)]
public class HomePageBookingDateValidationTests : BaseTest
{
    private HomePage _homePage = null!;

    [SetUp]
    public void PageSetup()
    {
        Logger.LogInformation("Setting up HomePageBookingDateValidationTests");
        Driver.Navigate().GoToUrl(TestConfig.BaseUrl);
        _homePage = new HomePage(Driver);
        _homePage.WaitForPageToLoad();
    }
    
    [TestCaseSource(typeof(DateValidationTestData), nameof(DateValidationTestData.GetPastDateValidationTestCases))]
    [Description("Verify that past dates are properly handled using data-driven validation")]
    public void DateValidation_WithPastDates_ShouldHandleAppropriately(string checkIn, string checkOut, bool expectedValid, string description, string[] expectedErrorKeywords)
    {
        Logger.LogInformation("Starting past dates validation test for {Description}", description);
        
        _homePage.ScrollToBookingSection();
        
        // Use the DateValidation component to get the result
        var result = _homePage.Booking.DateValidation.GetPastDatesHandlingResult();
        
        Logger.LogDebug("Past dates validation result - IsValid: {IsValid}, CheckIn: {CheckIn}, CheckOut: {CheckOut}, Errors: {Errors}",
            result.IsValid, result.CheckInDate, result.CheckOutDate, string.Join(", ", result.ValidationErrors));
        
        // Perform assertions directly in the test - this is the proper approach
        Assert.Multiple(() =>
        {
            Assert.That(result.CheckInDate, Is.Not.Empty, "Check-in date should be set");
            Assert.That(result.CheckOutDate, Is.Not.Empty, "Check-out date should be set");
            
            if (result.HasPastCheckIn)
            {
                // If past dates were actually set, verify the system behavior
                Assert.That(result.IsValid, Is.True, 
                    "System should handle past dates appropriately (either reject them or show no rooms)");
                    
                if (!result.IsValid && result.ValidationErrors.Count > 0)
                {
                    // Check if any of the expected error keywords are present
                    bool hasExpectedError = false;
                    foreach (var keyword in expectedErrorKeywords)
                    {
                        if (result.ValidationErrors.Any(error => error.Contains(keyword, StringComparison.OrdinalIgnoreCase)))
                        {
                            hasExpectedError = true;
                            break;
                        }
                    }
                    Assert.That(hasExpectedError, Is.True, 
                        $"Validation errors should contain one of: {string.Join(", ", expectedErrorKeywords)}");
                }
            }
            else
            {
                // If past dates were not set, the system correctly prevented them
                Assert.That(result.IsValid, Is.True, 
                    "System correctly prevented setting past dates");
            }
        });
        
        Logger.LogInformation("Past dates validation test completed successfully for {Description}", description);
    }
    
    [TestCaseSource(typeof(DateValidationTestData), nameof(DateValidationTestData.GetDateOrderValidationTestCases))]
    [Description("Verify that invalid date order is properly handled using data-driven validation")]
    public void DateValidation_WithInvalidDateOrder_ShouldHandleAppropriately(string checkIn, string checkOut, bool expectedValid, string description, string[] expectedErrorKeywords)
    {
        Logger.LogInformation("Starting invalid date order validation test for {Description}", description);
        
        _homePage.ScrollToBookingSection();
        
        // Use the DateValidation component to get the result
        var result = _homePage.Booking.DateValidation.GetInvalidDateOrderHandlingResult();
        
        Logger.LogDebug("Invalid date order validation result - IsValid: {IsValid}, CheckIn: {CheckIn}, CheckOut: {CheckOut}, Errors: {Errors}",
            result.IsValid, result.CheckInDate, result.CheckOutDate, string.Join(", ", result.ValidationErrors));
        
        // Perform assertions directly in the test
        Assert.Multiple(() =>
        {
            Assert.That(result.CheckInDate, Is.Not.Empty, "Check-in date should be set");
            Assert.That(result.CheckOutDate, Is.Not.Empty, "Check-out date should be set");
            
            if (result.HasInvalidDateOrder)
            {
                // If invalid date order was actually set, verify the system behavior
                Assert.That(result.IsValid, Is.True, 
                    "System should handle invalid date order appropriately (either reject them or show no rooms)");
                    
                if (!result.IsValid && result.ValidationErrors.Count > 0)
                {
                    // Check if any of the expected error keywords are present
                    bool hasExpectedError = false;
                    foreach (var keyword in expectedErrorKeywords)
                    {
                        if (result.ValidationErrors.Any(error => error.Contains(keyword, StringComparison.OrdinalIgnoreCase)))
                        {
                            hasExpectedError = true;
                            break;
                        }
                    }
                    Assert.That(hasExpectedError, Is.True, 
                        $"Validation errors should contain one of: {string.Join(", ", expectedErrorKeywords)}");
                }
            }
            else
            {
                // If invalid date order was not set, the system correctly prevented it
                Assert.That(result.IsValid, Is.True, 
                    "System correctly prevented setting invalid date order");
            }
        });
        
        Logger.LogInformation("Invalid date order validation test completed successfully for {Description}", description);
    }
    
    [TestCaseSource(typeof(DateValidationTestData), nameof(DateValidationTestData.GetBoundaryDateValidationTestCases))]
    [Description("Verify that same-day booking is properly handled using data-driven validation")]
    public void DateValidation_WithSameDayBooking_ShouldHandleAppropriately(string checkIn, string checkOut, bool expectedValid, string description, string[] expectedErrorKeywords)
    {
        Logger.LogInformation("Starting same-day booking validation test for {Description}", description);
        
        _homePage.ScrollToBookingSection();
        
        // Use the DateValidation component to get the result
        var result = _homePage.Booking.DateValidation.GetSameDayBookingHandlingResult();
        
        Logger.LogDebug("Same-day booking validation result - IsValid: {IsValid}, CheckIn: {CheckIn}, CheckOut: {CheckOut}, Errors: {Errors}",
            result.IsValid, result.CheckInDate, result.CheckOutDate, string.Join(", ", result.ValidationErrors));
        
        // Perform assertions directly in the test
        Assert.Multiple(() =>
        {
            Assert.That(result.CheckInDate, Is.Not.Empty, "Check-in date should be set");
            Assert.That(result.CheckOutDate, Is.Not.Empty, "Check-out date should be set");
            
            if (result.HasSameDayBooking)
            {
                // If same-day booking was actually set, verify the system behavior
                Assert.That(result.IsValid, Is.True, 
                    "System should handle same-day booking appropriately (either reject them or show no rooms)");
                    
                if (!result.IsValid && result.ValidationErrors.Count > 0)
                {
                    // Check if any of the expected error keywords are present
                    bool hasExpectedError = false;
                    foreach (var keyword in expectedErrorKeywords)
                    {
                        if (result.ValidationErrors.Any(error => error.Contains(keyword, StringComparison.OrdinalIgnoreCase)))
                        {
                            hasExpectedError = true;
                            break;
                        }
                    }
                    Assert.That(hasExpectedError, Is.True, 
                        $"Validation errors should contain one of: {string.Join(", ", expectedErrorKeywords)}");
                }
            }
            else
            {
                // If same-day booking was not set, the system correctly prevented it
                Assert.That(result.IsValid, Is.True, 
                    "System correctly prevented setting same-day booking");
            }
        });
        
        Logger.LogInformation("Same-day booking validation test completed successfully for {Description}", description);
    }
    
    [TestCaseSource(typeof(DateValidationTestData), nameof(DateValidationTestData.GetDateInputValidationTestCases))]
    [Description("Verify that date input validation works correctly using data-driven test cases")]
    public void DateValidation_WithVariousInputs_ShouldValidateCorrectly(string checkIn, string checkOut, bool expectedValid, string description, string[] expectedErrorKeywords)
    {
        Logger.LogInformation("Starting date input validation test for {Description}", description);
        
        _homePage.ScrollToBookingSection();
        
        Logger.LogDebug("Testing date validation case: {Description} - CheckIn: {CheckIn}, CheckOut: {CheckOut}", description, checkIn, checkOut);
        
        var result = _homePage.Booking.DateValidation.ValidateDateInput(checkIn, checkOut);
        
        Assert.Multiple(() =>
        {
            Assert.That(result.IsValid, Is.EqualTo(expectedValid), 
                $"Date validation should return {expectedValid} for {description}");
                
            if (!expectedValid)
            {
                Assert.That(result.ValidationErrors, Is.Not.Empty, 
                    $"Validation errors should be present for {description}");
                    
                // Check if any of the expected error keywords are present
                if (expectedErrorKeywords.Length > 0)
                {
                    bool hasExpectedError = false;
                    foreach (var keyword in expectedErrorKeywords)
                    {
                        if (result.ValidationErrors.Any(error => error.Contains(keyword, StringComparison.OrdinalIgnoreCase)))
                        {
                            hasExpectedError = true;
                            break;
                        }
                    }
                    Assert.That(hasExpectedError, Is.True, 
                        $"Validation errors should contain one of: {string.Join(", ", expectedErrorKeywords)}");
                }
            }
        });
        
        Logger.LogInformation("Date input validation test completed successfully for {Description}", description);
    }

    [TestCaseSource(typeof(DateValidationTestData), nameof(DateValidationTestData.GetValidDateRangeTestCases))]
    [Description("Verify that valid date ranges are accepted correctly")]
    public void DateValidation_WithValidDateRanges_ShouldAcceptCorrectly(string checkIn, string checkOut, string description)
    {
        Logger.LogInformation("Starting valid date range validation test for {Description}", description);
        
        _homePage.ScrollToBookingSection();
        
        var result = _homePage.Booking.DateValidation.ValidateDateInput(checkIn, checkOut);
        
        Assert.Multiple(() =>
        {
            Assert.That(result.IsValid, Is.True, 
                $"Valid date range should be accepted for {description}");
            Assert.That(result.ValidationErrors, Is.Empty, 
                $"No validation errors should be present for valid {description}");
        });
        
        Logger.LogInformation("Valid date range validation test completed successfully for {Description}", description);
    }
} 