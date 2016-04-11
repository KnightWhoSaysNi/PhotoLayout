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
        private Dictionary<Folder, List<Photo>> allPhotos;
        private IList<string> photoExtensions;

        #endregion

        #region - Constructors -

        public SelectionViewModel()
        {
            allPhotos = new Dictionary<Folder, List<Photo>>();
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
            //PreviousFolder = new RelayCommand(OnPreviousFolder, HasPreviousFolder);
            PhotoCollect = new RelayCommand(OnPhotoCollect);
        }

        #region - OpenFolder command -

        private void OnOpenFolder(object parameter)
        {

        }

        #endregion

        private bool CanPhotoCollect(object parameter)
        {
            // TODO See when each button can be used
            return true;
        }

        private void OnPhotoCollect(object parameter)
        {
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += Worker_DoWork;
            // TODO Enable cancelling of photo collection (additions to Photos)
            worker.WorkerSupportsCancellation = true;
            worker.RunWorkerAsync(parameter);            
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
                        break;
                }
            }
        }

        /// <summary>
        /// Collects folders and photos from the usb drive.
        /// </summary>
        /// <remarks>
        /// For testing purposes it gets all logical drives, not just the fixed usb drive
        /// </remarks>
        private void CollectUsbPhotos()
        {
            DriveInfo[] localDrives = DriveInfo.GetDrives();

            // ****************** TEST CODE **********************************
            // All drives will be shown and have this folder as root (parent)        
            Folder virtualRoot = new Folder("root", null, photoExtensions); 

            foreach (DriveInfo drive in localDrives)
            {
                Folder driveFolder = new Folder(drive.Name, virtualRoot, photoExtensions);
                virtualRoot.SubFolders.Add(driveFolder);
            }

            CurrentFolder = virtualRoot;
            RefreshCurrentFolder();

            // ***************** RELEASE CODE ********************************
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
        }

        private void RefreshCurrentFolder()
        {
            // Refreshes this.Folders so it holds CurrentFolder's SubFolders
            this.Folders.Clear();
            foreach (Folder folder in CurrentFolder.SubFolders)
            {
                System.Windows.Application.Current.Dispatcher.BeginInvoke((Action)(() => this.Folders.Add(folder)), DispatcherPriority.Background);
            }

            // Refreshes this.Photos so it holds CurrentFolder's Files - Photos
            this.Photos.Clear();
            if (!allPhotos.ContainsKey(CurrentFolder))
            {
                // First time initialization of the CurrentFolder's Photos OR the dictionary has released some memory and needs reinitialization
                allPhotos[CurrentFolder] = new List<Photo>();
                foreach (FileInfo file in CurrentFolder.Files)
                {
                    Photo photo = new Photo(new Uri(file.FullName), file.Name, file.Extension);
                    photo.RefreshBitmapSources();
                    allPhotos[CurrentFolder].Add(photo);
                }
            }

            foreach (Photo photo in allPhotos[CurrentFolder])
            {
                System.Windows.Application.Current.Dispatcher.BeginInvoke((Action)(() => this.Photos.Add(photo)), DispatcherPriority.Background);
            }
        }

        #endregion

        #region - Public methods -

        #endregion
    }
}
