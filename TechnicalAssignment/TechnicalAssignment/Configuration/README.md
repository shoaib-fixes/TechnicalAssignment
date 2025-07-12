# Test Framework Architecture: Dependency Injection Model

## Overview

The test framework uses a modern Dependency Injection (DI) architecture, powered by `Microsoft.Extensions.DependencyInjection`. This design moves away from static singletons to a more flexible, scalable, and maintainable model where dependencies are provided to classes instead of being created by them.

## Architecture

### Dependency Injection Flow
1. **Service Registration (`BaseTest.OneTimeSetUp`)**: Before any tests run, a service container is configured.
   - Core services like logging (`ILogger`), configuration (`ConfigurationManager`, `TestConfiguration`), and the `WebDriverFactory` are registered as services.
   - The `IWebDriver` is registered with a **transient lifetime**, meaning a brand-new, isolated browser instance is created for every single test case.

2. **Test Execution (`BaseTest.SetUp`)**: Before each test starts:
   - The test class requests the necessary services from the DI container.
   - It receives a fresh `IWebDriver` instance, the `TestConfiguration` object, and a `Logger`.

3. **Test Teardown (`BaseTest.TearDown`)**: After each test completes:
   - The `IWebDriver` instance for that specific test is automatically torn down (`Quit()` and `Dispose()`), ensuring perfect test isolation and preventing resource leaks.

### Configuration Structure
The JSON configuration structure for browser and timeout settings remains the same.

```json
{
  "TestConfiguration": {
    "Browser": {
      "DefaultBrowser": "Chrome",
      "Headless": false,
      "WindowSize": {
        "Width": 1920,
        "Height": 1080
      }
    },
    "Timeouts": {
        "DefaultTimeoutSeconds": 30
    }
  }
}
```

## Environment Variable Overrides

The ability to override configuration using environment variables remains a key feature and is unchanged.

```bash
# Override browser settings
TEST_TestConfiguration__Browser__DefaultBrowser=Firefox
TEST_TestConfiguration__Browser__Headless=true

# Override timeouts
TEST_TestConfiguration__Timeouts__DefaultTimeoutSeconds=45
```

## Usage Examples

### Basic Test Setup
The test class inherits from `BaseTest`, and its dependencies (`Driver`, `TestConfig`, `Logger`) are automatically populated before each test.

```csharp
[TestFixture]
public class MyTest : BaseTest
{
    [Test]
    public void MySimpleTest()
    {
        // Driver is a fresh instance for this test.
        // TestConfig is the strongly-typed configuration object.
        Driver.Navigate().GoToUrl(TestConfig.BaseUrl);
        
        // ... rest of the test logic
    }
}
```

## Benefits of the New Architecture

1.  **Improved Test Isolation**: Creating a new driver per test eliminates the risk of state leaking between tests, making them more reliable.
2.  **Enhanced Scalability**: The architecture now fully supports test-level parallelism, allowing for much faster execution times on multi-core machines.
3.  **Better Maintainability**: Dependencies are explicit (in constructors) rather than hidden (in static calls), making the code easier to understand, refactor, and test.
4.  **Flexibility**: It's now trivial to substitute dependencies (e.g., mock `IWebDriver` for unit tests) without changing the core code.

## Migration Notes (From Static to DI)

### Breaking Changes
- The static `ConfigurationManager.Instance` and `WebDriverFactory.CreateDriver()` have been removed.
- `BaseTest` no longer has a `Config` property; it has been replaced by `TestConfig`.
- The driver lifecycle has changed from **per-fixture** (`OneTimeSetUp`) to **per-test** (`SetUp`/`TearDown`). Any logic that relied on the driver persisting between tests in the same class will need to be refactored.

## Testing

The testing methodology using environment variables remains the same:

```bash
# Test headless mode
TEST_TestConfiguration__Browser__Headless=true dotnet test

# Test different browser
TEST_TestConfiguration__Browser__DefaultBrowser=Firefox dotnet test
```

