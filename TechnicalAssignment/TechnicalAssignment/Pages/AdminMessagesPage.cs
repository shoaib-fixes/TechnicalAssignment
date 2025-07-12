using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using TechnicalAssignment.Utilities;

namespace TechnicalAssignment.Pages;

public class AdminMessagesPage : BasePage
{
    private static readonly By MessagesContainer = By.CssSelector(".messages");
    private static readonly By MessageRows = By.CssSelector(".row.detail");
    private static readonly By MessageModal = By.CssSelector(".ReactModal__Content.message-modal");
    
    // Locators updated for robustness, preferring relationship-based selectors over positional ones.
    private static readonly By ModalFromName = By.XPath("//p[span[text()='From: ']]");
    private static readonly By ModalPhone = By.XPath("//p[span[text()='Phone: ']]");
    private static readonly By ModalEmail = By.XPath("//p[span[text()='Email: ']]");
    
    // Using following-sibling to find elements is more robust than relying on DOM order (e.g., nth-child).
    private static readonly By ModalSubject = By.XPath("//p[span[text()='Email: ']]/ancestor::div[contains(@class, 'form-row')]/following-sibling::div[contains(@class, 'form-row')][1]//span");
    private static readonly By ModalMessage = By.XPath("//p[span[text()='Email: ']]/ancestor::div[contains(@class, 'form-row')]/following-sibling::div[contains(@class, 'form-row')][2]//p");
    
    private static readonly By ModalCloseButton = By.XPath("//div[contains(@class, 'message-modal')]//button[text()='Close']");

    public AdminMessagesPage(IWebDriver driver, ILogger<AdminMessagesPage> logger) : base(driver, logger)
    {
    }

    public override bool IsPageLoaded(TimeSpan? timeout = null)
    {
        Logger.LogDebug("Checking if admin messages page is loaded");
        return ElementHelper.IsElementVisible(Driver, MessagesContainer, timeout);
    }

    public override void WaitForPageToLoad(TimeSpan? timeout = null)
    {
        Logger.LogDebug("Waiting for admin messages page to load");
        WaitHelper.WaitForElement(Driver, MessagesContainer, timeout);
    }

    public bool IsMessagePresent(string name, string subject)
    {
        Logger.LogDebug("Checking if message is present with name: {Name}, subject: {Subject}", name, subject);
        
        var messageRows = Driver.FindElements(MessageRows);
        
        foreach (var row in messageRows)
        {
            try
            {
                var nameElement = row.FindElement(By.CssSelector(".col-sm-2 p"));
                var subjectElement = row.FindElement(By.CssSelector(".col-sm-9 p"));
                
                var messageName = nameElement.Text.Trim();
                var messageSubject = subjectElement.Text.Trim();
                
                Logger.LogDebug("Found message - Name: {MessageName}, Subject: {MessageSubject}", messageName, messageSubject);
                
                if (messageName.Equals(name, StringComparison.OrdinalIgnoreCase) &&
                    messageSubject.Equals(subject, StringComparison.OrdinalIgnoreCase))
                {
                    Logger.LogDebug("Message found matching criteria");
                    return true;
                }
            }
            catch (Exception ex)
            {
                Logger.LogWarning(ex, "Error reading message row");
            }
        }
        
        Logger.LogDebug("Message not found with specified name and subject");
        return false;
    }

    public bool DeleteMessage(string name, string subject)
    {
        Logger.LogDebug("Attempting to delete message with name: {Name}, subject: {Subject}", name, subject);
        
        var messageRows = Driver.FindElements(MessageRows);
        
        for (int i = 0; i < messageRows.Count; i++)
        {
            try
            {
                var row = messageRows[i];
                var nameElement = row.FindElement(By.CssSelector(".col-sm-2 p"));
                var subjectElement = row.FindElement(By.CssSelector(".col-sm-9 p"));
                
                var messageName = nameElement.Text.Trim();
                var messageSubject = subjectElement.Text.Trim();
                
                if (messageName.Equals(name, StringComparison.OrdinalIgnoreCase) &&
                    messageSubject.Equals(subject, StringComparison.OrdinalIgnoreCase))
                {
                    var rowLocator = By.XPath($"//div[contains(@class,'row detail')][.//p[normalize-space()='{subject}']]");
                    var deleteButtonLocator = By.XPath($"//div[contains(@class,'row detail')][.//p[normalize-space()='{subject}']]//span[contains(@class,'fa-remove')]");
                    ScrollHelper.ScrollToElement(Driver, rowLocator, ScrollBehavior.Instant, ScrollAlignment.Center);
                    WaitHelper.WaitForElement(Driver, deleteButtonLocator, TimeSpan.FromSeconds(5));
                    ElementHelper.JavaScriptClick(Driver, deleteButtonLocator);
                    
                    WaitHelper.WaitForCondition(Driver, d => 
                    {
                        try 
                        {
                            _ = row.Text;
                            return false;
                        }
                        catch (StaleElementReferenceException)
                        {
                            return true;
                        }
                    }, TimeSpan.FromSeconds(5));
                    
                    Logger.LogDebug("Message deletion completed");
                    return true;
                }
            }
            catch (Exception ex)
            {
                Logger.LogWarning(ex, "Error processing message row {Index}", i);
            }
        }
        
        Logger.LogWarning("Message not found for deletion");
        return false;
    }

