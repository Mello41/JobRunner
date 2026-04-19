using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace JobRunner.Avalonia.Converters
{
    /// <summary>
    /// Необходимая конвертация на русский значений true/false типов bool
    /// </summary>
    public class BooleanToTextConverter : IValueConverter
    {
        public string True { get; set; } = "Да";
        public string False { get; set; } = "Нет";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return value is true ? True : False;
        }

        /// <summary>
        /// Нужен для TwoWay привязки
        /// исп. когда значение из UI нужно записать обратно в ViewModel.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public object? ConvertBack(object? value, Type targetType,
            object? parameter, CultureInfo culture)
        {
            if (value is string str)
            {
                if (str == True) return true;
                if (str == False) return false;
            }
            return false;
        }
    }
}
