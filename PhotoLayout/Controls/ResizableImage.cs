using PhotoLayout.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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
    public class ResizableImage : Control, INotifyPropertyChanged
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
        protected void OnPropertyChanged([CallerMemberName]string propertyName="")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        #region - Dependency Properties -

        #region - Source -

            public static readonly DependencyProperty SourceProperty = 
                DependencyProperty.Register("Source", typeof(BitmapSource), typeof(ResizableImage), new PropertyMetadata(null,null,OnSourceCoerceValue));

            private static object OnSourceCoerceValue(DependencyObject d, object baseValue)
            {
                BitmapSource newSource = (BitmapSource)baseValue;

                // TODO Check if this is necessary and if null should be returned
                // Source is set to an image that might be too big to preview so this check (might be) necessary
                return (newSource.PixelHeight * newSource.PixelWidth < Constants.MaximumPixelsForPreview) ? newSource : null;
            }

            public BitmapSource Source
        {
            get { return (BitmapSource)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        #endregion


        
        #endregion

        #region - Properties -

        public bool HasLeftSplitter
        {
            get { return hasLeftSplitter; }
            set
            {
                hasLeftSplitter = value;
                OnPropertyChanged();
            }
        }
        public bool HasTopSplitter
        {
            get { return hasTopSplitter; }
            set
            {
                hasTopSplitter = value;
                OnPropertyChanged();
            }
        }
        public bool HasRightSplitter
        {
            get { return hasRightSplitter; }
            set
            {
                hasRightSplitter = value;
                OnPropertyChanged();
            }
        }
        public bool HasBottomSplitter
        {
            get { return hasBottomSplitter; }
            set
            {
                hasBottomSplitter = value;
                OnPropertyChanged();
            }
        }


        #endregion
    }
}
