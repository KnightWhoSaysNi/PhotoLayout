using PhotoLayout.Converters;
using PhotoLayout.Helpers;
using PhotoLayout.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    ///     <MyNamespace:LayoutGrid/>
    ///
    /// </summary>
    public class LayoutGrid : Grid
    {
        #region - Fields -
        


        #endregion

        #region - Constructors -

        static LayoutGrid()
        {
            //DefaultStyleKeyProperty.OverrideMetadata(typeof(LayoutGrid), new FrameworkPropertyMetadata(typeof(LayoutGrid))); // TODO Check if this will be necessary                   
        }     
        
        #endregion        

        #region - Dependency Properties -

        #region - MaxPhotoCount -

        public static readonly DependencyProperty MaxPhotoCountProperty = DependencyProperty.Register("MaxPhotoCount", typeof(int), typeof(LayoutGrid), 
            new PropertyMetadata(Constants.MaxSelectedPhotos, OnMaxPhotoCountChanged, OnMaxPhotoCoerceValue));

        /// <summary>
        /// Coerces the value of MaxPhotoCount property to <see cref="Constants.MaxSelectedPhotos"/>
        /// if it's being set to a negative value or a number above 50.
        /// </summary>
        /// <param name="d">LayoutGrid whose MaxPhotoCount property's value is coerced.</param>
        /// <param name="baseValue">Base value of MaxPhotoCount property.</param>
        /// <returns></returns>
        private static object OnMaxPhotoCoerceValue(DependencyObject d, object baseValue)
        {
            int count = (int)baseValue;
            if (count < 0 || count > 50)
            {
                count = Constants.MaxSelectedPhotos;
            }
            return count;
        }

        /// <summary>
        /// If MinPhotoCount property has higher value than the new MaxPhotoCount this method limits the MinPhotoCount to MaxPhotoCount.
        /// </summary>       
        private static void OnMaxPhotoCountChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            LayoutGrid layoutGrid = d as LayoutGrid;
            int count = (int)e.NewValue;
            if (layoutGrid != null && layoutGrid.MinPhotoCount > count)
            {
                layoutGrid.MinPhotoCount = count;
            }
        }

        /// <summary>
        /// Gets or sets the maximum count of photos that can be shown in LayoutGrid.
        /// </summary>
        public int MaxPhotoCount
        {
            get { return (int)GetValue(MaxPhotoCountProperty); }
            set { SetValue(MaxPhotoCountProperty, value); }
        }

        #endregion

        #region - Photos -

        public static DependencyProperty PhotosProperty = DependencyProperty.Register("Photos", typeof(ObservableCollection<Photo>), typeof(LayoutGrid), 
            new PropertyMetadata(new ObservableCollection<Photo>(), OnPhotosChanged));

        /// <summary>
        /// Clears the old ObservableCollection and calls GC.Collect when the Photos dependency property changes.
        /// </summary>        
        private static void OnPhotosChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            LayoutGrid layoutGrid = d as LayoutGrid;
            var newCollection = e.NewValue as ObservableCollection<Photo>;

            Binding cellCountBinding = new Binding("Count");
            cellCountBinding.Source = newCollection;
            layoutGrid.SetBinding(CellCountProperty, cellCountBinding);
        }

        /// <summary>
        /// Gets or sets the collection of photos that the layout grid is supposed to display in its Image controls (images static field).
        /// </summary>
        public ObservableCollection<Photo> Photos
        {
            get { return (ObservableCollection<Photo>)GetValue(PhotosProperty); }
            set { SetValue(PhotosProperty, value); }
        }

        #endregion

        #region - MinPhotoCount -

        public static DependencyProperty MinPhotoCountProperty = DependencyProperty.Register("MinPhotoCount", typeof(int), typeof(LayoutGrid),
            new PropertyMetadata(0, null, OnMinPhotoCountCoerceValue));

        /// <summary>
        /// Coerces the MinPhotoCount property to acceptable value. If the value is below 0 coerces it to 0,
        /// and if it's above MaxPhotoCount limit is set to MaxPhotoCount.
        /// </summary>
        /// <param name="d">LayoutGrid whose property is being coerced.</param>
        /// <param name="baseValue">Specified value for the MinPhotoCount property.</param>
        /// <returns>Returns 0 if the value is negative, or MaxPhotoCount if it's above that property.</returns>
        private static object OnMinPhotoCountCoerceValue(DependencyObject d, object baseValue)
        {
            LayoutGrid layoutGrid = d as LayoutGrid;
            if (layoutGrid == null)
            {
                // TODO Decide if validation should be used instead and throw an exception
                return 0;
            }

            int count = (int)baseValue;
            if (count < 0)
            {
                count = 0;
            }
            else if (count > layoutGrid.MaxPhotoCount)
            {
                count = layoutGrid.MaxPhotoCount;
            }
            return count;
        }

        /// <summary>
        /// Gets or sets the minimum count of photographs needed for LayoutGrid to be visible. If LayoutGrid has less photos 
        /// than MinPhotoCount's value the Visibility of LayoutGrid should be set to Collapsed.
        /// </summary>
        public int MinPhotoCount
        {
            get { return (int)GetValue(MinPhotoCountProperty); }
            set { SetValue(MinPhotoCountProperty, value); }
        }

        #endregion

        #region - CellCount -

        private static readonly DependencyProperty CellCountProperty =
            DependencyProperty.Register("CellCount", typeof(int), typeof(LayoutGrid), new PropertyMetadata(0, OnCellCountChanged, CellCountCoerce));

        private static object CellCountCoerce(DependencyObject d, object baseValue)
        {
            int newCount = (int)baseValue;
            // Since CellCount is bound to Photos.Count, if it's 0 (zero) the cound cell must be 1, as it can't drop below this value -> Default grid with 1 Row and 1 Column
            return newCount == 0 ? 1 : newCount;
        }

        private static void OnCellCountChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // TODO Do something with this   
        }

        private int CellCount
        {
            get { return (int)GetValue(CellCountProperty); }
            set { SetValue(CellCountProperty, value); }
        }

        #endregion

        #endregion

        #region - Properties -

        #endregion

        #region - Overriden methods: MeasureOverride and ArrangeOverride -

        protected override Size MeasureOverride(Size constraint)
        {
            return base.MeasureOverride(constraint);
        }

        protected override Size ArrangeOverride(Size arrangeSize)
        {
            return base.ArrangeOverride(arrangeSize);
        }

        #endregion

        #region - Private methods -

      

        #endregion
    }
}
