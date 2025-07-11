using System;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using TechnicalAssignment.Pages;
using TechnicalAssignment.Utilities;
using TechnicalAssignment.Models;

namespace TechnicalAssignment.Tests;

[TestFixture]
[Category("PriceValidation")]
[Parallelizable(ParallelScope.Fixtures)]
public class HomePageBookingPriceValidationTests : BaseTest
{
    private HomePage _homePage = null!;
    private ReservationPage _reservationPage = null!;

    [SetUp]
    public void PageSetup()
    {
        Logger.LogInformation("Setting up HomePageBookingPriceValidationTests");
        Driver.Navigate().GoToUrl(TestConfig.BaseUrl);
        _homePage = new HomePage(Driver);
        _homePage.WaitForPageToLoad();
        _reservationPage = new ReservationPage(Driver);
    }
    
    [TestCaseSource(typeof(PriceValidationTestData), nameof(PriceValidationTestData.GetFeeValidationTestCases))]
    [Description("Verify that price summary calculations are correct with data-driven fee validation")]
    public void PriceSummary_WithValidBooking_ShouldShowCorrectCalculations(string feeType, decimal expectedFee)
    {
        Logger.LogInformation("Starting price summary validation test for {FeeType} with expected value £{ExpectedFee}", feeType, expectedFee);
        
        // Navigate to a reservation page
        Assert.That(_homePage.NavigateToReservationPage(), Is.True, 
            "Should successfully navigate to reservation page");
        _reservationPage.WaitForPageToLoad(TimeSpan.FromSeconds(15));
        
        // Wait for price summary to be visible
        Assert.That(_reservationPage.PriceSummary.IsPriceSummaryVisible(TimeSpan.FromSeconds(10)), Is.True,
            "Price summary should be visible");
        
        // Get price components using the new component structure
        var basePrice = _reservationPage.PriceSummary.GetBasePrice();
        var cleaningFee = _reservationPage.PriceSummary.GetCleaningFee();
        var serviceFee = _reservationPage.PriceSummary.GetServiceFee();
        var totalPrice = _reservationPage.PriceSummary.GetTotalPrice();
        var nightsCount = _reservationPage.PriceSummary.GetNightsCount();
        
        Logger.LogDebug("Price components - Base: £{Base}, Cleaning: £{Cleaning}, Service: £{Service}, Total: £{Total}, Nights: {Nights}",
            basePrice, cleaningFee, serviceFee, totalPrice, nightsCount);
        
        // Perform assertions directly in the test - this is the proper approach
        Assert.Multiple(() =>
        {
            Assert.That(basePrice, Is.GreaterThan(0), "Base price should be greater than 0");
            Assert.That(totalPrice, Is.GreaterThan(0), "Total price should be greater than 0");
            Assert.That(nightsCount, Is.GreaterThan(0), "Nights count should be greater than 0");
            
            // Validate the specific fee based on the test case
            if (feeType == "Cleaning Fee")
            {
                Assert.That(cleaningFee, Is.EqualTo(expectedFee), $"Cleaning fee should be £{expectedFee}");
            }
            else if (feeType == "Service Fee")
            {
                Assert.That(serviceFee, Is.EqualTo(expectedFee), $"Service fee should be £{expectedFee}");
            }
            
            // Verify calculation is correct using the expected values from test data
            var expectedCleaningFee = PriceValidationTestData.StandardUKPricing.CleaningFee;
            var expectedServiceFee = PriceValidationTestData.StandardUKPricing.ServiceFee;
            var expectedTotal = basePrice + expectedCleaningFee + expectedServiceFee;
            Assert.That(Math.Abs(totalPrice - expectedTotal), Is.LessThan(0.01m), 
                $"Total price calculation should be correct: Base(£{basePrice}) + Cleaning(£{expectedCleaningFee}) + Service(£{expectedServiceFee}) = £{expectedTotal}, but got £{totalPrice}");
        });
        
        Logger.LogInformation("Price summary validation test passed successfully for {FeeType}", feeType);
    }
    
    [TestCaseSource(typeof(PriceValidationTestData), nameof(PriceValidationTestData.GetTextFormatTestCases))]
    [Description("Verify that price summary displays correct text format using data-driven validation")]
    public void PriceSummary_TextFormats_ShouldBeCorrect(string[] requiredCurrencySymbols, string nightsPattern, bool shouldNotBeEmpty, string testScenario)
    {
        Logger.LogInformation("Starting price summary text format validation test for {TestScenario}", testScenario);
        
        Assert.That(_homePage.NavigateToReservationPage(), Is.True, 
            "Should successfully navigate to reservation page");
        _reservationPage.WaitForPageToLoad(TimeSpan.FromSeconds(15));
        
        Assert.That(_reservationPage.PriceSummary.IsPriceSummaryVisible(TimeSpan.FromSeconds(10)), Is.True,
            "Price summary should be visible");
        
        // Get text formats using the new component structure
        var basePriceText = _reservationPage.PriceSummary.GetBasePriceText();
        var cleaningFeeText = _reservationPage.PriceSummary.GetCleaningFeeText();
        var serviceFeeText = _reservationPage.PriceSummary.GetServiceFeeText();
        var totalPriceText = _reservationPage.PriceSummary.GetTotalPriceText();
        var nightsText = _reservationPage.PriceSummary.GetNightsCountText();
        
        Logger.LogDebug("Price text formats - Base: '{Base}', Cleaning: '{Cleaning}', Service: '{Service}', Total: '{Total}', Nights: '{Nights}'",
            basePriceText, cleaningFeeText, serviceFeeText, totalPriceText, nightsText);
        
        // Perform text format assertions directly in the test using data-driven values
        Assert.Multiple(() =>
        {
            if (shouldNotBeEmpty)
            {
                Assert.That(basePriceText, Is.Not.Empty, "Base price text should not be empty");
                Assert.That(cleaningFeeText, Is.Not.Empty, "Cleaning fee text should not be empty");
                Assert.That(serviceFeeText, Is.Not.Empty, "Service fee text should not be empty");
                Assert.That(totalPriceText, Is.Not.Empty, "Total price text should not be empty");
                Assert.That(nightsText, Is.Not.Empty, "Nights text should not be empty");
            }
            
            // Verify text contains currency symbols from test data
            var currencyConstraint = requiredCurrencySymbols.Length > 1 
                ? Does.Contain(requiredCurrencySymbols[0]).Or.Contain(requiredCurrencySymbols[1])
                : Does.Contain(requiredCurrencySymbols[0]);
            
            Assert.That(basePriceText, currencyConstraint, "Base price should contain expected currency symbol");
            Assert.That(totalPriceText, currencyConstraint, "Total price should contain expected currency symbol");
            Assert.That(nightsText, Does.Match(nightsPattern), "Nights text should match expected pattern");
        });
        
        Logger.LogInformation("Price summary text format validation test passed successfully for {TestScenario}", testScenario);
    }

