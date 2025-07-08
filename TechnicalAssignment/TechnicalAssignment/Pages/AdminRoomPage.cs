using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using TechnicalAssignment.Utilities;

namespace TechnicalAssignment.Pages;

public class AdminRoomPage : BasePage
{
    private static readonly By RoomNumberInput = By.Id("roomName");
    private static readonly By TypeSelect = By.Id("type");
    private static readonly By AccessibleSelect = By.Id("accessible");
    private static readonly By PriceInput = By.Id("roomPrice");
    private static readonly By ImageUrlInput = By.Id("image");
    private static readonly By UpdateButton = By.Id("update");
    private static readonly By CancelButton = By.Id("cancelEdit");
    private static readonly By ErrorAlert = By.CssSelector("div.alert-danger");
    private static readonly By RoomImage = By.CssSelector("div.room-details img");
    private static readonly By EditButton = By.XPath("//button[normalize-space()='Edit']");
    
    // NOTE: These selectors are based on visible text because no other stable locators (like data-testid) are available in the view mode HTML.
    // This is a potential point of fragility if the UI text changes.
    private static readonly By DisplayedType = By.XPath("//p[starts-with(text(), 'Type:')]/span");
    private static readonly By DisplayedAccessibility = By.XPath("//p[starts-with(text(), 'Accessible:')]/span");
    private static readonly By DisplayedPrice = By.XPath("//p[starts-with(text(), 'Room price:')]/span");
    private static readonly By DisplayedFeatures = By.XPath("//p[starts-with(text(), 'Features:')]/span");
    private static readonly By DisplayedRoomNumberHeader = By.CssSelector("div.room-details h2");

    public AdminRoomPage(IWebDriver driver) : base(driver)
    {
    }

    public override bool IsPageLoaded(TimeSpan? timeout = null)
    {
        return ElementHelper.IsElementVisible(Driver, EditButton, timeout) || 
               ElementHelper.IsElementVisible(Driver, RoomNumberInput, timeout);
    }

    public override void WaitForPageToLoad(TimeSpan? timeout = null)
    {
        WaitHelper.WaitForCondition(Driver, d => 
            IsElementVisible(d, EditButton) || IsElementVisible(d, RoomNumberInput), timeout);
    }

    public void WaitForEditMode(TimeSpan? timeout = null)
    {
        WaitHelper.WaitForElement(Driver, RoomNumberInput, timeout);
    }

    public void ClickEditButton(TimeSpan? timeout = null)
    {
        if (ElementHelper.IsElementPresent(Driver, EditButton, timeout))
        {
            ElementHelper.SafeClick(Driver, EditButton, timeout);
        }
    }

    public void ClickEditButtonAndWait(TimeSpan? timeout = null)
    {
        ClickEditButton(timeout);
        WaitForEditMode(timeout);
    }

    public string GetRoomNumber() => Driver.FindElement(RoomNumberInput).GetAttribute("value") ?? string.Empty;
    public string GetRoomType() => new SelectElement(Driver.FindElement(TypeSelect)).SelectedOption.Text ?? string.Empty;
    public string GetRoomAccessibility() => new SelectElement(Driver.FindElement(AccessibleSelect)).SelectedOption.GetAttribute("value") ?? string.Empty;
    public string GetRoomPrice() => Driver.FindElement(PriceInput).GetAttribute("value") ?? string.Empty;

    public List<string> GetRoomFeatures()
    {
        var features = new List<string>();
        var allFeatureCheckboxes = Driver.FindElements(By.CssSelector("input[type='checkbox'][name='featureCheck']"));
    
        foreach (var checkbox in allFeatureCheckboxes)
        {
            if (checkbox.Selected)
            {
                var featureValue = checkbox.GetAttribute("value");
                if (!string.IsNullOrEmpty(featureValue))
                {
                    features.Add(featureValue);
                }
            }
        }
        return features;
    }

    public void UpdateRoomDetails(string type, string accessible, string price, List<string> features)
    {
        EnterRoomDetails(type, accessible, price, features);
        ElementHelper.SafeClick(Driver, UpdateButton);
    }
    
