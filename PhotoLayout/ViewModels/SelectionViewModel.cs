using PhotoLayout.Enums;
using PhotoLayout.Helpers;
using PhotoLayout.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;

namespace PhotoLayout.ViewModels
{
    public class SelectionViewModel : BindableBase
    {
        #region - Fields -

        private Folder currentFolder;
        private Dictionary<Folder, List<Photo>> photoDictionary;
        private IList<string> photoExtensions;

        private BackgroundWorker refreshWorker;
        private BackgroundWorker photoCollectionWorker;

        #endregion

        #region - Constructors -

        public SelectionViewModel()
        {   
            InitializeFields();
            InitializeWorkers();
            InitializeCommands();            
        }

        #endregion

        #region - Properties -

        #region - Commands -

        public ICommand OpenFolder { get; set; }
        public ICommand PreviousFolder { get; set; }
        public ICommand PhotoCollect { get; set; }

        #endregion

        public ObservableCollection<Folder> Folders { get; set; }
        public ObservableCollection<Photo> Photos { get; set; }

        public Folder CurrentFolder
        {
            get { return currentFolder; }
            set
            {
                currentFolder = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region - Private methods -

        #region - Constructor initializations -

        private void InitializeFields()
        {
            // TODO Check which extensions can be used and mayhaps put them somewhere else
            this.photoExtensions=new List<string>() { ".jpg", ".jpeg", ".png", ".bmp", ".tiff" };
            this.photoDictionary = new Dictionary<Folder, List<Photo>>();
                        
            Folders = new ObservableCollection<Folder>();
            Photos = new ObservableCollection<Photo>();
        }

        private void InitializeWorkers()
        {
            photoCollectionWorker = new BackgroundWorker();
            photoCollectionWorker.WorkerSupportsCancellation = true; // TODO Write cancellation logic
            photoCollectionWorker.DoWork += PhotoCollectionWorkerDoWork;
            photoCollectionWorker.RunWorkerCompleted += RunPhotoCollectionWorkerCompleted;

            refreshWorker = new BackgroundWorker();
            refreshWorker.WorkerReportsProgress = true;
            refreshWorker.WorkerSupportsCancellation = true; // TODO Write cancellation logic
            refreshWorker.DoWork += RefreshWorkerDoWork;
            refreshWorker.ProgressChanged += OnRefreshWorkerProgressChanged;
        }

        private void InitializeCommands()
        {
            OpenFolder = new RelayCommand(OnOpenFolder);
            PreviousFolder = new RelayCommand(x => OnPreviousFolder(), x => HasPreviousFolder());
            PhotoCollect = new RelayCommand(OnPhotoCollect);
        }

        #endregion

        #region - OpenFolder Command -

        private void OnOpenFolder(object parameter)
        {
            Folder selectedFolder = parameter as Folder;
            if (selectedFolder != null)
            {
                CurrentFolder = selectedFolder;

                RefreshCurrentFolder();
            }
        }

        #endregion

        #region - PreviousFolder Command -

        private void OnPreviousFolder()
        {
            CurrentFolder = CurrentFolder.Parent;

            if (refreshWorker.IsBusy || photoCollectionWorker.IsBusy)
            {
                refreshWorker.CancelAsync();
                photoCollectionWorker.CancelAsync();                
            }

            RefreshCurrentFolder();            
        }

        private bool HasPreviousFolder()
        {
            bool hasPreviousFolder =  CurrentFolder?.Parent != null;
            return hasPreviousFolder;
        }

        #endregion

        #region - PhotoCollect Command -        

        private void OnPhotoCollect(object parameter)
        {
            // Background worker way
            // TODO Initial collecting from a file source -> Some animation should be enabled
            this.photoCollectionWorker.RunWorkerAsync(parameter);
            // TODO Stop loading/photo collecting animation at this point   

            // Threadpool way            
        }

        private void RunPhotoCollectionWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            RefreshCurrentFolder();
            // TODO Stop animation ^
        }

        /*************************************************************************************
        ******************   Threadpool methods   ********************************************
        *************************************************************************************/

        private void CollectPhotosWithTaskFactory()
        {
            Task.Factory.StartNew(() =>
            {
                foreach (var item in photoDictionary[CurrentFolder])
                {
                    Photos.Add(item);
                }
            });
            
        }


        /*************************************************************************************
        ****************** Background worker methods *****************************************
        *************************************************************************************/

        private void PhotoCollectionWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            string source = e.Argument as string;
            
            if (source != null)
            {
                switch (source)
                {
                    case "Facebook":
                        // Get albums and photos from facebook
                        break;
                    case "Instagram":
                        // Get photos from instagram
                        break;
                    case "Usb":
                        DisplayUsbPhotos();
                        break;
                    case "EmailCode":
                        // Get photos sent to Easy2U email
                        break;
                    case "Dropbox":
                        // Get folders and photos from dropbox
                        break;
                    case "GoogleDrive":
                        // Get folders and photos from google drive
                        break;
                    case "Recent":
                        // TODO Collect recent photos
                        DisplayRecentPhotos();
                        break;
                }
            }
        }        