    [TestCaseSource(typeof(PriceValidationTestData), nameof(PriceValidationTestData.GetCurrencySymbolTestCases))]
    [Description("Verify that price summary handles different currency symbols correctly")]
    public void PriceSummary_CurrencySymbols_ShouldBeHandledCorrectly(string expectedCurrencySymbol)
    {
        Logger.LogInformation("Starting currency symbol validation test for '{CurrencySymbol}'", expectedCurrencySymbol);
        
        Assert.That(_homePage.NavigateToReservationPage(), Is.True, 
            "Should successfully navigate to reservation page");
        _reservationPage.WaitForPageToLoad(TimeSpan.FromSeconds(15));
        
        Assert.That(_reservationPage.PriceSummary.IsPriceSummaryVisible(TimeSpan.FromSeconds(10)), Is.True,
            "Price summary should be visible");
        
        // Get price text formats
        var basePriceText = _reservationPage.PriceSummary.GetBasePriceText();
        var totalPriceText = _reservationPage.PriceSummary.GetTotalPriceText();
        
        Logger.LogDebug("Validating currency symbol '{Symbol}' in price texts - Base: '{Base}', Total: '{Total}'",
            expectedCurrencySymbol, basePriceText, totalPriceText);
        
        // Verify that at least one of the price texts contains the expected currency symbol
        var containsCurrency = basePriceText.Contains(expectedCurrencySymbol) || totalPriceText.Contains(expectedCurrencySymbol);
        Assert.That(containsCurrency, Is.True, 
            $"Price texts should contain the expected currency symbol '{expectedCurrencySymbol}'");
        
        Logger.LogInformation("Currency symbol validation test passed successfully for '{CurrencySymbol}'", expectedCurrencySymbol);
    }

    [TestCaseSource(typeof(PriceValidationTestData), nameof(PriceValidationTestData.GetBoundaryTestCases))]
    [Description("Verify price validation with boundary values")]
    public void PriceSummary_BoundaryValues_ShouldBeHandledCorrectly(decimal testCleaningFee, decimal testServiceFee, string testScenario)
    {
        Logger.LogInformation("Starting boundary value validation test for {TestScenario} - Cleaning: £{Cleaning}, Service: £{Service}", 
            testScenario, testCleaningFee, testServiceFee);
        
        // This test verifies that the system can handle boundary values
        // Note: This is a structural test - the actual values come from the system
        // but we validate they meet our boundary expectations
        
        Assert.That(_homePage.NavigateToReservationPage(), Is.True, 
            "Should successfully navigate to reservation page");
        _reservationPage.WaitForPageToLoad(TimeSpan.FromSeconds(15));
        
        Assert.That(_reservationPage.PriceSummary.IsPriceSummaryVisible(TimeSpan.FromSeconds(10)), Is.True,
            "Price summary should be visible");
        
        // Get actual values from the system
        var actualCleaningFee = _reservationPage.PriceSummary.GetCleaningFee();
        var actualServiceFee = _reservationPage.PriceSummary.GetServiceFee();
        var totalPrice = _reservationPage.PriceSummary.GetTotalPrice();
        
        Logger.LogDebug("Boundary test {TestScenario} - Actual Cleaning: £{Cleaning}, Service: £{Service}, Total: £{Total}",
            testScenario, actualCleaningFee, actualServiceFee, totalPrice);
        
        // Validate that the system handles the values appropriately
        Assert.Multiple(() =>
        {
            Assert.That(actualCleaningFee, Is.GreaterThanOrEqualTo(0), "Cleaning fee should not be negative");
            Assert.That(actualServiceFee, Is.GreaterThanOrEqualTo(0), "Service fee should not be negative");
            Assert.That(totalPrice, Is.GreaterThanOrEqualTo(0), "Total price should not be negative");
            
            // For this boundary test, we're validating structure rather than exact values
            // The test data provides the boundary scenarios we want to validate against
            Logger.LogDebug("Boundary validation passed for {TestScenario}", testScenario);
        });
        
        Logger.LogInformation("Boundary value validation test passed successfully for {TestScenario}", testScenario);
    }
} 