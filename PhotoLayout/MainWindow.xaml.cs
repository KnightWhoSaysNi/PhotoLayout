using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
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

        private int currentImageIndex = 0;
        private ImageSource image;

        #endregion

        #region - Contructor - 

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;
            
            this.CommandBindings.Add(new CommandBinding(ApplicationCommands.Open, OnOpen));

            AllPhotos = new ObservableCollection<BitmapImage>();
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

        public ImageSource Image
        {
            get { return image; }
            set
            {
                this.image = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<BitmapImage> AllPhotos { get; set; }

        #endregion  

        private void OnOpen(object sender, ExecutedRoutedEventArgs e)
        {
            var openDialog = new OpenFileDialog();
            openDialog.Filter = "Image files (*.BMP;*.JPG;*.GIF)|*.BMP;*.JPG;*.GIF";
            openDialog.Multiselect = true;
            if (openDialog.ShowDialog() == true)
            {
                var imagePaths = openDialog.FileNames;


                var worker = new BackgroundWorker();
                //worker.WorkerReportsProgress = true;

                worker.DoWork += (s, workerArgs) =>
                {
                    int count = 0;
                    foreach (var imagePath in imagePaths)
                    {
                        if (count % 10 == 0)
                        {
                            Thread.Sleep(100);
                        }
                        BitmapImage bitmap = CreateImage(imagePath);
                        //worker.ReportProgress(count / imagePaths.Length, bitmap);
                        Dispatcher.BeginInvoke((Action)(() => AllPhotos.Add(bitmap)), DispatcherPriority.Background);
                        count++;
                    }
                };

                //worker.ProgressChanged += (s, workerArgs) =>
                //{
                //    BitmapImage bitmap = (BitmapImage)workerArgs.UserState;

                //Dispatcher.BeginInvoke((Action)(() => AllPhotos.Add(bitmap)), DispatcherPriority.Background);
                //    //AllPhotos.Add(bitmap);
                //};

                //Dispatcher.BeginInvoke((Action)(() => worker.RunWorkerAsync()), DispatcherPriority.ApplicationIdle);
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

        private void ShowFirstImage(object sender, RoutedEventArgs e)
        {
            //if (AllPhotos == null || AllPhotos.Count == 0)
            //{
            //    // No images to show
            //    // TODO Add a 'No Image' image to show in the image preview
            //    return;
            //}

            //if (!CreateImage(AllPhotos[0]))
            //{
            //    // TODO Same as the above check, but with some error message 
            //}
            //currentImageIndex = 0;
        }

        private BitmapImage CreateImage(string filePath)
        {
            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            bitmapImage.DecodePixelHeight = 800;
            
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

            //imagePreview.Source = bitmapImage;
            return bitmapImage;
        }

        private void NextImage(object sender, RoutedEventArgs e)
        {
            //    if (AllPhotos == null || AllPhotos.Count == 0)
            //    {
            //        System.Diagnostics.Debug.WriteLine("There are no images");
            //        return;
            //    }

            //    currentImageIndex++;
            //    if (currentImageIndex == AllPhotos.Count)
            //    {
            //        currentImageIndex = 0;
            //    }

            //    CreateImage(AllPhotos[currentImageIndex]);
        }

        private void PreviousImage(object sender, RoutedEventArgs e)
        {
            //if (AllPhotos == null || AllPhotos.Count == 0)
            //{
            //    System.Diagnostics.Debug.WriteLine("There are no images");
            //    return;
            //}

            //if (currentImageIndex == 0)
            //{
            //    currentImageIndex = AllPhotos.Count;
            //}
            //currentImageIndex--;

            //CreateImage(AllPhotos[currentImageIndex]);
        }
    }
}
