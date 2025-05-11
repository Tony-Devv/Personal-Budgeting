using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Media;
using Avalonia.Styling;

namespace PersonalBudgeting
{
    public static class ThemeManager
    {
        // Default colors
        private static readonly Color DefaultNavigationColor = Color.Parse("#3C3F41");
        private static readonly Color DefaultContentColor = Color.Parse("#2E2E2E");
        
        // Theme presets
        public static readonly Dictionary<string, (Color Navigation, Color Content)> ThemePresets = new()
        {
            { "Dark", (Color.Parse("#3C3F41"), Color.Parse("#2E2E2E")) },
            { "Light", (Color.Parse("#F0F0F0"), Color.Parse("#FFFFFF")) },
            { "Blue", (Color.Parse("#1E3A8A"), Color.Parse("#1E293B")) },
            { "Green", (Color.Parse("#1B4D3E"), Color.Parse("#0F2922")) },
            { "Purple", (Color.Parse("#4A1D96"), Color.Parse("#2E1065")) }
        };

        /// <summary>
        /// Apply theme colors to the application
        /// </summary>
        /// <param name="navigationColor">Color for the navigation sidebar</param>
        /// <param name="contentColor">Color for the main content area</param>
        public static void ApplyTheme(Color navigationColor, Color contentColor)
        {
            try
            {
                // Update the resources
                if (Application.Current != null)
                {
                    Application.Current.Resources["NavigationColor"] = navigationColor;
                    Application.Current.Resources["ContentColor"] = contentColor;
                    
                    Application.Current.Resources["NavigationBrush"] = new SolidColorBrush(navigationColor);
                    Application.Current.Resources["ContentBrush"] = new SolidColorBrush(contentColor);
                    
                }
                else
                {
                }
            }
            catch (Exception ex)
            {
            }
        }

        /// <summary>
        /// Apply a preset theme by name
        /// </summary>
        /// <param name="themeName">Name of the preset theme</param>
        public static void ApplyPresetTheme(string themeName)
        {
            if (ThemePresets.TryGetValue(themeName, out var colors))
            {
                ApplyTheme(colors.Navigation, colors.Content);
            }
            else
            {
                ApplyTheme(DefaultNavigationColor, DefaultContentColor);
            }
        }
        
        /// <summary>
        /// Reset theme to default
        /// </summary>
        public static void ResetToDefault()
        {
            ApplyTheme(DefaultNavigationColor, DefaultContentColor);
        }
    }
} 
