using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace LocalFit.Converters;

public class NumericUpDownConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        // Convert from ViewModel (decimal) to NumericUpDown (decimal?)
        // This conversion is straightforward - decimal can be implicitly converted to decimal?
        return value;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        // Convert from NumericUpDown (decimal?) to ViewModel (decimal)
        // If value is null (empty text), return the Minimum value from parameter or 0
        if (value == null || value == DBNull.Value)
        {
            // Try to get Minimum from parameter
            if (parameter != null)
            {
                if (decimal.TryParse(parameter.ToString(), NumberStyles.Any, culture, out decimal minValue))
                {
                    return minValue;
                }
            }
            // Default to 0 if no parameter or parsing fails
            return 0m;
        }

        // If value is a decimal?, extract the decimal value
        if (value is decimal decimalValue)
        {
            return decimalValue;
        }

        // Try to convert to decimal
        if (decimal.TryParse(value.ToString(), NumberStyles.Any, culture, out decimal parsedValue))
        {
            return parsedValue;
        }

        // Fallback to 0 if conversion fails
        return 0m;
    }
}
