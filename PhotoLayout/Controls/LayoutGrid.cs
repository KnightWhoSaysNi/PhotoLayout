using PhotoLayout.Converters;
using PhotoLayout.Enums;
using PhotoLayout.Helpers;
using PhotoLayout.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
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

        private List<Image> images;
        private List<ManipulationBorder> borders;

        #endregion

        #region - Constructors -

        static LayoutGrid()
        {
            //DefaultStyleKeyProperty.OverrideMetadata(typeof(LayoutGrid), new FrameworkPropertyMetadata(typeof(LayoutGrid))); // TODO Check if this will be necessary                   
        }     

        public LayoutGrid()
        {
            CreateImages();
            CreateManipulationBorders();
        }

        #endregion

        #region - Dependency Properties -

        #region - PhotoType -

        public static readonly DependencyProperty PhotoTypeProperty =
            DependencyProperty.Register("PhotoType", typeof(BitmapType), typeof(LayoutGrid), new PropertyMetadata(BitmapType.Thumbnail, null, CoercePhotoTypeValue));

        private static object CoercePhotoTypeValue(DependencyObject d, object baseValue)
        {
            BitmapType type = (BitmapType)baseValue;
            // If type is set to OriginalBitmap coerce it to Thumbnail, as there is no need for a grid to hold original bitmap photograph
            return type != BitmapType.OriginalBitmap ? type : BitmapType.Thumbnail;
        }

        public BitmapType PhotoType
        {
            get { return (BitmapType)GetValue(PhotoTypeProperty); }
            set { SetValue(PhotoTypeProperty, value); }
        }

        #endregion

        #region - MaxPhotoCount -

        public static readonly DependencyProperty MaxPhotoCountProperty = DependencyProperty.Register("MaxPhotoCount", typeof(int), typeof(LayoutGrid), 
            new PropertyMetadata(Constants.MaxSelectedPhotos, OnMaxPhotoCountChanged, OnMaxPhotoCoerceValue));

        /// <summary>
        /// Gets or sets the maximum count of photos that can be shown in LayoutGrid.
        /// </summary>
        public int MaxPhotoCount
        {
            get { return (int)GetValue(MaxPhotoCountProperty); }
            set { SetValue(MaxPhotoCountProperty, value); }
        }

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

        #endregion

        #region - Photos -

        public static DependencyProperty PhotosProperty = DependencyProperty.Register("Photos", typeof(ObservableCollection<Photo>), typeof(LayoutGrid), 
            new PropertyMetadata(new ObservableCollection<Photo>(), OnPhotosChanged));

        /// <summary>
        /// Gets or sets the collection of photos that the layout grid is supposed to display in its Image controls (images static field).
        /// </summary>
        public ObservableCollection<Photo> Photos
        {
            get { return (ObservableCollection<Photo>)GetValue(PhotosProperty); }
            set { SetValue(PhotosProperty, value); }
        }

        /// <summary>
        /// Resolves addition, removal and swapping/replacing of photos in the layout grid 
        /// and binds cell count (private dependency property) to the count of photos in the new collection.
        /// </summary>        
        private static void OnPhotosChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            LayoutGrid layoutGrid = d as LayoutGrid;
            if (layoutGrid == null)
            {
                return;
            }           

            NotifyCollectionChangedEventHandler handler = (sender, collectionChangedEventArgs) =>
              {
                  var action = collectionChangedEventArgs.Action;
                  Photo photo;
                  switch (action)
                  {
                      case NotifyCollectionChangedAction.Add:
                          photo = (Photo)collectionChangedEventArgs.NewItems[0];
                          OnNewPhotoAdded(layoutGrid, photo);
                          break;
                      case NotifyCollectionChangedAction.Remove:
                          photo = (Photo)collectionChangedEventArgs.OldItems[0];
                          OnPhotoRemoved(layoutGrid, photo);
                          break;
                      case NotifyCollectionChangedAction.Move:
                          // TODO Determine how to resolve Replace function, as there must always be 2 Move operations for swapping/replacing
                          // Or if Remove, Add and another Remove, Add will be used to swap 2 Photos
                          break;
                  }
              };

            var oldCollection = e.OldValue as ObservableCollection<Photo>;
            if (oldCollection != null)
            {
                // TODO Determine if this is necessary for releasing memory, i.e. if this can really cause memory leak
                oldCollection.Clear();
                oldCollection.CollectionChanged -= handler;
                GC.Collect();
            }

            var newCollection = e.NewValue as ObservableCollection<Photo>;
            if (newCollection != null)
            {
                newCollection.CollectionChanged += handler;
            }

            BindCellCountToPhotosCount(d, newCollection);
        }

        private static void OnNewPhotoAdded(LayoutGrid layoutGrid, Photo photo)
        {
            System.Diagnostics.Debug.WriteLine($"PhotoCollectionChanged -> Added {photo}");

            // Go through every image control and set the Source for the first one that doesn't host a photo/image, i.e. whose Source == null
            for (int i = 0; i < layoutGrid.images.Count; i++)
            {
                if (layoutGrid.images[i].Source == null)
                {
                    if (layoutGrid.PhotoType == BitmapType.Thumbnail)
                    {
                        // This layout grid is one of many displaying possible layouts that the user can choose from
                        // and it must not have the option of manipulating individual photos
                        layoutGrid.images[i].Source = photo.Thumbnail;
                        foreach (Image img in layoutGrid.images)
                        {
                            //img.IsHitTestVisible = false;
                        }
                    }
                    else
                    {
                        // This layout grid is the one the user will be using for manipulating individual photos
                        layoutGrid.images[i].Source = photo.PreviewBitmap;
                    }

                    // Each individual photo can only be displayed once in the grid, so no need for continuation of the for loop -> the photo has been added
                    return;
                }
            }
        }

        private static void OnPhotoRemoved(LayoutGrid layoutGrid, Photo photo)
        {
            System.Diagnostics.Debug.WriteLine($"PhotoCollectionChanged -> Removed {photo}");

            // Go through every image control and set its Source to null if it hosts the photo (either a Thumbnail or a PreviewBitmap version)
            for (int i = 0; i < layoutGrid.images.Count; i++)
            {
                if (layoutGrid.images[i].Source != null && (layoutGrid.images[i].Source == photo.Thumbnail || layoutGrid.images[i].Source == photo.PreviewBitmap))
                {
                    RearangeImages(layoutGrid, i);
                    
                    // Each individual photo can only be displayed once in the grid, so no need for continuation of the for loop -> the one photo has been removed
                    return;
                }                
            }
        }

        private static void RearangeImages(LayoutGrid layoutGrid, int photoIndex)
        {
            for (int i = photoIndex; i < layoutGrid.images.Count - 1; i++)
            {
                layoutGrid.images[i].Source = layoutGrid.images[i + 1].Source;
            }
            layoutGrid.images[layoutGrid.images.Count - 1].Source = null;
        }

        /// <summary>
        /// Binds the count of photos to the private CellCount dependency property.
        /// </summary>
        /// <param name="d">LayoutGrid for which the binding is done.</param>
        /// <param name="collection">The collection of photos in Photos property.</param>
        private static void BindCellCountToPhotosCount(DependencyObject d, ObservableCollection<Photo> collection)
        {
            LayoutGrid layoutGrid = d as LayoutGrid;
            if (layoutGrid != null)
            {
                Binding cellCountBinding = new Binding("Count");
                cellCountBinding.Source = collection;
                layoutGrid.SetBinding(CellCountProperty, cellCountBinding);
            }
        }

        #endregion

        #region - MinPhotoCount -

        public static DependencyProperty MinPhotoCountProperty = DependencyProperty.Register("MinPhotoCount", typeof(int), typeof(LayoutGrid),
            new PropertyMetadata(0, null, OnMinPhotoCountCoerceValue));

        /// <summary>
        /// Gets or sets the minimum count of photographs needed for LayoutGrid to be visible. If LayoutGrid has less photos 
        /// than MinPhotoCount's value the Visibility of LayoutGrid should be set to Collapsed.
        /// </summary>
        public int MinPhotoCount
        {
            get { return (int)GetValue(MinPhotoCountProperty); }
            set { SetValue(MinPhotoCountProperty, value); }
        }

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

        #endregion
            
        /****************************************************************************************************************
                                                LayoutGrid's Layout Creator
        *****************************************************************************************************************/    
        #region - CellCount private dependency property -

        private static readonly DependencyProperty CellCountProperty =
            DependencyProperty.Register("CellCount", typeof(int), typeof(LayoutGrid), new PropertyMetadata(0, OnCellCountChanged));

        private static void OnCellCountChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            LayoutGrid layoutGrid = d as LayoutGrid;
            if (layoutGrid != null)
            {
                if ((int)e.NewValue > (int)e.OldValue)
                {                    
                    UpdateLayout(layoutGrid, LayoutAction.Addition);
                }
                else
                {
                    UpdateLayout(layoutGrid, LayoutAction.Removal);
                }
            }
        }

        private static void UpdateLayout(LayoutGrid layoutGrid, LayoutAction action)
        {
            int cellCount = layoutGrid.CellCount;
            byte rowCount = layoutGrid.LayoutMatrix[0];
            byte columnCount = layoutGrid.LayoutMatrix[1];

            if (layoutGrid.LayoutOrientation == LayoutOrientation.ColumnFirst)
            {
                UpdateLayoutColumnFirst(rowCount, columnCount, cellCount, layoutGrid, action);
            }
            else
            {
                UpdateLayoutRowFirst(rowCount, columnCount, cellCount, layoutGrid);
            }
        }

        private static void UpdateLayoutColumnFirst(byte rowCount, byte columnCount, int cellCount, LayoutGrid layoutGrid, LayoutAction action)
        {
            int row = (cellCount - 1) / columnCount;
            int col = (cellCount - 1) % columnCount;

            if (action == LayoutAction.Removal && (cellCount % columnCount == 0))
            {
                row = cellCount / columnCount;
                ManipulationBorder removedBorder = (ManipulationBorder)((layoutGrid.Children[row] as Grid).Children[0]);
                (layoutGrid.Children[row] as Grid).Children.RemoveAt(0);
                layoutGrid.borders.Insert(0, removedBorder);

                // Remove last child (sub-grid)
                layoutGrid.Children.Remove(layoutGrid.Children[row] as Grid);

                // Remove the last row
                layoutGrid.RowDefinitions.RemoveAt(row);

                return;
            }

            if (action == LayoutAction.Removal)
            {
                ManipulationBorder removedBorder = (ManipulationBorder)((layoutGrid.Children[row] as Grid).Children[(layoutGrid.Children[row] as Grid).ColumnDefinitions.Count - 1]);
                (layoutGrid.Children[row] as Grid).Children.RemoveAt((layoutGrid.Children[row] as Grid).ColumnDefinitions.Count - 1);
                (layoutGrid.Children[row] as Grid).ColumnDefinitions.RemoveAt((layoutGrid.Children[row] as Grid).ColumnDefinitions.Count - 1);
                layoutGrid.borders.Insert(0, removedBorder);

                return;
            }

            if (col == 0)
            {
                // Add new row to the layout grid
                layoutGrid.RowDefinitions.Add(new RowDefinition());

                // Add sub grid to the layout grid and set its row to the current row
                Grid subGrid = new Grid();
                Grid.SetRow(subGrid, row);
                layoutGrid.Children.Add(subGrid);
            }

            // Add new column to the sub grid
            (layoutGrid.Children[row] as Grid).ColumnDefinitions.Add(new ColumnDefinition());

            // Remove first manipulation border from the layout grid and set it to the sub grid (the border cannot be a logical child of 2 panels)
            ManipulationBorder border = layoutGrid.borders[0];
            layoutGrid.borders.RemoveAt(0);
            (layoutGrid.Children[row] as Grid).Children.Add(border);

            // Set column for the manipulation border
            Grid.SetColumn(border, col);
        }

        private static void UpdateLayoutRowFirst(byte rowCount, byte columnCount, int cellCount, LayoutGrid layoutGrid)
        {
            int col = (cellCount - 1) / rowCount;
            int row = (cellCount - 1) % rowCount;

            if (row == 0)
            {
                // Add new column to the layout grid
                layoutGrid.ColumnDefinitions.Add(new ColumnDefinition());

                // Add sub grid to the layout grid and set its column to the current column
                Grid subGrid = new Grid();
                Grid.SetColumn(subGrid, col);
                layoutGrid.Children.Add(subGrid);
            }

            // Add new row to the sub grid
            (layoutGrid.Children[col] as Grid).RowDefinitions.Add(new RowDefinition());            

            // Remove first manipulation border from the layotu grid and set it to the sub grid (the border cannot be a logical child of 2 panles)
            ManipulationBorder border = layoutGrid.borders[0];
            layoutGrid.borders.RemoveAt(0);
            (layoutGrid.Children[col] as Grid).Children.Add(border);

            // Set row for the manipulation border
            Grid.SetRow(border, row);
        }

        private int CellCount
        {
            get { return (int)GetValue(CellCountProperty); }
            set { SetValue(CellCountProperty, value); }
        }

        #endregion

        #region - LayoutOrientation -

        public static readonly DependencyProperty LayoutOrientationProperty =
            DependencyProperty.Register("LayoutOrientation", typeof(LayoutOrientation), typeof(LayoutGrid), new PropertyMetadata(LayoutOrientation.ColumnFirst));

        public LayoutOrientation LayoutOrientation
        {
            get { return (LayoutOrientation)GetValue(LayoutOrientationProperty); }
            set { SetValue(LayoutOrientationProperty, value); }
        }

        #endregion

        #region - LayoutMatrix -

        public static readonly DependencyProperty LayoutMatrixProperty =
            DependencyProperty.Register("LayoutMatrix", typeof(byte[]), typeof(LayoutGrid), new PropertyMetadata(Constants.ThreeByThreeLayout));

        public byte[] LayoutMatrix
        {
            get { return (byte[])GetValue(LayoutMatrixProperty); }
            set { SetValue(LayoutMatrixProperty, value); }
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

        private void CreateImages()
        {
            this.images = new List<Image>();
            for (int i = 0; i < Constants.MaxSelectedPhotos; i++)
            {
                Image img = new Image();
                img.Stretch = Stretch.UniformToFill; // TODO This must not be UniformToFill in the final version, because it crops the image
                img.HorizontalAlignment = HorizontalAlignment.Center;
                img.VerticalAlignment = VerticalAlignment.Center;
                this.images.Add(img);
            }
        }

        private void CreateManipulationBorders()
        {
            this.borders = new List<ManipulationBorder>();
            for (int i = 0; i < Constants.MaxSelectedPhotos; i++)
            {
                this.borders.Add(new ManipulationBorder());
                this.borders[i].Child = this.images[i];
                this.borders[i].ClipToBounds = true;
                this.borders[i].HorizontalAlignment = HorizontalAlignment.Stretch;
                this.borders[i].VerticalAlignment = VerticalAlignment.Stretch;

                this.borders[i].BorderBrush = Brushes.Red;
                this.borders[i].BorderThickness = new Thickness(1);
            }
        }

        #endregion
    }
}
