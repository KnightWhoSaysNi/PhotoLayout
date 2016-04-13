using Microsoft.Win32;
using PhotoLayout.Helpers;
using PhotoLayout.Models;
using PhotoLayout.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
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
using System.Windows.Threading;

namespace PhotoLayout
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        #region - Fields -

        private Folder currentFolder;
        private Dictionary<Folder, ObservableCollection<Photo>> PhotosByFolders { get; set; }        

        #endregion

        #region - Contructor - 

        public MainWindow()
        {
            InitializeComponent();

            this.DataContext = this;

            AllPhotos = new ObservableCollection<Photo>();
            AllFolders = new ObservableCollection<Folder>();
            PhotosByFolders = new Dictionary<Folder, ObservableCollection<Photo>>();            
        }

        #endregion

        #region - Events -

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName]string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        #region - Properties -        

        public Folder CurrentFolder
        {
            get { return currentFolder; }
            set
            {
                currentFolder = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Photo> AllPhotos { get; set; }
        public ObservableCollection<Folder> AllFolders { get; set; }

        #endregion

        #region  - Private methods - 

        private void CallGC(object sender, RoutedEventArgs e)
        {
            GC.Collect();
        }

        private void ClearAllPhotos(object sender, RoutedEventArgs e)
        {
            if (AllPhotos != null)
            {
                AllPhotos.Clear();                
            }
        }

        #endregion       
    }
}
