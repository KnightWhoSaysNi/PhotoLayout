using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PhotoLayout.Views
{
    /// <summary>
    /// Interaction logic for PhotoLayoutSelection.xaml
    /// </summary>
    public partial class PhotoLayoutSelection : UserControl
    {
        public PhotoLayoutSelection()
        {
            InitializeComponent();            
        }

        /// <summary>
        /// Stops auto scrolling of selected item into view. When selected item is partially visible stops the ListBox from auto scrolling to it.
        /// </summary>
        private void OnRequestBringIntoView(object sender, RequestBringIntoViewEventArgs e)
        {
            e.Handled = true;
        }
    }
}
