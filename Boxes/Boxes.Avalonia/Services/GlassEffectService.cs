using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace Boxes.Avalonia.Services;

public class GlassEffectService
{
    public static void ApplyGlassEffect(Border border)
    {
        if (border == null)
        {
            System.Diagnostics.Debug.WriteLine("GlassEffectService: Border is null");
            return;
        }

        try
        {
            // In Avalonia, we use the TransparencyLevelHint on the window
            // and apply a semi-transparent background with blur effect
            border.Background = new SolidColorBrush(Color.FromArgb(204, 255, 255, 255)); // 80% white
            border.BorderBrush = new SolidColorBrush(Color.FromArgb(128, 128, 128, 128)); // 50% gray
            border.BorderThickness = new Thickness(1, 1, 1, 1);

            // Add a subtle drop shadow for depth
            border.Effect = new DropShadowEffect
            {
                BlurRadius = 10,
                Opacity = 0.3,
                Color = Colors.Black
            };

            System.Diagnostics.Debug.WriteLine("GlassEffectService: Applied glass effect successfully");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"GlassEffectService: Error applying glass effect - {ex.Message}");
        }
    }
}
