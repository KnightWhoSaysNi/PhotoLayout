using Microsoft.Win32;
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
        
        //private ImageSource image;

        #endregion

        #region - Contructor - 

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;
            
            this.CommandBindings.Add(new CommandBinding(ApplicationCommands.Open, OnOpen));

            AllPhotos = new ObservableCollection<BitmapSource>();
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

        //public ImageSource Image
        //{
        //    get { return image; }
        //    set
        //    {
        //        this.image = value;
        //        OnPropertyChanged();
        //    }
        //}

        public ObservableCollection<BitmapSource> AllPhotos { get; set; }
        public List<BitmapImage> StoredPhotos { get; set; }

        #endregion  

        private void OnOpen(object sender, ExecutedRoutedEventArgs e)
        {
            // TODO Check if this releases memory!!! --- it does not without direct call to GC
            //AllPhotos.Clear();

            var openDialog = new OpenFileDialog();
            openDialog.Filter = "Image files (*.BMP;*.JPG;*.GIF)|*.BMP;*.JPG;*.GIF";
            openDialog.Multiselect = true;

            if (openDialog.ShowDialog() == true)
            {
                var imagePaths = openDialog.FileNames;

                //// Just creates a list of bitmap images
                //List<BitmapImage> bitmaps = new List<BitmapImage>();
                //foreach (var imagePath in imagePaths)
                //{
                //    var count = bitmaps.Count;
                //    bitmaps.Add(CreateImage(imagePath));
                //}
                //if (StoredPhotos == null)
                //{
                //    StoredPhotos = new List<BitmapImage>(bitmaps);
                //}
                //else
                //{
                //    StoredPhotos.AddRange(bitmaps);
                //}
                //return;
                

                // TODO Is this the best way of updating UI?
                var worker = new BackgroundWorker();

                worker.DoWork += (s, workerArgs) =>
                {
                    foreach (var imagePath in imagePaths)
                    {
                        BitmapSource bitmap = CreateImage(imagePath);
                        Dispatcher.BeginInvoke((Action)(() => AllPhotos.Add(bitmap)), DispatcherPriority.Background);
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

        private void ShowTopBar(object sender, RoutedEventArgs e)
        {
            topBorder.Visibility = Visibility.Visible;
            showTop.Visibility = Visibility.Collapsed;
        }

        private void CallGC(object sender, RoutedEventArgs e)
        {
            GC.Collect();
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
            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;

            // If this is not set memory is not being released at all, until AllPhotos is cleared AND GC is called. 
            // But with this set to a low value, memory is being released automatically, maybe after AllPhotos has been filled up
            bitmapImage.DecodePixelWidth = 1000;
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

        #region - Temp test methods -

        private void ClearStoredPhotos(object sender, RoutedEventArgs e)
        {
            if (AllPhotos != null)
            {
                AllPhotos.Clear();
                return;
            }

            if (StoredPhotos != null)
            {
                StoredPhotos.Clear();
            }

            if (AllPhotos.Count > 0)
            {
                AllPhotos.RemoveAt(0);
            }
        }

        private int count = 0;
        private void PreviewLastPhoto(object sender, RoutedEventArgs e)
        {
            AllPhotos.Add(StoredPhotos[count]);
            count++;
            if (count == StoredPhotos.Count)
            {
                count = 0;
            }
        }

        #endregion
    }
}
