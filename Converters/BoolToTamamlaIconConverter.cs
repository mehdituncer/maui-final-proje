using Microsoft.Maui.Controls;
using System;
using System.Globalization;

namespace MyMauiApp.Converters
{
    public class BoolToTamamlaIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isCompleted)
            {
                // These are placeholders. You'll need to add actual image files to your project.
                // For example, in Resources/Images/
                return isCompleted ? "undo_icon.png" : "check_icon.png"; // Placeholder icon names
            }
            return "check_icon.png";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
