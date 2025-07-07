using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using TechnicalAssignment.Utilities;

namespace TechnicalAssignment.Pages;

public class AdminRoomsPage : BasePage
{
    private static readonly By RoomRows = By.CssSelector("div[data-testid='roomlisting']");
    private static readonly By CreateForm = By.CssSelector("div.row.room-form");
    private static readonly By RoomNumberInput = By.Id("roomName");
    private static readonly By TypeSelect = By.Id("type");
    private static readonly By AccessibleSelect = By.Id("accessible");
    private static readonly By RoomPriceInput = By.Id("roomPrice");
    private static readonly By CreateButton = By.Id("createRoom");
    private static readonly By ErrorAlert = By.CssSelector("div.alert-danger p");

    public AdminRoomsPage(IWebDriver driver) : base(driver) { }

    public override bool IsPageLoaded(TimeSpan? timeout = null) => ElementHelper.IsElementVisible(Driver, CreateForm, timeout);

    public override void WaitForPageToLoad(TimeSpan? timeout = null) => WaitHelper.WaitForElement(Driver, CreateForm, timeout);

    public void ClickCreateRoom()
    {
        ScrollHelper.ScrollToElement(Driver, CreateButton, ScrollBehavior.Instant, ScrollAlignment.Center);
        ElementHelper.JavaScriptClick(Driver, CreateButton);
    }

    public void CreateRoom(int roomNumber, string type, bool accessible, int price, IEnumerable<string> features)
    {
        CreateRoom(roomNumber.ToString(), type, accessible, price.ToString(), features);
    }

    public void CreateRoom(int roomNumber, string type, bool accessible, decimal price, IEnumerable<string> features)
    {
        CreateRoom(roomNumber.ToString(), type, accessible, price.ToString("F2"), features);
    }

    public void CreateRoom(string roomNumber, string type, bool accessible, string price, IEnumerable<string> features)
    {
        Driver.FindElement(RoomNumberInput).Clear();
        Driver.FindElement(RoomPriceInput).Clear();
        
        ElementHelper.SafeSendKeys(Driver, RoomNumberInput, roomNumber);
        if (!string.IsNullOrEmpty(type))
            ElementHelper.SelectDropdownByText(Driver, TypeSelect, type);
        ElementHelper.SelectDropdownByValue(Driver, AccessibleSelect, accessible.ToString().ToLower());
        ElementHelper.SafeSendKeys(Driver, RoomPriceInput, price);
        
        foreach (var feature in features)
        {
            var featureLocator = By.CssSelector($"input[name='featureCheck'][value='{feature}']");
            ScrollHelper.ScrollToElement(Driver, featureLocator, ScrollBehavior.Instant, ScrollAlignment.Center);
            ElementHelper.JavaScriptClick(Driver, featureLocator);
        }
        
        ScrollHelper.ScrollToElement(Driver, CreateButton, ScrollBehavior.Instant, ScrollAlignment.Center);
        ElementHelper.JavaScriptClick(Driver, CreateButton);
    }

    public bool IsRoomPresent(int roomNumber, string? type = null, bool? accessible = null, int? price = null, IEnumerable<string>? features = null)
    {
        var rows = Driver.FindElements(RoomRows);
        foreach (var row in rows)
        {
            var name = row.FindElement(By.CssSelector("p[id^='roomName']")).Text.Trim();
            if (!name.Equals(roomNumber.ToString(), StringComparison.OrdinalIgnoreCase))
                continue;
            if (type != null && !row.FindElement(By.CssSelector("p[id^='type']")).Text.Equals(type, StringComparison.OrdinalIgnoreCase))
                return false;
            if (accessible.HasValue && !row.FindElement(By.CssSelector("p[id^='accessible']")).Text.Equals(accessible.Value.ToString().ToLower(), StringComparison.OrdinalIgnoreCase))
                return false;
            if (price.HasValue && !row.FindElement(By.CssSelector("p[id^='roomPrice']")).Text.Equals(price.Value.ToString(), StringComparison.OrdinalIgnoreCase))
                return false;
            if (features != null)
            {
                var details = row.FindElement(By.CssSelector("p[id^='details']")).Text;
                if (!features.All(f => details.Contains(f, StringComparison.OrdinalIgnoreCase)))
                    return false;
            }
            return true;
        }
        return false;
    }

    public (string, bool) GetDefaultDropdownValues()
    {
        var typeElement = new SelectElement(Driver.FindElement(TypeSelect));
        var accessibleElement = new SelectElement(Driver.FindElement(AccessibleSelect));
        
        var defaultType = typeElement.SelectedOption.Text;
        var defaultAccessibility = bool.Parse(accessibleElement.SelectedOption.GetAttribute("value") ?? "false");
        
        return (defaultType, defaultAccessibility);
    }

