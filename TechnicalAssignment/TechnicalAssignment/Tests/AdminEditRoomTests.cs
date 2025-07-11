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
[Category("EditRoom")]
[Parallelizable(ParallelScope.Fixtures)]
public class AdminEditRoomTests : AdminRoomsBaseTest
{
    [Test]
    [Description("TC006: Navigate to Room Details Page")]
    public void NavigateToRoomDetails_ClickingRoomNumber_ShouldOpenDetailsPage()
    {
        Logger.LogInformation("Starting TC006: Navigate to room details test for browser: {Browser}", CurrentBrowser);
        
        var roomNumber = GetRandomRoomNumber();
        Logger.LogDebug("Creating room {RoomNumber} for navigation test", roomNumber);
        _roomsPage.CreateRoom(roomNumber, "Single", false, 100, new List<string> { "WiFi" });
        TrackRoomForCleanup(roomNumber);
        _roomsPage.WaitForRoomToAppear(roomNumber);

        Logger.LogDebug("Navigating to details for room {RoomNumber}", roomNumber);
        _roomPage = _roomsPage.NavigateToRoomDetails(roomNumber);
        
        _roomPage.WaitForViewMode();

        Logger.LogDebug("Verifying navigation to the correct room details page");
        Assert.That(_roomPage.GetDisplayedRoomNumber(), Is.EqualTo(roomNumber),
            "The room number in the header should match the navigated room.");

        Logger.LogInformation("TC006: Navigate to room details test passed successfully for browser: {Browser}", CurrentBrowser);
    }

    [Test]
    [Description("TC007: Verify Room Details Display")]
    public void RoomDetailsDisplay_ShouldShowCorrectInformation()
    {
        Logger.LogInformation("Starting TC007: Verify room details display test for browser: {Browser}", CurrentBrowser);
        
        var roomNumber = GetRandomRoomNumber();
        var price = 150;
        var features = new List<string> { "TV", "WiFi" };

        Logger.LogDebug("Creating room {RoomNumber} for details display test", roomNumber);
        _roomsPage.CreateRoom(roomNumber, "Double", true, price, features);
        TrackRoomForCleanup(roomNumber);
        _roomsPage.WaitForRoomToAppear(roomNumber);

        Logger.LogDebug("Navigating to details page for room {RoomNumber}", roomNumber);
        _roomPage = _roomsPage.NavigateToRoomDetails(roomNumber);
        _roomPage.WaitForViewMode();

        Logger.LogDebug("Verifying room details on the page");
        Assert.Multiple(() =>
        {
            Assert.That(_roomPage.GetDisplayedRoomNumber(), Is.EqualTo(roomNumber), "Room number should match");
            Assert.That(_roomPage.GetDisplayedRoomType(), Is.EqualTo("Double"), "Room type should match");
            Assert.That(_roomPage.GetDisplayedAccessibility(), Is.True, "Room accessibility should match");
            Assert.That(_roomPage.GetDisplayedPrice(), Is.EqualTo(price), "Room price should match");
            Assert.That(_roomPage.GetDisplayedFeatures(), Is.EquivalentTo(features), "Room features should match");
        });

        Logger.LogInformation("TC007: Verify room details display test passed successfully for browser: {Browser}", CurrentBrowser);
    }

