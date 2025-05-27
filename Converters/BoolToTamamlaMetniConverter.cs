using Microsoft.Maui.Controls;
using System;
using System.Globalization;

namespace MyMauiApp.Converters
{
    public class BoolToTamamlaMetniConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isCompleted)
            {
                return isCompleted ? "Geri Al" : "Tamamla"; // "Undo" : "Complete"
            }
            return "Tamamla";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
