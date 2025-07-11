using System;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using TechnicalAssignment.Utilities;

namespace TechnicalAssignment.Pages;

public class ReservationPageCalendarComponent
{
    protected readonly IWebDriver Driver;
    protected readonly ILogger Logger;
    
    private static readonly By CalendarContainer = By.CssSelector(".rbc-calendar");
    private static readonly By CalendarNextButton = By.XPath("//div[@class='rbc-toolbar']//button[text()='Next']");
    private static readonly By CalendarBackButton = By.XPath("//div[@class='rbc-toolbar']//button[text()='Back']");
    private static readonly By CalendarTodayButton = By.XPath("//div[@class='rbc-toolbar']//button[text()='Today']");
    private static readonly By CalendarToolbarLabel = By.CssSelector(".rbc-toolbar-label");

    public ReservationPageCalendarComponent(IWebDriver driver, ILogger logger)
    {
        Driver = driver;
        Logger = logger;
    }

    public bool IsCalendarVisible(TimeSpan? timeout = null)
    {
        Logger.LogDebug("Checking if calendar is visible");
        return ElementHelper.IsElementVisible(Driver, CalendarContainer, timeout ?? TimeSpan.FromSeconds(5));
    }

    public void ClickDate(int day)
    {
        Logger.LogDebug("Clicking calendar date: {Day}", day);
        var dayLocator = By.XPath($"//div[@class='rbc-date-cell']//button[text()='{day}']");
        ElementHelper.SafeClick(Driver, dayLocator);
    }

    public void ClickNext()
    {
        Logger.LogDebug("Clicking calendar next button");
        ElementHelper.SafeClick(Driver, CalendarNextButton);
    }

    public void ClickBack()
    {
        Logger.LogDebug("Clicking calendar back button");
        ElementHelper.SafeClick(Driver, CalendarBackButton);
    }

    public void ClickToday()
    {
        Logger.LogDebug("Clicking calendar today button");
        ElementHelper.SafeClick(Driver, CalendarTodayButton);
    }

    public string GetCurrentMonth()
    {
        Logger.LogDebug("Getting current calendar month");
        return ElementHelper.GetElementText(Driver, CalendarToolbarLabel);
    }

    public bool IsDateAvailable(int day)
    {
        Logger.LogDebug("Checking if date {Day} is available", day);
        try
        {
            var dayLocator = By.XPath($"//div[@class='rbc-date-cell']//button[text()='{day}' and not(@disabled)]");
            return ElementHelper.IsElementVisible(Driver, dayLocator, TimeSpan.FromSeconds(2));
        }
        catch (Exception ex)
        {
            Logger.LogDebug("Date {Day} availability check failed: {Message}", day, ex.Message);
            return false;
        }
    }

    public bool IsDateBooked(int day)
    {
        Logger.LogDebug("Checking if date {Day} is booked", day);
        try
        {
            var bookedDayLocator = By.XPath($"//div[@class='rbc-date-cell']//button[text()='{day}' and contains(@class, 'booked')]");
            return ElementHelper.IsElementVisible(Driver, bookedDayLocator, TimeSpan.FromSeconds(2));
        }
        catch (Exception ex)
        {
            Logger.LogDebug("Date {Day} booking check failed: {Message}", day, ex.Message);
            return false;
        }
    }

    public void NavigateToMonth(int monthOffset)
    {
        Logger.LogDebug("Navigating to month with offset: {Offset}", monthOffset);
        
        if (monthOffset > 0)
        {
            for (int i = 0; i < monthOffset; i++)
            {
                ClickNext();
                System.Threading.Thread.Sleep(500); // Small delay to allow calendar to update
            }
        }
        else if (monthOffset < 0)
        {
            for (int i = 0; i < Math.Abs(monthOffset); i++)
            {
                ClickBack();
                System.Threading.Thread.Sleep(500); // Small delay to allow calendar to update
            }
        }
    }

    public void NavigateToCurrentMonth()
    {
        Logger.LogDebug("Navigating to current month");
        ClickToday();
    }

    public IWebElement GetCalendarElement()
    {
        Logger.LogDebug("Getting calendar element");
        return WaitHelper.WaitForElement(Driver, CalendarContainer);
    }

    public IWebElement GetNextButton()
    {
        return WaitHelper.WaitForElement(Driver, CalendarNextButton);
    }

    public IWebElement GetBackButton()
    {
        return WaitHelper.WaitForElement(Driver, CalendarBackButton);
    }

    public IWebElement GetTodayButton()
    {
        return WaitHelper.WaitForElement(Driver, CalendarTodayButton);
    }

    public IWebElement GetToolbarLabel()
    {
        return WaitHelper.WaitForElement(Driver, CalendarToolbarLabel);
    }
} 