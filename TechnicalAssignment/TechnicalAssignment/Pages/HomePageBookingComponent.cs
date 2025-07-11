using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using TechnicalAssignment.Utilities;
using TechnicalAssignment.Models;

namespace TechnicalAssignment.Pages;

public class HomePageBookingComponent
{
    protected readonly IWebDriver Driver;
    protected readonly ILogger Logger;
    
    private static readonly By RoomCards = By.CssSelector(".room-card");
    private static readonly By RoomCardBookButton = By.CssSelector(".btn-primary");
    private static readonly By BookingFormSection = By.CssSelector("section#booking");
    private static readonly By BookingCard = By.CssSelector(".booking-card");
    private static readonly By CheckInDatePicker = By.CssSelector("#booking .col-md-6:nth-child(1) input.form-control");
    private static readonly By CheckOutDatePicker = By.CssSelector("#booking .col-md-6:nth-child(2) input.form-control");
    private static readonly By CheckAvailabilityButton = By.CssSelector("#booking button.btn-primary");
    private static readonly By BookingFormTitle = By.CssSelector("#booking h3.card-title");
    private static readonly By RoomCardBookNowButtons = By.CssSelector(".room-card .btn-primary");

    public HomePageBookingDateValidationComponent DateValidation { get; }

    public HomePageBookingComponent(IWebDriver driver, ILogger logger)
    {
        Driver = driver;
        Logger = logger;
        DateValidation = new HomePageBookingDateValidationComponent(driver, logger);
    }

    public bool IsPageLoaded(TimeSpan? timeout = null)
    {
        return IsBookingFormSectionVisible(timeout);
    }

    public void WaitForPageToLoad(TimeSpan? timeout = null)
    {
        WaitHelper.WaitForElement(Driver, BookingFormSection, timeout);
        Logger.LogDebug("HomePageBookingComponent loaded successfully");
    }

    public void ScrollToRoomsSection()
    {
        ScrollHelper.ScrollToElement(Driver, RoomCards);
    }

    public IWebElement? FindRoomCard(string roomType, int price, IEnumerable<string> features)
    {
        Logger.LogDebug("Searching for room card with type {RoomType}, price £{Price}, and features {Features}", 
            roomType, price, string.Join(", ", features));
        
        var roomCards = Driver.FindElements(RoomCards);
        Logger.LogDebug("Found {Count} room cards to search", roomCards.Count);
        
        var targetRoomCard = roomCards.FirstOrDefault(card => 
            card.Text.Contains(roomType) && 
            card.Text.Contains($"£{price}") &&
            features.Any(feature => card.Text.Contains(feature)));
        
        if (targetRoomCard != null)
        {
            Logger.LogDebug("Found matching room card with type {RoomType} and price £{Price}", roomType, price);
        }
        else
        {
            Logger.LogDebug("No matching room card found for type {RoomType} and price £{Price}", roomType, price);
        }
        
        return targetRoomCard;
    }

    public bool VerifyRoomCardDetails(IWebElement roomCard, string roomType, int price)
    {
        Logger.LogDebug("Verifying room card details for type {RoomType} and price £{Price}", roomType, price);
        
        try
        {
            var hasCorrectType = roomCard.Text.Contains(roomType);
            var hasCorrectPrice = roomCard.Text.Contains($"£{price}");
            var hasBookingLink = roomCard.FindElements(By.TagName("a")).Count > 0;
            var hasBookButton = roomCard.FindElements(RoomCardBookButton).Count > 0;
            
            Logger.LogDebug("Room card verification results - Type: {HasType}, Price: {HasPrice}, Booking Link: {HasLink}, Book Button: {HasButton}",
                hasCorrectType, hasCorrectPrice, hasBookingLink, hasBookButton);
            
            return hasCorrectType && hasCorrectPrice && hasBookingLink && hasBookButton;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error verifying room card details");
            return false;
        }
    }

    public IList<IWebElement> GetAllRoomCards()
    {
        Logger.LogDebug("Getting all room cards from homepage");
        var roomCards = Driver.FindElements(RoomCards);
        Logger.LogDebug("Found {Count} room cards", roomCards.Count);
        return roomCards;
    }

