using System.Globalization;
using System.Windows.Data;
using Wpf.Ui.Appearance;

namespace WPFUiDesktopApp.Converters;

/// <summary>
/// Converts an enum value to a boolean and vice versa.
/// </summary>
internal class EnumToBooleanConverter : IValueConverter
{
    /// <summary>
    /// Converts an enum value to a boolean.
    /// </summary>
    /// <param name="value">The enum value to convert.</param>
    /// <param name="targetType">The target type (not used).</param>
    /// <param name="parameter">The enum name as a string.</param>
    /// <param name="culture">The culture info (not used).</param>
    /// <returns>True if the enum value matches the parameter; otherwise, false.</returns>
    /// <exception cref="ArgumentException">Thrown when the parameter is not a string or the value is not a defined enum value.</exception>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (parameter is not String enumString)
        {
            throw new ArgumentException("ExceptionEnumToBooleanConverterParameterMustBeAnEnumName");
        }

        if (!Enum.IsDefined(typeof(ApplicationTheme), value))
        {
            throw new ArgumentException("ExceptionEnumToBooleanConverterValueMustBeAnEnum");
        }

        var enumValue = Enum.Parse(typeof(ApplicationTheme), enumString);

        return enumValue.Equals(value);
    }

    /// <summary>
    /// Converts a boolean back to an enum value.
    /// </summary>
    /// <param name="value">The boolean value to convert (not used).</param>
    /// <param name="targetType">The target type (not used).</param>
    /// <param name="parameter">The enum name as a string.</param>
    /// <param name="culture">The culture info (not used).</param>
    /// <returns>The enum value corresponding to the parameter.</returns>
    /// <exception cref="ArgumentException">Thrown when the parameter is not a string.</exception>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (parameter is not String enumString)
        {
            throw new ArgumentException("ExceptionEnumToBooleanConverterParameterMustBeAnEnumName");
        }

        return Enum.Parse(typeof(ApplicationTheme), enumString);
    }
}
