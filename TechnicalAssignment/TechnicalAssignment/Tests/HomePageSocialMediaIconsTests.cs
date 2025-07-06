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
} 