    public void EnterRoomDetails(string type, string accessible, string price, List<string> features)
    {
        ElementHelper.SelectDropdownByText(Driver, TypeSelect, type);
        ElementHelper.SelectDropdownByValue(Driver, AccessibleSelect, accessible);
        ElementHelper.SafeSendKeys(Driver, PriceInput, price);

        var allFeatureCheckboxes = Driver.FindElements(By.CssSelector("input[type='checkbox'][name='featureCheck']"));
        foreach (var checkbox in allFeatureCheckboxes)
        {
            if (checkbox.Selected)
            {
                checkbox.Click();
            }
        }
        
        foreach (var feature in features)
        {
            var checkbox = Driver.FindElement(GetFeatureCheckboxLocator(feature));
            if (!checkbox.Selected)
            {
                checkbox.Click();
            }
        }
    }
    
    public void ClickCancel()
    {
        ElementHelper.SafeClick(Driver, CancelButton);
    }

    public void UpdateRoomNumber(string newNumber)
    {
        ElementHelper.SafeSendKeys(Driver, RoomNumberInput, newNumber);
        ElementHelper.SafeClick(Driver, UpdateButton);
    }

    public void UpdateImageUrl(string newUrl)
    {
        ElementHelper.SafeSendKeys(Driver, ImageUrlInput, newUrl);
        ElementHelper.SafeClick(Driver, UpdateButton);
    }

    public bool IsErrorAlertVisible(TimeSpan? timeout = null)
    {
        return ElementHelper.IsElementVisible(Driver, ErrorAlert, timeout);
    }
    
    public string GetErrorAlertText()
    {
        return WaitHelper.WaitForElement(Driver, ErrorAlert).Text;
    }

    private By GetFeatureCheckboxLocator(string feature)
    {
        return feature.ToLower() switch
        {
            "wifi" => By.Id("wifiCheckbox"),
            "tv" => By.Id("tvCheckbox"),
            "radio" => By.Id("radioCheckbox"),
            "refreshments" => By.Id("refreshCheckbox"),
            "safe" => By.Id("safeCheckbox"),
            "views" => By.Id("viewsCheckbox"),
            _ => throw new ArgumentException($"Unsupported feature: {feature}", nameof(feature)),
        };
    }

    public string GetImageUrl()
    {
        var image = WaitHelper.WaitForElement(Driver, RoomImage);
        var imageUrl = image.GetAttribute("src");
        if (string.IsNullOrEmpty(imageUrl))
        {
            throw new InvalidOperationException("The 'src' attribute is missing or empty for the room image.");
        }
        return imageUrl;
    }

    public void WaitForViewMode(TimeSpan? timeout = null)
    {
        WaitHelper.WaitForElement(Driver, EditButton, timeout);
    }

    public int GetDisplayedRoomNumber()
    {
        var element = WaitHelper.WaitForElement(Driver, DisplayedRoomNumberHeader, TimeSpan.FromSeconds(10));
        var text = element.Text;
        if (!text.StartsWith("Room: "))
        {
            throw new InvalidOperationException($"Unexpected room header text: '{text}'");
        }
        var numberPart = text.Substring("Room: ".Length);
        return int.Parse(numberPart.Trim());
    }

    public string GetDisplayedRoomType()
    {
        var element = WaitHelper.WaitForElement(Driver, DisplayedType);
        return element.Text;
    }
    
    public bool GetDisplayedAccessibility()
    {
        var element = WaitHelper.WaitForElement(Driver, DisplayedAccessibility);
        return bool.Parse(element.Text);
    }
    
    public decimal GetDisplayedPrice()
    {
        var element = WaitHelper.WaitForElement(Driver, DisplayedPrice);
        return decimal.Parse(element.Text);
    }
    
    public List<string> GetDisplayedFeatures()
    {
        var element = WaitHelper.WaitForElement(Driver, DisplayedFeatures);
        var featuresText = element.Text;
        if (featuresText.Contains("No features added")) return new List<string>();
        return featuresText.Split(new[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList();
    }

    public bool IsImageFallbackActive()
    {
        try
        {
            var image = WaitHelper.WaitForElement(Driver, RoomImage);
            return image.GetAttribute("naturalWidth") == "0";
        }
        catch (WebDriverTimeoutException)
        {
            // If the image element doesn't appear in time, consider it a fallback state.
            return true;
        }
        catch (StaleElementReferenceException)
        {
            // If the element becomes stale, it's likely due to a page update; treat as fallback.
            return true;
        }
    }

    private static bool IsElementVisible(IWebDriver driver, By locator)
    {
        try
        {
            return driver.FindElement(locator).Displayed;
        }
        catch (NoSuchElementException)
        {
            return false;
        }
    }
} 