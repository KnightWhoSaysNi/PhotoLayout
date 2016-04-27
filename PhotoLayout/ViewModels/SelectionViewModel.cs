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
using System.Windows.Input;
using System.Windows.Threading;

namespace PhotoLayout.ViewModels
{
    /* TODO
    Work in conjunction with VirtualizingWrapPanel and its UI and  Data virtualization. 
    Folder A loads 100 photos, then folders B and C load 150 photos each. Lets say that the limit is set to 300 photos to be held in memory.
    When folder D tries to load its photos the ViewModel should go through the photosByFolders dictionary and empty the amount of photos that
    D has or if it's above the max VirtualizingWrapPanel's value set it to that max value; or if D only wants to load 50photos, then only A needs to be cleared.
    If D wants to load 200 photos, both A and B need to release memory -> photo.Thumbnail (and maybe other bitmap types) need to be set to null
    and GC.Collect needs to be called before D actually starts loading its photos.
    */
    public class SelectionViewModel : BindableBase
    {
        #region - Fields -

        private Folder currentFolder;
        /// <summary>
        /// A collection of folders, each of which contains a list of photos.
        /// </summary>
        private Dictionary<Folder, List<Photo>> photoDictionary;
        /// <summary>
        /// Number of photos by folders. Used in data virtualization.
        /// </summary>
        private Dictionary<Folder, int> photosByFolders;        

        private BackgroundWorker refreshWorker;
        private BackgroundWorker photoCollectionWorker;

        #endregion

        #region - Constructors -

        /// <summary>
        /// Creates a view model responsible for the logic around getting and selecting the photos used in a manipulation view model.
        /// </summary>
        public SelectionViewModel()
        {   
            InitializeFields();
            InitializeWorkers();
            InitializeCommands();
        }

        #endregion

        #region - Properties -

        #region - Commands -

        public ICommand OpenFolder { get; set; } // Command for opening the folder
        public ICommand PreviousFolder { get; set; } // Command for going up the folder hierarchy tree
        public ICommand PhotoCollect { get; set; } // Command for collecting photos from a file source

        #endregion

        /// <summary>
        /// Gets or sets a collection of folders.
        /// </summary>
        public ObservableCollection<Folder> Folders { get; set; }

        /// <summary>
        /// Gets or sets a collection of photos.
        /// </summary>
        public ObservableCollection<Photo> Photos { get; set; }

        /// <summary>
        /// Gets or sets a collection of selected photos.
        /// </summary>
        public ObservableCollection<Photo> SelectedPhotos { get; set; }

        /// <summary>
        /// Gets or sets the currently selected folder.
        /// </summary>
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

        #region - Public methods -

        #endregion

        #region - Private methods -

        #region - Constructor initializations -

        /// <summary>
        /// Initializes fields and properties that need initialization through constructor.
        /// </summary>
        private void InitializeFields()
        {
            this.photoDictionary = new Dictionary<Folder, List<Photo>>();
            this.photosByFolders = new Dictionary<Folder, int>();        

            Folders = new ObservableCollection<Folder>();
            Photos = new ObservableCollection<Photo>();
            SelectedPhotos = new ObservableCollection<Photo>();
        }

        /// <summary>
        /// Initializes background workers for collecting new and refreshing current folders and photos.
        /// </summary>
        private void InitializeWorkers()
        {
            photoCollectionWorker = new BackgroundWorker();
            photoCollectionWorker.WorkerSupportsCancellation = true; 
            photoCollectionWorker.DoWork += PhotoCollectionWorkerDoWork;
            photoCollectionWorker.RunWorkerCompleted += RunPhotoCollectionWorkerCompleted;

            refreshWorker = new BackgroundWorker();
            refreshWorker.WorkerReportsProgress = true;
            refreshWorker.WorkerSupportsCancellation = true;
            refreshWorker.DoWork += RefreshWorkerDoWork;
            refreshWorker.ProgressChanged += OnRefreshWorkerProgressChanged;
        }

