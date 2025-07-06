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
    public void PageSetup()
    {
        Driver.Navigate().GoToUrl(TestConfig.BaseUrl);
        _homePage = new HomePage(Driver);
        _homePage.WaitForPageToLoad();
    }

    [Test]
    [Description("Verify that social media icons are visible and link to the correct URLs.")]
    public void SocialMediaLinks_ShouldBeVisibleAndHaveCorrectUrls()
    {
        var socialMediaComponent = _homePage.SocialMedia;
        socialMediaComponent.ScrollToSocialMediaIcons();

        Assert.Multiple(() =>
        {
            // Check Visibility
            Assert.That(socialMediaComponent.IsFacebookIconVisible(), Is.True, "Facebook icon should be visible.");
            Assert.That(socialMediaComponent.IsTwitterIconVisible(), Is.True, "Twitter icon should be visible.");
            Assert.That(socialMediaComponent.IsInstagramIconVisible(), Is.True, "Instagram icon should be visible.");

            // Check URLs
            var facebookHref = socialMediaComponent.GetFacebookIconElement().GetAttribute("href");
            Assert.That(facebookHref, Does.Contain("facebook.com"), "Facebook icon should link to facebook.com.");

            var twitterHref = socialMediaComponent.GetTwitterIconElement().GetAttribute("href");
            Assert.That(twitterHref, Does.Contain("twitter.com"), "Twitter icon should link to twitter.com.");

            var instagramHref = socialMediaComponent.GetInstagramIconElement().GetAttribute("href");
            Assert.That(instagramHref, Does.Contain("instagram.com"), "Instagram icon should link to instagram.com.");
        });
    }

    [Test]
    [Description("Verify that clicking a social media icon opens the link in a new tab")]
    public void SocialMediaLinks_ShouldOpenInNewTab()
    {
        Logger.LogInformation("Social media new tab test");

        var socialMediaComponent = _homePage.SocialMedia;
        
        Logger.LogDebug("Scrolling to Social Media Icons section for new tab test");
        socialMediaComponent.ScrollToSocialMediaIcons();
        
        Logger.LogDebug("Clicking Facebook icon to open in new tab");
        socialMediaComponent.ClickFacebookIcon();
        
        Logger.LogDebug("Verifying new tab is opened");
        Assert.That(Driver.WindowHandles.Count, Is.EqualTo(2), 
            "Clicking a social media icon should open a new tab");
        
        Logger.LogDebug("Switching to new tab");
        Driver.SwitchTo().Window(Driver.WindowHandles[^1]);
        
        Logger.LogDebug("Verifying new tab URL is correct");
        Assert.That(Driver.Url, Does.Contain("facebook.com"), 
            "New tab URL should be Facebook URL");
        
        Logger.LogInformation("Social media new tab test passed successfully");
    }
} 