using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace JobRunner.Avalonia.Converters
{
    /// <summary>
    /// Конвертер для строковых значений
    /// </summary>
    public class NullOrEmptyConverter : IValueConverter
    {
        public static NullOrEmptyConverter Instance { get; } = new();

        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            var trueValue = "нет";
            var falseValue = "есть";

            if (parameter is string param)
            {
                var parts = param.Split(',');
                if (parts.Length == 2)
                {
                    trueValue = parts[0].Trim();
                    falseValue = parts[1].Trim();
                }
            }

            return string.IsNullOrEmpty(value as string) ? trueValue : falseValue;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}