using System;
using System.Text.RegularExpressions;

namespace TechnicalAssignment.Utilities;

public static class PriceHelper
{
    public static decimal ExtractPriceValue(string priceText)
    {
        if (string.IsNullOrEmpty(priceText))
            return 0;

        var numericText = Regex.Replace(priceText, @"[^\d.,]", "").Replace(",", "").Trim();
        
        return decimal.TryParse(numericText, out var value) ? value : 0;
    }

    public static int ExtractPriceValueAsInt(string priceText)
    {
        var decimalValue = ExtractPriceValue(priceText);
        return (int)Math.Round(decimalValue);
    }

    public static int ExtractNightsCount(string nightsText)
    {
        if (string.IsNullOrEmpty(nightsText))
            return 0;

        var match = Regex.Match(nightsText, @"x\s*(\d+)\s*night");
        if (match.Success && int.TryParse(match.Groups[1].Value, out var nights))
        {
            return nights;
        }
        return 0;
    }

    public static decimal ExtractRoomPriceFromNightsText(string nightsText)
    {
        if (string.IsNullOrEmpty(nightsText))
            return 0;

        var match = Regex.Match(nightsText, @"£([\d,]+(?:\.\d{2})?)");
        if (match.Success)
        {
            var priceText = match.Groups[1].Value.Replace(",", "");
            return decimal.TryParse(priceText, out var price) ? price : 0;
        }
        return 0;
    }

    public static string FormatPrice(decimal price, string currencySymbol = "£")
    {
        return $"{currencySymbol}{price:F2}";
    }

    public static bool IsValidPrice(decimal price, decimal maxPrice = 10000)
    {
        return price > 0 && price <= maxPrice;
    }
} 