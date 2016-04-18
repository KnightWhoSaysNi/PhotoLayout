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
    /// ListBox selected items count to specified color converter. When a certain number of items are selected, the selection changes color.
    /// </summary>
    public class SelectedItemCountToColorConverter : IValueConverter
    {
        /// <summary>
        /// Converts the given integer value to a certain color.
        /// </summary>
        /// <param name="value">Number of selected items.</param>
        /// <param name="parameter">Default color used for selection.</param>
        /// <returns>Returns parameter specified color, or Red.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int count = (int)value;

            if (count < Constants.MaxPhotosInLayoutGrid)
            {
                var brush = (SolidColorBrush)parameter;
                return brush;
            }
            else
            {
                // Max number of selected photos reached, so the selection color is Red
                return Brushes.Red;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
