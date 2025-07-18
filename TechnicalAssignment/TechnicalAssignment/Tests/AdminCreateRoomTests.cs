using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using OpenQA.Selenium;
using TechnicalAssignment.Pages;
using TechnicalAssignment.Utilities;

namespace TechnicalAssignment.Tests;

[TestFixture]
[Category("Admin")]
[Category("Rooms")]
[Category("CreateRoom")]
[Parallelizable(ParallelScope.Fixtures)]
public class AdminCreateRoomTests : AdminRoomsBaseTest
{
    [Test]
    [Description("TC001: Create Rooms for All Type and Accessibility Combinations")]
    public void CreateRoom_AllTypesAndAccessibilityCombinations_ShouldSucceed()
    {
        Logger.LogInformation("Starting TC001: Create rooms for all type and accessibility combinations for browser: {Browser}", CurrentBrowser);
        
        var types = new[] { "Single", "Twin", "Double", "Family", "Suite" };
        var accessibilityOptions = new[] { false, true };

        foreach (var type in types)
        {
            foreach (var accessible in accessibilityOptions)
            {
                var roomNumber = GetRandomRoomNumber();
                var price = 100 + roomNumber;
                var features = new List<string> { "WiFi" };
                
                Logger.LogDebug("Creating room {RoomNumber} with type {Type}, accessible {Accessible}, price {Price}", 
                    roomNumber, type, accessible, price);
                
                _roomsPage.CreateRoom(roomNumber, type, accessible, price, features);
                TrackRoomForCleanup(roomNumber);
                RefreshAndWaitForPage();
                
                Logger.LogDebug("Verifying room {RoomNumber} was created correctly", roomNumber);
                Assert.Multiple(() =>
                {
                    Assert.That(_roomsPage.IsRoomPresent(roomNumber), Is.True, 
                        $"Room {roomNumber} should be created successfully");
                    Assert.That(_roomsPage.GetRoomType(roomNumber), Is.EqualTo(type),
                        $"Room {roomNumber} should have type {type}");
                    Assert.That(_roomsPage.GetRoomAccessibility(roomNumber), Is.EqualTo(accessible),
                        $"Room {roomNumber} should have accessibility {accessible}");
                    Assert.That(_roomsPage.GetRoomPrice(roomNumber), Is.EqualTo(price),
                        $"Room {roomNumber} should have price {price}");
                });
                
                Logger.LogDebug("Room {RoomNumber} verified successfully", roomNumber);
            }
        }
        
        Logger.LogInformation("TC001: Create rooms for all type and accessibility combinations passed successfully for browser: {Browser}", CurrentBrowser);
    }

    [Test]
    [Description("TC002: Room Number Field Validation – Non-Numeric Input")]
    public void CreateRoom_NonNumericRoomNumber_ShouldFail()
    {
        Logger.LogInformation("Starting TC002: Room number field validation with non-numeric input for browser: {Browser}", CurrentBrowser);
        
        const string nonNumericRoom = "ABC";
        TrackRoomForCleanup(nonNumericRoom);
        Logger.LogDebug("Attempting to create room with non-numeric room number 'ABC'");
        _roomsPage.CreateRoom(nonNumericRoom, "Single", false, "100", new List<string>());

        Logger.LogDebug("Verifying error message appears for non-numeric room number");
        Assert.That(_roomsPage.IsErrorAlertVisible(), Is.True, "Error alert should be displayed for non-numeric room number");
        
        Logger.LogInformation("TC002: Room number field validation with non-numeric input passed successfully for browser: {Browser}", CurrentBrowser);
    }

    [Test]
    [Description("TC003: Duplicate Room Number Rejection")]
    public void CreateRoom_DuplicateRoomNumber_ShouldFail()
    {
        Logger.LogInformation("Starting TC003: Duplicate room number rejection test for browser: {Browser}", CurrentBrowser);
        
        var roomNumber = GetRandomRoomNumber();
        Logger.LogDebug("Creating initial room {RoomNumber}", roomNumber);
        _roomsPage.CreateRoom(roomNumber, "Single", false, 100, new List<string> { "WiFi" });
        TrackRoomForCleanup(roomNumber);
        RefreshAndWaitForPage();

        Logger.LogDebug("Attempting to create duplicate room with number {RoomNumber}", roomNumber);
        _roomsPage.CreateRoom(roomNumber.ToString(), "Double", true, "150", new List<string>());

        Logger.LogDebug("Verifying error message appears for duplicate room number");
        Assert.That(_roomsPage.IsErrorAlertVisible(), Is.True, "Error alert should be displayed for duplicate room number");
        
        Logger.LogInformation("TC003: Duplicate room number rejection test passed successfully for browser: {Browser}", CurrentBrowser);
    }

    [Test]
    [Description("TC004: Price Field Validation – Non-Numeric Input")]
    public void CreateRoom_NonNumericPrice_ShouldFail()
    {
        Logger.LogInformation("Starting TC004: Price field validation with non-numeric input for browser: {Browser}", CurrentBrowser);
        
        var roomNumber = GetRandomRoomNumber();
        Logger.LogDebug("Attempting to create room {RoomNumber} with non-numeric price", roomNumber);
        _roomsPage.CreateRoom(roomNumber.ToString(), "Single", false, "invalid", new List<string>());
        TrackRoomForCleanup(roomNumber);
        
        Logger.LogDebug("Verifying error message appears for non-numeric price");
        Assert.That(_roomsPage.IsErrorAlertVisible(), Is.True, "Error alert should be displayed for non-numeric price");

        Logger.LogInformation("TC004: Price field validation with non-numeric input passed successfully for browser: {Browser}", CurrentBrowser);
    }

