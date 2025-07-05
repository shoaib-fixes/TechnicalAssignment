# Browser Management & Configuration Integration

## Overview

The browser management system has been fully integrated with the ConfigurationManager to provide centralized, flexible, and environment-aware browser configuration for the test framework.

## Architecture

### Configuration Flow
1. **BaseTest.GetTargetBrowser()** → Determines browser selection with priority:
   - `test.runsettings` (TestContext.Parameters)
   - `Config.Browser.DefaultBrowser` (appsettings.json)
   - Fallback: "Chrome"

2. **WebDriverFactory.CreateDriver()** → Creates browser instance with configuration:
   - Loads ConfigurationManager.Instance
   - Applies browser-specific options (headless, stability arguments)
   - Sets timeouts and window size from configuration

3. **BrowserHelper** → Provides configuration-aware utilities:
   - Uses config defaults for viewport sizes
   - Applies config timeouts for JavaScript waits
   - Provides configuration-based utility methods

## Configuration Structure

### Browser Settings
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
    }
  }
}
```

### Timeout Settings
```json
{
  "TestConfiguration": {
    "Timeouts": {
      "DefaultTimeoutSeconds": 30,
      "PollingIntervalMilliseconds": 500
    }
  }
}
```

## Browser-Specific Features

### Chrome Options
- Headless mode support
- Stability arguments: `--no-sandbox`, `--disable-dev-shm-usage`, `--disable-gpu`
- Extension and notification disabling

### Firefox Options
- Headless mode support
- Basic stability configuration

### Edge Options
- Headless mode support
- Chrome-like stability arguments

## Environment Variable Overrides

All configuration can be overridden using environment variables with the `TEST_` prefix:

```bash
# Override browser settings
TEST_TestConfiguration__Browser__DefaultBrowser=Firefox
TEST_TestConfiguration__Browser__Headless=true
TEST_TestConfiguration__Browser__WindowSize__Width=1366
TEST_TestConfiguration__Browser__WindowSize__Height=768

# Override timeouts
TEST_TestConfiguration__Timeouts__DefaultTimeoutSeconds=45
```

## Usage Examples

### Basic Test Setup
```csharp
[Test]
public void MyTest()
{
    // Browser is automatically created using configuration
    // Window size, timeouts, and options are applied automatically
    Driver.Navigate().GoToUrl(Config.BaseUrl);
}
```

### Using BrowserHelper with Configuration
```csharp
// Reset to configured default size
BrowserHelper.ResetToDefaultSize(Driver);

// Set specific size with config fallback
BrowserHelper.SetViewportSize(Driver, width: 1024); // Height from config

// Wait for JavaScript with configured timeout
BrowserHelper.WaitForJavaScriptToComplete(Driver);

// Configure timeouts from settings
BrowserHelper.ConfigureTimeouts(Driver);
```

### Headless Mode Testing
```csharp
// In appsettings.json or environment variable
"Headless": true

// Or via environment variable
TEST_TestConfiguration__Browser__Headless=true
```

## Integration Points

### WebDriverFactory
- **Before**: Hardcoded 3-second implicit wait, no configuration
- **After**: Uses only explicit waits, applies window size, headless mode, page load timeout

### BrowserHelper
- **Before**: Independent utility methods with hardcoded defaults
- **After**: Configuration-aware methods with smart defaults

### BaseTest
- **Before**: Only checked test.runsettings
- **After**: Hierarchical browser selection with configuration fallback

## Benefits

1. **Centralized Configuration**: All browser settings in one place
2. **Environment Flexibility**: Easy overrides for different environments
3. **Consistent Behavior**: Same configuration used across all components
4. **Headless Support**: Built-in CI/CD friendly headless mode
5. **Proper Timeouts**: Configuration-driven timeout management
6. **Smart Defaults**: Fallback values for all settings

## Migration Notes

### Breaking Changes
- `BrowserHelper.SetViewportSize()` now accepts optional parameters
- WebDriverFactory now requires Configuration namespace

### New Features
- Headless mode support
- Configuration-based window sizing
- Environment variable overrides
- Integrated timeout management

## Testing

The integration can be tested using environment variables:

```bash
# Test headless mode
TEST_TestConfiguration__Browser__Headless=true dotnet test

# Test different browser
TEST_TestConfiguration__Browser__DefaultBrowser=Firefox dotnet test

# Test custom window size
TEST_TestConfiguration__Browser__WindowSize__Width=1366 dotnet test
``` 