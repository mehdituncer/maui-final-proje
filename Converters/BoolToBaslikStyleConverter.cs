using Microsoft.Maui.Controls;
using System;
using System.Globalization;

namespace MyMauiApp.Converters
{
    public class BoolToBaslikStyleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isCompleted)
            {
                string styleKey = isCompleted ? "TamamlandiGorevBaslikStyle" : "GorevBaslikStyle";
                if (Application.Current != null && Application.Current.Resources.TryGetValue(styleKey, out var style))
                {
                    return style;
                }
            }
            // Fallback to default style if not found or value is not bool
            if (Application.Current != null && Application.Current.Resources.TryGetValue("GorevBaslikStyle", out var defaultStyle))
            {
                return defaultStyle;
            }
            return null; // Or some very basic default style
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
