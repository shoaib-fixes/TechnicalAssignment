using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using OpenQA.Selenium;
using TechnicalAssignment.Pages;
using TechnicalAssignment.Utilities;
using TechnicalAssignment.Models;
using TechnicalAssignment.Configuration;

namespace TechnicalAssignment.Tests;

[TestFixture]
public abstract class AdminRoomsBaseTest : BaseTest
{
    protected AdminRoomsPage _roomsPage = null!;
    protected AdminRoomPage _roomPage = null!;
    protected AdminDashboardPage _adminDashboard = null!;
    protected AdminLoginPage _adminLogin = null!;
    
    private readonly List<string> _roomsToCleanup = new();
    private readonly Random _random = new();

    protected string CurrentBrowser => TestConfig.Browser.DefaultBrowser;
    protected ConfigurationManager Config => new ConfigurationManager(LoggingHelper.CreateLogger<ConfigurationManager>());

    [SetUp]
    public void AdminRoomsSetUp()
    {
        Logger.LogInformation("Starting Admin Rooms test setup for browser: {Browser}", CurrentBrowser);
        
        Driver.Navigate().GoToUrl($"{TestConfig.BaseUrl}/admin");
        
        _adminLogin = new AdminLoginPage(Driver);
        _adminLogin.WaitForPageToLoad();
        
        Logger.LogDebug("Logging into admin panel");
        _adminDashboard = _adminLogin.Login(TestConfig.AdminCredentials.Username, TestConfig.AdminCredentials.Password);
        _adminDashboard.WaitForPageToLoad();
        
        Logger.LogDebug("Navigating to rooms section");
        _adminDashboard.NavigateToRooms();
        
        _roomsPage = new AdminRoomsPage(Driver);
        _roomsPage.WaitForPageToLoad();
        
        Logger.LogDebug("Admin rooms test setup completed successfully");
    }