    [Test]
    [Description("TC008: Edit Room Fields")]
    public void EditRoom_ModifyFields_ShouldUpdateSuccessfully()
    {
        Logger.LogInformation("Starting TC008: Edit room fields test for browser: {Browser}", CurrentBrowser);
        
        var roomNumber = GetRandomRoomNumber();
        var initialPrice = 120;
        Logger.LogDebug("Creating room {RoomNumber} for field modification test", roomNumber);
        _roomsPage.CreateRoom(roomNumber, "Single", true, initialPrice, new List<string> { "WiFi" });
        TrackRoomForCleanup(roomNumber);
        _roomsPage.WaitForRoomToAppear(roomNumber);

        Logger.LogDebug("Navigating to details page for room {RoomNumber}", roomNumber);
        _roomPage = _roomsPage.NavigateToRoomDetails(roomNumber);
        _roomPage.WaitForViewMode();
        
        var newPrice = 250;
        var newFeatures = new List<string> { "Safe", "Views" };
        Logger.LogDebug("Updating room details: Price={NewPrice}, Features={NewFeatures}", newPrice, string.Join(",", newFeatures));
        
        _roomPage.ClickEditButtonAndWait();
        _roomPage.UpdateRoomDetails("Family", "false", newPrice.ToString(), newFeatures);

        _roomPage.WaitForViewMode();

        Logger.LogDebug("Verifying updated room details on the details page");
        Assert.Multiple(() =>
        {
            Assert.That(_roomPage.GetDisplayedRoomType(), Is.EqualTo("Family"), "Room type should be updated on details page");
            Assert.That(_roomPage.GetDisplayedAccessibility(), Is.EqualTo(false), "Room accessibility should be updated on details page");
            Assert.That(_roomPage.GetDisplayedPrice(), Is.EqualTo(newPrice), "Room price should be updated on details page");
            Assert.That(_roomPage.GetDisplayedFeatures(), Is.EquivalentTo(newFeatures), "Room features should be updated on details page");
        });

        Logger.LogInformation("TC008: Edit room fields test passed successfully for browser: {Browser}", CurrentBrowser);
    }

    [Test]
    [Description("TC009: Cancel Edit Operation")]
    public void EditRoom_CancelOperation_ShouldLeaveDataUnchanged()
    {
        Logger.LogInformation("Starting TC009: Cancel edit operation test for browser: {Browser}", CurrentBrowser);
        
        var roomNumber = GetRandomRoomNumber();
        var initialPrice = 100;
        var initialType = "Single";
        var initialAccessibility = false;
        var initialFeatures = new List<string> { "WiFi" };

        Logger.LogDebug("Creating room {RoomNumber} for cancel edit test", roomNumber);
        _roomsPage.CreateRoom(roomNumber, initialType, initialAccessibility, initialPrice, initialFeatures);
        TrackRoomForCleanup(roomNumber);
        _roomsPage.WaitForRoomToAppear(roomNumber);

        Logger.LogDebug("Navigating to details page for room {RoomNumber}", roomNumber);
        _roomPage = _roomsPage.NavigateToRoomDetails(roomNumber);
        _roomPage.WaitForViewMode();
        
        Logger.LogDebug("Entering new data but clicking Cancel");
        _roomPage.ClickEditButtonAndWait();
        _roomPage.EnterRoomDetails("Suite", "true", "500", new List<string> { "Safe" });
        _roomPage.ClickCancel();
        
        _roomPage.WaitForViewMode();

        Logger.LogDebug("Verifying room details remain unchanged on the listing page");
        Assert.Multiple(() =>
        {
            Assert.That(_roomPage.GetDisplayedRoomType(), Is.EqualTo(initialType), "Room type should not change");
            Assert.That(_roomPage.GetDisplayedAccessibility(), Is.EqualTo(initialAccessibility), "Room accessibility should not change");
            Assert.That(_roomPage.GetDisplayedPrice(), Is.EqualTo(initialPrice), "Room price should not change");
            Assert.That(_roomPage.GetDisplayedFeatures(), Is.EquivalentTo(initialFeatures), "Room features should not change");
        });

        Logger.LogInformation("TC009: Cancel edit operation test passed successfully for browser: {Browser}", CurrentBrowser);
    }

