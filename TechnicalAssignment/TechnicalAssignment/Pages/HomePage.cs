using System;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using TechnicalAssignment.Utilities;
using TechnicalAssignment.Models;

namespace TechnicalAssignment.Pages;

public class HomePage : BasePage
{
    private static readonly By PageLoadIndicator = By.CssSelector("body");
    private static readonly By PageTitle = By.TagName("title");
    
    public HomePageMainNavigationComponent Navigation { get; }
    public HomePageNavigationComponent SocialMedia { get; }
    public HomePageQuickLinksComponent QuickLinks { get; }
    public HomePageContactComponent Contact { get; }
    public HomePageBookingComponent Booking { get; }

    public HomePage(IWebDriver driver) : base(driver)
    {
        Navigation = new HomePageMainNavigationComponent(driver, Logger);
        SocialMedia = new HomePageNavigationComponent(driver, Logger);
        QuickLinks = new HomePageQuickLinksComponent(driver, Logger);
        Contact = new HomePageContactComponent(driver, Logger);
        Booking = new HomePageBookingComponent(driver, Logger);
    }

    public override bool IsPageLoaded(TimeSpan? timeout = null)
    {
        try
        {
            return ElementHelper.IsElementVisible(Driver, PageLoadIndicator, timeout ?? TimeSpan.FromSeconds(10));
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error checking if HomePage is loaded");
            return false;
        }
    }

    public override void WaitForPageToLoad(TimeSpan? timeout = null)
    {
        Logger.LogDebug("Waiting for HomePage to load");
        WaitHelper.WaitForElement(Driver, PageLoadIndicator, timeout ?? TimeSpan.FromSeconds(30));
        Logger.LogDebug("HomePage loaded successfully");
    }

    public void ScrollToRoomsSection()
    {
        Logger.LogDebug("Scrolling to rooms section");
        Navigation.ClickRoomsLink();
    }

    public bool IsOnHomePage()
    {
        return Navigation.IsOnHomePage();
    }

    public bool NavigateToReservationPage()
    {
        return Booking.NavigateToReservationPage();
    }

    public bool NavigateToReservationPageForRoomType(string roomType)
    {
        return Booking.NavigateToReservationPageForRoomType(roomType);
    }

    public bool EnsureOnHomePageForBooking(string baseUrl)
    {
        return Booking.EnsureOnHomePageForBooking(baseUrl);
    }

    public void ScrollToBookingSection()
    {
        Booking.ScrollToBookingSection();
    }

    public void ClickCheckAvailability()
    {
        Booking.ClickCheckAvailability();
    }

    public void CheckAvailability(string checkInDate, string checkOutDate)
    {
        Booking.CheckAvailability(checkInDate, checkOutDate);
    }

    public bool WaitForRoomsToUpdate(TimeSpan? timeout = null)
    {
        return Booking.WaitForRoomsToUpdate(timeout);
    }

    public void ClickFirstAvailableRoom()
    {
        Booking.ClickFirstAvailableRoom();
    }

    public void ClickRoomByType(string roomType)
    {
        Booking.ClickRoomByType(roomType);
    }

    public int GetAvailableRoomsCount()
    {
        return Booking.GetAvailableRoomsCount();
    }

    public System.Collections.Generic.List<string> GetAvailableRoomTypesWithRetry(string context)
    {
        return Booking.GetAvailableRoomTypesWithRetry(context);
    }

    public IWebElement? FindRoomCard(string roomTitle, int roomPrice, System.Collections.Generic.IEnumerable<string> roomFeatures)
    {
        return Booking.FindRoomCard(roomTitle, roomPrice, roomFeatures);
    }

    public bool ValidateBookingFormElements()
    {
        return Booking.ValidateBookingFormElements();
    }

    public string GetBookingFormTitle()
    {
        return Booking.GetBookingFormTitle();
    }

    public string GetCheckInDate()
    {
        return Booking.GetCheckInDate();
    }

    public string GetCheckOutDate()
    {
        return Booking.GetCheckOutDate();
    }

    public void SetCheckInDate(string date)
    {
        Booking.SetCheckInDate(date);
    }

    public void SetCheckOutDate(string date)
    {
        Booking.SetCheckOutDate(date);
    }

    public DateValidationResult ValidatePastDatesHandling()
    {
        return Booking.ValidatePastDatesHandling();
    }

    public DateValidationResult ValidateInvalidDateOrderHandling()
    {
        return Booking.ValidateInvalidDateOrderHandling();
    }

    public DateValidationResult ValidateSameDayBookingHandling()
    {
        return Booking.ValidateSameDayBookingHandling();
    }

    public bool TestBookingFormResponsiveDisplay(int width, int height, string deviceName)
    {
        return Booking.TestBookingFormResponsiveDisplay(width, height, deviceName);
    }

    public IWebElement GetBookingFormElement()
    {
        return Booking.GetBookingFormElement();
    }

    public static ViewportSize[] GetAllViewportSizes()
    {
        return ViewportTestData.GetAllViewportSizes();
    }
} 