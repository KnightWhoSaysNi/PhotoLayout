using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace PhotoLayout.Controls
{
    /// <summary>
    /// Follow steps 1a or 1b and then 2 to use this custom control in a XAML file.
    ///
    /// Step 1a) Using this custom control in a XAML file that exists in the current project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:PhotoLayout.Controls"
    ///
    ///
    /// Step 1b) Using this custom control in a XAML file that exists in a different project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:PhotoLayout.Controls;assembly=PhotoLayout.Controls"
    ///
    /// You will also need to add a project reference from the project where the XAML file lives
    /// to this project and Rebuild to avoid compilation errors:
    ///
    ///     Right click on the target project in the Solution Explorer and
    ///     "Add Reference"->"Projects"->[Browse to and select this project]
    ///
    ///
    /// Step 2)
    /// Go ahead and use your control in the XAML file.
    ///
    ///     <MyNamespace:ResizableImage/>
    ///
    /// </summary>
    public class ResizableImage : Image, INotifyPropertyChanged
    {
        #region - Fields -

        private bool hasLeftSplitter;
        private bool hasTopSplitter;
        private bool hasRightSplitter;
        private bool hasBottomSplitter;

        #endregion

        static ResizableImage()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ResizableImage), new FrameworkPropertyMetadata(typeof(ResizableImage)));
        }

        #region - Events -

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        #region - Dependency Properties -

        #endregion

        #region - Properties -

        public bool HasLeftSplitter
        {
            get { return hasLeftSplitter; }
            set
            {
                hasLeftSplitter = value;
                OnPropertyChanged(nameof(HasLeftSplitter));
            }
        }
        public bool HasTopSplitter
        {
            get { return hasTopSplitter; }
            set
            {
                hasTopSplitter = value;
                OnPropertyChanged(nameof(HasTopSplitter));
            }
        }
        public bool HasRightSplitter
        {
            get { return hasRightSplitter; }
            set
            {
                hasRightSplitter = value;
                OnPropertyChanged(nameof(HasRightSplitter));
            }
        }
        public bool HasBottomSplitter
        {
            get { return hasBottomSplitter; }
            set
            {
                hasBottomSplitter = value;
                OnPropertyChanged(nameof(HasBottomSplitter));
            }
        }


        #endregion
    }
}