    [Test]
    [Description("TC015a: Edit Room Number – Valid Update")]
    public void EditRoom_ValidRoomNumber_ShouldUpdateSuccessfully()
    {
        Logger.LogInformation("Starting TC015a: Valid room number update test for browser: {Browser}", CurrentBrowser);
        
        var originalRoomNumber = GetRandomRoomNumber();
        var newRoomNumber = GetRandomRoomNumber();
        TrackRoomForCleanup(newRoomNumber);
        
        Logger.LogDebug("Creating room {OriginalRoomNumber} for room number edit test", originalRoomNumber);
        _roomsPage.CreateRoom(originalRoomNumber, "Single", false, 100, new List<string>());
        TrackRoomForCleanup(originalRoomNumber);
        _roomsPage.WaitForRoomToAppear(originalRoomNumber);

        Logger.LogDebug("Navigating to details page for room {OriginalRoomNumber}", originalRoomNumber);
        _roomPage = _roomsPage.NavigateToRoomDetails(originalRoomNumber);
        _roomPage.WaitForViewMode();
        
        Logger.LogDebug("Updating room number from {OriginalRoomNumber} to {NewRoomNumber}", originalRoomNumber, newRoomNumber);
        _roomPage.ClickEditButtonAndWait();
        _roomPage.UpdateRoomNumber(newRoomNumber.ToString());
        
        _roomPage.WaitForViewMode();

        Logger.LogDebug("Verifying room number was updated on the details page");
        Assert.That(_roomPage.GetDisplayedRoomNumber(), Is.EqualTo(newRoomNumber), 
            "The room number in the header should match the new room number.");
        
        RemoveRoomFromCleanup(originalRoomNumber);

        Logger.LogInformation("TC015a: Valid room number update test passed successfully for browser: {Browser}", CurrentBrowser);
    }

    [Test]
    [Description("TC015b: Edit Room Number – Duplicate Number Rejection")]
    public void EditRoom_DuplicateRoomNumber_ShouldFail()
    {
        Logger.LogInformation("Starting TC015b: Duplicate room number edit test for browser: {Browser}", CurrentBrowser);
        
        var roomNumber1 = GetRandomRoomNumber();
        var roomNumber2 = GetRandomRoomNumber();
        
        Logger.LogDebug("Creating rooms {RoomNumber1} and {RoomNumber2}", roomNumber1, roomNumber2);
        _roomsPage.CreateRoom(roomNumber1, "Single", false, 100, new List<string>());
        TrackRoomForCleanup(roomNumber1);
        _roomsPage.WaitForRoomToAppear(roomNumber1);
        _roomsPage.CreateRoom(roomNumber2, "Double", true, 200, new List<string>());
        TrackRoomForCleanup(roomNumber2);
        _roomsPage.WaitForRoomToAppear(roomNumber2);

        Logger.LogDebug("Navigating to details page for room {RoomNumber2}", roomNumber2);
        _roomPage = _roomsPage.NavigateToRoomDetails(roomNumber2);
        _roomPage.WaitForViewMode();
        
        Logger.LogDebug("Attempting to update room number from {RoomNumber2} to existing number {RoomNumber1}", roomNumber2, roomNumber1);
        _roomPage.ClickEditButtonAndWait();
        _roomPage.UpdateRoomNumber(roomNumber1.ToString());
        
        Logger.LogDebug("Verifying error message is displayed for duplicate room number");
        Assert.That(_roomPage.IsErrorAlertVisible(TimeSpan.FromSeconds(5)), Is.True, 
            "Error alert should be displayed for duplicate room number");

        Logger.LogInformation("TC015b: Duplicate room number edit test passed successfully for browser: {Browser}", CurrentBrowser);
    }

    [Test]
    [Description("TC015c: Edit Room Number – Invalid Number Rejection")]
    public void EditRoom_InvalidRoomNumber_ShouldFail()
    {
        Logger.LogInformation("Starting TC015c: Invalid room number edit test for browser: {Browser}", CurrentBrowser);
        
        var roomNumber = GetRandomRoomNumber();
        Logger.LogDebug("Creating room {RoomNumber} for invalid room number edit test", roomNumber);
        _roomsPage.CreateRoom(roomNumber, "Single", false, 100, new List<string>());
        TrackRoomForCleanup(roomNumber);
        _roomsPage.WaitForRoomToAppear(roomNumber);

        Logger.LogDebug("Navigating to details page for room {RoomNumber}", roomNumber);
        _roomPage = _roomsPage.NavigateToRoomDetails(roomNumber);
        _roomPage.WaitForViewMode();

        const string invalidRoomNumber = "invalid-room";
        Logger.LogDebug("Attempting to update room number to invalid string '{InvalidRoomNumber}'", invalidRoomNumber);
        TrackRoomForCleanup(invalidRoomNumber);
        _roomPage.ClickEditButtonAndWait();
        _roomPage.UpdateRoomNumber(invalidRoomNumber);
        
        Logger.LogDebug("Verifying error message is displayed for invalid room number");
        Assert.That(_roomPage.IsErrorAlertVisible(TimeSpan.FromSeconds(5)), Is.True, 
            "Error alert should be displayed for invalid room number");

        Logger.LogInformation("TC015c: Invalid room number edit test passed successfully for browser: {Browser}", CurrentBrowser);
    }

