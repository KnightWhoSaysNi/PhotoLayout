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

            this.CommandBindings.Add(new CommandBinding(ApplicationCommands.Open, OnOpenPartitions));
            this.CommandBindings.Add(new CommandBinding(ApplicationCommands.New, OnNew));
            this.CommandBindings.Add(new CommandBinding(NavigationCommands.BrowseBack, OnBack));

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

        private void OnBack(object sender, ExecutedRoutedEventArgs e)
        {
            AllFolders.Clear();
            CurrentFolder = currentFolder.Parent == null ? null : currentFolder.Parent;
            if (currentFolder == null) // TODO Change logic for this - It must not be null
            {
                return;
            }

            foreach (var folder in currentFolder.SubFolders)
            {
                AllFolders.Add(folder);
            }

            AllPhotos.Clear();
            foreach (var item in PhotosByFolders[CurrentFolder])
            {
                Dispatcher.BeginInvoke((Action)(() => AllPhotos.Add(item)), DispatcherPriority.ApplicationIdle);
            }
        }

        private void OnNew(object sender, ExecutedRoutedEventArgs e)
        {
            Folder fold = e.Parameter as Folder;
            CurrentFolder = fold;

            AllFolders.Clear();
            foreach (var item in fold.SubFolders)
            {
                AllFolders.Add(item);
            }

            AllPhotos.Clear();
            if (!PhotosByFolders.ContainsKey(fold))
            {
                PhotosByFolders[fold] = new ObservableCollection<Photo>();
                foreach (var item in CurrentFolder.Files)
                {
                    Photo newPhoto = new Photo(new Uri(item.FullName), item.Name, item.Extension);
                    newPhoto.RefreshBitmapSources();
                    PhotosByFolders[fold].Add(newPhoto);
                    //Dispatcher.BeginInvoke((Action)(() => AllPhotos.Add(newPhoto)), DispatcherPriority.ApplicationIdle);
                }
                //AllPhotos = PhotosByFolders[fold];
            }
            
            foreach (var item in PhotosByFolders[fold])
            {
                Dispatcher.BeginInvoke((Action)(() => AllPhotos.Add(item)), DispatcherPriority.ApplicationIdle);    
            }            
        }
      
        private void OnOpenPartitions(object sender, ExecutedRoutedEventArgs e)
        {
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += Worker_DoWork;
            worker.RunWorkerAsync();
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            string[] partitions = Environment.GetLogicalDrives();
            Folder partitionE = new Folder(@"E:\", null, new List<string>() { ".jpg" });           

            // FIles in sub folders
            PopulateAllPhotos(partitionE);
        }

        private void PopulateAllPhotos(Folder root)
        {
            foreach (var file in root.Files)
            {
                Photo newPhoto = new Photo(new Uri(file.FullName), file.Name, file.Extension);
                newPhoto.RefreshBitmapSources();
                Dispatcher.BeginInvoke((Action)(() => AllPhotos.Add(newPhoto)), DispatcherPriority.ApplicationIdle);
            }

            foreach (var folder in root.SubFolders)
            {
                //PopulateAllPhotos(folder);
                Dispatcher.BeginInvoke((Action)(() => AllFolders.Add(folder)), DispatcherPriority.ApplicationIdle);
            }
        }

        private void OnOpen(object sender, ExecutedRoutedEventArgs e)
        {
            var openDialog = new OpenFileDialog();
            openDialog.Filter = "Image files (*.BMP;*.JPG;*.GIF)|*.BMP;*.JPG;*.GIF";
            openDialog.Multiselect = true;

            if (openDialog.ShowDialog() == true)
            {
                var imagePaths = openDialog.FileNames;

                // TODO Is this the best way of updating UI? Additional checks needed to see if the file is indeed a valid image (photo)
                var worker = new BackgroundWorker();

                worker.DoWork += (s, workerArgs) =>
                {
                    foreach (var imagePath in imagePaths)
                    {
                        BitmapSource bitmap = CreateImage(imagePath);
                        Dispatcher.BeginInvoke((Action)(() =>
                        {
                            Photo photo = new Photo(new Uri(imagePath), "some pic", ".jpg"); // TODO Get photo name from the image path first, or change Photo constructor
                            photo.RefreshBitmapSources();
                            AllPhotos.Add(photo);
                        }
                        ), DispatcherPriority.ApplicationIdle);
                    }
                };

                worker.RunWorkerAsync();


                //foreach (var imagePath in imagePaths)
                //{
                //    Dispatcher.BeginInvoke((Action)(() =>
                //    {
                //        BitmapImage bitmap = CreateImage(imagePath);
                //        AllPhotos.Add(bitmap);
                //    }), DispatcherPriority.ApplicationIdle);
                //}
            }
        }               

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

        #region - Temp test methods -

        private void ShowTopBar(object sender, RoutedEventArgs e)
        {
            topBorder.Visibility = Visibility.Visible;
            showTop.Visibility = Visibility.Collapsed;
        }

        // ***************************************************************
        // ******** NOT INTENDED FOR DIRECT LOADING OF PHOTOS ************
        // ******** SET TO ONLOAD FOR DIRECT LOAD, NONE OTHERWISE ********
        // ***************************************************************
        private BitmapSource CreateImage(string filePath)
        {
            //var source = new BitmapImage();
            //using (Stream stream = new FileStream(filePath, FileMode.Open))
            //{
            //    source.BeginInit();
            //    source.StreamSource = stream;
            //    source.CacheOption = BitmapCacheOption.OnLoad; // intentionally OnLoad
            //    source.EndInit();
            //    source.Freeze(); // TODO Check if this is necessary
            //}
            //return source;
            
            BitmapImage bitmapImage = new BitmapImage();

            bitmapImage.BeginInit();
            //bitmapImage.CreateOptions = BitmapCreateOptions.DelayCreation;
            bitmapImage.CacheOption = BitmapCacheOption.None; // None or OnLoad - doesn't matter for direct load as it is now

            // If this is not set memory is not being released at all, until AllPhotos is cleared AND GC is called. 
            // But with this set to a low value, memory is being released automatically, maybe after AllPhotos has been filled up
            bitmapImage.DecodePixelWidth = 300; // up to 800 might also be ok (more testing needed)
            bitmapImage.UriSource = new Uri(filePath, UriKind.RelativeOrAbsolute);
            // To save significant application memory, set the DecodePixelWidth or  
            // DecodePixelHeight of the BitmapImage value of the image source to the desired 
            // height or width of the rendered image. If you don't do this, the application will 
            // cache the image as though it were rendered as its normal size rather then just 
            // the size that is displayed.
            // Note: In order to preserve aspect ratio, set DecodePixelWidth
            // or DecodePixelHeight but not both.

            //bitmapImage.DecodePixelWidth = (int)imagePreview.ActualWidth; // Weird blur effect appears because of this
            bitmapImage.EndInit();
            bitmapImage.Freeze(); // TODO Check if the images can be resized after this

            return bitmapImage;
        }

        private void CreatePhotosAndAddToLayoutGrid(object sender, RoutedEventArgs e)
        {
            Photos.Clear();                                                   // HD - Wallpapers1_QlwttNW
            Photo p1 = new Photo(new Uri(@"C:\Users\bsod\Desktop\Sample Pictures\HD-Wallpapers1_QlwttNW.jpeg", UriKind.Absolute), "pic 1", ".jpg");
            Photo p2 = new Photo(new Uri(@"C:\Users\bsod\Desktop\Sample Pictures\hd_wallpapers_a13_qXpvPrU.jpg", UriKind.Absolute), "pic 2", ".jpg");
            Photo p3 = new Photo(new Uri(@"C:\Users\bsod\Desktop\Sample Pictures\Desktop-Wallpaper-HD2.jpg", UriKind.Absolute), "pic 3", ".jpg");
            Photo p4 = new Photo(new Uri(@"C:\Users\bsod\Desktop\Sample Pictures\Penguins.jpg", UriKind.Absolute), "pic 4", ".jpg");
            Photo p5 = new Photo(new Uri(@"C:\Users\bsod\Desktop\Sample Pictures\23-animation-wallpaper.preview.jpg", UriKind.Absolute), "pic 5", ".jpg");
            p1.RefreshBitmapSources();
            p2.RefreshBitmapSources();
            p3.RefreshBitmapSources();
            p4.RefreshBitmapSources();
            p5.RefreshBitmapSources();

            Photos.Add(p1);
            Photos.Add(p2);
            Photos.Add(p3);
            Photos.Add(p4);
            Photos.Add(p5);            
        }   
        
        public ObservableCollection<Photo> Photos { get; set; }

        #endregion
    }
}
