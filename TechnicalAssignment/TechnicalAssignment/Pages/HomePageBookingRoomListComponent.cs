using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using TechnicalAssignment.Utilities;

namespace TechnicalAssignment.Pages;

public class HomePageBookingRoomListComponent
{
    private readonly IWebDriver _driver;
    private readonly ILogger<HomePageBookingRoomListComponent> _logger;
    
    private static readonly By RoomCards = By.CssSelector(".room-card");
    private static readonly By RoomCardBookButton = By.CssSelector(".btn-primary");
    private static readonly By RoomCardBookNowButtons = By.CssSelector(".room-card .btn-primary");

    public HomePageBookingRoomListComponent(IWebDriver driver, ILogger<HomePageBookingRoomListComponent> logger)
    {
        _driver = driver;
        _logger = logger;
    }

    public void ScrollToRoomsSection()
    {
        ScrollHelper.ScrollToElement(_driver, RoomCards);
    }

    public IWebElement? FindRoomCard(string roomType, int price, IEnumerable<string> features)
    {
        _logger.LogDebug("Searching for room card with type {RoomType}, price £{Price}, and features {Features}", 
            roomType, price, string.Join(", ", features));
        
        var roomCards = _driver.FindElements(RoomCards);
        _logger.LogDebug("Found {Count} room cards to search", roomCards.Count);
        
        var targetRoomCard = roomCards.FirstOrDefault(card => 
            card.Text.Contains(roomType) && 
            card.Text.Contains($"£{price}") &&
            features.Any(feature => card.Text.Contains(feature)));
        
        if (targetRoomCard != null)
        {
            _logger.LogDebug("Found matching room card with type {RoomType} and price £{Price}", roomType, price);
        }
        else
        {
            _logger.LogDebug("No matching room card found for type {RoomType} and price £{Price}", roomType, price);
        }
        
        return targetRoomCard;
    }

    public bool VerifyRoomCardDetails(IWebElement roomCard, string roomType, int price)
    {
        _logger.LogDebug("Verifying room card details for type {RoomType} and price £{Price}", roomType, price);
        
        try
        {
            var hasCorrectType = roomCard.Text.Contains(roomType);
            var hasCorrectPrice = roomCard.Text.Contains($"£{price}");
            var hasBookingLink = roomCard.FindElements(By.TagName("a")).Count > 0;
            var hasBookButton = roomCard.FindElements(RoomCardBookButton).Count > 0;
            
            _logger.LogDebug("Room card verification results - Type: {HasType}, Price: {HasPrice}, Booking Link: {HasLink}, Book Button: {HasButton}",
                hasCorrectType, hasCorrectPrice, hasBookingLink, hasBookButton);
            
            return hasCorrectType && hasCorrectPrice && hasBookingLink && hasBookButton;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error verifying room card details");
            return false;
        }
    }

    public IList<IWebElement> GetAllRoomCards()
    {
        _logger.LogDebug("Getting all room cards from homepage");
        var roomCards = _driver.FindElements(RoomCards);
        _logger.LogDebug("Found {Count} room cards", roomCards.Count);
        return roomCards;
    }

    public IWebElement? FindRoomByType(string roomType)
    {
        var roomCards = _driver.FindElements(RoomCards);
        foreach (var card in roomCards)
        {
            try
            {
                var titleElement = card.FindElement(By.CssSelector("h5.card-title"));
                var title = titleElement.Text.Trim();
                
                if (title.Equals(roomType, StringComparison.OrdinalIgnoreCase))
                {
                    return card;
                }
            }
            catch (NoSuchElementException)
            {
                // Skip cards without titles
            }
        }
        
        return null;
    }

    public List<string> GetAvailableRoomTypes()
    {
        var roomTypes = new List<string>();
        var roomCards = _driver.FindElements(RoomCards);
        
        foreach (var card in roomCards)
        {
            try
            {
                var titleElement = card.FindElement(By.CssSelector("h5.card-title"));
                var title = titleElement.Text.Trim();
                roomTypes.Add(title);
            }
            catch (NoSuchElementException)
            {
                // Skip cards without titles
            }
        }
        
        return roomTypes;
    }

    public List<string> GetAvailableRoomTypesWithRetry(string context)
    {
        List<string> availableRooms = new List<string>();
        int retryCount = 0;
        const int maxRetries = 3;
        
        while (retryCount < maxRetries)
        {
            try
            {
                availableRooms = GetAvailableRoomTypes();
                break;
            }
            catch (StaleElementReferenceException)
            {
                retryCount++;
                
                if (retryCount >= maxRetries)
                {
                    _logger.LogError("Failed to get available rooms {Context} after {MaxRetries} retries due to stale elements", context, maxRetries);
                    throw;
                }
                
                WaitHelper.WaitForCondition(_driver, _ => 
                {
                    try
                    {
                        var cards = _driver.FindElements(RoomCards);
                        return cards.Count > 0;
                    }
                    catch (StaleElementReferenceException)
                    {
                        return false;
                    }
                }, TimeSpan.FromSeconds(5));
                WaitForRoomsToUpdate(TimeSpan.FromSeconds(10));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected exception while getting available rooms {Context}", context);
                throw;
            }
        }
        
        return availableRooms;
    }

    public int GetAvailableRoomsCount()
    {
        var roomCards = _driver.FindElements(RoomCards);
        _logger.LogDebug("Found {Count} available rooms", roomCards.Count);
        return roomCards.Count;
    }