    [Test]
    [Description("TC015d: Edit Room Number – Large Number Over 1000")]
    public void EditRoom_LargeRoomNumber_ShouldFail()
    {
        Logger.LogInformation("Starting TC015d: Large room number edit test for browser: {Browser}", CurrentBrowser);

        var roomNumber = GetRandomRoomNumber();
        Logger.LogDebug("Creating room {RoomNumber} for large room number edit test", roomNumber);
        _roomsPage.CreateRoom(roomNumber, "Single", false, 100, new List<string>());
        TrackRoomForCleanup(roomNumber);
        _roomsPage.WaitForRoomToAppear(roomNumber);

        Logger.LogDebug("Navigating to details page for room {RoomNumber}", roomNumber);
        _roomPage = _roomsPage.NavigateToRoomDetails(roomNumber);
        _roomPage.WaitForViewMode();

        var largeRoomNumber = GetExoticRoomNumber();
        TrackRoomForCleanup(largeRoomNumber);
        Logger.LogDebug("Attempting to update room number to large number {LargeRoomNumber}", largeRoomNumber);
        _roomPage.ClickEditButtonAndWait();
        _roomPage.UpdateRoomNumber(largeRoomNumber.ToString());

        Logger.LogDebug("Verifying error message is displayed for large room number");
        Assert.That(_roomPage.IsErrorAlertVisible(TimeSpan.FromSeconds(5)), Is.True, 
            "Error alert should be displayed for room number over 1000");
        
        var errorMessage = _roomPage.GetErrorAlertText();
        Logger.LogDebug("Error message received: {ErrorMessage}", errorMessage);
        Assert.That(errorMessage, Does.Contain("must be between 1 and 1000"), 
            "Error message should specify valid room number range");

        Logger.LogInformation("TC015d: Large room number edit test passed successfully for browser: {Browser}", CurrentBrowser);
    }

    [Test]
    [Description("TC016: Invalid Image URL on Edit")]
    public void EditRoom_InvalidImageUrl_ShouldFallbackOrFailProperly()
    {
        Logger.LogInformation("Starting TC016: Invalid image URL edit test for browser: {Browser}", CurrentBrowser);
        
        var roomNumber = GetRandomRoomNumber();
        Logger.LogDebug("Creating room {RoomNumber} for invalid image URL test", roomNumber);
        _roomsPage.CreateRoom(roomNumber, "Single", false, 100, new List<string>());
        TrackRoomForCleanup(roomNumber);
        _roomsPage.WaitForRoomToAppear(roomNumber);

        Logger.LogDebug("Navigating to details page for room {RoomNumber}", roomNumber);
        _roomPage = _roomsPage.NavigateToRoomDetails(roomNumber);
        _roomPage.WaitForViewMode();
        
        const string invalidUrl = "nonsenseURL";
        Logger.LogDebug("Updating image URL to invalid URL: {InvalidUrl}", invalidUrl);
        _roomPage.ClickEditButtonAndWait();
        _roomPage.UpdateImageUrl(invalidUrl);

        _roomPage.WaitForViewMode();
        
        Logger.LogDebug("Verifying image fallback mechanism");
        var actualImageUrl = _roomPage.GetImageUrl();
        Assert.That(actualImageUrl.Contains(invalidUrl), Is.False,
            $"Image URL should not contain the invalid fragment, but was: {actualImageUrl}");

        Logger.LogInformation("TC016: Invalid image URL edit test passed successfully for browser: {Browser}", CurrentBrowser);
    }
} 