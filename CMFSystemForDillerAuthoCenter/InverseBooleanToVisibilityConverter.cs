using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows;

namespace CMFSystemForDillerAuthoCenter.Converters
{
    //Чтобы поле CompanyNameTextBox отображалось только при выборе "Юрлицо" и не мешало при выборе "Физлицо", нужно правильно настроить видимость.
    //Проблема с ConverterParameter = True в том, что BooleanToVisibilityConverter не поддерживает инверсию через параметр.
    public class InverseBooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isVisible)
            {
                return isVisible ? Visibility.Collapsed : Visibility.Visible;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}