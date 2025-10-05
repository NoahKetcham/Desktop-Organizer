using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Boxes.Models;

namespace Boxes.Avalonia.Services;

public static class GlassEffectService
{
    /// <summary>
    /// Applies a glass effect to a border control based on the specified box style
    /// </summary>
    /// <param name="border">The border control to apply the effect to</param>
    /// <param name="style">The box style to apply</param>
    public static void ApplyGlassEffect(Border border, BoxStyle style = BoxStyle.Windows)
    {
        if (border == null)
        {
            System.Diagnostics.Debug.WriteLine("GlassEffectService: Border is null");
            return;
        }

        try
        {
            // Apply background and border based on style
            var backgroundColor = GetBackgroundColor(style);
            var borderColor = GetBorderColor(style);

            border.Background = new SolidColorBrush(backgroundColor);
            border.BorderBrush = new SolidColorBrush(borderColor);
            border.BorderThickness = new Thickness(1, 1, 1, 1);

            // Add a subtle drop shadow for depth
            border.Effect = new DropShadowEffect
            {
                BlurRadius = 10,
                Opacity = 0.3,
                Color = Colors.Black
            };

            System.Diagnostics.Debug.WriteLine($"GlassEffectService: Applied {style} glass effect successfully");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"GlassEffectService: Error applying glass effect - {ex.Message}");
        }
    }

    /// <summary>
    /// Creates an ExperimentalAcrylicMaterial for the specified box style
    /// </summary>
    /// <param name="style">The box style to create material for</param>
    /// <returns>An ExperimentalAcrylicMaterial configured for the style</returns>
    public static ExperimentalAcrylicMaterial CreateAcrylicMaterial(BoxStyle style = BoxStyle.Windows)
    {
        return style switch
        {
            BoxStyle.Windows => new ExperimentalAcrylicMaterial
            {
                BackgroundSource = AcrylicBackgroundSource.Digger,
                TintColor = Color.FromArgb(255, 0, 120, 212), // Windows blue
                TintOpacity = 0.15,
                MaterialOpacity = 0.8
            },

            BoxStyle.Acetate => new ExperimentalAcrylicMaterial
            {
                BackgroundSource = AcrylicBackgroundSource.Digger,
                TintColor = Color.FromArgb(255, 50, 50, 50), // Dark tint
                TintOpacity = 0.2,
                MaterialOpacity = 0.7
            },

            BoxStyle.Acrylic => new ExperimentalAcrylicMaterial
            {
                BackgroundSource = AcrylicBackgroundSource.Digger,
                TintColor = Color.FromArgb(255, 255, 255, 255), // White tint
                TintOpacity = 0.1,
                MaterialOpacity = 0.9
            },

            BoxStyle.Frosted => new ExperimentalAcrylicMaterial
            {
                BackgroundSource = AcrylicBackgroundSource.Digger,
                TintColor = Color.FromArgb(255, 200, 200, 200), // Light gray
                TintOpacity = 0.25,
                MaterialOpacity = 0.6
            },

            BoxStyle.Minimal => new ExperimentalAcrylicMaterial
            {
                BackgroundSource = AcrylicBackgroundSource.Digger,
                TintColor = Color.FromArgb(255, 0, 0, 0), // Black tint
                TintOpacity = 0.05,
                MaterialOpacity = 0.95
            },

            _ => new ExperimentalAcrylicMaterial
            {
                BackgroundSource = AcrylicBackgroundSource.Digger,
                TintColor = Color.FromArgb(255, 0, 120, 212), // Default Windows blue
                TintOpacity = 0.15,
                MaterialOpacity = 0.8
            }
        };
    }

    /// <summary>
    /// Gets the recommended border color for a box style
    /// </summary>
    /// <param name="style">The box style</param>
    /// <returns>A color for the border</returns>
    public static Color GetBorderColor(BoxStyle style = BoxStyle.Windows)
    {
        return style switch
        {
            BoxStyle.Windows => Color.FromArgb(255, 0, 120, 212),
            BoxStyle.Acetate => Color.FromArgb(255, 100, 100, 100),
            BoxStyle.Acrylic => Color.FromArgb(255, 200, 200, 200),
            BoxStyle.Frosted => Color.FromArgb(255, 150, 150, 150),
            BoxStyle.Minimal => Color.FromArgb(255, 50, 50, 50),
            _ => Color.FromArgb(255, 0, 120, 212)
        };
    }

    /// <summary>
    /// Gets the recommended background color for a box style
    /// </summary>
    /// <param name="style">The box style</param>
    /// <returns>A color for the background</returns>
    public static Color GetBackgroundColor(BoxStyle style = BoxStyle.Windows)
    {
        return style switch
        {
            BoxStyle.Windows => Color.FromArgb(200, 0, 120, 212),
            BoxStyle.Acetate => Color.FromArgb(180, 50, 50, 50),
            BoxStyle.Acrylic => Color.FromArgb(220, 255, 255, 255),
            BoxStyle.Frosted => Color.FromArgb(190, 200, 200, 200),
            BoxStyle.Minimal => Color.FromArgb(240, 0, 0, 0),
            _ => Color.FromArgb(200, 0, 120, 212)
        };
    }

    /// <summary>
    /// Applies glass effect to an ExperimentalAcrylicBorder
    /// </summary>
    /// <param name="acrylicBorder">The acrylic border control</param>
    /// <param name="style">The box style to apply</param>
    public static void ApplyAcrylicEffect(ExperimentalAcrylicBorder acrylicBorder, BoxStyle style = BoxStyle.Windows)
    {
        if (acrylicBorder == null)
        {
            System.Diagnostics.Debug.WriteLine("GlassEffectService: AcrylicBorder is null");
            return;
        }

        try
        {
            acrylicBorder.Material = CreateAcrylicMaterial(style);
            System.Diagnostics.Debug.WriteLine($"GlassEffectService: Applied {style} acrylic effect successfully");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"GlassEffectService: Error applying acrylic effect - {ex.Message}");
        }
    }
}
