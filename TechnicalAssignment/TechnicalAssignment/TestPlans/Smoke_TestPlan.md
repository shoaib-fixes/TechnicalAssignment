# Smoke Test Plan

## Overview
This is a set of high-level smoke tests for the home page. They are meant to be a quick health check to make sure the site is stable. We check that the page loads, critical parts are visible, navigation works, and the layout isn't broken on standard devices.

## Test Strategy
These tests are not deep. They just check that the most important user-facing components are there and basically working. The goal is fast feedback to make sure a new build isn't completely broken.

## Test Cases

### TC001: Home page loads without errors
Objective: The most basic check: make sure the home page loads, has the right title, and content is visible.
Steps: Go to the home page URL, get the title, and check that the main content is loaded.
Expected: The title is correct and the page content is visible.

### TC002: Quick Links component is visible
Objective: Confirm the Quick Links in the footer are visible.
Steps: Check for the visibility of the Quick Links section.
Expected: The Quick Links section is visible.

### TC003: Social Media component is visible
Objective: Confirm the Social Media icons in the footer are visible.
Steps: Check for the visibility of the Social Media icons container.
Expected: The Social Media section is visible.

### TC004: Basic navigation works
Objective: Test that the internal page scroll navigation works.
Steps: Click the "Rooms" link in the main navigation.
Expected: The URL updates to include #rooms.

### TC005: Responsive on mobile
Objective: A quick check that the layout works on a mobile screen size.
Steps: Resize the browser to an iPhone 8 size and check that the page is still loaded.
Expected: The page is still functional and content is accessible.

### TC006: Responsive on tablet
Objective: A quick check that the layout works on a tablet screen size.
Steps: Resize the browser to an iPad size and check that the page is still loaded.
Expected: The page is still functional and content is accessible. 