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
    protected AdminNavBarComponent _adminNavBar = null!;
    protected AdminLoginPage _adminLogin = null!;
    
    private readonly List<string> _roomsToCleanup = new();
    
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
        _adminNavBar = _adminLogin.Login(TestConfig.AdminCredentials.Username, TestConfig.AdminCredentials.Password);
        _adminNavBar.WaitForPageToLoad();
        
        Logger.LogDebug("Navigating to rooms section");
        _adminNavBar.NavigateToRooms();
        
        _roomsPage = new AdminRoomsPage(Driver);
        _roomsPage.WaitForPageToLoad();
        
        Logger.LogDebug("Admin rooms test setup completed successfully");
    }

    [TearDown]
    public void AdminRoomsTearDown()
    {
        var cleanupExceptions = new List<Exception>();
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
                        var cleanupEx = new InvalidOperationException($"Failed to cleanup room {roomNumber}", ex);
                        Logger.LogWarning(cleanupEx, "Cleanup failure for room {RoomNumber}", roomNumber);
                        cleanupExceptions.Add(cleanupEx);
                    }
                }
                
                _roomsToCleanup.Clear();
                Logger.LogDebug("Room cleanup completed");
            }
        }
        catch (Exception ex)
        {
            var teardownEx = new InvalidOperationException("A critical error occurred during the main teardown process, interrupting cleanup.", ex);
            Logger.LogWarning(teardownEx, "Critical teardown error");
            cleanupExceptions.Add(teardownEx);
        }
        finally
        {
            try
            {
                if (_adminNavBar != null)
                {
                    Logger.LogDebug("Logging out of admin panel");
                    _adminNavBar.Logout();
                }
            }
            catch (Exception ex)
            {
                var logoutEx = new InvalidOperationException("Failed to logout during cleanup.", ex);
                Logger.LogWarning(logoutEx, "Error during admin logout");
                cleanupExceptions.Add(logoutEx);
            }
            
            if (cleanupExceptions.Any())
            {
                throw new AggregateException($"Test teardown failed with {cleanupExceptions.Count} error(s). See inner exceptions for details.", cleanupExceptions);
            }
        }
    }

    /// <summary>
    /// Generates a random room number, tracking it for automated cleanup.
    /// This method delegates number generation to RoomNumberFactory to separate concerns.
    /// </summary>
    /// <returns>A valid random room number for creation tests.</returns>
    protected int GetRandomRoomNumber()
    {
        return RoomNumberFactory.GetRandomRoomNumber();
    }

    /// <summary>
    /// Generates a room number outside the standard validation range, tracking it for automated cleanup.
    /// This method delegates number generation to RoomNumberFactory to separate concerns.
    /// </summary>
    /// <returns>A random room number intended to bypass weak validation.</returns>
    protected int GetExoticRoomNumber()
    {
        return RoomNumberFactory.GetExoticRoomNumber();
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
        _roomsPage.CreateRoom(roomNumber, type, accessible, price, features);
        TrackRoomForCleanup(roomNumber);
    }

    protected void CreateAndTrackRoom(int roomNumber, string type, bool accessible, decimal price, IEnumerable<string> features)
    {
        _roomsPage.CreateRoom(roomNumber, type, accessible, price, features);
        TrackRoomForCleanup(roomNumber);
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