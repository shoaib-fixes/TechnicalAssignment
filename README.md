# Test Automation Framework

## Overview
This repository contains the automated test suite as requested. It is built with C# and NUnit using Selenium for browser automation, Selenium.Axe for Accessibility testing, Lighthouse for performance testing and generates reports with ExtentReports.

A BugReport.md file can be found the project root.

## Key Features
- Modern .NET Architecture: Built with C# on .NET, using Dependency Injection (Microsoft.Extensions.DependencyInjection) for a clean, decoupled, and scalable design.
- Test-Level Isolation: Each test runs in a new, isolated browser instance, eliminating state leakage between tests and enabling reliable parallel execution.
- Comprehensive Test Coverage: The framework is designed to support multiple types of testing:
    - Functional UI Testing with the Page Object Model (POM).
    - Accessibility Testing via integrated Selenium.Axe checks against WCAG standards.
    - Performance Testing using Lighthouse to capture performance score.
- Robust Selenium Implementation: Includes a suite of helper classes that solve common automation challenges:
    - Smart Waits: WaitHelper automatically detects and waits for loading spinners to disappear, preventing flakiness.
    - Safe Interactions: ElementHelper provides reliable methods for clicks and sending keys that handle common exceptions like stale elements or intercepted clicks.
- Flexible Configuration: A powerful configuration system that allows settings to be controlled via appsettings.json, environment variables, or NUnit .runsettings files, providing flexibility for local and CI/CD environments.
- Rich Test Reporting: Generates detailed HTML reports using ExtentReports, including system information, test steps, and automatic screenshot capture on failure for easy debugging.

## Project Structure
- TechnicalAssignment/: The root directory of the C# solution.
    - Configuration/: Manages test configuration (appsettings.json, environment variables) and provides strongly-typed configuration objects.
    - Drivers/: Contains the WebDriverFactory responsible for creating and configuring Selenium WebDriver instances for different browsers.
    - Models/: Defines Plain Old C# Objects (POCOs) used for test data (e.g., BookingTestData, ContactFormTestData).
    - Pages/: Implements the Page Object Model (POM). Each class represents a page or a component on a page, encapsulating its elements and user interactions.
    - Tests/: Contains all the NUnit test suites. Tests are organised by feature or page.
    - Utilities/: A collection of static helper classes that provide reusable functionality for browser interactions, waits, logging, reporting, and more.
    - TestPlans/: Contains detailed manual test plans written in Markdown.
- TestResults/: The output directory for all test artifacts, including HTML reports, logs, and screenshots, organised by test run.

## Prerequisites

- .NET runtime (https://learn.microsoft.com/en-us/dotnet/core/install/) Windows/Linux/MacOS
- For performance testing Node.js v22 LTS (https://nodejs.org/en/download) will be needed to run lighthouse.net


## Setup

`git clone https://github.com/shoaib-fixes/TechnicalAssignment.git`

`cd TechnicalAssignment\TechnicalAssignment`

`dotnet restore`

## How to Run Tests
To execute the test suite with default settings (4 parallel test workers using Chrome), run the following command from the TechnicalAssignment subdirectory:

`dotnet test --settings test.runsettings`

Parallel workers are configured in the test.runsettings file.

## Filtering Tests
You can run specific tests or categories using the --filter option with the dotnet test command. This is useful for debugging a single test or running a focused test suite.

### Filtering by Category
To run all tests within a specific category, use the following syntax:
`dotnet test --filter "Category=YourCategoryName"`

Example:
`dotnet test --filter "Category=Booking"`

### Available Test Categories
- SmokeTests
- Navigation
- HomePage
- Performance
- SocialMedia
- QuickLinks
- Contact
- Booking
- ReservationPage
- GuestForm
- Admin
- Rooms
- RoomManagement
- EditRoom
- CreateRoom
- AccessibilityTests

### Filtering by Test Name
To run a specific test by its name, use a "Contains" filter:
`dotnet test --filter "FullyQualifiedName~YourTestMethodName"`

Example:
dotnet test --filter `"FullyQualifiedName~ShouldDisplayCorrectRoomDetails_WhenValidRoomIsSelected"`

## Configuration
The framework can be configured through appsettings.json, environment variables, or the test.runsettings file. The Configuration folder contains a more comprehensive README.md file regarding different approaches to configuring the framework.

### Configuration Files
The primary configuration is managed in appsettings.json. It contains settings for:
- Base URL
- Admin credentials
- Browser settings (default browser, headless mode, window size)
- Timeouts
- Accessibility test parameters

You can create an appsettings.Development.json to override settings for a local development environment (example provided)

### Environment Variables
Any setting in appsettings.json can be overridden using environment variables. The variables must be prefixed with TEST_ and use a double underscore __ to separate nested keys.

Example for setting the browser:
TEST_TestConfiguration__Browser__DefaultBrowser=Firefox

Example for enabling headless mode:
TEST_TestConfiguration__Browser__Headless=true

### Run Settings File
The test.runsettings file is used to configure the NUnit test runner. It controls test execution parameters like parallelization. You can also specify the browser for a test run here.

Example:
<Parameter name="Browser" value="Firefox" />

The browser setting in .runsettings takes precedence over appsettings.json. Although other browsers are supported and tests will run, I have not debugged test reports for browsers other than Chrome.

## Test Plans
Detailed test plans for each test suite are located in the following directory:
TechnicalAssignment/TechnicalAssignment/TestPlans/

## Test Results
After a test run is complete, all results are stored in the TestResults directory at the root of the solution. A new timestamped sub-directory is created for each run. This includes:
- An HTML report (Report.html)
- Log files
- Screenshots for failed tests where relevant

Example test results with screenshots, log and Results.html report in the /Results folder

## Typical Result

### 1 Test Worker
Test summary: total: 152, failed: 33, succeeded: 108, skipped: 11, duration: 853.9s
Build failed with 33 error(s) in 855.0s

### 2 Test Workers
Test summary: total: 152, failed: 33, succeeded: 108, skipped: 11, duration: 467.3s
Build failed with 33 error(s) in 469.2s

### 4 Test Workers
Test summary: total: 152, failed: 33, succeeded: 108, skipped: 11, duration: 331.6s
Build failed with 33 error(s) in 332.5s

## Potential Improvements
- Containerisation: Add Docker support to run tests in a consistent, containerised environment, simplifying setup.
- Once dockerised include ZAP for security and JMeter for API load testing
- Cloud Testing Integration: Extend the WebDriverFactory to support cloud-based testing grids like BrowserStack or Sauce Labs for cross-browser testing on a wider range of platforms.
- CI/CD Pipeline: Create a sample CI/CD pipeline configuration (e.g., for GitHub Actions or Azure DevOps) to demonstrate continuous testing.
- Data-Driven Testing: Enhance tests to pull data from external sources like JSON files or an API to easily expand test coverage with different data sets (currently such Data Driven Testing data sits in the Models folder).