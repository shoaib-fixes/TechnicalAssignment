using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using TechnicalAssignment.Utilities;
using TechnicalAssignment.Models;

namespace TechnicalAssignment.Pages;

public class ReservationPage : BasePage
{
    private static readonly By RoomTitle = By.CssSelector("h1.fw-bold");
    private static readonly By AccessibilityBadge = By.CssSelector(".badge.bg-success");
    private static readonly By GuestCount = By.CssSelector(".text-muted i.bi-people-fill");
    private static readonly By RoomImage = By.CssSelector("img.hero-image");
    private static readonly By RoomDescription = By.XPath("//h2[text()='Room Description']/following-sibling::p");
    private static readonly By RoomFeatures = By.CssSelector(".amenity-icon");
    private static readonly By RoomPolicies = By.CssSelector(".card.border-0.shadow-sm");
    
    private static readonly By BookingCard = By.CssSelector(".card.border-0.shadow.booking-card");
    private static readonly By RoomPriceDisplay = By.CssSelector(".fs-2.fw-bold.text-primary");
    private static readonly By ReserveNowButton = By.Id("doReservation");
    
    private static readonly By BookingConfirmedTitle = By.XPath("//h2[text()='Booking Confirmed']");
    private static readonly By ConfirmationMessage = By.XPath("//p[contains(text(), 'Your booking has been confirmed')]");
    private static readonly By ConfirmedDates = By.CssSelector("p.text-center strong");
    private static readonly By ReturnHomeButton = By.CssSelector("a.btn.btn-primary");
    
    private static readonly By SimilarRoomsSection = By.XPath("//h2[text()='Similar Rooms You Might Like']");
    private static readonly By SimilarRoomCards = By.CssSelector(".col-md-6.col-lg-4 .card");

    public ReservationPagePriceSummaryComponent PriceSummary { get; }
    public ReservationPageGuestFormComponent GuestForm { get; }
    public ReservationPageCalendarComponent Calendar { get; }

    public ReservationPage(
        IWebDriver driver, 
        ILogger<ReservationPage> logger,
        ReservationPagePriceSummaryComponent priceSummary,
        ReservationPageGuestFormComponent guestForm,
        ReservationPageCalendarComponent calendar) : base(driver, logger)
    {
        PriceSummary = priceSummary;
        GuestForm = guestForm;
        Calendar = calendar;
    }

    public override bool IsPageLoaded(TimeSpan? timeout = null)
    {
        return ElementHelper.IsElementVisible(Driver, RoomTitle, timeout) &&
               ElementHelper.IsElementVisible(Driver, BookingCard, timeout);
    }

    public override void WaitForPageToLoad(TimeSpan? timeout = null)
    {
        WaitHelper.WaitForElement(Driver, RoomTitle, timeout);
        WaitHelper.WaitForElement(Driver, BookingCard, timeout);
        Logger.LogDebug("ReservationPage loaded successfully");
    }

    public string GetRoomTitle()
    {
        Logger.LogDebug("Getting room title");
        return ElementHelper.GetElementText(Driver, RoomTitle);
    }

    public bool HasAccessibilityBadge()
    {
        Logger.LogDebug("Checking for accessibility badge");
        return ElementHelper.IsElementVisible(Driver, AccessibilityBadge, TimeSpan.FromSeconds(2));
    }

    public string GetAccessibilityBadgeText()
    {
        Logger.LogDebug("Getting accessibility badge text");
        return ElementHelper.GetElementText(Driver, AccessibilityBadge);
    }

    public string GetGuestCount()
    {
        Logger.LogDebug("Getting guest count");
        
        try
        {
            var guestElements = Driver.FindElements(By.CssSelector(".text-muted"));
            foreach (var element in guestElements)
            {
                var text = element.Text.Trim();
                if (text.ToLower().Contains("guest") || text.ToLower().Contains("people") || text.ToLower().Contains("max"))
                {
                    Logger.LogDebug("Found guest count text: {Text}", text);
                    return text;
                }
            }
            
            var peopleIcon = Driver.FindElement(GuestCount);
            var parentElement = peopleIcon.FindElement(By.XPath(".."));
            var guestText = parentElement.Text.Trim();
            Logger.LogDebug("Found guest count from parent element: {Text}", guestText);
            return guestText;
        }
        catch (Exception ex)
        {
            Logger.LogWarning("Could not find guest count information: {Message}", ex.Message);
            return "";
        }
    }

