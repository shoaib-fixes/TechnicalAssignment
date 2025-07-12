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
    public HomePageBookingFormComponent BookingForm { get; }
    public HomePageBookingRoomListComponent RoomList { get; }
    public HomePageBookingDateValidationComponent DateValidation { get; }

    public HomePage(IWebDriver driver, ILogger<HomePage> logger,
                   HomePageMainNavigationComponent navigation,
                   HomePageNavigationComponent socialMedia,
                   HomePageQuickLinksComponent quickLinks,
                   HomePageContactComponent contact,
                   HomePageBookingFormComponent bookingForm,
                   HomePageBookingRoomListComponent roomList,
                   HomePageBookingDateValidationComponent dateValidation) : base(driver, logger)
    {
        Navigation = navigation;
        SocialMedia = socialMedia;
        QuickLinks = quickLinks;
        Contact = contact;
        BookingForm = bookingForm;
        RoomList = roomList;
        DateValidation = dateValidation;
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

    public static ViewportSize[] GetAllViewportSizes()
    {
        return ViewportTestData.GetAllViewportSizes();
    }
} 