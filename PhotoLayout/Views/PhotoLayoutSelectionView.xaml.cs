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
            SelectedPhotos = new ObservableCollection<Photo>();
        }
        
        public ObservableCollection<Photo> SelectedPhotos { get; set; }

        /// <summary>
        /// Stops auto scrolling of selected item into view. When selected item is partially visible stops the ListBox from auto scrolling to it.
        /// </summary>
        private void OnRequestBringIntoView(object sender, RequestBringIntoViewEventArgs e)
        {
            e.Handled = true;
        }

        private void allPhotosListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox listBox = sender as ListBox;

            if (listBox != null)
            {
                if (listBox.SelectedItems.Count > Constants.MaxPhotosInLayoutGrid)
                {
                    // Maximum number of selected photos reached
                    listBox.SelectedItems.RemoveAt(listBox.SelectedItems.Count - 1);
                    return;
                }

                System.Collections.IList eAdded = e.AddedItems;
                System.Collections.IList eRemoved = e.RemovedItems;
                ObservableCollection<Photo> selectedItems = AttachedProperties.GetSelectedPhotos(listBox);

                if (eAdded.Count != 0)
                {
                    selectedItems.Add((Photo)eAdded[0]);
                }
                else if (eRemoved.Count != 0)
                {
                    selectedItems.Remove((Photo)eRemoved[0]);
                }
            }
        }
    }
}
