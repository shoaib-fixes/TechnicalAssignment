namespace TechnicalAssignment.Models;

/// <summary>
/// Represents a viewport size configuration for responsive testing
/// </summary>
public class ViewportSize
{
    /// <summary>
    /// Width of the viewport in pixels
    /// </summary>
    public int Width { get; set; }

    /// <summary>
    /// Height of the viewport in pixels
    /// </summary>
    public int Height { get; set; }

    /// <summary>
    /// Descriptive name for the viewport size
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Creates a new ViewportSize instance
    /// </summary>
    /// <param name="width">Width in pixels</param>
    /// <param name="height">Height in pixels</param>
    /// <param name="name">Descriptive name</param>
    public ViewportSize(int width, int height, string name)
    {
        Width = width;
        Height = height;
        Name = name;
    }

    /// <summary>
    /// Parameterless constructor for object initialization
    /// </summary>
    public ViewportSize() { }

    /// <summary>
    /// Returns a string representation of the viewport size
    /// </summary>
    public override string ToString()
    {
        return $"{Name} ({Width}x{Height})";
    }
} 