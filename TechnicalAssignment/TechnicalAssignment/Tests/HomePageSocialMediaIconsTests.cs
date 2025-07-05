using System;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using OpenQA.Selenium;
using TechnicalAssignment.Pages;
using TechnicalAssignment.Utilities;

namespace TechnicalAssignment.Tests;

[TestFixture]
[Category("SocialMedia")]
[Category("HomePage")]
[Parallelizable(ParallelScope.Fixtures)]
public class HomePageSocialMediaIconsTests : BaseTest
{
    private HomePage _homePage = null!;

    [SetUp]
    public void HomePageSetUp()
    {
        Logger.LogInformation("Starting HomePage Social Media Icons test setup for browser: {Browser}", CurrentBrowser);
        Driver.Navigate().GoToUrl(Config.BaseUrl);
        Logger.LogDebug("Navigated to URL: {Url}", Config.BaseUrl);
        
        _homePage = new HomePage(Driver);
        _homePage.WaitForPageToLoad();
        Logger.LogDebug("HomePage loaded successfully for Social Media Icons tests");
    }

    [Test]
    [Description("TC001: Verify that all social media icons are visible in the footer")]
    public void SocialMediaIcons_ShouldBeVisibleInFooter()
    {
        Logger.LogInformation("Starting TC001: Social media icons visibility test for browser: {Browser}", CurrentBrowser);
        
        Logger.LogDebug("Scrolling to Social Media Icons section");
        _homePage.ScrollToSocialMediaIcons();
        
        Logger.LogDebug("Checking Social Media Icons container visibility");
        Assert.That(_homePage.IsSocialMediaIconsContainerVisible(TimeSpan.FromSeconds(5)), Is.True, 
            "Social Media Icons container should be visible");
        
        Logger.LogDebug("Checking all 3 Social Media Icons visibility");
        Assert.That(_homePage.AreAllSocialMediaIconsVisible(TimeSpan.FromSeconds(5)), Is.True, 
            "All 3 Social Media Icons should be visible");
        
        Logger.LogInformation("TC001: Social Media Icons section visibility test passed successfully for browser: {Browser}", CurrentBrowser);
    }

    [Test]
    [Description("TC002: Verify that the Facebook icon links to the correct URL")]
    public void FacebookIcon_ShouldLinkToCorrectUrl()
    {
        Logger.LogInformation("Starting TC002: Facebook icon URL test for browser: {Browser}", CurrentBrowser);
        
        Logger.LogDebug("Scrolling to Social Media Icons section for Facebook link test");
        _homePage.ScrollToSocialMediaIcons();
        
        Logger.LogDebug("Getting Facebook icon href attribute");
        var facebookElement = _homePage.GetFacebookIconElement();
        var facebookHref = facebookElement.GetAttribute("href");
        Logger.LogDebug("Facebook icon href: {Href}", facebookHref);
        
        Logger.LogDebug("Validating Facebook icon link URL");
        Assert.That(facebookHref, Does.Contain("facebook.com"), 
            "Facebook icon should point to Facebook URL");
        
        Logger.LogInformation("TC002: Facebook icon link URL test passed for browser: {Browser}", CurrentBrowser);
    }

    [Test]
    [Description("TC003: Verify that the Twitter icon links to the correct URL")]
    public void TwitterIcon_ShouldLinkToCorrectUrl()
    {
        Logger.LogInformation("Starting TC003: Twitter icon URL test for browser: {Browser}", CurrentBrowser);
        
        Logger.LogDebug("Scrolling to Social Media Icons section for Twitter link test");
        _homePage.ScrollToSocialMediaIcons();
        
        Logger.LogDebug("Getting Twitter icon href attribute");
        var twitterElement = _homePage.GetTwitterIconElement();
        var twitterHref = twitterElement.GetAttribute("href");
        Logger.LogDebug("Twitter icon href: {Href}", twitterHref);
        
        Logger.LogDebug("Validating Twitter icon link URL");
        Assert.That(twitterHref, Does.Contain("twitter.com"), 
            "Twitter icon should point to Twitter URL");
        
        Logger.LogInformation("TC003: Twitter icon link URL test passed for browser: {Browser}", CurrentBrowser);
    }

    [Test]
    [Description("TC004: Verify that the Instagram icon links to the correct URL")]
    public void InstagramIcon_ShouldLinkToCorrectUrl()
    {
        Logger.LogInformation("Starting TC004: Instagram icon URL test for browser: {Browser}", CurrentBrowser);
        
        Logger.LogDebug("Scrolling to Social Media Icons section for Instagram link test");
        _homePage.ScrollToSocialMediaIcons();
        
        Logger.LogDebug("Getting Instagram icon href attribute");
        var instagramElement = _homePage.GetInstagramIconElement();
        var instagramHref = instagramElement.GetAttribute("href");
        Logger.LogDebug("Instagram icon href: {Href}", instagramHref);
        
        Logger.LogDebug("Validating Instagram icon link URL");
        Assert.That(instagramHref, Does.Contain("instagram.com"), 
            "Instagram icon should point to Instagram URL");
        
        Logger.LogInformation("TC004: Instagram icon link URL test passed for browser: {Browser}", CurrentBrowser);
    }

    [Test]
    [Description("TC005: Verify that clicking a social media icon opens the link in a new tab")]
    public void SocialMediaLinks_ShouldOpenInNewTab()
    {
        Logger.LogInformation("Starting TC005: Social media new tab test for browser: {Browser}", CurrentBrowser);

        _homePage.ScrollToSocialMediaIcons();
        
        var initialWindowCount = BrowserHelper.GetWindowCount(Driver);
        
        _homePage.ClickFacebookIcon();
        
        WaitHelper.WaitForCondition(Driver, d => BrowserHelper.GetWindowCount(d) > initialWindowCount, TimeSpan.FromSeconds(5));

        BrowserHelper.SwitchToLastWindow(Driver);

        Assert.Multiple(() =>
        {
            Assert.That(BrowserHelper.GetWindowCount(Driver), Is.EqualTo(initialWindowCount + 1), "Clicking a social media icon should open a new tab.");
            Assert.That(BrowserHelper.GetCurrentUrl(Driver), Does.Contain("facebook.com"), "New tab URL should be for Facebook.");
        });
        
        Logger.LogInformation("TC005: Social media new tab test passed successfully for browser: {Browser}", CurrentBrowser);
    }
} 