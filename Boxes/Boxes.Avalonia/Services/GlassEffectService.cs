using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Boxes.Models;
using System;

namespace Boxes.Avalonia.Services;

public static class GlassEffectService
{
    /// <summary>
    /// Applies an acetate effect to a border control based on the specified box style
    /// </summary>
    /// <param name="border">The border control to apply the effect to</param>
    /// <param name="style">The box style to apply</param>
    public static void ApplyAcetateEffect(Border border, BoxStyle style = BoxStyle.Acetate)
    {
        if (border == null)
        {
            System.Diagnostics.Debug.WriteLine("GlassEffectService: Border is null");
            return;
        }

        try
        {
            // Apply acetate styling based on style
            var borderBrush = GetAcetateBorderBrush(style);
            var backgroundBrush = GetAcetateBackgroundBrush(style);

            border.BorderBrush = borderBrush;
            border.BorderThickness = new Thickness(2, 2, 2, 2);
            border.Background = backgroundBrush;

            // Add a subtle drop shadow for depth
            border.Effect = new DropShadowEffect
            {
                BlurRadius = 8,
                Opacity = 0.3,
                Color = Colors.Black
            };

            System.Diagnostics.Debug.WriteLine($"GlassEffectService: Applied {style} acetate effect successfully");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"GlassEffectService: Error applying acetate effect - {ex.Message}");
        }
    }

    /// <summary>
    /// Gets an acetate-style border brush for the specified style
    /// </summary>
    /// <param name="style">The box style</param>
    /// <returns>A gradient brush for the border</returns>
    public static IBrush GetAcetateBorderBrush(BoxStyle style = BoxStyle.Acetate)
    {
        return style switch
        {
            BoxStyle.Acetate => new LinearGradientBrush
            {
                StartPoint = new RelativePoint(0, 0, RelativeUnit.Relative),
                EndPoint = new RelativePoint(1, 1, RelativeUnit.Relative),
                GradientStops =
                {
                    new GradientStop(Color.FromArgb(8, 192, 224, 240), 0.0),
                    new GradientStop(Color.FromArgb(5, 208, 232, 246), 0.25),
                    new GradientStop(Color.FromArgb(5, 208, 232, 246), 0.75),
                    new GradientStop(Color.FromArgb(3, 224, 240, 255), 1.0)
                }
            },

            BoxStyle.Windows => new LinearGradientBrush
            {
                StartPoint = new RelativePoint(0, 0, RelativeUnit.Relative),
                EndPoint = new RelativePoint(1, 1, RelativeUnit.Relative),
                GradientStops =
                {
                    new GradientStop(Color.FromArgb(128, 0, 120, 212), 0.0),
                    new GradientStop(Color.FromArgb(96, 0, 140, 240), 0.25),
                    new GradientStop(Color.FromArgb(96, 0, 140, 240), 0.75),
                    new GradientStop(Color.FromArgb(64, 0, 160, 255), 1.0)
                }
            },

            BoxStyle.Acrylic => new LinearGradientBrush
            {
                StartPoint = new RelativePoint(0, 0, RelativeUnit.Relative),
                EndPoint = new RelativePoint(1, 1, RelativeUnit.Relative),
                GradientStops =
                {
                    new GradientStop(Color.FromArgb(128, 255, 255, 255), 0.0),
                    new GradientStop(Color.FromArgb(96, 240, 240, 240), 0.25),
                    new GradientStop(Color.FromArgb(96, 240, 240, 240), 0.75),
                    new GradientStop(Color.FromArgb(64, 220, 220, 220), 1.0)
                }
            },

            BoxStyle.Frosted => new LinearGradientBrush
            {
                StartPoint = new RelativePoint(0, 0, RelativeUnit.Relative),
                EndPoint = new RelativePoint(1, 1, RelativeUnit.Relative),
                GradientStops =
                {
                    new GradientStop(Color.FromArgb(128, 200, 200, 200), 0.0),
                    new GradientStop(Color.FromArgb(96, 220, 220, 220), 0.25),
                    new GradientStop(Color.FromArgb(96, 220, 220, 220), 0.75),
                    new GradientStop(Color.FromArgb(64, 240, 240, 240), 1.0)
                }
            },

            BoxStyle.Minimal => new LinearGradientBrush
            {
                StartPoint = new RelativePoint(0, 0, RelativeUnit.Relative),
                EndPoint = new RelativePoint(1, 1, RelativeUnit.Relative),
                GradientStops =
                {
                    new GradientStop(Color.FromArgb(128, 0, 0, 0), 0.0),
                    new GradientStop(Color.FromArgb(96, 50, 50, 50), 0.25),
                    new GradientStop(Color.FromArgb(96, 50, 50, 50), 0.75),
                    new GradientStop(Color.FromArgb(64, 100, 100, 100), 1.0)
                }
            },

            _ => new LinearGradientBrush
            {
                StartPoint = new RelativePoint(0, 0, RelativeUnit.Relative),
                EndPoint = new RelativePoint(1, 1, RelativeUnit.Relative),
                GradientStops =
                {
                    new GradientStop(Color.FromArgb(128, 192, 224, 240), 0.0),
                    new GradientStop(Color.FromArgb(96, 208, 232, 246), 0.25),
                    new GradientStop(Color.FromArgb(96, 208, 232, 246), 0.75),
                    new GradientStop(Color.FromArgb(64, 224, 240, 255), 1.0)
                }
            }
        };
    }

    /// <summary>
    /// Gets an acetate-style background brush for the specified style
    /// </summary>
    /// <param name="style">The box style</param>
    /// <returns>A gradient brush for the background</returns>
    public static IBrush GetAcetateBackgroundBrush(BoxStyle style = BoxStyle.Acetate)
    {
        return style switch
        {
            BoxStyle.Acetate => new LinearGradientBrush
            {
                StartPoint = new RelativePoint(0, 0, RelativeUnit.Relative),
                EndPoint = new RelativePoint(1, 1, RelativeUnit.Relative),
                GradientStops =
                {
                    new GradientStop(Color.FromArgb(5, 255, 255, 255), 0.0),
                    new GradientStop(Color.FromArgb(3, 255, 255, 255), 0.3),
                    new GradientStop(Color.FromArgb(3, 255, 255, 255), 0.7),
                    new GradientStop(Color.FromArgb(1, 255, 255, 255), 1.0)
                }
            },

            BoxStyle.Windows => new LinearGradientBrush
            {
                StartPoint = new RelativePoint(0, 0, RelativeUnit.Relative),
                EndPoint = new RelativePoint(1, 1, RelativeUnit.Relative),
                GradientStops =
                {
                    new GradientStop(Color.FromArgb(255, 232, 244, 253), 0.0),
                    new GradientStop(Color.FromArgb(255, 240, 248, 255), 0.3),
                    new GradientStop(Color.FromArgb(255, 230, 243, 255), 0.7),
                    new GradientStop(Color.FromArgb(255, 209, 233, 246), 1.0)
                }
            },

            BoxStyle.Acrylic => new LinearGradientBrush
            {
                StartPoint = new RelativePoint(0, 0, RelativeUnit.Relative),
                EndPoint = new RelativePoint(1, 1, RelativeUnit.Relative),
                GradientStops =
                {
                    new GradientStop(Color.FromArgb(255, 248, 252, 253), 0.0),
                    new GradientStop(Color.FromArgb(255, 250, 254, 255), 0.3),
                    new GradientStop(Color.FromArgb(255, 246, 251, 255), 0.7),
                    new GradientStop(Color.FromArgb(255, 241, 247, 250), 1.0)
                }
            },

            BoxStyle.Frosted => new LinearGradientBrush
            {
                StartPoint = new RelativePoint(0, 0, RelativeUnit.Relative),
                EndPoint = new RelativePoint(1, 1, RelativeUnit.Relative),
                GradientStops =
                {
                    new GradientStop(Color.FromArgb(255, 234, 244, 253), 0.0),
                    new GradientStop(Color.FromArgb(255, 242, 250, 255), 0.3),
                    new GradientStop(Color.FromArgb(255, 238, 247, 255), 0.7),
                    new GradientStop(Color.FromArgb(255, 225, 241, 248), 1.0)
                }
            },

            BoxStyle.Minimal => new LinearGradientBrush
            {
                StartPoint = new RelativePoint(0, 0, RelativeUnit.Relative),
                EndPoint = new RelativePoint(1, 1, RelativeUnit.Relative),
                GradientStops =
                {
                    new GradientStop(Color.FromArgb(255, 248, 248, 248), 0.0),
                    new GradientStop(Color.FromArgb(255, 250, 250, 250), 0.3),
                    new GradientStop(Color.FromArgb(255, 246, 246, 246), 0.7),
                    new GradientStop(Color.FromArgb(255, 241, 241, 241), 1.0)
                }
            },

            _ => new LinearGradientBrush
            {
                StartPoint = new RelativePoint(0, 0, RelativeUnit.Relative),
                EndPoint = new RelativePoint(1, 1, RelativeUnit.Relative),
                GradientStops =
                {
                    new GradientStop(Color.FromArgb(255, 232, 244, 253), 0.0),
                    new GradientStop(Color.FromArgb(255, 240, 248, 255), 0.3),
                    new GradientStop(Color.FromArgb(255, 230, 243, 255), 0.7),
                    new GradientStop(Color.FromArgb(255, 209, 233, 246), 1.0)
                }
            }
        };
    }
}