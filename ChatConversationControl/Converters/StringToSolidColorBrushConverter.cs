using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace ChatConversationControl.Converters;

/// <summary>
/// Converts a string representing a color name to a SolidColorBrush and vice versa.
/// </summary>
public class StringToSolidColorBrushConverter : IValueConverter
{
    /// <summary>
    /// Converts a string color name to a SolidColorBrush.
    /// </summary>
    /// <param name="value">The string color name to convert.</param>
    /// <param name="targetType">The type of the binding target property.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the converter.</param>
    /// <returns>A SolidColorBrush if the conversion is successful; otherwise, a SolidColorBrush with Transparent color.</returns>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string colorName)
        {
            try
            {
                var color = (Color)ColorConverter.ConvertFromString(colorName);
                return new SolidColorBrush(color);
            }
            catch (FormatException)
            {
                // Handle the case where the color name is invalid
                return new SolidColorBrush(Colors.Transparent);
            }
        }
        return new SolidColorBrush(Colors.Transparent);
    }

    /// <summary>
    /// Converts a SolidColorBrush back to a string color name.
    /// </summary>
    /// <param name="value">The SolidColorBrush to convert.</param>
    /// <param name="targetType">The type of the binding target property.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the converter.</param>
    /// <returns>A string representing the color name if the conversion is successful; otherwise, an empty string.</returns>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is SolidColorBrush brush)
        {
            return brush.Color.ToString();
        }
        return string.Empty;
    }
}