        private bool CanPhotoCollect(object parameter)
        {
            // TODO See when each button can be used
            return true;
        }

        #endregion

        #region - Usb photos -

        /// <summary>
        /// Display folders and photos from the usb drive.
        /// </summary>
        /// <remarks>
        /// For testing purposes it gets all logical drives, not just the fixed usb drive.
        /// </remarks>
        private void DisplayUsbPhotos()
        {
            DriveInfo[] localDrives = DriveInfo.GetDrives();

            #region "TEST CODE"

            // All drives will be shown and have this folder as root (parent)        
            Folder virtualRoot = new Folder("root", null, photoExtensions);

            foreach (DriveInfo drive in localDrives)
            {                
                Folder driveAsFolder = new Folder(drive.Name, virtualRoot, photoExtensions);
                virtualRoot.SubFolders.Add(driveAsFolder);
            }

            CurrentFolder = virtualRoot;

            #endregion

            #region "RELEASE CODE"

            //foreach (DriveInfo drive in localDrives)
            //{
            //    if (drive.DriveType == DriveType.Removable)
            //    {
            //        // In Easy2U there can be only 1 usb device in the machine
            //        Folder usb = new Folder(drive.Name, null, photoExtensions);
            //        CurrentFolder = usb;

            //        return;
            //    }
            //}

            #endregion
        }

        #endregion

        #region - Recent photos -

        private void DisplayRecentPhotos()
        {
            // TODO Create 'Recent Photos' folder somewhere, where saved photos will reside            
            string recentFolderPath = @"../Recent Photos/";

            // TODO Refactor this so it works
            Folder recentFolder = new Folder(recentFolderPath, null, photoExtensions);
            CurrentFolder = recentFolder;
        }

        #endregion

        #region - RefreshCurrentFolder: Update Folders and Photos -

        /// <summary>
        /// Updates Folders and Photos so they reflect what CurrentFolder holds.
        /// </summary>
        private void RefreshCurrentFolder()
        {
            this.refreshWorker.RunWorkerAsync();
        }       

        private void RefreshWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            UpdateFolders(e); 
            UpdatePhotos(e);           
        }

        /// <summary>
        /// Updates <see cref="Folders"/> so it holds the CurrentFolder's SubFolders.
        /// </summary>
        private void UpdateFolders(DoWorkEventArgs e)
        {
            System.Windows.Application.Current.Dispatcher.BeginInvoke((Action)(() => Folders.Clear()), DispatcherPriority.Background);

            foreach (Folder folder in CurrentFolder.SubFolders)
            {
                if (photoCollectionWorker.CancellationPending || refreshWorker.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }
                System.Windows.Application.Current.Dispatcher.BeginInvoke((Action)(() => Folders.Add(folder)), DispatcherPriority.Background);
            }
        }

        /// <summary>
        /// Updates <see cref="Photos"/> so it holds the CurrentFolder's Files - Photos.
        /// </summary>
        private void UpdatePhotos(DoWorkEventArgs e)
        {
            System.Windows.Application.Current.Dispatcher.BeginInvoke((Action)(() => Photos.Clear()), DispatcherPriority.Background);

            if (!this.photoDictionary.ContainsKey(CurrentFolder))
            {
                // First time initialization of the CurrentFolder's Photos OR the dictionary has released some memory and needs reinitialization
                this.photoDictionary[CurrentFolder] = new List<Photo>();

                for (int i = 0; i < CurrentFolder.Files.Count; i++)                    
                {
                    if (photoCollectionWorker.CancellationPending || refreshWorker.CancellationPending)
                    {
                        e.Cancel = true;
                        return;
                    }
                    
                    FileInfo file = CurrentFolder.Files[i];                        
                    Photo photo = new Photo(new Uri(file.FullName), file.Name, file.Extension);
                    photo.RefreshBitmapSource(BitmapType.Thumbnail);
                    this.photoDictionary[CurrentFolder].Add(photo);

                    int percentProgress = i + 1 / CurrentFolder.Files.Count;
                    this.refreshWorker.ReportProgress(percentProgress, photo);
                }
            }
            else
            {
                foreach (Photo photo in this.photoDictionary[CurrentFolder])
                {
                    if (photoCollectionWorker.CancellationPending || refreshWorker.CancellationPending)
                    {
                        e.Cancel = true;
                        return;
                    }

                    System.Windows.Application.Current.Dispatcher.BeginInvoke((Action)(() => Photos.Add(photo)), DispatcherPriority.Background);
                }
            }
        }

        private void OnRefreshWorkerProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            Photo photo = e.UserState as Photo;
            if (photo != null)
            {
                System.Windows.Application.Current.Dispatcher.BeginInvoke((Action)(() => Photos.Add(photo)), DispatcherPriority.Background);
                //Photos.Add(photo); // Weird stuff happens with this
            }
        }

        #endregion

        #endregion

        #region - Public methods -

        #endregion
    }
}
