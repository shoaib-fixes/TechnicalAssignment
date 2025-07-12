using System;
using System.Linq;
using System.Threading.Tasks;
using lighthouse.net;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using TechnicalAssignment.Models;

namespace TechnicalAssignment.Tests;

[TestFixture]
[Category("Performance")]
public class PerformanceTests : BaseTest
{
    private const decimal MinAcceptablePerformanceScore = 0.8m;

    [Test(Description = "TC028: Verify that the home page meets performance standards")]
    public async Task HomePage_Performance_ShouldMeetThresholds()
    {
        // Will only run on Chrome as remote debugging port is configured for it
        if (TestConfig.Browser.DefaultBrowserType != BrowserType.Chrome)
        {
            Assert.Ignore("Performance tests are configured to run only on Chrome.");
        }
        
        Logger.LogInformation("Starting performance test for HomePage");

        Driver.Navigate().GoToUrl(TestConfig.BaseUrl);

        var lh = new Lighthouse();

        var result = await lh.Run(TestConfig.BaseUrl);

        Assert.That(result, Is.Not.Null, "Lighthouse audit should produce a result.");
        
        var performanceScore = result.Performance;
        
        Logger.LogInformation("Lighthouse Performance Score for HomePage: {PerformanceScore}", performanceScore);

        if (performanceScore < MinAcceptablePerformanceScore)
        {
            Assert.Fail(
                $"Performance score of {performanceScore} is below the threshold of {MinAcceptablePerformanceScore}.");
        }
        
        Assert.Pass($"Performance score of {performanceScore} meets or exceeds the threshold of {MinAcceptablePerformanceScore}.");
    }
} 