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
        #region - ListBox SelectedPhotos -

        public static readonly DependencyProperty SelectedPhotosProperty =
            DependencyProperty.RegisterAttached("SelectedPhotos", 
                typeof(ObservableCollection<Photo>), 
                typeof(AttachedProperties), 
                new PropertyMetadata(null, OnPropertyChanged));

        private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            
        }

        public static ObservableCollection<Photo> GetSelectedPhotos(ListBox obj)
        {
            return (ObservableCollection<Photo>)obj.GetValue(SelectedPhotosProperty);
        }

        public static void SetSelectedPhotos(DependencyObject obj, ObservableCollection<Photo> value)
        {
            obj.SetValue(SelectedPhotosProperty, value);
        }




        #endregion
    }
}