    public List<(string Name, string Subject)> GetAllMessages()
    {
        Logger.LogDebug("Getting all messages from admin panel");
        
        var messages = new List<(string Name, string Subject)>();
        
        var messageRows = Driver.FindElements(MessageRows);
        
        foreach (var row in messageRows)
        {
            try
            {
                var nameElement = row.FindElement(By.CssSelector(".col-sm-2 p"));
                var subjectElement = row.FindElement(By.CssSelector(".col-sm-9 p"));
                
                var name = nameElement.Text.Trim();
                var subject = subjectElement.Text.Trim();
                
                messages.Add((name, subject));
            }
            catch (Exception ex)
            {
                Logger.LogWarning(ex, "Error reading message row");
            }
        }
        
        Logger.LogDebug("Retrieved {MessageCount} messages", messages.Count);
        return messages;
    }

    public void ClickMessageSubject(string subject)
    {
        Logger.LogDebug("Clicking on message subject: {Subject}", subject);
        
        var messageRows = Driver.FindElements(MessageRows);
        
        foreach (var row in messageRows)
        {
            try
            {
                var subjectDiv = row.FindElement(By.CssSelector(".col-sm-9"));
                var subjectParagraph = subjectDiv.FindElement(By.TagName("p"));
                var messageSubject = subjectParagraph.Text.Trim();
                
                if (messageSubject.Equals(subject, StringComparison.OrdinalIgnoreCase))
                {
                    var rowLocator = By.XPath($"//div[contains(@class,'row detail')][.//p[normalize-space()='{subject}']]");
                    var subjectDivLocator = By.XPath($"//div[contains(@class,'row detail')][.//p[normalize-space()='{subject}']]//div[contains(@class,'col-sm-9')]");
                    ScrollHelper.ScrollToElement(Driver, rowLocator, ScrollBehavior.Instant, ScrollAlignment.Center);
                    ElementHelper.JavaScriptClick(Driver, subjectDivLocator);
                    WaitHelper.WaitForElement(Driver, MessageModal, TimeSpan.FromSeconds(5));
                    return;
                }
            }
            catch (Exception ex)
            {
                Logger.LogWarning(ex, "Error processing message row for subject click");
            }
        }
        
        throw new InvalidOperationException($"Message with subject '{subject}' not found");
    }

    public bool IsMessageModalVisible(TimeSpan? timeout = null)
    {
        Logger.LogDebug("Checking if message modal is visible");
        return ElementHelper.IsElementVisible(Driver, MessageModal, timeout ?? TimeSpan.FromSeconds(5));
    }

    public IWebElement GetMessageModalElement()
    {
        Logger.LogDebug("Getting message modal element");
        return WaitHelper.WaitForElement(Driver, MessageModal);
    }

    public string GetModalFromName()
    {
        Logger.LogDebug("Getting modal from name");
        var element = WaitHelper.WaitForElement(Driver, ModalFromName);
        var fullText = element.Text.Trim();
        return fullText.StartsWith("From: ") ? fullText.Substring("From: ".Length).Trim() : fullText;
    }

    public string GetModalEmail()
    {
        Logger.LogDebug("Getting modal email");
        var element = WaitHelper.WaitForElement(Driver, ModalEmail);
        var fullText = element.Text.Trim();
        return fullText.StartsWith("Email: ") ? fullText.Substring("Email: ".Length).Trim() : fullText;
    }

    public string GetModalPhone()
    {
        Logger.LogDebug("Getting modal phone");
        var element = WaitHelper.WaitForElement(Driver, ModalPhone);
        var fullText = element.Text.Trim();
        return fullText.StartsWith("Phone: ") ? fullText.Substring("Phone: ".Length).Trim() : fullText;
    }

    public string GetModalSubject()
    {
        Logger.LogDebug("Getting modal subject");
        var element = WaitHelper.WaitForElement(Driver, ModalSubject);
        return element.Text.Trim();
    }

    public string GetModalMessage()
    {
        Logger.LogDebug("Getting modal message");
        var element = WaitHelper.WaitForElement(Driver, ModalMessage);
        return element.Text.Trim();
    }

    public void CloseMessageModal()
    {
        Logger.LogDebug("Closing message modal");
        ElementHelper.SafeClick(Driver, ModalCloseButton);
    }

    public bool WaitForMessageModalToDisappear(TimeSpan? timeout = null)
    {
        Logger.LogDebug("Waiting for message modal to disappear");
        
        try
        {
            var modalElements = Driver.FindElements(MessageModal);
            if (modalElements.Count == 0 || !modalElements[0].Displayed)
            {
                Logger.LogDebug("Modal already disappeared");
                return true;
            }
            
            return WaitHelper.WaitForElementToDisappear(Driver, MessageModal, timeout ?? TimeSpan.FromSeconds(2));
        }
        catch (Exception ex)
        {
            Logger.LogWarning(ex, "Error waiting for modal to disappear, assuming it's gone");
            return true;
        }
    }

    public IWebElement GetMessagesContainerElement()
    {
        Logger.LogDebug("Getting messages container element");
        return WaitHelper.WaitForElement(Driver, MessagesContainer);
    }
} 