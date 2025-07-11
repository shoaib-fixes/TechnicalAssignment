using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using TechnicalAssignment.Utilities;

namespace TechnicalAssignment.Pages;

public class ReservationPageGuestFormComponent
{
    protected readonly IWebDriver Driver;
    protected readonly ILogger Logger;
    
    private static readonly By FirstNameInput = By.CssSelector(".room-firstname");
    private static readonly By LastNameInput = By.CssSelector(".room-lastname");
    private static readonly By EmailInput = By.CssSelector(".room-email");
    private static readonly By PhoneInput = By.CssSelector(".room-phone");
    private static readonly By GuestFormReserveButton = By.XPath("//div[contains(@class, 'room-booking-form')]/..//button[text()='Reserve Now']");
    private static readonly By CancelButton = By.XPath("//button[text()='Cancel']");
    private static readonly By ValidationErrors = By.CssSelector(".alert.alert-danger");
    private static readonly By ValidationErrorList = By.CssSelector(".alert.alert-danger ul li");

    public ReservationPageGuestFormComponent(IWebDriver driver, ILogger logger)
    {
        Driver = driver;
        Logger = logger;
    }

    public bool IsGuestFormVisible(TimeSpan? timeout = null)
    {
        Logger.LogDebug("Checking if guest form is visible");
        return ElementHelper.IsElementVisible(Driver, FirstNameInput, timeout ?? TimeSpan.FromSeconds(2));
    }

    public bool IsGuestFormVisibleFast()
    {
        try
        {
            return Driver.FindElements(FirstNameInput).Count > 0;
        }
        catch
        {
            return false;
        }
    }

    public void FillFirstName(string firstName)
    {
        Logger.LogDebug("Filling first name: {FirstName}", firstName);
        ElementHelper.SafeSendKeys(Driver, FirstNameInput, firstName);
    }

    public void FillLastName(string lastName)
    {
        Logger.LogDebug("Filling last name: {LastName}", lastName);
        ElementHelper.SafeSendKeys(Driver, LastNameInput, lastName);
    }

    public void FillEmail(string email)
    {
        Logger.LogDebug("Filling email: {Email}", email);
        ElementHelper.SafeSendKeys(Driver, EmailInput, email);
    }

    public void FillPhone(string phone)
    {
        Logger.LogDebug("Filling phone: {Phone}", phone);
        ElementHelper.SafeSendKeys(Driver, PhoneInput, phone);
    }

    public void FillGuestInformation(string firstName, string lastName, string email, string phone)
    {
        Logger.LogDebug("Filling guest information for {FirstName} {LastName}", firstName, lastName);
        FillFirstName(firstName);
        FillLastName(lastName);
        FillEmail(email);
        FillPhone(phone);
    }

    public string GetFirstName()
    {
        return Driver.FindElement(FirstNameInput).GetAttribute("value") ?? string.Empty;
    }

    public string GetLastName()
    {
        return Driver.FindElement(LastNameInput).GetAttribute("value") ?? string.Empty;
    }

    public string GetEmail()
    {
        return Driver.FindElement(EmailInput).GetAttribute("value") ?? string.Empty;
    }

    public string GetPhone()
    {
        return Driver.FindElement(PhoneInput).GetAttribute("value") ?? string.Empty;
    }

    public void ClearFirstName()
    {
        Logger.LogDebug("Clearing first name field");
        Driver.FindElement(FirstNameInput).Clear();
    }

    public void ClearLastName()
    {
        Logger.LogDebug("Clearing last name field");
        Driver.FindElement(LastNameInput).Clear();
    }

    public void ClearEmail()
    {
        Logger.LogDebug("Clearing email field");
        Driver.FindElement(EmailInput).Clear();
    }

    public void ClearPhone()
    {
        Logger.LogDebug("Clearing phone field");
        Driver.FindElement(PhoneInput).Clear();
    }

    public void ClearAllFields()
    {
        Logger.LogDebug("Clearing all guest form fields");
        ClearFirstName();
        ClearLastName();
        ClearEmail();
        ClearPhone();
    }

    public void SubmitForm()
    {
        Logger.LogDebug("Submitting guest form");
        try
        {
            var button = WaitHelper.WaitForElement(Driver, GuestFormReserveButton, TimeSpan.FromSeconds(5));
            ((IJavaScriptExecutor)Driver).ExecuteScript("arguments[0].click();", button);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to submit guest form via JavaScript click");
            // Fallback to original method if JS fails
            ElementHelper.SafeClick(Driver, GuestFormReserveButton);
        }
    }

    public void ClickCancel()
    {
        Logger.LogDebug("Clicking Cancel button");
        ElementHelper.SafeClick(Driver, CancelButton);
    }

    public bool AreValidationErrorsVisible(TimeSpan? timeout = null)
    {
        Logger.LogDebug("Checking if validation errors are visible");
        return ElementHelper.IsElementVisible(Driver, ValidationErrors, timeout ?? TimeSpan.FromSeconds(5));
    }

    public List<string> GetValidationErrors()
    {
        Logger.LogDebug("Getting validation errors");
        var errorElements = Driver.FindElements(ValidationErrorList);
        var errors = errorElements.Select(e => e.Text).ToList();
        Logger.LogDebug("Found {Count} validation errors", errors.Count);
        return errors;
    }

    public bool HasValidationError(string errorText)
    {
        var errors = GetValidationErrors();
        return errors.Any(error => error.Contains(errorText, StringComparison.OrdinalIgnoreCase));
    }

    public bool IsReserveButtonEnabled()
    {
        try
        {
            var button = Driver.FindElement(GuestFormReserveButton);
            return button.Enabled;
        }
        catch (NoSuchElementException)
        {
            return false;
        }
    }

    public bool IsCancelButtonEnabled()
    {
        try
        {
            var button = Driver.FindElement(CancelButton);
            return button.Enabled;
        }
        catch (NoSuchElementException)
        {
            return false;
        }
    }

    public IWebElement GetGuestFormElement()
    {
        Logger.LogDebug("Getting guest form element");
        return WaitHelper.WaitForElement(Driver, FirstNameInput).FindElement(By.XPath("ancestor::form | ancestor::div[contains(@class, 'form')]"));
    }

    public IWebElement GetFirstNameField()
    {
        return WaitHelper.WaitForElement(Driver, FirstNameInput);
    }

    public IWebElement GetLastNameField()
    {
        return WaitHelper.WaitForElement(Driver, LastNameInput);
    }

    public IWebElement GetEmailField()
    {
        return WaitHelper.WaitForElement(Driver, EmailInput);
    }

    public IWebElement GetPhoneField()
    {
        return WaitHelper.WaitForElement(Driver, PhoneInput);
    }

    public IWebElement GetReserveButton()
    {
        return WaitHelper.WaitForElement(Driver, GuestFormReserveButton);
    }

    public IWebElement GetCancelButton()
    {
        return WaitHelper.WaitForElement(Driver, CancelButton);
    }
} 