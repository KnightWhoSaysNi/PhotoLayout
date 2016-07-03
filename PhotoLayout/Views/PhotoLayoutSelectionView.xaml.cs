using PhotoLayout.Helpers;
using PhotoLayout.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public partial class PhotoLayoutSelectionView : UserControl
    {        
        public PhotoLayoutSelectionView()
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

        /// <summary>
        /// Adds selected item (photo) to the SelectedPhotos collection
        /// </summary>
        private void allPhotosListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // TODO When switching folders automatically select photos that are in the SelectedPhotos collection
            // TODO DO NOT remove the first selected photo when changing folders

            if (allPhotosListBox.SelectedItems.Count > Constants.MaxSelectedPhotos)
            {
                // Maximum number of selected photos reached
                allPhotosListBox.SelectedItems.RemoveAt(allPhotosListBox.SelectedItems.Count - 1);
                return;
            }

            System.Collections.IList eAdded = e.AddedItems;
            System.Collections.IList eRemoved = e.RemovedItems;
            ObservableCollection<Photo> selectedItems = AttachedProperties.GetSelectedPhotos(allPhotosListBox);
            
            if (eAdded.Count != 0)
            {
                // New photo selected
                selectedItems.Add((Photo)eAdded[0]);
            }
            else if (eRemoved.Count != 0)
            {
                // Photo unselected
                selectedItems.Remove((Photo)eRemoved[0]); // When folders change and SelectionChanged is raised this removes the first photo from the SelectedPhotos
            }
        }
    }
}