        /// <summary>
        /// Initializes commands.
        /// </summary>
        private void InitializeCommands()
        {
            OpenFolder = new RelayCommand(OnOpenFolder);
            PreviousFolder = new RelayCommand(x => OnPreviousFolder(), x => HasPreviousFolder());
            PhotoCollect = new RelayCommand(OnPhotoCollect);            
        }

        #endregion

        #region - OpenFolder Command -

        /// <summary>
        /// Sets CurrentFolder property to the specified new folder and calls <see cref="RefreshCurrentFolder"/> method to update folders and photos.
        /// </summary>
        /// <param name="parameter">Folder being set as CurrentFolder.</param>
        private void OnOpenFolder(object parameter)
        {
            Folder selectedFolder = parameter as Folder;
            if (selectedFolder != null)
            {
                if (this.refreshWorker.IsBusy)
                {
                    // Refresher worker is already working on something, so cancel the work as this method is being called for the second time
                    this.refreshWorker.CancelAsync();
                }

                CurrentFolder = selectedFolder;
                RefreshCurrentFolder();
            }
        }

        #endregion

        #region - PreviousFolder Command -

        /// <summary>
        /// Sets CurrentFolder to its parent -> goes back up the hierarchy tree.
        /// </summary>
        private void OnPreviousFolder()
        {
            if (refreshWorker.IsBusy)
            {
                // Refresher worker is already working on something, so cancel the work as this method is being called for the second time
                refreshWorker.CancelAsync();
            }

            CurrentFolder = CurrentFolder.Parent;
            RefreshCurrentFolder();            
        }
        
        /// <summary>
        /// Checks if CurrentFolder has a parent folder.
        /// </summary>
        /// <returns>True if CurrentFolder has a parent folder, false otherwise.</returns>
        private bool HasPreviousFolder()
        {
            bool hasPreviousFolder =  CurrentFolder?.Parent != null;
            return hasPreviousFolder;
        }

        #endregion

        #region - PhotoCollect Command -        

        /// <summary>
        /// Stops busy workers and calls poto collection worker to collect photos from the specified file source.
        /// </summary>
        /// <param name="parameter">Files source from which to get photos.</param>
        private void OnPhotoCollect(object parameter)
        {
            if (this.photoCollectionWorker.IsBusy || this.refreshWorker.IsBusy)
            {
                // The user chose a different file source while the first one is still collecting information and/or photos are
                // still being displayed on screen so work is cancelled for both workers so they can be restarted with new values
                this.photoCollectionWorker.CancelAsync();
                this.refreshWorker.CancelAsync();

                while (this.photoCollectionWorker.IsBusy || this.refreshWorker.IsBusy)
                {
                    System.Windows.Forms.Application.DoEvents();
                }
            }

            // TODO Initial collecting from a file source -> Some animation should be enabled
            this.photoCollectionWorker.RunWorkerAsync(parameter); // PhotoCollectionWorkerDoWork
        }

