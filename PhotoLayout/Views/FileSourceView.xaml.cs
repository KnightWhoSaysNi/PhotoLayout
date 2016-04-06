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
    /// Interaction logic for FileSourceView.xaml 
    /// </summary>
    public partial class FileSourceView : UserControl
    {
        // TODO In FileSourceView.xaml mayhaps bind radio buttons' IsChecked properties to UserControl/Window Visibility or some other property
        // so that every radio button is unchecked for each new visit to the photograph printing view (for Easy2U)
        public FileSourceView()
        {
            InitializeComponent();
        }
    }
}
