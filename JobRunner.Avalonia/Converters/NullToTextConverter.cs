using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace JobRunner.Avalonia.Converters
{
    /// <summary>
    /// Конвертер для преобразования null 
    /// или пустых значений в текст-заполнитель
    /// </summary>
    public class NullToTextConverter : IValueConverter
    {
        public string DefaultText { get; set; } = "не указано";

        /// <summary>
        /// Преобразует null или пустое значение в текст-заполнитель
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return value ?? DefaultText;
        }

        /// <summary>
        /// Обратное преобразование: из текста в исходное значение
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is string str && str != DefaultText)
                return str;
            return null;
        }
    }
}
