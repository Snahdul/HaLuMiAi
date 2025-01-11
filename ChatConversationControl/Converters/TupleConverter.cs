using System.Globalization;
using System.Windows.Data;

namespace ChatConversationControl.Converters;

/// <summary>
/// Converts two string values into a Tuple and vice versa.
/// </summary>
public class TupleConverter : IMultiValueConverter
{
    /// <summary>
    /// Converts two string values into a Tuple.
    /// </summary>
    /// <param name="values">The array of values to convert. Expects two string values.</param>
    /// <param name="targetType">The type of the binding target property.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the converter.</param>
    /// <returns>A Tuple containing the two string values, or null if the input is invalid.</returns>
    public object? Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values.Length == 2 && values[0] is string key && values[1] is string value)
        {
            return new Tuple<string, string>(key, value);
        }
        return null;
    }

    /// <summary>
    /// Converts a Tuple back into an array of two string values.
    /// </summary>
    /// <param name="value">The Tuple to convert back.</param>
    /// <param name="targetTypes">The array of types to convert to.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the converter.</param>
    /// <returns>An array containing the two string values from the Tuple, or an empty array if the input is invalid.</returns>
    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        if (value is Tuple<string, string> tuple)
        {
            return new object[] { tuple.Item1, tuple.Item2 };
        }
        return new object[2];
    }
}