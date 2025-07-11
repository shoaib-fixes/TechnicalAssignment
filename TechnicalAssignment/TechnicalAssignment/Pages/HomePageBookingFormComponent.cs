using System;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using TechnicalAssignment.Utilities;

namespace TechnicalAssignment.Pages;

public class HomePageBookingFormComponent
{
    private readonly IWebDriver _driver;
    private readonly ILogger<HomePageBookingFormComponent> _logger;
    
    private static readonly By BookingFormSection = By.CssSelector("section#booking");
    private static readonly By CheckInDatePicker = By.CssSelector("#booking .col-md-6:nth-child(1) input.form-control");
    private static readonly By CheckOutDatePicker = By.CssSelector("#booking .col-md-6:nth-child(2) input.form-control");
    private static readonly By CheckAvailabilityButton = By.CssSelector("#booking button.btn-primary");
    private static readonly By BookingFormTitle = By.CssSelector("#booking h3.card-title");

    public HomePageBookingFormComponent(IWebDriver driver, ILogger<HomePageBookingFormComponent> logger)
    {
        _driver = driver;
        _logger = logger;
    }

    public void ScrollToBookingSection()
    {
        ScrollHelper.ScrollToElement(_driver, BookingFormSection);
    }

    public bool IsBookingFormSectionVisible(TimeSpan? timeout = null)
    {
        return ElementHelper.IsElementVisible(_driver, BookingFormSection, timeout);
    }

    public string GetBookingFormTitle()
    {
        return ElementHelper.GetElementText(_driver, BookingFormTitle);
    }

    public bool IsCheckInDatePickerVisible(TimeSpan? timeout = null)
    {
        return ElementHelper.IsElementVisible(_driver, CheckInDatePicker, timeout);
    }

    public bool IsCheckOutDatePickerVisible(TimeSpan? timeout = null)
    {
        return ElementHelper.IsElementVisible(_driver, CheckOutDatePicker, timeout);
    }

    public bool IsCheckAvailabilityButtonVisible(TimeSpan? timeout = null)
    {
        return ElementHelper.IsElementVisible(_driver, CheckAvailabilityButton, timeout);
    }

    public string GetCheckInDate()
    {
        try
        {
            var dateValue = ElementHelper.GetElementAttribute(_driver, CheckInDatePicker, "value");
            _logger.LogDebug("Check-in date value retrieved: '{Value}'", dateValue);
            return dateValue;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get check-in date value");
            
            try
            {
                var elements = _driver.FindElements(By.CssSelector("#booking input[type='text']"));
                if (elements.Count >= 2)
                {
                    var value = elements[0].GetAttribute("value") ?? string.Empty;
                    _logger.LogDebug("Check-in date from alternative selector: '{Value}'", value);
                    return value;
                }
            }
            catch (Exception altEx)
            {
                _logger.LogError(altEx, "Alternative check-in date retrieval also failed");
            }
            
            throw;
        }
    }

    public string GetCheckOutDate()
    {
        try
        {
            var dateValue = ElementHelper.GetElementAttribute(_driver, CheckOutDatePicker, "value");
            _logger.LogDebug("Check-out date value retrieved: '{Value}'", dateValue);
            return dateValue;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get check-out date value");
            
            try
            {
                var elements = _driver.FindElements(By.CssSelector("#booking input[type='text']"));
                if (elements.Count >= 2)
                {
                    var value = elements[1].GetAttribute("value") ?? string.Empty;
                    _logger.LogDebug("Check-out date from alternative selector: '{Value}'", value);
                    return value;
                }
            }
            catch (Exception altEx)
            {
                _logger.LogError(altEx, "Alternative check-out date retrieval also failed");
            }
            
            throw;
        }
    }

    public void SetCheckInDate(string date)
    {
        var checkInElement = WaitHelper.WaitForElement(_driver, CheckInDatePicker, TimeSpan.FromSeconds(10));
        
        checkInElement.Click();
        WaitHelper.WaitForCondition(_driver, _ => checkInElement.Equals(_driver.SwitchTo().ActiveElement()), TimeSpan.FromSeconds(2));
        
        checkInElement.SendKeys(Keys.Control + "a");
        WaitHelper.WaitForCondition(_driver, _ => 
        {
            try
            {
                var script = "return window.getSelection().toString().length > 0 || arguments[0].selectionStart !== arguments[0].selectionEnd;";
                var result = ((IJavaScriptExecutor)_driver).ExecuteScript(script, checkInElement);
                return result is bool boolResult && boolResult;
            }
            catch
            {
                return true;
            }
        }, TimeSpan.FromMilliseconds(500));
        checkInElement.SendKeys(date);
        WaitHelper.WaitForCondition(_driver, _ => !string.IsNullOrEmpty(checkInElement.GetAttribute("value")), TimeSpan.FromSeconds(2));
        
        checkInElement.SendKeys(Keys.Tab);
        WaitHelper.WaitForCondition(_driver, _ => !checkInElement.Equals(_driver.SwitchTo().ActiveElement()), TimeSpan.FromSeconds(2));
        
        var actualDate = checkInElement.GetAttribute("value");
        _logger.LogDebug("Check-in date set to: {ActualDate}", actualDate);
    }

