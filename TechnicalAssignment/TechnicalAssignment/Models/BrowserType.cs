namespace TechnicalAssignment.Models;

/// <summary>
/// Supported browser types for test execution
/// </summary>
public enum BrowserType
{
    /// <summary>
    /// Google Chrome browser
    /// </summary>
    Chrome,
    
    /// <summary>
    /// Mozilla Firefox browser
    /// </summary>
    Firefox,
    
    /// <summary>
    /// Microsoft Edge browser
    /// </summary>
    Edge
}

/// <summary>
/// Extension methods for BrowserType enum
/// </summary>
public static class BrowserTypeExtensions
{
    /// <summary>
    /// Converts BrowserType enum to string representation
    /// </summary>
    /// <param name="browserType">The browser type</param>
    /// <returns>String representation of the browser type</returns>
    public static string ToStringValue(this BrowserType browserType)
    {
        return browserType switch
        {
            BrowserType.Chrome => "Chrome",
            BrowserType.Firefox => "Firefox",
            BrowserType.Edge => "Edge",
            _ => throw new ArgumentOutOfRangeException(nameof(browserType), browserType, "Unsupported browser type")
        };
    }
    
    /// <summary>
    /// Converts string to BrowserType enum
    /// </summary>
    /// <param name="browserString">String representation of browser type</param>
    /// <returns>BrowserType enum value</returns>
    /// <exception cref="ArgumentException">Thrown when browser string is not supported</exception>
    public static BrowserType ToBrowserType(string browserString)
    {
        return browserString?.ToLowerInvariant() switch
        {
            "chrome" => BrowserType.Chrome,
            "firefox" => BrowserType.Firefox,
            "edge" => BrowserType.Edge,
            _ => throw new ArgumentException($"Unsupported browser type: {browserString}", nameof(browserString))
        };
    }
    
    /// <summary>
    /// Tries to convert string to BrowserType enum
    /// </summary>
    /// <param name="browserString">String representation of browser type</param>
    /// <param name="browserType">The parsed browser type</param>
    /// <returns>True if parsing was successful, false otherwise</returns>
    public static bool TryParseBrowserType(string? browserString, out BrowserType browserType)
    {
        try
        {
            browserType = ToBrowserType(browserString!);
            return true;
        }
        catch
        {
            browserType = BrowserType.Chrome; // Default fallback
            return false;
        }
    }
} 