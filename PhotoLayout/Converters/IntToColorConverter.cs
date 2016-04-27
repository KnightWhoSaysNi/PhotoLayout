using PhotoLayout.Helpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace PhotoLayout.Converters
{
    /// <summary>
    /// Int to specified color converter. Below a certain value the parameter color is returned.
    /// </summary>
    public class IntToColorConverter : IValueConverter
    {
        /// <summary>
        /// Converts the given integer value to a certain color.
        /// </summary>
        /// <param name="value">Int getting compared to MaxSelectedPhotos value.</param>
        /// <param name="parameter">Color you wish to return if the value is below MaxSelectedPhotos.</param>
        /// <returns>Returns parameter specified color, or Red.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int count = (int)value;

            if (count < Constants.MaxSelectedPhotos)
            {
                var brush = (SolidColorBrush)parameter;
                return brush;
            }
            else
            {
                // Default value returned
                return Brushes.Red;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
