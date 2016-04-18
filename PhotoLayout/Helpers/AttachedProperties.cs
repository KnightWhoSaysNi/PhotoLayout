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
        #region - ListBox SelectedItems -

        public static readonly DependencyProperty SelectedItemsProperty =
            DependencyProperty.RegisterAttached("SelectedItems", 
                typeof(ObservableCollection<object>), 
                typeof(AttachedProperties), 
                new PropertyMetadata(null, OnPropertyChanged));

        private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            
        }

        /// <summary>
        /// Gets a value of SelectedItems for the specified ListBox.
        /// </summary>
        /// <param name="obj">ListBox whose SelectedItems property you're getting.</param>
        public static ObservableCollection<object> GetSelectedItems(ListBox obj)
        {
            return (ObservableCollection<object>)obj.GetValue(SelectedItemsProperty);
        }

        /// <summary>
        /// Sets a value of SelectedItems for the specified ListBox.
        /// </summary>
        /// <param name="obj">ListBox whose SelectedItems property you're setting.</param>
        /// <param name="value">Collection of items representing the SelectedItems.</param>
        public static void SetSelectedItems(DependencyObject obj, ObservableCollection<object> value)
        {
            obj.SetValue(SelectedItemsProperty, value);
        }

        #endregion
    }
}
