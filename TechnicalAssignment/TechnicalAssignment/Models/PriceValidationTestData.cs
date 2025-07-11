using System.Collections.Generic;
using System.Linq;

namespace TechnicalAssignment.Models;

/// <summary>
/// Contains test data for price validation tests
/// </summary>
public static class PriceValidationTestData
{
    /// <summary>
    /// Represents expected price components for validation
    /// </summary>
    public record PriceComponents(decimal CleaningFee, decimal ServiceFee, string[] AcceptedCurrencySymbols, string NightsTextPattern);

    /// <summary>
    /// Represents expected text format validation criteria
    /// </summary>
    public record TextFormatValidation(string[] RequiredCurrencySymbols, string NightsPattern, bool ShouldNotBeEmpty);

    /// <summary>
    /// Standard price components for UK bookings
    /// </summary>
    public static PriceComponents StandardUKPricing => new(
        CleaningFee: 25m,
        ServiceFee: 15m,
        AcceptedCurrencySymbols: new[] { "£" },
        NightsTextPattern: @"\d+\s*night"
    );

    /// <summary>
    /// Alternative price components for testing different scenarios
    /// </summary>
    public static PriceComponents AlternativePricing => new(
        CleaningFee: 30m,
        ServiceFee: 20m,
        AcceptedCurrencySymbols: new[] { "£", "$", "€" },
        NightsTextPattern: @"\d+\s*night"
    );

    /// <summary>
    /// Standard text format validation criteria
    /// </summary>
    public static TextFormatValidation StandardTextFormat => new(
        RequiredCurrencySymbols: new[] { "£" },
        NightsPattern: @"\d+\s*night",
        ShouldNotBeEmpty: true
    );

    /// <summary>
    /// Gets all price component test cases for comprehensive testing
    /// </summary>
    public static object[] GetPriceComponentTestCases()
    {
        return new object[]
        {
            new object[] { StandardUKPricing.CleaningFee, StandardUKPricing.ServiceFee, StandardUKPricing.AcceptedCurrencySymbols, StandardUKPricing.NightsTextPattern, "Standard UK Pricing" },
            new object[] { AlternativePricing.CleaningFee, AlternativePricing.ServiceFee, AlternativePricing.AcceptedCurrencySymbols, AlternativePricing.NightsTextPattern, "Alternative Pricing" }
        };
    }

    /// <summary>
    /// Gets text format validation test cases
    /// </summary>
    public static object[] GetTextFormatTestCases()
    {
        return new object[]
        {
            new object[] { StandardTextFormat.RequiredCurrencySymbols, StandardTextFormat.NightsPattern, StandardTextFormat.ShouldNotBeEmpty, "Standard Text Format" }
        };
    }

    /// <summary>
    /// Gets currency symbol test cases - focused on actual system behavior
    /// </summary>
    public static object[] GetCurrencySymbolTestCases()
    {
        return StandardUKPricing.AcceptedCurrencySymbols.Select(symbol => new object[] { symbol }).ToArray();
    }

    /// <summary>
    /// Gets fee validation test cases
    /// </summary>
    public static object[] GetFeeValidationTestCases()
    {
        return new object[]
        {
            new object[] { "Cleaning Fee", StandardUKPricing.CleaningFee },
            new object[] { "Service Fee", StandardUKPricing.ServiceFee }
        };
    }

    /// <summary>
    /// Boundary test cases for price validation
    /// </summary>
    public static object[] GetBoundaryTestCases()
    {
        return new object[]
        {
            new object[] { 0m, 0m, "Zero Fees" },
            new object[] { 0.01m, 0.01m, "Minimum Fees" },
            new object[] { 999.99m, 999.99m, "Maximum Fees" }
        };
    }

    /// <summary>
    /// Gets comprehensive price validation test cases combining multiple scenarios
    /// </summary>
    public static IEnumerable<object[]> GetComprehensivePriceTestCases()
    {
        yield return new object[] { StandardUKPricing, "Standard UK pricing validation" };
        yield return new object[] { AlternativePricing, "Alternative pricing validation" };
    }
} 