    public IWebElement? FindRoomByType(string roomType)
    {
        var roomCards = Driver.FindElements(RoomCards);
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
        var roomCards = Driver.FindElements(RoomCards);
        
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
                    Logger.LogError("Failed to get available rooms {Context} after {MaxRetries} retries due to stale elements", context, maxRetries);
                    throw;
                }
                
                WaitHelper.WaitForCondition(Driver, _ => 
                {
                    try
                    {
                        var cards = Driver.FindElements(RoomCards);
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
                Logger.LogError(ex, "Unexpected exception while getting available rooms {Context}", context);
                throw;
            }
        }
        
        return availableRooms;
    }

    public int GetAvailableRoomsCount()
    {
        var roomCards = Driver.FindElements(RoomCards);
        Logger.LogDebug("Found {Count} available rooms", roomCards.Count);
        return roomCards.Count;
    }

    public void ClickFirstAvailableRoom()
    {
        if (!WaitForRoomsToUpdate(TimeSpan.FromSeconds(15)))
        {
            throw new TimeoutException("Room cards did not appear within timeout");
        }
        
        WaitHelper.WaitForCondition(Driver, _ => Driver.FindElements(RoomCardBookNowButtons).Count > 0, TimeSpan.FromSeconds(5));
        
        for (int attempt = 0; attempt < 3; attempt++)
        {
            try
            {
                var bookButtons = Driver.FindElements(RoomCardBookNowButtons);
                if (bookButtons.Count > 0)
                {
                    var firstButton = bookButtons[0];
                    
                    ((IJavaScriptExecutor)Driver).ExecuteScript("arguments[0].scrollIntoView(true);", firstButton);
                    WaitHelper.WaitForCondition(Driver, _ => 
                    {
                        var result = ((IJavaScriptExecutor)Driver).ExecuteScript("return arguments[0].getBoundingClientRect().top >= 0 && arguments[0].getBoundingClientRect().bottom <= window.innerHeight;", firstButton);
                        return result != null && result.Equals(true);
                    }, TimeSpan.FromSeconds(2));
                    
                    var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(10));
                    wait.Until(ExpectedConditions.ElementToBeClickable(firstButton));
                    
                    try
                    {
                        firstButton.Click();
                        return;
                    }
                    catch (ElementClickInterceptedException)
                    {
                        ((IJavaScriptExecutor)Driver).ExecuteScript("arguments[0].click();", firstButton);
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
                Logger.LogWarning("Stale element when clicking first room on attempt {Attempt}: {Message}", attempt + 1, ex.Message);
                if (attempt == 2) throw;
                WaitHelper.WaitForCondition(Driver, _ => 
                {
                    try
                    {
                        var buttons = Driver.FindElements(RoomCardBookNowButtons);
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
        
        var bookButtons = Driver.FindElements(RoomCardBookNowButtons);
        if (bookButtons.Count > index)
        {
            var targetButton = bookButtons[index];
            
            ScrollHelper.ScrollToElement(Driver, RoomCardBookNowButtons);
            WaitHelper.WaitForElementToBeClickable(Driver, RoomCardBookNowButtons, TimeSpan.FromSeconds(10));
            
            try
            {
                targetButton.Click();
            }
            catch (ElementClickInterceptedException)
            {
                ((IJavaScriptExecutor)Driver).ExecuteScript("arguments[0].click();", targetButton);
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
        
        ((IJavaScriptExecutor)Driver).ExecuteScript("arguments[0].scrollIntoView(true);", bookButton);
        WaitHelper.WaitForCondition(Driver, _ => 
        {
            var result = ((IJavaScriptExecutor)Driver).ExecuteScript("return arguments[0].getBoundingClientRect().top >= 0 && arguments[0].getBoundingClientRect().bottom <= window.innerHeight;", bookButton);
            return result != null && result.Equals(true);
        }, TimeSpan.FromSeconds(2));
        
        var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(10));
        wait.Until(ExpectedConditions.ElementToBeClickable(bookButton));
        
        try
        {
            bookButton.Click();
        }
        catch (ElementClickInterceptedException)
        {
            ((IJavaScriptExecutor)Driver).ExecuteScript("arguments[0].click();", bookButton);
        }
    }

    public bool WaitForRoomsToUpdate(TimeSpan? timeout = null)
    {
        return WaitHelper.WaitForCondition(Driver, d => 
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

    public void ScrollToBookingSection()
    {
        ScrollHelper.ScrollToElement(Driver, BookingFormSection);
    }

    public bool IsBookingFormSectionVisible(TimeSpan? timeout = null)
    {
        return ElementHelper.IsElementVisible(Driver, BookingFormSection, timeout);
    }

    public bool IsBookingCardVisible(TimeSpan? timeout = null)
    {
        return ElementHelper.IsElementVisible(Driver, BookingCard, timeout);
    }

    public string GetBookingFormTitle()
    {
        return ElementHelper.GetElementText(Driver, BookingFormTitle);
    }

    public bool IsCheckInDatePickerVisible(TimeSpan? timeout = null)
    {
        return ElementHelper.IsElementVisible(Driver, CheckInDatePicker, timeout);
    }

    public bool IsCheckOutDatePickerVisible(TimeSpan? timeout = null)
    {
        return ElementHelper.IsElementVisible(Driver, CheckOutDatePicker, timeout);
    }

    public bool IsCheckAvailabilityButtonVisible(TimeSpan? timeout = null)
    {
        return ElementHelper.IsElementVisible(Driver, CheckAvailabilityButton, timeout);
    }

    public string GetCheckInDate()
    {
        try
        {
            var dateValue = ElementHelper.GetElementAttribute(Driver, CheckInDatePicker, "value");
            Logger.LogDebug("Check-in date value retrieved: '{Value}'", dateValue);
            return dateValue;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to get check-in date value");
            
            try
            {
                var elements = Driver.FindElements(By.CssSelector("#booking input[type='text']"));
                if (elements.Count >= 2)
                {
                    var value = elements[0].GetAttribute("value") ?? string.Empty;
                    Logger.LogDebug("Check-in date from alternative selector: '{Value}'", value);
                    return value;
                }
            }
            catch (Exception altEx)
            {
                Logger.LogError(altEx, "Alternative check-in date retrieval also failed");
            }
            
            throw;
        }
    }

    public string GetCheckOutDate()
    {
        try
        {
            var dateValue = ElementHelper.GetElementAttribute(Driver, CheckOutDatePicker, "value");
            Logger.LogDebug("Check-out date value retrieved: '{Value}'", dateValue);
            return dateValue;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to get check-out date value");
            
            try
            {
                var elements = Driver.FindElements(By.CssSelector("#booking input[type='text']"));
                if (elements.Count >= 2)
                {
                    var value = elements[1].GetAttribute("value") ?? string.Empty;
                    Logger.LogDebug("Check-out date from alternative selector: '{Value}'", value);
                    return value;
                }
            }
            catch (Exception altEx)
            {
                Logger.LogError(altEx, "Alternative check-out date retrieval also failed");
            }
            
            throw;
        }
    }

    public void SetCheckInDate(string date)
    {
        var checkInElement = WaitHelper.WaitForElement(Driver, CheckInDatePicker, TimeSpan.FromSeconds(10));
        
        checkInElement.Click();
        WaitHelper.WaitForCondition(Driver, _ => checkInElement.Equals(Driver.SwitchTo().ActiveElement()), TimeSpan.FromSeconds(2));
        
        checkInElement.SendKeys(Keys.Control + "a");
        WaitHelper.WaitForCondition(Driver, _ => 
        {
            try
            {
                var script = "return window.getSelection().toString().length > 0 || arguments[0].selectionStart !== arguments[0].selectionEnd;";
                var result = ((IJavaScriptExecutor)Driver).ExecuteScript(script, checkInElement);
                return result is bool boolResult && boolResult;
            }
            catch
            {
                return true;
            }
        }, TimeSpan.FromMilliseconds(500));
        checkInElement.SendKeys(date);
        WaitHelper.WaitForCondition(Driver, _ => !string.IsNullOrEmpty(checkInElement.GetAttribute("value")), TimeSpan.FromSeconds(2));
        
        checkInElement.SendKeys(Keys.Tab);
        WaitHelper.WaitForCondition(Driver, _ => !checkInElement.Equals(Driver.SwitchTo().ActiveElement()), TimeSpan.FromSeconds(2));
        
        var actualDate = checkInElement.GetAttribute("value");
        Logger.LogDebug("Check-in date set to: {ActualDate}", actualDate);
    }

    public void SetCheckOutDate(string date)
    {
        var checkOutElement = WaitHelper.WaitForElement(Driver, CheckOutDatePicker, TimeSpan.FromSeconds(10));
        
        checkOutElement.Click();
        WaitHelper.WaitForCondition(Driver, _ => checkOutElement.Equals(Driver.SwitchTo().ActiveElement()), TimeSpan.FromSeconds(2));
        
        checkOutElement.SendKeys(Keys.Control + "a");
        // Wait for text selection to complete
        WaitHelper.WaitForCondition(Driver, _ => 
        {
            try
            {
                var script = "return window.getSelection().toString().length > 0 || arguments[0].selectionStart !== arguments[0].selectionEnd;";
                var result = ((IJavaScriptExecutor)Driver).ExecuteScript(script, checkOutElement);
                return result is bool boolResult && boolResult;
            }
            catch
            {
                return true; // If we can't check selection, proceed anyway
            }
        }, TimeSpan.FromMilliseconds(500));
        checkOutElement.SendKeys(date);
        WaitHelper.WaitForCondition(Driver, _ => !string.IsNullOrEmpty(checkOutElement.GetAttribute("value")), TimeSpan.FromSeconds(2));
        
        checkOutElement.SendKeys(Keys.Tab);
        WaitHelper.WaitForCondition(Driver, _ => !checkOutElement.Equals(Driver.SwitchTo().ActiveElement()), TimeSpan.FromSeconds(2));
        
        var actualDate = checkOutElement.GetAttribute("value");
        Logger.LogDebug("Check-out date set to: {ActualDate}", actualDate);
    }

    public void ClickCheckAvailability()
    {
        ElementHelper.SafeClick(Driver, CheckAvailabilityButton);
    }

    public void CheckAvailability(string checkInDate, string checkOutDate)
    {
        Logger.LogDebug("Checking availability for dates: {CheckIn} to {CheckOut}", checkInDate, checkOutDate);
        SetCheckInDate(checkInDate);
        SetCheckOutDate(checkOutDate);
        ClickCheckAvailability();
        WaitForRoomsToUpdate(TimeSpan.FromSeconds(10));
    }

    public bool ValidateBookingFormElements()
    {
        Logger.LogDebug("Validating booking form elements");
        
        var isFormVisible = IsBookingFormSectionVisible(TimeSpan.FromSeconds(5));
        var isCheckInVisible = IsCheckInDatePickerVisible(TimeSpan.FromSeconds(5));
        var isCheckOutVisible = IsCheckOutDatePickerVisible(TimeSpan.FromSeconds(5));
        var isButtonVisible = IsCheckAvailabilityButtonVisible(TimeSpan.FromSeconds(5));
        
        Logger.LogDebug("Booking form validation - Form: {Form}, CheckIn: {CheckIn}, CheckOut: {CheckOut}, Button: {Button}",
            isFormVisible, isCheckInVisible, isCheckOutVisible, isButtonVisible);
        
        return isFormVisible && isCheckInVisible && isCheckOutVisible && isButtonVisible;
    }

    public IWebElement GetBookingFormElement()
    {
        return WaitHelper.WaitForElement(Driver, BookingFormSection);
    }

    public IWebElement GetCheckAvailabilityButtonElement()
    {
        return WaitHelper.WaitForElement(Driver, CheckAvailabilityButton);
    }

    public bool IsCheckOutDateAfterCheckIn()
    {
        var checkInDate = GetCheckInDate();
        var checkOutDate = GetCheckOutDate();
        
        Logger.LogDebug("Comparing dates - CheckIn: {CheckIn}, CheckOut: {CheckOut}", checkInDate, checkOutDate);
        
        var dateFormats = new[] { "dd/MM/yyyy", "MM/dd/yyyy", "yyyy-MM-dd", "dd-MM-yyyy", "MM-dd-yyyy" };
        
        DateTime checkIn, checkOut;
        bool checkInParsed = false, checkOutParsed = false;
        
        checkInParsed = DateTime.TryParseExact(checkInDate, dateFormats, System.Globalization.CultureInfo.InvariantCulture, 
            System.Globalization.DateTimeStyles.None, out checkIn);
        
        if (!checkInParsed)
        {
            checkInParsed = DateTime.TryParse(checkInDate, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out checkIn);
        }
        
        checkOutParsed = DateTime.TryParseExact(checkOutDate, dateFormats, System.Globalization.CultureInfo.InvariantCulture, 
            System.Globalization.DateTimeStyles.None, out checkOut);
        
        if (!checkOutParsed)
        {
            checkOutParsed = DateTime.TryParse(checkOutDate, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out checkOut);
        }
        
        if (checkInParsed && checkOutParsed)
        {
            var isValid = checkOut > checkIn;
            Logger.LogDebug("Date validation result: {IsValid} (CheckIn: {ParsedCheckIn}, CheckOut: {ParsedCheckOut})", 
                isValid, checkIn.ToString("yyyy-MM-dd"), checkOut.ToString("yyyy-MM-dd"));
            return isValid;
        }
        
        Logger.LogWarning("Could not parse dates for validation - CheckIn: '{CheckIn}' (Parsed: {CheckInParsed}), CheckOut: '{CheckOut}' (Parsed: {CheckOutParsed})", 
            checkInDate, checkInParsed, checkOutDate, checkOutParsed);
        return false;
    }

    public bool IsCheckOutDateValidForCheckIn()
    {
        var checkInDate = GetCheckInDate();
        var checkOutDate = GetCheckOutDate();
        
        Logger.LogDebug("Validating date order - CheckIn: {CheckIn}, CheckOut: {CheckOut}", checkInDate, checkOutDate);
        
        var dateFormats = new[] { "dd/MM/yyyy", "MM/dd/yyyy", "yyyy-MM-dd", "dd-MM-yyyy", "MM-dd-yyyy" };
        
        DateTime checkIn, checkOut;
        bool checkInParsed = false, checkOutParsed = false;
        
        checkInParsed = DateTime.TryParseExact(checkInDate, dateFormats, System.Globalization.CultureInfo.InvariantCulture, 
            System.Globalization.DateTimeStyles.None, out checkIn);
        
        if (!checkInParsed)
        {
            checkInParsed = DateTime.TryParse(checkInDate, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out checkIn);
        }
        
        checkOutParsed = DateTime.TryParseExact(checkOutDate, dateFormats, System.Globalization.CultureInfo.InvariantCulture, 
            System.Globalization.DateTimeStyles.None, out checkOut);
        
        if (!checkOutParsed)
        {
            checkOutParsed = DateTime.TryParse(checkOutDate, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out checkOut);
        }
        
        if (checkInParsed && checkOutParsed)
        {
            var isValid = checkOut >= checkIn;
            Logger.LogDebug("Date order validation result: {IsValid} (CheckIn: {ParsedCheckIn}, CheckOut: {ParsedCheckOut})", 
                isValid, checkIn.ToString("yyyy-MM-dd"), checkOut.ToString("yyyy-MM-dd"));
            return isValid;
        }
        
        Logger.LogWarning("Could not parse dates for order validation - CheckIn: '{CheckIn}' (Parsed: {CheckInParsed}), CheckOut: '{CheckOut}' (Parsed: {CheckOutParsed})", 
            checkInDate, checkInParsed, checkOutDate, checkOutParsed);
        return false;
    }

    public DateValidationResult ValidateDateInput(string checkInDate, string checkOutDate)
    {
        var result = new DateValidationResult
        {
            IsValid = true,
            CheckInDate = checkInDate,
            CheckOutDate = checkOutDate,
            ValidationErrors = new List<string>()
        };
        
        var dateFormats = new[] { "dd/MM/yyyy" };
        var checkInParsed = DateTime.TryParseExact(checkInDate, dateFormats, System.Globalization.CultureInfo.InvariantCulture, 
            System.Globalization.DateTimeStyles.None, out var checkIn) ||
            DateTime.TryParse(checkInDate, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out checkIn);
        
        var checkOutParsed = DateTime.TryParseExact(checkOutDate, dateFormats, System.Globalization.CultureInfo.InvariantCulture, 
            System.Globalization.DateTimeStyles.None, out var checkOut) ||
            DateTime.TryParse(checkOutDate, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out checkOut);
        
        if (!checkInParsed)
        {
            result.IsValid = false;
            result.ValidationErrors.Add($"Check-in date '{checkInDate}' is not in a valid format");
        }
        
        if (!checkOutParsed)
        {
            result.IsValid = false;
            result.ValidationErrors.Add($"Check-out date '{checkOutDate}' is not in a valid format");
        }
        
        if (checkInParsed && checkOutParsed)
        {
            result.ParsedCheckInDate = checkIn;
            result.ParsedCheckOutDate = checkOut;
            
            if (checkIn < DateTime.Today)
            {
                result.IsValid = false;
                result.ValidationErrors.Add($"Check-in date '{checkInDate}' is in the past");
                result.HasPastCheckIn = true;
            }
            
            if (checkOut < checkIn)
            {
                result.IsValid = false;
                result.ValidationErrors.Add($"Check-out date '{checkOutDate}' is before check-in date '{checkInDate}'");
                result.HasInvalidDateOrder = true;
            }
            
            if (checkOut.Date == checkIn.Date)
            {
                result.IsValid = false;
                result.ValidationErrors.Add($"Check-in and check-out dates cannot be the same day");
                result.HasSameDayBooking = true;
            }
        }
        
        return result;
    }

    public DateValidationResult ValidatePastDatesHandling()
    {
        return DateValidation.GetPastDatesHandlingResult();
    }

    public DateValidationResult ValidateInvalidDateOrderHandling()
    {
        return DateValidation.GetInvalidDateOrderHandlingResult();
    }

    public DateValidationResult ValidateSameDayBookingHandling()
    {
        return DateValidation.GetSameDayBookingHandlingResult();
    }

    public bool NavigateToReservationPage()
    {
        try
        {
            ScrollToBookingSection();
            
            var (checkIn, checkOut) = DateHelper.GenerateRandomTestDates();
            Logger.LogDebug("Using random dates for navigation: {CheckIn} to {CheckOut}", checkIn, checkOut);
            CheckAvailability(checkIn, checkOut);
            
            if (!WaitForRoomsToUpdate(TimeSpan.FromSeconds(15)))
            {
                Logger.LogWarning("No rooms available for booking with dates {CheckIn} - {CheckOut}", checkIn, checkOut);
                return false;
            }
            
            ClickFirstAvailableRoom();
            return true;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to navigate to reservation page");
            return false;
        }
    }

    public bool NavigateToReservationPageForRoomType(string roomType)
    {
        try
        {
            ScrollToBookingSection();

            var (checkIn, checkOut) = DateHelper.GenerateRandomTestDates();
            Logger.LogDebug("Using random dates for navigation: {CheckIn} to {CheckOut}", checkIn, checkOut);
            CheckAvailability(checkIn, checkOut);

            if (!WaitForRoomsToUpdate(TimeSpan.FromSeconds(15)))
            {
                Logger.LogWarning("No rooms available for booking with dates {CheckIn} - {CheckOut}", checkIn, checkOut);
                return false;
            }
            
            var roomCard = FindRoomByType(roomType);
            if (roomCard == null)
            {
                var availableRooms = string.Join(", ", GetAvailableRoomTypesWithRetry("RoomTypeNotFound"));
                Logger.LogWarning("Room type {RoomType} not found. Available rooms: {AvailableRooms}", roomType, availableRooms);
                return false;
            }
            
            ClickRoomByType(roomType);
            return true;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to navigate to reservation page for room type {RoomType}", roomType);
            return false;
        }
    }

    public bool EnsureOnHomePageForBooking(string baseUrl)
    {
        try
        {
            var currentUrl = Driver.Url;
            if (!currentUrl.Contains(baseUrl.Replace("http://", "").Replace("https://", "")))
            {
                Driver.Navigate().GoToUrl(baseUrl);
                WaitForPageToLoad(TimeSpan.FromSeconds(10));
            }
            return true;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to ensure on home page for booking");
            return false;
        }
    }

    public bool TestBookingFormResponsiveDisplay(int width, int height, string deviceName)
    {
        Logger.LogDebug("Testing booking form responsive display on {DeviceName} ({Width}x{Height})", deviceName, width, height);
        
        BrowserHelper.SetViewportSize(Driver, width, height);
        
        // Simple wait for viewport to stabilize - just wait for a moment rather than complex condition
        WaitHelper.WaitForCondition(Driver, _ => 
        {
            // Just check that we can interact with the page elements
            try
            {
                var elements = Driver.FindElements(BookingFormSection);
                return elements.Count > 0;
            }
            catch
            {
                return false;
            }
        }, TimeSpan.FromSeconds(5));
        
        ScrollToBookingSection();
        var isValid = ValidateBookingFormElements();
        
        Logger.LogDebug("Booking form validation result on {DeviceName}: {IsValid}", deviceName, isValid);
        return isValid;
    }
}

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