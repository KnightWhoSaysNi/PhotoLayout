using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace PhotoLayout.Converters
{
    public class SelectedItemCountToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int count = (int)value;
            // TODO See how to turn #FF6ACDF4 into a brush
            // var brush = parameter as something or something.get brush from parameter
            // var color = Color.FromArgb(255, 106, 205, 244);
            if (count <= 5)
            {
                return Brushes.CornflowerBlue;
            }
            else
            {
                return Brushes.Red;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
