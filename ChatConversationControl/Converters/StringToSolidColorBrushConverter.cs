using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace ChatConversationControl.Converters;

public class StringToSolidColorBrushConverter : IValueConverter
{
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

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is SolidColorBrush brush)
        {
            return brush.Color.ToString();
        }
        return string.Empty;
    }
}