    public void DeleteRoom(int roomNumber)
    {
        var rows = Driver.FindElements(RoomRows);
        foreach (var row in rows)
        {
            var name = row.FindElement(By.CssSelector("p[id^='roomName']")).Text.Trim();
            if (name.Equals(roomNumber.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                var deleteButton = row.FindElement(By.CssSelector("span.roomDelete"));
                ScrollHelper.ScrollToElement(Driver, By.CssSelector("span.roomDelete"), ScrollBehavior.Smooth);
                deleteButton.Click();
                return;
            }
        }
    }

    public string GetErrorAlertText() => Driver.FindElement(ErrorAlert).Text.Trim();

    public bool IsErrorAlertVisible() => ElementHelper.IsElementPresent(Driver, By.CssSelector("div.alert-danger"));

    public string GetFullErrorMessage()
    {
        var errorAlert = Driver.FindElement(By.CssSelector("div.alert-danger"));
        return errorAlert.Text.Trim();
    }

    public bool ErrorMessageContains(string expectedText)
    {
        try
        {
            var fullErrorMessage = GetFullErrorMessage();
            return fullErrorMessage.Contains(expectedText, StringComparison.OrdinalIgnoreCase);
        }
        catch (NoSuchElementException)
        {
            return false;
        }
    }

    public void WaitForErrorAlert(TimeSpan? timeout = null)
    {
        WaitHelper.WaitForElement(Driver, By.CssSelector("div.alert-danger"), timeout);
    }

    public string GetRoomType(int roomNumber)
    {
        var rows = Driver.FindElements(RoomRows);
        foreach (var row in rows)
        {
            var name = row.FindElement(By.CssSelector("p[id^='roomName']")).Text.Trim();
            if (name.Equals(roomNumber.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                return row.FindElement(By.CssSelector("p[id^='type']")).Text.Trim();
            }
        }
        return string.Empty;
    }

    public bool GetRoomAccessibility(int roomNumber)
    {
        var rows = Driver.FindElements(RoomRows);
        foreach (var row in rows)
        {
            var name = row.FindElement(By.CssSelector("p[id^='roomName']")).Text.Trim();
            if (name.Equals(roomNumber.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                var accessibleText = row.FindElement(By.CssSelector("p[id^='accessible']")).Text.Trim();
                return bool.Parse(accessibleText);
            }
        }
        return false;
    }

    public int GetRoomPrice(int roomNumber)
    {
        var rows = Driver.FindElements(RoomRows);
        foreach (var row in rows)
        {
            var name = row.FindElement(By.CssSelector("p[id^='roomName']")).Text.Trim();
            if (name.Equals(roomNumber.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                var priceText = row.FindElement(By.CssSelector("p[id^='roomPrice']")).Text.Trim();
                if (decimal.TryParse(priceText, out var decimalPrice))
                {
                    return (int)decimalPrice;
                }
                return int.Parse(priceText);
            }
        }
        return 0;
    }

    public decimal GetRoomPriceAsDecimal(int roomNumber)
    {
        var rows = Driver.FindElements(RoomRows);
        foreach (var row in rows)
        {
            var name = row.FindElement(By.CssSelector("p[id^='roomName']")).Text.Trim();
            if (name.Equals(roomNumber.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                var priceText = row.FindElement(By.CssSelector("p[id^='roomPrice']")).Text.Trim();
                return decimal.Parse(priceText);
            }
        }
        return 0;
    }

    public AdminRoomPage NavigateToRoomDetails(int roomNumber)
    {
        var roomNumberLocator = By.Id($"roomName{roomNumber}");
        ScrollHelper.ScrollToElement(Driver, roomNumberLocator);
        ElementHelper.JavaScriptClick(Driver, roomNumberLocator);
        
        return new AdminRoomPage(Driver);
    }

    public void BulkCreateRooms(IEnumerable<(int RoomNumber, string Type, bool Accessible, int Price, IEnumerable<string> Features)> roomConfigurations)
    {
        foreach (var (roomNumber, type, accessible, price, features) in roomConfigurations)
        {
            CreateRoom(roomNumber, type, accessible, price, features);
            Driver.Navigate().Refresh();
            WaitForPageToLoad();
        }
    }

    public void BulkDeleteRooms(IEnumerable<int> roomNumbers)
    {
        foreach (var roomNumber in roomNumbers)
        {
            try
            {
                DeleteRoom(roomNumber);
                Driver.Navigate().Refresh();
                WaitForPageToLoad();
            }
            catch (Exception ex)
            {
                Logger.LogWarning(ex, "Failed to delete room {RoomNumber}", roomNumber);
            }
        }
    }

    public bool VerifyRoomProperties(int roomNumber, string expectedType, bool expectedAccessible, int expectedPrice)
    {
        if (!IsRoomPresent(roomNumber))
            return false;

        var actualType = GetRoomType(roomNumber);
        var actualAccessible = GetRoomAccessibility(roomNumber);
        var actualPrice = GetRoomPrice(roomNumber);

        return actualType.Equals(expectedType, StringComparison.OrdinalIgnoreCase) &&
               actualAccessible == expectedAccessible &&
               actualPrice == expectedPrice;
    }

    public bool WaitForRoomToAppear(int roomNumber, TimeSpan? timeout = null)
    {
        return WaitHelper.WaitForCondition(Driver, d => 
        {
            d.Navigate().Refresh();
            WaitForPageToLoad();
            return IsRoomPresent(roomNumber);
        }, timeout ?? TimeSpan.FromSeconds(10));
    }

    public bool WaitForRoomToDisappear(int roomNumber, TimeSpan? timeout = null)
    {
        return WaitHelper.WaitForCondition(Driver, d => 
        {
            d.Navigate().Refresh();
            WaitForPageToLoad();
            return !IsRoomPresent(roomNumber);
        }, timeout ?? TimeSpan.FromSeconds(10));
    }
} 