    public int GetMaxGuestCount()
    {
        Logger.LogDebug("Getting max guest count");
        
        var guestText = GetGuestCount();
        if (string.IsNullOrEmpty(guestText))
        {
            Logger.LogWarning("No guest count text found");
            return 0;
        }
        
        var numbers = System.Text.RegularExpressions.Regex.Matches(guestText, @"\d+");
        if (numbers.Count > 0)
        {
            var maxGuests = int.Parse(numbers[numbers.Count - 1].Value);
            Logger.LogDebug("Extracted max guest count: {Count} from text: {Text}", maxGuests, guestText);
            return maxGuests;
        }
        
        Logger.LogWarning("Could not extract numeric guest count from text: {Text}", guestText);
        return 0;
    }

    public bool ValidateGuestCount(string roomType, int expectedGuestCount)
    {
        Logger.LogDebug("Validating guest count for room type: {RoomType}, expected: {Expected}", roomType, expectedGuestCount);
        
        var actualGuestCount = GetMaxGuestCount();
        var isValid = actualGuestCount == expectedGuestCount;
        
        Logger.LogDebug("Guest count validation - Room: {Room}, Expected: {Expected}, Actual: {Actual}, Valid: {Valid}", 
            roomType, expectedGuestCount, actualGuestCount, isValid);
        
        return isValid;
    }

    public bool IsRoomImageDisplayed()
    {
        Logger.LogDebug("Checking if room image is displayed");
        return ElementHelper.IsElementVisible(Driver, RoomImage, TimeSpan.FromSeconds(5));
    }