    [Test]
    [Description("TC005: Required-Field Validation – Empty Inputs")]
    public void CreateRoom_WithoutMandatoryFields_ShouldFail()
    {
        Logger.LogInformation("Starting TC005: Required-field validation test for browser: {Browser}", CurrentBrowser);
        
        Logger.LogDebug("Attempting to create room with empty mandatory fields");
        _roomsPage.CreateRoom("", "", false, "", new List<string>());
        
        Logger.LogDebug("Verifying error message is visible");
        Assert.That(_roomsPage.IsErrorAlertVisible(), Is.True, 
            "Error alert should be displayed for missing mandatory fields");

        Logger.LogInformation("TC005: Required-field validation test passed successfully for browser: {Browser}", CurrentBrowser);
    }

    [Test]
    [Description("TC006: Price Field Validation – Invalid Low Prices")]
    [TestCase(0, "Zero price")]
    [TestCase(-100, "Negative price")]
    public void CreateRoom_InvalidLowPrice_ShouldFail(int price, string description)
    {
        Logger.LogInformation("Starting TC006: {Description} validation test for browser: {Browser}", description, CurrentBrowser);
        
        var roomNumber = GetRandomRoomNumber();
        Logger.LogDebug("Attempting to create room {RoomNumber} with invalid price {Price}", roomNumber, price);
        _roomsPage.CreateRoom(roomNumber, "Single", false, price, new List<string>());
        TrackRoomForCleanup(roomNumber);

        Logger.LogDebug("Verifying error message for invalid price is visible");
        Assert.That(_roomsPage.IsErrorAlertVisible(), Is.True, "Error alert should be displayed for invalid price");
        
        var errorMessage = _roomsPage.GetErrorAlertText();
        Logger.LogDebug("Error message received: {ErrorMessage}", errorMessage);
        Assert.That(errorMessage, Does.Contain("must be greater than or equal to 1"), 
            "Error message should indicate price must be greater than 0");

        Logger.LogInformation("TC006: {Description} validation test passed successfully for browser: {Browser}", description, CurrentBrowser);
    }
    
    [Test]
    [Description("TC007: Price Field Validation – Decimal Price")]
    public void CreateRoom_DecimalPrice_ShouldSucceed()
    {
        Logger.LogInformation("Starting TC007: Decimal price validation test for browser: {Browser}", CurrentBrowser);
        
        var roomNumber = GetRandomRoomNumber();
        var price = 150.75m;
        Logger.LogDebug("Creating room {RoomNumber} with decimal price {Price}", roomNumber, price);
        _roomsPage.CreateRoom(roomNumber, "Suite", false, price, new List<string> { "TV" });
        TrackRoomForCleanup(roomNumber);
        RefreshAndWaitForPage();
        
        Logger.LogDebug("Verifying room {RoomNumber} was created successfully with decimal price", roomNumber);
        Assert.Multiple(() =>
        {
            Assert.That(_roomsPage.IsRoomPresent(roomNumber), Is.True, 
                "Room should be created with decimal price");
            Assert.That(_roomsPage.GetRoomPriceAsDecimal(roomNumber), Is.EqualTo(price),
                "Room price should match the specified decimal value");
        });

        Logger.LogInformation("TC007: Decimal price validation test passed successfully for browser: {Browser}", CurrentBrowser);
    }

    [Test]
    [Description("TC008: No-Feature Selection")]
    public void CreateRoom_NoFeatureSelection_ShouldSucceed()
    {
        Logger.LogInformation("Starting TC008: No-feature selection test for browser: {Browser}", CurrentBrowser);
        
        var roomNumber = GetRandomRoomNumber();
        Logger.LogDebug("Creating room {RoomNumber} with no features", roomNumber);
        _roomsPage.CreateRoom(roomNumber, "Single", false, 100, new List<string>());
        TrackRoomForCleanup(roomNumber);
        RefreshAndWaitForPage();
        
        Logger.LogDebug("Verifying room {RoomNumber} was created successfully with no features", roomNumber);
        Assert.That(_roomsPage.IsRoomPresent(roomNumber), Is.True, 
            "Room should be created successfully without any features selected");

        Logger.LogInformation("TC008: No-feature selection test passed successfully for browser: {Browser}", CurrentBrowser);
    }

    [Test]
    [Description("TC009: Default Dropdown Values on Load")]
    public void DefaultDropdownValues_OnLoad_ShouldBeCorrect()
    {
        Logger.LogInformation("Starting TC009: Default dropdown values test for browser: {Browser}", CurrentBrowser);
        
        Logger.LogDebug("Getting default values from room creation form");
        var (defaultType, defaultAccessibility) = _roomsPage.GetDefaultDropdownValues();
        
        Logger.LogDebug("Default values retrieved: Type={DefaultType}, Accessibility={DefaultAccessibility}", defaultType, defaultAccessibility);
        Assert.Multiple(() =>
        {
            Assert.That(defaultType, Is.EqualTo("Single"), 
                "Default room type should be 'Single'");
            Assert.That(defaultAccessibility, Is.False, 
                "Default accessibility should be 'false' (not accessible)");
        });

        Logger.LogInformation("TC009: Default dropdown values test passed successfully for browser: {Browser}", CurrentBrowser);
    }
} 