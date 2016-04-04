﻿using PhotoLayout.Models;
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
    public class LayoutGrid : Grid, INotifyPropertyChanged
    {
        #region - Fields -

        private static List<ResizableImage> images;

        #endregion

        #region - Constructors -

        static LayoutGrid()
        {
            //DefaultStyleKeyProperty.OverrideMetadata(typeof(LayoutGrid), new FrameworkPropertyMetadata(typeof(LayoutGrid))); // TODO Check if this will be necessary                   
        }

        public LayoutGrid()
        {
            images = new List<ResizableImage>();
            Loaded += LayoutGrid_Loaded;
        }

        private void LayoutGrid_Loaded(object sender, RoutedEventArgs e)
        {
            CreateResizableImages();

            int index = 0;
            for (int row = 0; row < RowCount; row++)
            {
                for (int col = 0; col < ColumnCount; col++, index++)
                {
                    Grid.SetRow(images[index], row);
                    Grid.SetColumn(images[index], col);
                }
            }
        }

        private void CreateResizableImages()
        {
            int count = RowCount * ColumnCount;
            for (int i = 0; i < count; i++)
            {
                ResizableImage img = new ResizableImage();
                img.Stretch = Stretch.UniformToFill;                
                images.Add(img);
                this.Children.Add(img);
            }
        }
        
        #endregion

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

        #region - Photos -

        public static DependencyProperty PhotosProperty = DependencyProperty.Register("Photos", typeof(ObservableCollection<Photo>), typeof(LayoutGrid), 
            new PropertyMetadata(new ObservableCollection<Photo>(), OnPhotosChanged));

        /// <summary>
        /// Clears the old ObservableCollection and calls GC.Collect when the Photos dependency property changes.
        /// </summary>        
        private static void OnPhotosChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // TODO Check if this is necessary for releasing memory before new collection is assigned to the property
            var oldCollection = e.OldValue as ObservableCollection<Photo>;
            oldCollection.CollectionChanged -= OnCollectionChanged;
            oldCollection.Clear();
            GC.Collect();


            var newCollection = e.NewValue as ObservableCollection<Photo>;
            //LayoutGrid layoutGrid = d as LayoutGrid;

            //if (layoutGrid != null && newCollection != null)
            //{
            //    for (int i = 0; i < newCollection.Count; i++)
            //    {
            //        ((Image)layoutGrid.Children[i]).Source = newCollection[i].Thumbnail;
            //    }                
            //}

            if (newCollection != null)
            {
                newCollection.CollectionChanged += OnCollectionChanged;
            }
        }

        private static void OnCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            var action = e.Action;

            if (action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                var photo = e.NewItems[0] as Photo;
                System.Diagnostics.Debug.WriteLine($"OnCollectionChanged ->  Added: {photo}");

                for (int i = 0; i < images.Count; i++)
                {
                    if (images[i].Source == null)
                    {
                        images[i].Source = photo.Thumbnail;
                        return;
                    }
                }
                
            }
        }

        /// <summary>
        /// Gets or sets the collection of photos that the layout grid is supposed to display.
        /// </summary>
        public ObservableCollection<Photo> Photos
        {
            get { return (ObservableCollection<Photo>)GetValue(PhotosProperty); }
            set { SetValue(PhotosProperty, value); }
        }

        #endregion

        #region - MinPhotoCount -

        public static DependencyProperty MinPhotoCountProperty = DependencyProperty.Register("MinPhotoCount", typeof(int), typeof(LayoutGrid),
            new PropertyMetadata(0, OnMinPhotoCountChanged, OnMinPhotoCountCoerceValue));

        /// <summary>
        /// Coerces the MinPhotoCount property to acceptable value. If the value is below 0 coerces it to 0,
        /// and if it's above MaxPhotoCount limits it to MaxPhotoCount.
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
        /// Empty method. No code/logic needed for OnMinPhotoCountChanged.
        /// </summary>        
        private static void OnMinPhotoCountChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {            
        }

        /// <summary>
        /// Gets or sets the minimum count of photographs needed for LayoutGrid to be visible.
        /// </summary>
        public int MinPhotoCount
        {
            get { return (int)GetValue(MinPhotoCountProperty); }
            set { SetValue(MinPhotoCountProperty, value); }
        }

        #endregion

        #region - RowCount -

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RowCountProperty =
            DependencyProperty.Register("RowCount", typeof(int), typeof(LayoutGrid), 
                new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange, OnRowCountChanged, OnRowCountCoerceValue));

        private static object OnRowCountCoerceValue(DependencyObject d, object baseValue)
        {
            int rows = (int)baseValue;
            if (rows < 0)
            {
                rows = 0;
            }
            // Just a precaution
            else if (rows > 100)
            {
                rows = 100;
            }
            return rows;
        }

        private static void OnRowCountChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            LayoutGrid layoutGrid = d as LayoutGrid;
            int rows = (int)e.NewValue;            

            if (layoutGrid != null)
            {
                // Clear previous RowDefinitions just in case this property is being set multiple times
                layoutGrid.RowDefinitions.Clear();

                for (int i = 0; i < rows; i++)
                {
                    RowDefinition row = new RowDefinition();
                    row.Height = GridLength.Auto;
                    layoutGrid.RowDefinitions.Add(row);
                }
            }
        }

        public int RowCount
        {
            get { return (int)GetValue(RowCountProperty); }
            set { SetValue(RowCountProperty, value); }
        }

        #endregion

        #region - ColumnCount -

        public static readonly DependencyProperty ColumnCountProperty =
            DependencyProperty.Register("ColumnCount", typeof(int), typeof(LayoutGrid), 
                new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange, OnColumnCountChanged, OnColumnCountCoerceValue));

        private static object OnColumnCountCoerceValue(DependencyObject d, object baseValue)
        {
            int columns = (int)baseValue;
            if (columns < 0)
            {
                columns = 0;
            }
            // Just a precaution
            else if (columns > 100)
            {
                columns = 100;
            }
            return columns;
        }

        private static void OnColumnCountChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            LayoutGrid layoutGrid = d as LayoutGrid;
            int columns = (int)e.NewValue;

            if (layoutGrid != null)
            {
                // Clear previous ColumnDefinitions just in case this property is being set multiple times
                layoutGrid.ColumnDefinitions.Clear();

                for (int i = 0; i < columns; i++)
                {
                    ColumnDefinition column = new ColumnDefinition();
                    column.Width = GridLength.Auto;
                    layoutGrid.ColumnDefinitions.Add(column);
                }
            }
        }

        public int ColumnCount
        {
            get { return (int)GetValue(ColumnCountProperty); }
            set { SetValue(ColumnCountProperty, value); }
        }

        #endregion

        #endregion

        #region - Properties -

        public int MaxPhotoCount { get; set; }
        //public bool HasThumbnails { get; set; }

        #endregion

        #region - Overriden methods -

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