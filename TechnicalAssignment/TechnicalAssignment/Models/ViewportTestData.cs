using System.Collections.Generic;
using System.Linq;

namespace TechnicalAssignment.Models;

/// <summary>
/// Contains predefined viewport sizes for responsive testing
/// </summary>
public static class ViewportTestData
{
    /// <summary>
    /// Standard mobile viewport sizes
    /// </summary>
    public static readonly ViewportSize[] MobileViewports = new[]
    {
        new ViewportSize(375, 667, "iPhone SE"),
        new ViewportSize(414, 896, "iPhone 11 Pro")
    };

    /// <summary>
    /// Standard tablet viewport sizes
    /// </summary>
    public static readonly ViewportSize[] TabletViewports = new[]
    {
        new ViewportSize(768, 1024, "iPad Portrait"),
        new ViewportSize(1024, 768, "iPad Landscape")
    };

    /// <summary>
    /// Standard desktop viewport sizes
    /// </summary>
    public static readonly ViewportSize[] DesktopViewports = new[]
    {
        new ViewportSize(1920, 1080, "Full HD"),
        new ViewportSize(1366, 768, "Standard Laptop")
    };

    /// <summary>
    /// Gets all viewport sizes for comprehensive responsive testing
    /// </summary>
    public static ViewportSize[] GetAllViewportSizes()
    {
        return MobileViewports
            .Concat(TabletViewports)
            .Concat(DesktopViewports)
            .ToArray();
    }

    /// <summary>
    /// Gets mobile viewport sizes only
    /// </summary>
    public static ViewportSize[] GetMobileViewportSizes()
    {
        return MobileViewports;
    }

    /// <summary>
    /// Gets tablet viewport sizes only
    /// </summary>
    public static ViewportSize[] GetTabletViewportSizes()
    {
        return TabletViewports;
    }

    /// <summary>
    /// Gets desktop viewport sizes only
    /// </summary>
    public static ViewportSize[] GetDesktopViewportSizes()
    {
        return DesktopViewports;
    }

    /// <summary>
    /// Converts viewport sizes to NUnit test case format
    /// </summary>
    /// <param name="viewportSizes">Viewport sizes to convert</param>
    /// <returns>Array of test case objects</returns>
    public static object[] ToTestCases(ViewportSize[] viewportSizes)
    {
        return viewportSizes.Select(v => new object[] { v.Width, v.Height, v.Name }).ToArray();
    }

    /// <summary>
    /// Gets all viewport sizes as NUnit test cases
    /// </summary>
    public static object[] GetAllViewportTestCases()
    {
        return ToTestCases(GetAllViewportSizes());
    }

    /// <summary>
    /// Gets mobile viewport sizes as NUnit test cases
    /// </summary>
    public static object[] GetMobileViewportTestCases()
    {
        return ToTestCases(GetMobileViewportSizes());
    }

    /// <summary>
    /// Gets tablet viewport sizes as NUnit test cases
    /// </summary>
    public static object[] GetTabletViewportTestCases()
    {
        return ToTestCases(GetTabletViewportSizes());
    }

    /// <summary>
    /// Gets desktop viewport sizes as NUnit test cases
    /// </summary>
    public static object[] GetDesktopViewportTestCases()
    {
        return ToTestCases(GetDesktopViewportSizes());
    }
} 