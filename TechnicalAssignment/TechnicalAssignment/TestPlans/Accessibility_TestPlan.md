# Accessibility Test Plan

## Overview
This plan covers the automated accessibility testing for key pages. We use the axe-core engine to check pages against WCAG standards and find violations.

## Test Strategy
We use the Selenium.Axe library, which is a wrapper for the axe-core engine. The tests are highly configurable through the appsettings.json file. A master switch can enable or disable all accessibility tests. We can specify which WCAG tags to test against, and we can configure whether a violation fails the test or just logs a warning. When violations are found, they are logged in detail and a screenshot is captured to help with debugging.

## Test Cases

### TC001: Home page accessibility
Objective: Audit the home page for accessibility violations against different WCAG standards.
Steps: Go to the home page. For each configured WCAG level, run the axe-core analysis.
Expected: The engine reports zero violations. The test will fail if violations are found and the FailOnViolations flag is set.

### TC002: Admin login page accessibility
Objective: Audit the admin login page for accessibility violations.
Steps: Go to the admin login page. For each configured WCAG level, run the axe-core analysis.
Expected: The analysis reports no violations.

### TC003: Admin rooms page accessibility
Objective: Audit the main admin rooms page, which requires login, for accessibility violations.
Steps: Go to the admin login page, log in, and wait for the rooms page to load. For each configured WCAG level, run the axe-core analysis.
Expected: The analysis reports no violations on the authenticated admin page. 