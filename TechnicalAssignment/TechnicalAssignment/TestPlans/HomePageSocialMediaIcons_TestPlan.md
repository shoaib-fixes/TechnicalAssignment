# Home Page Social Media Icons Test Plan

## Overview
This plan tests the social media icons on the home page. It checks that the icons are visible, link to the correct sites, and open in a new browser tab.

## Test Strategy
All tests use a specific page component to interact with the social media icons. The tests also check the browser's tab count to make sure links open in a new tab.

## Test Cases

### TC001: Verify icons are visible and link correctly
Objective: Confirm the Facebook, Twitter, and Instagram icons are visible and link to the right domains.
Steps: Scroll to the icons. For each one, check that it's visible and get its link.
Expected: All three icons are visible and their links contain facebook.com, twitter.com, and instagram.com respectively.

### TC002: Verify links open in a new tab
Objective: Ensure that clicking an icon opens the link in a new tab.
Steps: Scroll to the icons, get the current tab count, and click the Facebook icon.
Expected: The tab count increases to 2, and the new tab's URL is for Facebook. 