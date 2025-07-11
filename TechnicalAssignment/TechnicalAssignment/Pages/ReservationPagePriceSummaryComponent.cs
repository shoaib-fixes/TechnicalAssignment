using System;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using TechnicalAssignment.Utilities;

namespace TechnicalAssignment.Pages;

public class ReservationPagePriceSummaryComponent
{
    protected readonly IWebDriver Driver;
    protected readonly ILogger Logger;
    
    private static readonly By PriceSummaryCard = By.CssSelector(".card.bg-light.border-0");
    private static readonly By BasePrice = By.XPath("//div[@class='d-flex justify-content-between mb-2'][1]/span[2]");
    private static readonly By CleaningFee = By.XPath("//span[text()='Cleaning fee']/parent::div/span[2]");
    private static readonly By ServiceFee = By.XPath("//span[text()='Service fee']/parent::div/span[2]");
    private static readonly By TotalPrice = By.XPath("//div[@class='d-flex justify-content-between fw-bold']/span[2]");
    private static readonly By NightsCount = By.XPath("//div[@class='d-flex justify-content-between mb-2'][1]/span[1]");

    public ReservationPagePriceSummaryComponent(IWebDriver driver, ILogger logger)
    {
        Driver = driver;
        Logger = logger;
    }

    public bool IsPriceSummaryVisible(TimeSpan? timeout = null)
    {
        return ElementHelper.IsElementVisible(Driver, PriceSummaryCard, timeout);
    }

    public string GetBasePriceText()
    {
        Logger.LogDebug("Getting base price text");
        return ElementHelper.GetElementText(Driver, BasePrice);
    }

    public decimal GetBasePrice()
    {
        Logger.LogDebug("Getting base price value");
        var basePriceText = GetBasePriceText();
        return PriceHelper.ExtractPriceValue(basePriceText);
    }

    public string GetCleaningFeeText()
    {
        Logger.LogDebug("Getting cleaning fee text");
        return ElementHelper.GetElementText(Driver, CleaningFee);
    }

    public decimal GetCleaningFee()
    {
        Logger.LogDebug("Getting cleaning fee value");
        var cleaningFeeText = GetCleaningFeeText();
        return PriceHelper.ExtractPriceValue(cleaningFeeText);
    }

    public string GetServiceFeeText()
    {
        Logger.LogDebug("Getting service fee text");
        return ElementHelper.GetElementText(Driver, ServiceFee);
    }

    public decimal GetServiceFee()
    {
        Logger.LogDebug("Getting service fee value");
        var serviceFeeText = GetServiceFeeText();
        return PriceHelper.ExtractPriceValue(serviceFeeText);
    }

    public string GetTotalPriceText()
    {
        Logger.LogDebug("Getting total price text");
        return ElementHelper.GetElementText(Driver, TotalPrice);
    }

    public decimal GetTotalPrice()
    {
        Logger.LogDebug("Getting total price value");
        var totalPriceText = GetTotalPriceText();
        return PriceHelper.ExtractPriceValue(totalPriceText);
    }

    public string GetNightsCountText()
    {
        Logger.LogDebug("Getting nights count text");
        return ElementHelper.GetElementText(Driver, NightsCount);
    }

    public int GetNightsCount()
    {
        Logger.LogDebug("Getting nights count value");
        var nightsText = GetNightsCountText();
        
        var match = System.Text.RegularExpressions.Regex.Match(nightsText, @"(\d+)\s*night");
        if (match.Success && int.TryParse(match.Groups[1].Value, out var nights))
        {
            Logger.LogDebug("Extracted nights count: {Nights} from text: {Text}", nights, nightsText);
            return nights;
        }
        
        Logger.LogWarning("Could not extract nights count from text: {Text}", nightsText);
        return 0;
    }

    public IWebElement GetPriceSummaryElement()
    {
        Logger.LogDebug("Getting price summary element");
        return WaitHelper.WaitForElement(Driver, PriceSummaryCard);
    }
} 