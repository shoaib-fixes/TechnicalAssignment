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
[Category("RoomManagement")]
[Parallelizable(ParallelScope.Fixtures)]
public class AdminRoomManagementTests : AdminRoomsBaseTest
{
    [Test]
    [Description("TC020: Bulk-Delete Sanity Check")]
    public void BulkDelete_MultipleRooms_ShouldUpdateListingCorrectly()
    {
        Logger.LogInformation("Starting TC020: Bulk delete multiple rooms test for browser: {Browser}", CurrentBrowser);
        
        var room1 = GetRandomRoomNumber();
        var room2 = GetRandomRoomNumber();
        var room3 = GetRandomRoomNumber();
        
        Logger.LogDebug("Creating rooms {Room1}, {Room2}, {Room3} for bulk delete test", room1, room2, room3);
        var roomConfigurations = new[]
        {
            (room1, "Single", false, 100, (IEnumerable<string>)new List<string> { "WiFi" }),
            (room2, "Double", true, 150, (IEnumerable<string>)new List<string> { "TV" }),
            (room3, "Family", false, 300, (IEnumerable<string>)new List<string> { "Views" })
        };
        _roomsPage.BulkCreateRooms(roomConfigurations);

        Logger.LogDebug("Deleting middle room {Room2}", room2);
        _roomsPage.DeleteRoom(room2);
        RemoveRoomFromCleanup(room2);
        RefreshAndWaitForPage();
        
        Logger.LogDebug("Verifying room states after deleting middle room");
        Assert.Multiple(() =>
        {
            Assert.That(_roomsPage.IsRoomPresent(room1), Is.True, $"Room {room1} should still be present");
            Assert.That(_roomsPage.IsRoomPresent(room2), Is.False, $"Room {room2} should be deleted");
            Assert.That(_roomsPage.IsRoomPresent(room3), Is.True, $"Room {room3} should still be present");
        });

        Logger.LogDebug("Deleting remaining room {Room1}", room1);
        _roomsPage.DeleteRoom(room1);
        RemoveRoomFromCleanup(room1);
        RefreshAndWaitForPage();
        
        Logger.LogDebug("Deleting remaining room {Room3}", room3);
        _roomsPage.DeleteRoom(room3);
        RemoveRoomFromCleanup(room3);
        RefreshAndWaitForPage();
        
        Logger.LogDebug("Verifying all rooms are deleted");
        Assert.Multiple(() =>
        {
            Assert.That(_roomsPage.IsRoomPresent(room1), Is.False, $"Room {room1} should be deleted");
            Assert.That(_roomsPage.IsRoomPresent(room3), Is.False, $"Room {room3} should be deleted");
        });
        
        Logger.LogInformation("TC020: Bulk delete multiple rooms test passed successfully for browser: {Browser}", CurrentBrowser);
    }

    [Test]
    [Description("TC005: Delete Room from Listing")]
    public void DeleteRoom_FromListing_ShouldRemoveRoom()
    {
        Logger.LogInformation("Starting TC005: Delete room from listing test for browser: {Browser}", CurrentBrowser);
        
        var roomNumber = GetRandomRoomNumber();
        Logger.LogDebug("Creating room {RoomNumber} for deletion test", roomNumber);
        _roomsPage.CreateRoom(roomNumber, "Single", false, 100, new List<string> { "WiFi" });
        RefreshAndWaitForPage();
        
        Logger.LogDebug("Verifying room {RoomNumber} is present before deletion", roomNumber);
        Assert.That(_roomsPage.IsRoomPresent(roomNumber), Is.True, "Room should exist before deletion");

        Logger.LogDebug("Deleting room {RoomNumber}", roomNumber);
        _roomsPage.DeleteRoom(roomNumber);
        RemoveRoomFromCleanup(roomNumber);
        RefreshAndWaitForPage();
        
        Logger.LogDebug("Verifying room {RoomNumber} is no longer present after deletion", roomNumber);
        Assert.That(_roomsPage.IsRoomPresent(roomNumber), Is.False, "Room should be removed after deletion");
        
        Logger.LogInformation("TC005: Delete room from listing test passed successfully for browser: {Browser}", CurrentBrowser);
    }

    [Test]
    [Description("TC010: Verify New Rooms on Public HomePage")]
    public void NewRooms_ShouldAppearOnPublicHomePage()
    {
        Logger.LogInformation("Starting TC010: Verify new rooms appear on public home page for browser: {Browser}", CurrentBrowser);
        
        var roomNumber = GetRandomRoomNumber();
        var price = 200m;
        var roomType = "Family";
        var features = new List<string> { "WiFi", "TV", "Radio", "Refreshments", "Safe", "Views" };
        
        Logger.LogDebug("Creating room {RoomNumber} with type {RoomType} and price {Price}", roomNumber, roomType, price);
        _roomsPage.CreateRoom(roomNumber, roomType, true, price, features);

        Logger.LogDebug("Navigating to public home page");
        Driver.Navigate().GoToUrl(TestConfig.BaseUrl);
        var homePage = new HomePage(Driver);
        homePage.WaitForPageToLoad();
        homePage.ScrollToRoomsSection();
        
        Logger.LogDebug("Searching for newly created room on home page");
        var roomCard = homePage.Booking.FindRoomCard(roomType, (int)price, features);
        
        Assert.That(roomCard, Is.Not.Null, $"Room with type {roomType} and price {price} should be found on the public page");
        
        Logger.LogDebug("Verifying room card details on home page");
        Assert.That(homePage.Booking.VerifyRoomCardDetails(roomCard!, roomType, (int)price), Is.True, 
            "Room card details on public page should match created room");

        Logger.LogInformation("TC010: Verify new rooms appear on public home page passed successfully for browser: {Browser}", CurrentBrowser);
    }

    [Test]
    [Description("TC018: Unauthorized Access Redirect to LoginPage")]
    public void UnauthorizedAccess_AdminRooms_ShouldRedirectToLoginPage()
    {
        Logger.LogInformation("Starting TC018: Unauthorized access redirect to login page test for browser: {Browser}", CurrentBrowser);
        
        Logger.LogDebug("Deleting cookies to simulate unauthorized state");
        Driver.Manage().Cookies.DeleteAllCookies();

        Logger.LogDebug("Attempting to navigate directly to admin rooms page");
        Driver.Navigate().GoToUrl($"{TestConfig.BaseUrl}/admin/rooms");
        
        Logger.LogDebug("Verifying redirection to login page");
        var loginPage = new AdminLoginPage(Driver);
        loginPage.WaitForPageToLoad();
        Assert.That(loginPage.IsOnLoginPage(), Is.True, 
            "Should be redirected to login page when accessing admin rooms without authorization");
        
        Logger.LogInformation("TC018: Unauthorized access redirect to login page test passed successfully for browser: {Browser}", CurrentBrowser);
    }
} 