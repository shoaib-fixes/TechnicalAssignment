using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using TechnicalAssignment.Utilities;

namespace TechnicalAssignment.Pages;

public class HomePageContactComponent
{
    protected readonly IWebDriver Driver;
    protected readonly ILogger Logger;
    
    private static readonly By ContactSection = By.Id("contact");
    private static readonly By ContactForm = By.CssSelector("#contact form");
    private static readonly By NameField = By.Id("name");
    private static readonly By EmailField = By.Id("email");
    private static readonly By PhoneField = By.Id("phone");
    private static readonly By SubjectField = By.Id("subject");
    private static readonly By MessageField = By.Id("description");
    private static readonly By SubmitButton = By.CssSelector("#contact button[type='button']");
    private static readonly By ValidationErrors = By.CssSelector(".alert.alert-danger");
    private static readonly By SuccessMessage = By.CssSelector("#contact .card .card-body h3");

    public HomePageContactComponent(IWebDriver driver, ILogger logger)
    {
        Driver = driver;
        Logger = logger;
    }

    public void ScrollToContactForm()
    {
        Logger.LogDebug("Scrolling to contact form section");
        ScrollHelper.ScrollToElement(Driver, ContactSection, TimeSpan.FromSeconds(10));
    }

    public bool IsContactFormVisible(TimeSpan? timeout = null)
    {
        return ElementHelper.IsElementVisible(Driver, ContactForm, timeout);
    }

    public void FillContactForm(string name, string email, string phone, string subject, string message)
    {
        Logger.LogDebug("Filling contact form with provided data");
        
        ElementHelper.SafeSendKeys(Driver, NameField, name);
        ElementHelper.SafeSendKeys(Driver, EmailField, email);
        ElementHelper.SafeSendKeys(Driver, PhoneField, phone);
        ElementHelper.SafeSendKeys(Driver, SubjectField, subject);
        ElementHelper.SafeSendKeys(Driver, MessageField, message);
    }

    public void SubmitContactForm()
    {
        Logger.LogDebug("Submitting contact form");
        ElementHelper.SafeClick(Driver, SubmitButton);
    }

    public bool WaitForContactSubmissionSuccess(TimeSpan timeout)
    {
        Logger.LogDebug("Waiting for contact form submission success message");
        return WaitHelper.WaitForCondition(Driver, d => IsContactSubmissionSuccessful(TimeSpan.FromSeconds(1)), timeout);
    }

    public bool IsContactSubmissionSuccessful(TimeSpan? timeout = null)
    {
        try
        {
            if (ElementHelper.IsElementVisible(Driver, SuccessMessage, timeout))
            {
                var successElement = Driver.FindElement(SuccessMessage);
                var successText = successElement.Text.Trim();
                return successText.StartsWith("Thanks for getting in touch", StringComparison.OrdinalIgnoreCase);
            }
            return false;
        }
        catch
        {
            return false;
        }
    }

    public string GetContactSuccessMessage()
    {
        Logger.LogDebug("Getting contact form success message");
        try
        {
            var successElement = Driver.FindElement(SuccessMessage);
            return successElement.Text.Trim();
        }
        catch
        {
            return string.Empty;
        }
    }

    public bool AreContactValidationErrorsVisible(TimeSpan? timeout = null)
    {
        return ElementHelper.IsElementVisible(Driver, ValidationErrors, timeout);
    }

    public List<string> GetContactValidationErrors()
    {
        Logger.LogDebug("Getting contact form validation errors");
        
        var errorElements = Driver.FindElements(ValidationErrors);
        var errors = new List<string>();
        
        foreach (var errorElement in errorElements)
        {
            var errorText = errorElement.Text.Trim();
            if (!string.IsNullOrEmpty(errorText))
            {
                var individualErrors = errorText.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var error in individualErrors)
                {
                    var trimmedError = error.Trim();
                    if (!string.IsNullOrEmpty(trimmedError))
                    {
                        errors.Add(trimmedError);
                    }
                }
            }
        }
        
        Logger.LogDebug("Found {ErrorCount} validation errors", errors.Count);
        return errors;
    }

    public IWebElement GetContactFormElement()
    {
        Logger.LogDebug("Getting contact form element");
        return WaitHelper.WaitForElement(Driver, ContactForm);
    }

    public IWebElement GetContactSectionElement()
    {
        Logger.LogDebug("Getting contact section element");
        return WaitHelper.WaitForElement(Driver, ContactSection);
    }
    
    // Accessibility Test Helpers
    public IWebElement GetNameField() => Driver.FindElement(NameField);
    public IWebElement GetEmailField() => Driver.FindElement(EmailField);
    public IWebElement GetPhoneField() => Driver.FindElement(PhoneField);
    public IWebElement GetSubjectField() => Driver.FindElement(SubjectField);
    public IWebElement GetMessageField() => Driver.FindElement(MessageField);
    public IWebElement GetSubmitButton() => Driver.FindElement(SubmitButton);

    public IWebElement GetNameLabel() => Driver.FindElement(By.CssSelector("label[for='name']"));
    public IWebElement GetEmailLabel() => Driver.FindElement(By.CssSelector("label[for='email']"));
    public IWebElement GetPhoneLabel() => Driver.FindElement(By.CssSelector("label[for='phone']"));
    public IWebElement GetSubjectLabel() => Driver.FindElement(By.CssSelector("label[for='subject']"));
    public IWebElement GetMessageLabel() => Driver.FindElement(By.CssSelector("label[for='message']"));
    
    public IEnumerable<IWebElement> GetAllFormFields() => Driver.FindElements(By.CssSelector("#contact input, #contact textarea"));
} 