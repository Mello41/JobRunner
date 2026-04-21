using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace JobRunner.Avalonia.Converters
{
    /// <summary>
    /// Конвертер для инвертирования bool value
    /// </summary>
    public class InverseBooleanConverter : IValueConverter
    {
        /// <summary>
        /// Преобразование bool value в противоположное
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return !(value is true);
        }

        /// <summary>
        /// Обратное преобразование bool значения
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return !(value is true);
        }
    }
}