    [TearDown]
    public void AdminRoomsTearDown()
    {
        try
        {
            Logger.LogInformation("Starting admin rooms cleanup");
            
            if (_roomsToCleanup.Any())
            {
                Logger.LogDebug("Cleaning up {Count} rooms", _roomsToCleanup.Count);
                
                Driver.Navigate().GoToUrl($"{TestConfig.BaseUrl}/admin/rooms");
                _roomsPage.WaitForPageToLoad();
                
                foreach (var roomNumber in _roomsToCleanup.ToList())
                {
                    try
                    {
                        if (int.TryParse(roomNumber, out var roomNum))
                        {
                            if (_roomsPage.IsRoomPresent(roomNum))
                            {
                                Logger.LogDebug("Deleting room {RoomNumber}", roomNum);
                                _roomsPage.DeleteRoom(roomNum);
                                Driver.Navigate().Refresh();
                                _roomsPage.WaitForPageToLoad();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.LogWarning(ex, "Failed to cleanup room {RoomNumber}", roomNumber);
                    }
                }
                
                _roomsToCleanup.Clear();
                Logger.LogDebug("Room cleanup completed");
            }
        }
        catch (Exception ex)
        {
            Logger.LogWarning(ex, "Error during admin rooms cleanup");
        }
        finally
        {
            try
            {
                if (_adminDashboard != null)
                {
                    Logger.LogDebug("Logging out of admin panel");
                    _adminDashboard.Logout();
                }
            }
            catch (Exception ex)
            {
                Logger.LogWarning(ex, "Error during admin logout");
            }
        }
    }

    /// <summary>
    /// Generates a random room number within the valid range for creation.
    /// The application's validator for room creation is set to 1-999.
    /// However, since some tests calculate the price as (room number + 100) and the maximum allowed price is 1000,
    /// this method restricts the range to 1-899 to ensure test data remains valid.
    /// </summary>
    /// <returns>A valid random room number for creation tests.</returns>
    protected int GetRandomRoomNumber()
    {
        var roomNumber = _random.Next(1, 900); // 1 to 899
        TrackRoomForCleanup(roomNumber.ToString());
        return roomNumber;
    }

    /// <summary>
    /// Generates a room number outside the standard validation range (e.g., > 1000).
    /// This is used specifically for testing scenarios where the edit functionality
    /// has a validation gap and does not correctly enforce the maximum room number limit,
    /// unlike the creation form.
    /// </summary>
    /// <returns>A random room number intended to bypass weak validation.</returns>
    protected int GetExoticRoomNumber()
    {
        var roomNumber = _random.Next(1001, 9999);
        TrackRoomForCleanup(roomNumber.ToString());
        return roomNumber;
    }

    protected void TrackRoomForCleanup(string roomNumber)
    {
        if (!_roomsToCleanup.Contains(roomNumber))
        {
            _roomsToCleanup.Add(roomNumber);
            Logger.LogDebug("Room {RoomNumber} added to cleanup list", roomNumber);
        }
    }

    protected void TrackRoomForCleanup(int roomNumber)
    {
        TrackRoomForCleanup(roomNumber.ToString());
    }

    protected void RemoveRoomFromCleanup(string roomNumber)
    {
        if (_roomsToCleanup.Remove(roomNumber))
        {
            Logger.LogDebug("Room {RoomNumber} removed from cleanup list", roomNumber);
        }
    }

    protected void RemoveRoomFromCleanup(int roomNumber)
    {
        RemoveRoomFromCleanup(roomNumber.ToString());
    }

    protected void CreateAndTrackRoom(int roomNumber, string type, bool accessible, int price, IEnumerable<string> features)
    {
        TrackRoomForCleanup(roomNumber);
        _roomsPage.CreateRoom(roomNumber, type, accessible, price, features);
    }

    protected void CreateAndTrackRoom(int roomNumber, string type, bool accessible, decimal price, IEnumerable<string> features)
    {
        TrackRoomForCleanup(roomNumber);
        _roomsPage.CreateRoom(roomNumber, type, accessible, price, features);
    }

    protected void BulkCreateAndTrackRooms(IEnumerable<(int RoomNumber, string Type, bool Accessible, int Price, IEnumerable<string> Features)> roomConfigurations)
    {
        foreach (var config in roomConfigurations)
        {
            TrackRoomForCleanup(config.RoomNumber);
        }
        _roomsPage.BulkCreateRooms(roomConfigurations);
    }

    protected void WaitForRoomOperation(int roomNumber, bool shouldExist = true, TimeSpan? timeout = null)
    {
        var waitTimeout = timeout ?? TimeSpan.FromSeconds(10);
        
        if (shouldExist)
        {
            var roomAppeared = _roomsPage.WaitForRoomToAppear(roomNumber, waitTimeout);
            if (!roomAppeared)
            {
                Logger.LogWarning("Room {RoomNumber} did not appear within timeout", roomNumber);
            }
        }
        else
        {
            var roomDisappeared = _roomsPage.WaitForRoomToDisappear(roomNumber, waitTimeout);
            if (!roomDisappeared)
            {
                Logger.LogWarning("Room {RoomNumber} did not disappear within timeout", roomNumber);
            }
        }
    }

    protected void RefreshAndWaitForPage()
    {
        Driver.Navigate().Refresh();
        _roomsPage.WaitForPageToLoad();
    }

    protected void AssertRoomExists(int roomNumber, string message = "")
    {
        RefreshAndWaitForPage();
        var exists = _roomsPage.IsRoomPresent(roomNumber);
        var assertMessage = string.IsNullOrEmpty(message) ? $"Room {roomNumber} should exist" : message;
        Assert.That(exists, Is.True, assertMessage);
    }

    protected void AssertRoomDoesNotExist(int roomNumber, string message = "")
    {
        RefreshAndWaitForPage();
        var exists = _roomsPage.IsRoomPresent(roomNumber);
        var assertMessage = string.IsNullOrEmpty(message) ? $"Room {roomNumber} should not exist" : message;
        Assert.That(exists, Is.False, assertMessage);
    }

    protected void AssertRoomProperties(int roomNumber, string expectedType, bool expectedAccessible, int expectedPrice, string message = "")
    {
        RefreshAndWaitForPage();
        var verified = _roomsPage.VerifyRoomProperties(roomNumber, expectedType, expectedAccessible, expectedPrice);
        var assertMessage = string.IsNullOrEmpty(message) ? $"Room {roomNumber} should have expected properties" : message;
        Assert.That(verified, Is.True, assertMessage);
    }
} 