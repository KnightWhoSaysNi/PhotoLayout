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

        #endregion

        #region - Constructors -

        public SelectionViewModel()
        {
            photoDictionary = new Dictionary<Folder, List<Photo>>();
            Folders = new ObservableCollection<Folder>();
            Photos = new ObservableCollection<Photo>();

            // TODO Check which extensions can be used and mayhaps put them somewhere else
            photoExtensions = new List<string>() { ".jpg", ".png", ".gif", ".jpeg" };

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

        private void InitializeCommands()
        {
            OpenFolder = new RelayCommand(OnOpenFolder);
            PreviousFolder = new RelayCommand(x => OnPreviousFolder(), x => HasPreviousFolder());
            PhotoCollect = new RelayCommand(OnPhotoCollect);
        }

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
            // TODO Initial collecting from a file source -> Some animation should be enabled
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += Worker_DoWork;
            // TODO Enable cancelling of photo collection (additions to Photos)
            worker.WorkerSupportsCancellation = true;
            worker.RunWorkerAsync(parameter);
            worker.ProgressChanged += Worker_ProgressChanged;                      
            // TODO Stop loading/photo collecting animation at this point   
        }

        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
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
            RefreshCurrentFolder();

            #endregion

            #region "RELEASE CODE"

            //foreach (DriveInfo drive in localDrives)
            //{
            //    if (drive.DriveType == DriveType.Removable)
            //    {
            //        // In Easy2U there can be only 1 usb device in the machine
            //        Folder usb = new Folder(drive.Name, null, photoExtensions);
            //        CurrentFolder = usb;
            //        RefreshCurrentFolder();

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
            RefreshCurrentFolder();
        }

        #endregion

        #region - RefreshCurrentFolder: Update Folders and Photos -

        /// <summary>
        /// Updates Folders and Photos so they reflect what CurrentFolder holds.
        /// </summary>
        private void RefreshCurrentFolder()
        {
            UpdateFolders();
            UpdatePhotos();            
        }

        /// <summary>
        /// Updates <see cref="Folders"/> so it holds the CurrentFolder's SubFolders.
        /// </summary>
        private void UpdateFolders()
        {
            System.Windows.Application.Current.Dispatcher.BeginInvoke((Action)(() => Folders.Clear()), DispatcherPriority.Background);
            foreach (Folder folder in CurrentFolder.SubFolders)
            {
                System.Windows.Application.Current.Dispatcher.BeginInvoke((Action)(() => Folders.Add(folder)), DispatcherPriority.Background);
            }
        }

        /// <summary>
        /// Updates <see cref="Photos"/> so it holds the CurrentFolder's Files - Photos.
        /// </summary>
        private void UpdatePhotos()
        {
            System.Windows.Application.Current.Dispatcher.BeginInvoke((Action)(() => Photos.Clear()), DispatcherPriority.Background);
            if (!this.photoDictionary.ContainsKey(CurrentFolder))
            {
                // First time initialization of the CurrentFolder's Photos OR the dictionary has released some memory and needs reinitialization
                this.photoDictionary[CurrentFolder] = new List<Photo>();
                foreach (FileInfo file in CurrentFolder.Files)
                {
                    Photo photo = new Photo(new Uri(file.FullName), file.Name, file.Extension);
                    photo.RefreshBitmapSources();
                    this.photoDictionary[CurrentFolder].Add(photo);                    
                }
            }

            foreach (Photo photo in this.photoDictionary[CurrentFolder])
            {
                System.Windows.Application.Current.Dispatcher.BeginInvoke((Action)(() => Photos.Add(photo)), DispatcherPriority.ApplicationIdle);
            }
        }

        #endregion


        #endregion

        #region - Public methods -

        #endregion
    }
}
