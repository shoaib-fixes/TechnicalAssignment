using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using TechnicalAssignment.Utilities;

namespace TechnicalAssignment.Pages;

public class HomePageBookingComponent
{
    protected readonly IWebDriver Driver;
    protected readonly ILogger Logger;
    
    private static readonly By BookingSection = By.Id("booking");
    private static readonly By RoomCards = By.CssSelector(".room-card");
    private static readonly By RoomTypeElements = By.CssSelector(".room-type");
    private static readonly By RoomPriceElements = By.CssSelector(".room-price");
    private static readonly By RoomFeaturesElements = By.CssSelector(".room-features");

    public HomePageBookingComponent(IWebDriver driver, ILogger logger)
    {
        Driver = driver;
        Logger = logger;
    }

    public void ScrollToBookingSection()
    {
        Logger.LogDebug("Scrolling to booking section");
        ScrollHelper.ScrollToElement(Driver, BookingSection, TimeSpan.FromSeconds(10));
    }

    public bool IsBookingSectionVisible(TimeSpan? timeout = null)
    {
        return ElementHelper.IsElementVisible(Driver, BookingSection, timeout);
    }

    public IWebElement? FindRoomCard(string roomType, int price, IEnumerable<string> features)
    {
        Logger.LogDebug("Searching for room card with type: {RoomType}, price: {Price}, features: {Features}", 
            roomType, price, string.Join(", ", features));
        
        var roomCards = Driver.FindElements(RoomCards);
        
        foreach (var card in roomCards)
        {
            try
            {
                var cardType = GetRoomCardType(card);
                var cardPrice = GetRoomCardPrice(card);
                var cardFeatures = GetRoomCardFeatures(card);
                
                Logger.LogDebug("Checking room card - Type: {CardType}, Price: {CardPrice}, Features: {CardFeatures}", 
                    cardType, cardPrice, string.Join(", ", cardFeatures));
                
                if (cardType.Equals(roomType, StringComparison.OrdinalIgnoreCase) &&
                    cardPrice == price &&
                    features.All(f => cardFeatures.Contains(f, StringComparer.OrdinalIgnoreCase)))
                {
                    Logger.LogDebug("Found matching room card");
                    return card;
                }
            }
            catch (Exception ex)
            {
                Logger.LogWarning(ex, "Error checking room card");
            }
        }
        
        Logger.LogDebug("No matching room card found");
        return null;
    }

    public bool VerifyRoomCardDetails(IWebElement roomCard, string expectedType, int expectedPrice)
    {
        try
        {
            var actualType = GetRoomCardType(roomCard);
            var actualPrice = GetRoomCardPrice(roomCard);
            
            Logger.LogDebug("Verifying room card details - Expected: {ExpectedType}/{ExpectedPrice}, Actual: {ActualType}/{ActualPrice}", 
                expectedType, expectedPrice, actualType, actualPrice);
            
            return actualType.Equals(expectedType, StringComparison.OrdinalIgnoreCase) && 
                   actualPrice == expectedPrice;
        }
        catch (Exception ex)
        {
            Logger.LogWarning(ex, "Error verifying room card details");
            return false;
        }
    }

    private string GetRoomCardType(IWebElement roomCard)
    {
        try
        {
            var typeElement = roomCard.FindElement(RoomTypeElements);
            return typeElement.Text.Trim();
        }
        catch (NoSuchElementException)
        {
            var typeElement = roomCard.FindElement(By.CssSelector("h5, .card-title, [data-testid*='type']"));
            return typeElement.Text.Trim();
        }
    }

    private int GetRoomCardPrice(IWebElement roomCard)
    {
        try
        {
            var priceElement = roomCard.FindElement(RoomPriceElements);
            var priceText = priceElement.Text.Trim();
            var numericText = new string(priceText.Where(char.IsDigit).ToArray());
            return int.Parse(numericText);
        }
        catch (NoSuchElementException)
        {
            var priceElement = roomCard.FindElement(By.CssSelector(".price, [data-testid*='price'], .cost"));
            var priceText = priceElement.Text.Trim();
            var numericText = new string(priceText.Where(char.IsDigit).ToArray());
            return int.Parse(numericText);
        }
    }

    private List<string> GetRoomCardFeatures(IWebElement roomCard)
    {
        try
        {
            var featuresElements = roomCard.FindElements(RoomFeaturesElements);
            return featuresElements.Select(e => e.Text.Trim()).Where(t => !string.IsNullOrEmpty(t)).ToList();
        }
        catch (NoSuchElementException)
        {
            var featuresElements = roomCard.FindElements(By.CssSelector(".features, [data-testid*='features'], .amenities"));
            return featuresElements.Select(e => e.Text.Trim()).Where(t => !string.IsNullOrEmpty(t)).ToList();
        }
    }

    public IWebElement GetBookingSectionElement()
    {
        Logger.LogDebug("Getting booking section element");
        return WaitHelper.WaitForElement(Driver, BookingSection);
    }

    public List<IWebElement> GetAllRoomCards()
    {
        Logger.LogDebug("Getting all room cards");
        return Driver.FindElements(RoomCards).ToList();
    }

    public int GetRoomCardCount()
    {
        var cards = GetAllRoomCards();
        Logger.LogDebug("Found {Count} room cards", cards.Count);
        return cards.Count;
    }
} 