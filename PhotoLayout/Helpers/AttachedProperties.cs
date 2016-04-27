using PhotoLayout.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace PhotoLayout.Helpers
{
    public class AttachedProperties
    {
        // TODO Delete this if it's not being used
        #region - ListBox SelectedPhotos -

        /// <summary>
        /// Attached property representing the selected photo items of a list box, as an ObservableCollection.
        /// </summary>        
        public static readonly DependencyProperty SelectedPhotosProperty =
             DependencyProperty.RegisterAttached("SelectedPhotos",
                 typeof(ObservableCollection<Photo>),
                 typeof(AttachedProperties),
                 new PropertyMetadata(new ObservableCollection<Photo>()));

        /// <summary>
        /// Gets a value of SelectedPhotos for the specified ListBox.
        /// </summary>
        /// <param name="obj">ListBox whose SelectedPhotos property you're getting.</param>
        public static ObservableCollection<Photo> GetSelectedPhotos(ListBox obj)
        {
            return (ObservableCollection<Photo>)obj.GetValue(SelectedPhotosProperty);
        }

        /// <summary>
        /// Sets a value of SelectedPhotos for the specified ListBox.
        /// </summary>
        /// <param name="obj">ListBox whose SelectedPhotos property you're setting.</param>
        /// <param name="value">Collection of items representing the SelectedPhotos.</param>
        public static void SetSelectedPhotos(DependencyObject obj, ObservableCollection<Photo> value)
        {
            obj.SetValue(SelectedPhotosProperty, value);
        }

        #endregion
    }
}