    public void SetCheckOutDate(string date)
    {
        var checkOutElement = WaitHelper.WaitForElement(_driver, CheckOutDatePicker, TimeSpan.FromSeconds(10));
        
        checkOutElement.Click();
        WaitHelper.WaitForCondition(_driver, _ => checkOutElement.Equals(_driver.SwitchTo().ActiveElement()), TimeSpan.FromSeconds(2));
        
        checkOutElement.SendKeys(Keys.Control + "a");
        WaitHelper.WaitForCondition(_driver, _ => 
        {
            try
            {
                var script = "return window.getSelection().toString().length > 0 || arguments[0].selectionStart !== arguments[0].selectionEnd;";
                var result = ((IJavaScriptExecutor)_driver).ExecuteScript(script, checkOutElement);
                return result is bool boolResult && boolResult;
            }
            catch
            {
                return true;
            }
        }, TimeSpan.FromMilliseconds(500));
        checkOutElement.SendKeys(date);
        WaitHelper.WaitForCondition(_driver, _ => !string.IsNullOrEmpty(checkOutElement.GetAttribute("value")), TimeSpan.FromSeconds(2));
        
        checkOutElement.SendKeys(Keys.Tab);
        WaitHelper.WaitForCondition(_driver, _ => !checkOutElement.Equals(_driver.SwitchTo().ActiveElement()), TimeSpan.FromSeconds(2));
        
        var actualDate = checkOutElement.GetAttribute("value");
        _logger.LogDebug("Check-out date set to: {ActualDate}", actualDate);
    }

    public void ClickCheckAvailability()
    {
        ElementHelper.SafeClick(_driver, CheckAvailabilityButton);
    }

    public void CheckAvailability(string checkInDate, string checkOutDate)
    {
        _logger.LogDebug("Checking availability for dates: {CheckIn} to {CheckOut}", checkInDate, checkOutDate);
        SetCheckInDate(checkInDate);
        SetCheckOutDate(checkOutDate);
        ClickCheckAvailability();
    }

    public bool ValidateBookingFormElements()
    {
        _logger.LogDebug("Validating booking form elements");
        
        var isFormVisible = IsBookingFormSectionVisible(TimeSpan.FromSeconds(5));
        var isCheckInVisible = IsCheckInDatePickerVisible(TimeSpan.FromSeconds(5));
        var isCheckOutVisible = IsCheckOutDatePickerVisible(TimeSpan.FromSeconds(5));
        var isButtonVisible = IsCheckAvailabilityButtonVisible(TimeSpan.FromSeconds(5));
        
        _logger.LogDebug("Booking form validation - Form: {Form}, CheckIn: {CheckIn}, CheckOut: {CheckOut}, Button: {Button}",
            isFormVisible, isCheckInVisible, isCheckOutVisible, isButtonVisible);
        
        return isFormVisible && isCheckInVisible && isCheckOutVisible && isButtonVisible;
    }

    public IWebElement GetBookingFormElement()
    {
        return WaitHelper.WaitForElement(_driver, BookingFormSection);
    }

    public IWebElement GetCheckAvailabilityButtonElement()
    {
        return WaitHelper.WaitForElement(_driver, CheckAvailabilityButton);
    }

    public bool TestBookingFormResponsiveDisplay(int width, int height, string deviceName)
    {
        _logger.LogDebug("Testing booking form responsive display on {DeviceName} ({Width}x{Height})", deviceName, width, height);
        
        BrowserHelper.SetViewportSize(_driver, width, height);
        
        WaitHelper.WaitForCondition(_driver, _ => 
        {
            try
            {
                var elements = _driver.FindElements(BookingFormSection);
                return elements.Count > 0;
            }
            catch
            {
                return false;
            }
        }, TimeSpan.FromSeconds(5));
        
        ScrollToBookingSection();
        var isValid = ValidateBookingFormElements();
        
        _logger.LogDebug("Booking form validation result on {DeviceName}: {IsValid}", deviceName, isValid);
        return isValid;
    }
} 