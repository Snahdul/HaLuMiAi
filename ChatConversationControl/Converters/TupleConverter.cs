using System.Globalization;
using System.Windows.Data;

namespace ChatConversationControl.Converters;

public class TupleConverter : IMultiValueConverter
{
    public object? Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values.Length == 2 && values[0] is string key && values[1] is string value)
        {
            return new Tuple<string, string>(key, value);
        }
        return null;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        if (value is Tuple<string, string> tuple)
        {
            return new object[] { tuple.Item1, tuple.Item2 };
        }
        return new object[2];
    }
}