        /// <summary>
        /// Collects photos from the file source specified in the argument.
        /// </summary>
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
                        CollectUsbPhotos();
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
                        CollectRecentPhotos();
                        break;
                }
            }
        }        

        /// <summary>
        /// Photo collection is completed, calling for refresh of the current folder.
        /// </summary>
        private void RunPhotoCollectionWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            RefreshCurrentFolder();
        }

        private bool CanPhotoCollect(object parameter)
        {
            // TODO See when each button can be used
            return true;
        }

        #endregion

        #region - Usb photos -

        /// <summary>
        /// Collects folders and photos from the usb drive. Sets the usb drive as the current folder.
        /// </summary>
        /// <remarks>
        /// For testing purposes it gets all logical drives, not just the fixed usb drive.
        /// </remarks>
        private void CollectUsbPhotos()
        {
            DriveInfo[] localDrives = DriveInfo.GetDrives();

            #region "TEST CODE"

            // All drives will be shown and have this folder as root (parent)        
            Folder virtualRoot = new Folder("Root folder", null, Constants.PhotoExtensions);

            // Each drive is represented as a folder and added to virtualRoot, which represents the CurrentFolder
            foreach (DriveInfo drive in localDrives)
            {                
                Folder driveAsFolder = new Folder(drive.Name, virtualRoot, Constants.PhotoExtensions);
                virtualRoot.SubFolders.Add(driveAsFolder);
            }
            CurrentFolder = virtualRoot;

            #endregion

            #region "RELEASE CODE"

            //foreach (DriveInfo drive in localDrives)
            //{
            //    if (drive.DriveType == DriveType.Removable)
            //    {
            //        // There can be only 1 usb device in the machine
            //        Folder usb = new Folder(drive.Name, null, Constants.PhotoExtensions);
            //        CurrentFolder = usb;

            //        return;
            //    }
            //}

            #endregion
        }

        #endregion

        #region - Recent photos -

        /// <summary>
        /// Collects recent folders and photos. Sets the recent folder as the current folder.
        /// </summary>
        private void CollectRecentPhotos()
        {
            // TODO Create 'Recent Photos' folder somewhere, where saved photos will reside            
            string recentFolderPath = @"../Recent Photos/";
            Folder recentFolder = new Folder(recentFolderPath, null, Constants.PhotoExtensions);
            CurrentFolder = recentFolder;
        }

        #endregion

        #region - RefreshCurrentFolder: Update Folders and Photos -

        /// <summary>
        /// Updates Folders and Photos so they reflect what CurrentFolder holds.
        /// </summary>
        private void RefreshCurrentFolder()
        {
            while (this.refreshWorker.IsBusy)
            {
                // Waiting for refresh worker to stop his current work, before starting a new one
                System.Windows.Forms.Application.DoEvents();
            }

            this.refreshWorker.RunWorkerAsync(); // RefreshWorkerDoWork
        }

        /// <summary>
        /// Calls for updates of Folders and Photos collections.
        /// </summary>
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
            // First clear the current folders
            System.Windows.Application.Current.Dispatcher.BeginInvoke((Action)(() => Folders.Clear()), DispatcherPriority.Background);

            foreach (Folder folder in CurrentFolder.SubFolders)
            {
                if (this.refreshWorker.CancellationPending)
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
            // First clear the current photos
            System.Windows.Application.Current.Dispatcher.BeginInvoke((Action)(() => Photos.Clear()), DispatcherPriority.Background);
            
            // Check if the folder was created/loaded before
            if (!this.photoDictionary.ContainsKey(CurrentFolder))
            {
                // This is the first time initialization of the CurrentFolder's Photos OR the dictionary has removed some key-value pairs and needs reinitialization
                this.photoDictionary[CurrentFolder] = new List<Photo>();
            }

            for (int i = 0; i < CurrentFolder.Files.Count; i++)
            {
                if (this.refreshWorker.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }

                FileInfo file = CurrentFolder.Files[i];                
                Photo photo = new Photo(new Uri(file.FullName), file.Name, file.Extension);

                if (this.photoDictionary[CurrentFolder].Contains(photo))
                {
                    // Work was cancelled, but photo was already created, so no need to do it twice. Go to the next iteration and check the next photo
                    continue;
                }
                photo.RefreshBitmapSource(BitmapType.Thumbnail);
                this.photoDictionary[CurrentFolder].Add(photo);

                int progressPercentage = i + 1 / CurrentFolder.Files.Count;
                this.refreshWorker.ReportProgress(progressPercentage, photo);
            }
        }

        /// <summary>
        /// Reporting on refresh worker's progress. Adding a photo to the Photos collection.
        /// </summary>
        private void OnRefreshWorkerProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            Photo photo = e.UserState as Photo;
            if (photo != null && photo.Thumbnail != null)
            {
                System.Windows.Application.Current.Dispatcher.BeginInvoke((Action)(() => Photos.Add(photo)), DispatcherPriority.Background);
                //Photos.Add(photo); // Weird stuff happens with this even though ProgressChanged works in UI thread
            }
        }

        #endregion

        #endregion

    }
}