    public bool IsRoomImageLoadedSuccessfully()
    {
        Logger.LogDebug("Validating room image loads successfully");
        
        try
        {
            var imageElement = WaitHelper.WaitForElement(Driver, RoomImage, TimeSpan.FromSeconds(10));
            
            if (!imageElement.Displayed)
            {
                Logger.LogDebug("Image element is not visible");
                return false;
            }
            
            var imageSrc = imageElement.GetAttribute("src");
            if (string.IsNullOrEmpty(imageSrc))
            {
                Logger.LogDebug("Image src attribute is empty");
                return false;
            }
            
            Logger.LogDebug("Image src: {ImageSrc}", imageSrc);
            
            var jsExecutor = (IJavaScriptExecutor)Driver;
            var imageLoadStatus = jsExecutor.ExecuteScript(@"
                var img = arguments[0];
                if (img.complete && img.naturalWidth !== 0) {
                    return 'loaded';
                } else if (img.complete && img.naturalWidth === 0) {
                    return 'error';
                } else {
                    return 'loading';
                }
            ", imageElement);
            
            Logger.LogDebug("Image load status: {Status}", imageLoadStatus);
            
            if (imageLoadStatus?.ToString() == "loading")
            {
                Logger.LogDebug("Image still loading, waiting for completion");
                
                var loadResult = WaitHelper.WaitForCondition(Driver, _ => 
                {
                    var status = jsExecutor.ExecuteScript(@"
                        var img = arguments[0];
                        return img.complete && img.naturalWidth !== 0;
                    ", imageElement);
                    return status is bool boolStatus && boolStatus;
                }, TimeSpan.FromSeconds(10));
                
                return loadResult;
            }
            
            var isLoaded = imageLoadStatus?.ToString() == "loaded";
            Logger.LogDebug("Image load validation result: {IsLoaded}", isLoaded);
            return isLoaded;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error validating image load status");
            return false;
        }
    }

    public string GetRoomImageSrc()
    {
        Logger.LogDebug("Getting room image source URL");
        try
        {
            var imageElement = WaitHelper.WaitForElement(Driver, RoomImage, TimeSpan.FromSeconds(5));
            var imageSrc = imageElement.GetAttribute("src");
            Logger.LogDebug("Room image src: {ImageSrc}", imageSrc);
            return imageSrc ?? string.Empty;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error getting room image source");
            return string.Empty;
        }
    }

    public string GetRoomDescription()
    {
        Logger.LogDebug("Getting room description");
        return ElementHelper.GetElementText(Driver, RoomDescription);
    }

    public List<string> GetRoomFeatures()
    {
        Logger.LogDebug("Getting room features");
        var featureElements = Driver.FindElements(RoomFeatures);
        var features = new List<string>();
        
        foreach (var element in featureElements)
        {
            var featureText = element.FindElement(By.XPath("following-sibling::span")).Text;
            features.Add(featureText);
        }
        
        Logger.LogDebug("Found {Count} room features", features.Count);
        return features;
    }

    public bool AreRoomPoliciesDisplayed()
    {
        Logger.LogDebug("Checking if room policies are displayed");
        return ElementHelper.IsElementVisible(Driver, RoomPolicies, TimeSpan.FromSeconds(5));
    }

    public string GetRoomPrice()
    {
        Logger.LogDebug("Getting room price");
        return ElementHelper.GetElementText(Driver, RoomPriceDisplay);
    }

    public bool IsCalendarVisible()
    {
        return Calendar.IsCalendarVisible();
    }

    public void ClickCalendarDate(int day)
    {
        Logger.LogDebug("Clicking calendar date: {Day}", day);
        var dateButton = Driver.FindElement(By.XPath($"//button[@class='rbc-button-link' and text()='{day}']"));
        dateButton.Click();
    }

    public void ClickCalendarNext()
    {
        Calendar.ClickNext();
    }

    public void ClickCalendarBack()
    {
        Calendar.ClickBack();
    }

    public void ClickCalendarToday()
    {
        Calendar.ClickToday();
    }

    public string GetCalendarMonth()
    {
        return Calendar.GetCurrentMonth();
    }

    public string GetBasePriceFromSummary()
    {
        return PriceSummary.GetBasePriceText();
    }

    public string GetCleaningFee()
    {
        return PriceSummary.GetCleaningFeeText();
    }

    public string GetServiceFee()
    {
        return PriceSummary.GetServiceFeeText();
    }

    public string GetTotalPrice()
    {
        return PriceSummary.GetTotalPriceText();
    }

    public string GetNightsCount()
    {
        return PriceSummary.GetNightsCountText();
    }

    // Removed ValidatePriceSummary method - validation logic should be in test methods
    // Use PriceSummary component methods to get individual price values for assertions

    public void ClickReserveNow()
    {
        Logger.LogDebug("Clicking Reserve Now button");
        
        ScrollHelper.ScrollToElement(Driver, ReserveNowButton);
        WaitHelper.WaitForElementToBeClickable(Driver, ReserveNowButton, TimeSpan.FromSeconds(10));
        
        try
        {
            ElementHelper.SafeClick(Driver, ReserveNowButton);
            Logger.LogDebug("Successfully clicked Reserve Now button");
        }
        catch (ElementClickInterceptedException ex)
        {
            Logger.LogDebug("Regular click intercepted, trying JavaScript click: {Message}", ex.Message);
            ElementHelper.JavaScriptClick(Driver, ReserveNowButton);
            Logger.LogDebug("Successfully clicked Reserve Now button with JavaScript");
        }
    }

    public bool IsGuestFormVisible()
    {
        return GuestForm.IsGuestFormVisible();
    }

    public bool IsGuestFormVisibleFast()
    {
        return GuestForm.IsGuestFormVisibleFast();
    }

    public void FillGuestInformation(string firstName, string lastName, string email, string phone)
    {
        GuestForm.FillGuestInformation(firstName, lastName, email, phone);
    }

    public void SubmitGuestForm()
    {
        GuestForm.SubmitForm();
    }

    public void ClickCancel()
    {
        GuestForm.ClickCancel();
    }

    public void CompleteBooking(string firstName, string lastName, string email, string phone)
    {
        Logger.LogDebug("Completing booking process");
        ClickReserveNow();
        WaitHelper.WaitForCondition(Driver, d => IsGuestFormVisible(), TimeSpan.FromSeconds(5));
        FillGuestInformation(firstName, lastName, email, phone);
        SubmitGuestForm();
    }

    public bool AreValidationErrorsVisible()
    {
        return GuestForm.AreValidationErrorsVisible();
    }

    public List<string> GetValidationErrors()
    {
        return GuestForm.GetValidationErrors();
    }

    public bool HasValidationError(string errorText)
    {
        return GuestForm.HasValidationError(errorText);
    }

    public bool IsBookingConfirmed()
    {
        Logger.LogDebug("Checking if booking is confirmed");
        return ElementHelper.IsElementVisible(Driver, BookingConfirmedTitle, TimeSpan.FromSeconds(10));
    }

    public bool IsBookingConfirmedFast()
    {
        try
        {
            return Driver.FindElements(BookingConfirmedTitle).Count > 0;
        }
        catch
        {
            return false;
        }
    }

    public string GetConfirmationMessage()
    {
        Logger.LogDebug("Getting confirmation message");
        return ElementHelper.GetElementText(Driver, ConfirmationMessage);
    }

    public string GetConfirmedDates()
    {
        Logger.LogDebug("Getting confirmed dates");
        return ElementHelper.GetElementText(Driver, ConfirmedDates);
    }

    public void ClickReturnHome()
    {
        Logger.LogDebug("Clicking Return Home button");
        ScrollHelper.ScrollToElement(Driver, ReturnHomeButton);
        ElementHelper.SafeClick(Driver, ReturnHomeButton);
    }

    public bool ValidateBookingConfirmation(string expectedCheckIn, string expectedCheckOut)
    {
        Logger.LogDebug("Validating booking confirmation");
        
        var isConfirmed = IsBookingConfirmed();
        var confirmedDates = GetConfirmedDates();
        
        var hasCorrectDates = confirmedDates.Contains(expectedCheckIn) && 
                              confirmedDates.Contains(expectedCheckOut);
        
        Logger.LogDebug("Confirmation validation - Confirmed: {Confirmed}, Dates: {Dates}", 
            isConfirmed, hasCorrectDates);
        
        return isConfirmed && hasCorrectDates;
    }

    public bool IsSimilarRoomsSectionVisible()
    {
        Logger.LogDebug("Checking if similar rooms section is visible");
        return ElementHelper.IsElementVisible(Driver, SimilarRoomsSection, TimeSpan.FromSeconds(5));
    }

    public int GetSimilarRoomsCount()
    {
        var roomCards = Driver.FindElements(SimilarRoomCards);
        Logger.LogDebug("Found {Count} similar rooms", roomCards.Count);
        return roomCards.Count;
    }

    public static BookingTestData.GuestInfo GenerateGuestInfo()
    {
        return BookingTestData.GenerateRandomGuest();
    }

    public void WaitForBookingState()
    {
        Logger.LogDebug("Waiting for booking state");
        WaitHelper.WaitForElement(Driver, ReserveNowButton, TimeSpan.FromSeconds(10));
    }

    public void WaitForGuestFormState()
    {
        Logger.LogDebug("Waiting for guest form state");
        WaitHelper.WaitForCondition(Driver, d => GuestForm.IsGuestFormVisible(), TimeSpan.FromSeconds(2));
    }

    public void WaitForConfirmationState()
    {
        Logger.LogDebug("Waiting for confirmation state");
        WaitHelper.WaitForElement(Driver, BookingConfirmedTitle, TimeSpan.FromSeconds(15));
    }

    public bool IsOnReservationPage()
    {
        var currentUrl = Driver.Url;
        Logger.LogDebug("Current URL: {Url}", currentUrl);
        return currentUrl.Contains("/reservation/");
    }

    public string GetRoomIdFromUrl()
    {
        var url = Driver.Url;
        var match = System.Text.RegularExpressions.Regex.Match(url, @"/reservation/(\d+)");
        return match.Success ? match.Groups[1].Value : string.Empty;
    }

    public string GetCheckInDateFromUrl()
    {
        var url = Driver.Url;
        Logger.LogDebug("Extracting check-in date from URL: {Url}", url);
        
        var uri = new Uri(url);
        var queryParams = QueryHelpers.ParseQuery(uri.Query);
        var checkInParam = queryParams.TryGetValue("checkin", out var checkinValues) ? checkinValues.FirstOrDefault() : null;
        
        Logger.LogDebug("Check-in parameter from URL: {CheckIn}", checkInParam);
        return checkInParam ?? string.Empty;
    }

    public string GetCheckOutDateFromUrl()
    {
        var url = Driver.Url;
        Logger.LogDebug("Extracting check-out date from URL: {Url}", url);
        
        var uri = new Uri(url);
        var queryParams = QueryHelpers.ParseQuery(uri.Query);
        var checkOutParam = queryParams.TryGetValue("checkout", out var checkoutValues) ? checkoutValues.FirstOrDefault() : null;
        
        Logger.LogDebug("Check-out parameter from URL: {CheckOut}", checkOutParam);
        return checkOutParam ?? string.Empty;
    }

    public Dictionary<string, string> GetAllUrlParameters()
    {
        var url = Driver.Url;
        Logger.LogDebug("Extracting all URL parameters from: {Url}", url);
        
        var parameters = new Dictionary<string, string>();
        
        try
        {
            var uri = new Uri(url);
            var queryParams = QueryHelpers.ParseQuery(uri.Query);
            
            foreach (var kvp in queryParams)
            {
                if (!string.IsNullOrEmpty(kvp.Key))
                {
                    parameters[kvp.Key] = kvp.Value.FirstOrDefault() ?? string.Empty;
                }
            }
            
            Logger.LogDebug("Found {Count} URL parameters", parameters.Count);
            return parameters;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error parsing URL parameters");
            return parameters;
        }
    }

    public (string CheckIn, string CheckOut) GetDisplayedDatesFromPage()
    {
        Logger.LogDebug("Getting displayed dates from reservation page");
        
        try
        {
            var nightsText = GetNightsCount();
            Logger.LogDebug("Nights text: {NightsText}", nightsText);
            
            var possibleDateSelectors = new[]
            {
                By.CssSelector(".booking-dates"),
                By.CssSelector(".check-in-date"),
                By.CssSelector(".check-out-date"),
                By.XPath("//text()[contains(., 'Check-in')]"),
                By.XPath("//text()[contains(., 'Check-out')]"),
                By.CssSelector(".date-range"),
                By.CssSelector(".booking-period")
            };
            
            foreach (var selector in possibleDateSelectors)
            {
                try
                {
                    var elements = Driver.FindElements(selector);
                    if (elements.Count > 0)
                    {
                        var dateText = elements[0].Text;
                        Logger.LogDebug("Found date text with selector {Selector}: {DateText}", selector, dateText);
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogDebug("Selector {Selector} not found: {Message}", selector, ex.Message);
                }
            }
            
            return (string.Empty, string.Empty);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error getting displayed dates from page");
            return (string.Empty, string.Empty);
        }
    }

    public IWebElement GetBookingCardElement()
    {
        return WaitHelper.WaitForElement(Driver, BookingCard);
    }

    public IWebElement GetGuestFormElement()
    {
        return GuestForm.GetGuestFormElement();
    }

    public static bool TryParseDateFromUrl(string urlDate, out DateTime parsedDate)
    {
        parsedDate = default;
        
        if (string.IsNullOrEmpty(urlDate))
            return false;
        
        var formats = new[]
        {
            "yyyy-MM-dd",
            "dd/MM/yyyy",
            "MM/dd/yyyy",
            "dd-MM-yyyy",
            "yyyy/MM/dd",
            "yyyyMMdd"
        };
        
        foreach (var format in formats)
        {
            if (DateTime.TryParseExact(urlDate, format, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out parsedDate))
            {
                return true;
            }
        }
        
        return DateTime.TryParse(urlDate, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out parsedDate);
    }

    public bool NavigateToGuestForm()
    {
        Logger.LogDebug("Navigating to guest form");
        
        try
        {
            if (!IsOnReservationPage())
            {
                Logger.LogWarning("Not on reservation page, cannot navigate to guest form");
                return false;
            }
            
            ClickReserveNow();
            WaitForGuestFormState();
            
            Logger.LogDebug("Successfully navigated to guest form");
            return true;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to navigate to guest form");
            return false;
        }
    }

    public bool NavigateToGuestFormSafe()
    {
        Logger.LogDebug("Safely navigating to guest form");
        
        try
        {
            if (IsBookingConfirmedFast())
            {
                Logger.LogWarning("Currently on booking confirmation page - cannot navigate to guest form");
                return false;
            }
            
            if (IsGuestFormVisible())
            {
                Logger.LogDebug("Guest form is already visible");
                return true;
            }
            
            if (IsOnReservationPage())
            {
                Logger.LogDebug("Already on reservation page, clicking Reserve Now");
                ClickReserveNow();
                WaitForGuestFormState();
                return true;
            }
            
            Logger.LogWarning("Not on a reservation page, cannot navigate to guest form");
            return false;
        }
        catch (Exception ex)
        {
            Logger.LogWarning(ex, "Safe navigation to guest form failed");
            return false;
        }
    }

    public ValidationOrConfirmationResult WaitForValidationOrConfirmation(string testDescription)
    {
        Logger.LogDebug("Waiting for validation error or booking confirmation for: {TestDescription}", testDescription);
        
        var result = new ValidationOrConfirmationResult
        {
            TestDescription = testDescription,
            ValidationErrors = new List<string>()
        };
        
        try
        {
            var waitResult = WaitHelper.WaitForCondition(Driver, _ => 
            {
                try
                {
                    if (AreValidationErrorsVisible())
                    {
                        result.HasValidationError = true;
                        result.ValidationErrors = GetValidationErrors();
                        Logger.LogDebug("Validation errors detected for {TestDescription}: {Errors}", 
                            testDescription, string.Join(", ", result.ValidationErrors));
                        return true;
                    }
                    
                    if (!IsGuestFormVisibleFast())
                    {
                        Logger.LogDebug("Guest form disappeared for {TestDescription} - checking what happened", testDescription);
                        
                        result.HasBookingConfirmation = IsBookingConfirmedFast();
                        if (result.HasBookingConfirmation)
                        {
                            Logger.LogDebug("Guest form disappeared and booking confirmed for {TestDescription}", testDescription);
                        }
                        else
                        {
                            Logger.LogDebug("Guest form disappeared but no confirmation detected for {TestDescription}", testDescription);
                            
                            WaitHelper.WaitForCondition(Driver, _ => IsBookingConfirmedFast(), TimeSpan.FromSeconds(3));
                            result.HasBookingConfirmation = IsBookingConfirmedFast();
                        }
                        return true;
                    }
                    
                    result.HasBookingConfirmation = IsBookingConfirmedFast();
                    if (result.HasBookingConfirmation)
                    {
                        Logger.LogDebug("Booking confirmation detected while form still visible for {TestDescription}", testDescription);
                        return true;
                    }
                    
                    return false;
                }
                catch (StaleElementReferenceException)
                {
                    Logger.LogDebug("Stale element detected for {TestDescription} - page likely changed due to booking", testDescription);
                    try
                    {
                        result.HasBookingConfirmation = IsBookingConfirmedFast();
                        return true;
                    }
                    catch (Exception)
                    {
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogDebug(ex, "Exception while checking validation/confirmation state for {TestDescription}", testDescription);
                    return false;
                }
            }, TimeSpan.FromSeconds(10));
            
            if (!waitResult)
            {
                Logger.LogWarning("Neither validation error nor booking confirmation appeared within timeout for: {TestDescription}", testDescription);
                
                try
                {
                    result.HasValidationError = AreValidationErrorsVisible();
                    if (result.HasValidationError)
                    {
                        result.ValidationErrors = GetValidationErrors();
                    }
                    result.HasBookingConfirmation = IsBookingConfirmedFast();
                }
                catch (Exception ex)
                {
                    Logger.LogDebug(ex, "Exception during final state check for {TestDescription}", testDescription);
                }
            }
        }
        catch (Exception ex)
        {
            Logger.LogWarning(ex, "Exception while waiting for validation or confirmation for: {TestDescription}", testDescription);
            
            try
            {
                result.HasValidationError = AreValidationErrorsVisible();
                if (result.HasValidationError)
                {
                    result.ValidationErrors = GetValidationErrors();
                }
                result.HasBookingConfirmation = IsBookingConfirmedFast();
            }
            catch (Exception finalEx)
            {
                Logger.LogDebug(finalEx, "Final exception check failed for {TestDescription}", testDescription);
            }
        }
        
        Logger.LogDebug("Validation/Confirmation result for {TestDescription}: HasError={HasError}, HasConfirmation={HasConfirmation}, ErrorCount={ErrorCount}",
            testDescription, result.HasValidationError, result.HasBookingConfirmation, result.ValidationErrors.Count);
        
        return result;
    }

    public bool CompleteBookingWorkflow(string firstName, string lastName, string email, string phone)
    {
        Logger.LogDebug("Completing booking workflow for guest: {FirstName} {LastName}", firstName, lastName);
        
        try
        {
            if (!IsOnReservationPage())
            {
                Logger.LogWarning("Not on reservation page, cannot complete booking workflow");
                return false;
            }
            
            if (!NavigateToGuestForm())
            {
                Logger.LogError("Failed to navigate to guest form");
                return false;
            }
            
            FillGuestInformation(firstName, lastName, email, phone);
            SubmitGuestForm();
            
            WaitForConfirmationState();
            
            var isConfirmed = IsBookingConfirmed();
            Logger.LogDebug("Booking workflow completed. Confirmed: {IsConfirmed}", isConfirmed);
            
            return isConfirmed;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to complete booking workflow");
            return false;
        }
    }

    public PricingValidationResult ValidatePricingForInvalidDates()
    {
        Logger.LogDebug("Validating pricing for invalid date ranges");
        
        var result = new PricingValidationResult();
        
        try
        {
            if (!IsOnReservationPage())
            {
                result.ValidationErrors.Add("Not on reservation page - cannot validate pricing");
                result.IsValid = false;
                return result;
            }
            
            var totalPrice = GetTotalPrice();
            var nightsCount = GetNightsCount();
            var basePrice = GetBasePriceFromSummary();
            var cleaningFee = GetCleaningFee();
            var serviceFee = GetServiceFee();
            
            result.TotalPrice = totalPrice;
            result.NightsCount = nightsCount;
            result.BasePrice = basePrice;
            result.CleaningFee = cleaningFee;
            result.ServiceFee = serviceFee;
            
            Logger.LogDebug("Pricing details - Total: {Total}, Nights: {Nights}, Base: {Base}, Cleaning: {Cleaning}, Service: {Service}", 
                totalPrice, nightsCount, basePrice, cleaningFee, serviceFee);
            
            var hasNegativeIndicators = false;
            
            if (nightsCount.Contains("-"))
            {
                result.ValidationErrors.Add($"Negative nights count detected: {nightsCount}");
                result.HasNegativeNights = true;
                hasNegativeIndicators = true;
            }
            
            if (totalPrice.Contains("-"))
            {
                result.ValidationErrors.Add($"Negative total price detected: {totalPrice}");
                result.HasNegativeTotal = true;
                hasNegativeIndicators = true;
            }
            
            if (basePrice.Contains("-"))
            {
                result.ValidationErrors.Add($"Negative base price detected: {basePrice}");
                result.HasNegativeBase = true;
                hasNegativeIndicators = true;
            }
            
            if (nightsCount.Contains("x -") || nightsCount.StartsWith('-'))
            {
                result.ValidationErrors.Add($"Invalid nights calculation pattern detected: {nightsCount}");
                result.HasInvalidNightsPattern = true;
                hasNegativeIndicators = true;
            }
            
            if (hasNegativeIndicators)
            {
                try
                {
                    ScreenshotHelper.CaptureScreenshot(Driver, "InvalidDateRange_NegativePricing", Logger);
                }
                catch (Exception screenshotEx)
                {
                    Logger.LogWarning(screenshotEx, "Could not capture screenshot for negative pricing");
                }
            }
            
            result.IsValid = !hasNegativeIndicators;
            
            if (result.IsValid)
            {
                Logger.LogDebug("Pricing validation passed - no negative indicators found");
            }
            else
            {
                Logger.LogWarning("Pricing validation failed - negative indicators found: {Errors}", 
                    string.Join(", ", result.ValidationErrors));
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error validating pricing for invalid dates");
            result.ValidationErrors.Add($"Error during pricing validation: {ex.Message}");
            result.IsValid = false;
        }
        
        return result;
    }
}

public class ValidationOrConfirmationResult
{
    public bool HasValidationError { get; set; }
    public bool HasBookingConfirmation { get; set; }
    public List<string> ValidationErrors { get; set; } = new List<string>();
    public string TestDescription { get; set; } = "";
}

public class PricingValidationResult
{
    public bool IsValid { get; set; }
    public string TotalPrice { get; set; } = "";
    public string NightsCount { get; set; } = "";
    public string BasePrice { get; set; } = "";
    public string CleaningFee { get; set; } = "";
    public string ServiceFee { get; set; } = "";
    public bool HasNegativeNights { get; set; }
    public bool HasNegativeTotal { get; set; }
    public bool HasNegativeBase { get; set; }
    public bool HasInvalidNightsPattern { get; set; }
    public bool HasCalculationError { get; set; }
    public List<string> ValidationErrors { get; set; } = new List<string>();
} 