    public void ClickFirstAvailableRoom()
    {
        if (!WaitForRoomsToUpdate(TimeSpan.FromSeconds(15)))
        {
            throw new TimeoutException("Room cards did not appear within timeout");
        }
        
        WaitHelper.WaitForCondition(_driver, _ => _driver.FindElements(RoomCardBookNowButtons).Count > 0, TimeSpan.FromSeconds(5));
        
        for (int attempt = 0; attempt < 3; attempt++)
        {
            try
            {
                var bookButtons = _driver.FindElements(RoomCardBookNowButtons);
                if (bookButtons.Count > 0)
                {
                    var firstButton = bookButtons[0];
                    
                    ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", firstButton);
                    WaitHelper.WaitForCondition(_driver, _ => 
                    {
                        var result = ((IJavaScriptExecutor)_driver).ExecuteScript("return arguments[0].getBoundingClientRect().top >= 0 && arguments[0].getBoundingClientRect().bottom <= window.innerHeight;", firstButton);
                        return result != null && result.Equals(true);
                    }, TimeSpan.FromSeconds(2));
                    
                    var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
                    wait.Until(ExpectedConditions.ElementToBeClickable(firstButton));
                    
                    try
                    {
                        firstButton.Click();
                        return;
                    }
                    catch (ElementClickInterceptedException)
                    {
                        ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", firstButton);
                        return;
                    }
                }
                else
                {
                    throw new NoSuchElementException("No room booking buttons found");
                }
            }
            catch (StaleElementReferenceException ex)
            {
                _logger.LogWarning("Stale element when clicking first room on attempt {Attempt}: {Message}", attempt + 1, ex.Message);
                if (attempt == 2) throw;
                WaitHelper.WaitForCondition(_driver, _ => 
                {
                    try
                    {
                        var buttons = _driver.FindElements(RoomCardBookNowButtons);
                        return buttons.Count > 0;
                    }
                    catch (StaleElementReferenceException)
                    {
                        return false;
                    }
                }, TimeSpan.FromSeconds(3));
            }
        }
    }

    public void ClickRoomByIndex(int index)
    {
        if (!WaitForRoomsToUpdate(TimeSpan.FromSeconds(15)))
        {
            throw new TimeoutException("Room cards did not appear within timeout");
        }
        
        var bookButtons = _driver.FindElements(RoomCardBookNowButtons);
        if (bookButtons.Count > index)
        {
            var targetButton = bookButtons[index];
            
            ScrollHelper.ScrollToElement(_driver, RoomCardBookNowButtons);
            WaitHelper.WaitForElementToBeClickable(_driver, RoomCardBookNowButtons, TimeSpan.FromSeconds(10));
            
            try
            {
                targetButton.Click();
            }
            catch (ElementClickInterceptedException)
            {
                ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", targetButton);
            }
        }
        else
        {
            throw new NoSuchElementException($"Room at index {index} not found. Available rooms: {bookButtons.Count}");
        }
    }

    public void ClickRoomByType(string roomType)
    {
        if (!WaitForRoomsToUpdate(TimeSpan.FromSeconds(15)))
        {
            throw new TimeoutException("Room cards did not appear within timeout");
        }
        
        var roomCard = FindRoomByType(roomType);
        if (roomCard == null)
        {
            throw new NoSuchElementException($"Room type '{roomType}' not found");
        }
        
        var bookButton = roomCard.FindElement(By.CssSelector(".btn-primary"));
        
        ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", bookButton);
        WaitHelper.WaitForCondition(_driver, _ => 
        {
            var result = ((IJavaScriptExecutor)_driver).ExecuteScript("return arguments[0].getBoundingClientRect().top >= 0 && arguments[0].getBoundingClientRect().bottom <= window.innerHeight;", bookButton);
            return result != null && result.Equals(true);
        }, TimeSpan.FromSeconds(2));
        
        var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
        wait.Until(ExpectedConditions.ElementToBeClickable(bookButton));
        
        try
        {
            bookButton.Click();
        }
        catch (ElementClickInterceptedException)
        {
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", bookButton);
        }
    }

    public bool WaitForRoomsToUpdate(TimeSpan? timeout = null)
    {
        return WaitHelper.WaitForCondition(_driver, d => 
        {
            try
            {
                var roomCards = d.FindElements(RoomCards);
                return roomCards.Any();
            }
            catch (StaleElementReferenceException)
            {
                return false;
            }
        }, timeout);
    }

    public bool NavigateToReservationPage()
    {
        try
        {
            var (checkIn, checkOut) = DateHelper.GenerateRandomTestDates();
            _logger.LogDebug("Using random dates for navigation: {CheckIn} to {CheckOut}", checkIn, checkOut);
            
            if (!WaitForRoomsToUpdate(TimeSpan.FromSeconds(15)))
            {
                _logger.LogWarning("No rooms available for booking with dates {CheckIn} - {CheckOut}", checkIn, checkOut);
                return false;
            }
            
            ClickFirstAvailableRoom();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to navigate to reservation page");
            return false;
        }
    }

    public bool NavigateToReservationPageForRoomType(string roomType)
    {
        try
        {
            var (checkIn, checkOut) = DateHelper.GenerateRandomTestDates();
            _logger.LogDebug("Using random dates for navigation: {CheckIn} to {CheckOut}", checkIn, checkOut);

            if (!WaitForRoomsToUpdate(TimeSpan.FromSeconds(15)))
            {
                _logger.LogWarning("No rooms available for booking with dates {CheckIn} - {CheckOut}", checkIn, checkOut);
                return false;
            }
            
            var roomCard = FindRoomByType(roomType);
            if (roomCard == null)
            {
                var availableRooms = string.Join(", ", GetAvailableRoomTypesWithRetry("RoomTypeNotFound"));
                _logger.LogWarning("Room type {RoomType} not found. Available rooms: {AvailableRooms}", roomType, availableRooms);
                return false;
            }
            
            ClickRoomByType(roomType);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to navigate to reservation page for room type {RoomType}", roomType);
            return false;
        }
    }
} 