## Accessibility Testing Configuration
The framework includes built-in accessibility testing powered by `axe-core`. These settings can be configured in `appsettings.json` under the `Accessibility` section.

### Accessibility Configuration Structure

```json
{
  "TestConfiguration": {
    "Accessibility": {
      "EnableAccessibilityTesting": true,
      "AccessibilityTags": [ "wcag2aa" ],
      "CaptureScreenshotsOnViolations": true,
      "HighlightViolatingElements": true,
      "FailOnViolations": false
    }
  }
}
```

-   `EnableAccessibilityTesting`: Set to `false` to disable all accessibility tests.
-   `AccessibilityTags`: An array of WCAG tags to validate against (e.g., `wcag2a`, `wcag2aa`, `wcag21aa`).
-   `CaptureScreenshotsOnViolations`: Set to `true` to automatically capture screenshots of elements that violate accessibility rules.
-   `HighlightViolatingElements`: Set to `true` to draw a red border around violating elements in the page screenshot.
-   `FailOnViolations`: Set to `true` to fail the test if any accessibility violations are found. If `false`, violations will be logged without failing the test.

### Overriding Accessibility Settings with Environment Variables

You can override these settings from the command line for specific test runs.

```bash
# Disable accessibility tests entirely
TEST_TestConfiguration__Accessibility__EnableAccessibilityTesting=false

# Change the WCAG level to WCAG 2.1 AAA
TEST_TestConfiguration__Accessibility__AccessibilityTags:0=wcag21aaa

# Run tests but do not fail on violations
TEST_TestConfiguration__Accessibility__FailOnViolations=false
``` 

## Performance Testing Configuration

The framework includes Lighthouse performance testing to measure web page performance metrics. This feature requires additional setup and configuration.

### Prerequisites

Before running performance tests, ensure the following are installed:

1. **Node.js**: Download and install from [nodejs.org](https://nodejs.org/)
2. **Lighthouse**: Install globally using npm
   ```bash
   npm install -g lighthouse@8.6.0
   ```
   
   **Note**: Use Lighthouse version 8.6.0 for compatibility with the `lighthouse.net` library. Newer versions may cause compatibility issues.

3. **Google Chrome**: Required for performance testing (tests will only run on Chrome)

### Performance Configuration Structure

The performance testing uses the following configuration in `appsettings.json`:

```json
{
  "TestConfiguration": {
    "Browser": {
      "DefaultBrowser": "Chrome",
      "Headless": false,
      "WindowSize": {
        "Width": 1920,
        "Height": 1080
      },
      "RemoteDebuggingPort": 9222
    }
  }
}
```

**Note**: The `RemoteDebuggingPort` setting is included for configuration completeness, but the framework automatically uses dynamic port assignment (port 0) to avoid conflicts during parallel test execution. Chrome will automatically find available ports for each test instance.

### Behavior

- **Browser Compatibility**: Performance tests only run on Chrome (automatically skipped for other browsers)
- **Parallel Execution**: Performance tests run sequentially to avoid resource conflicts and ensure accurate measurements
- **Screenshots**: Automatic screenshot capture on test failures
- **Logging**: Performance scores are logged for monitoring and tracking purposes

### Overriding Performance Settings with Environment Variables

```bash
# Change the remote debugging port
TEST_TestConfiguration__Browser__RemoteDebuggingPort=9223

# Run performance tests in headless mode
TEST_TestConfiguration__Browser__Headless=true
```

### Troubleshooting Performance Tests

If performance tests fail with setup errors:

1. **Verify Node.js installation**: `node --version`
2. **Verify Lighthouse installation**: `lighthouse --version`
3. **Check Lighthouse version**: Must be 8.6.0 for compatibility
4. **Ensure Chrome is available**: Performance tests require Chrome browser
5. **Check port conflicts**: Ensure the `RemoteDebuggingPort` is not in use by other applications

### Performance Test Thresholds

Performance tests use configurable thresholds (currently set to 80% in the code). Tests will fail if the Lighthouse performance score falls below the threshold, helping maintain performance standards over time. 