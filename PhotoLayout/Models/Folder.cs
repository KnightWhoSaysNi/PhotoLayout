using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoLayout.Models
{
    public class Folder
    {
        #region - Fields -
        private Folder parent;
        private DirectoryInfo folder;
        private ObservableCollection<Folder> subFolders;
        private ObservableCollection<FileInfo> files;
        private IList<string> extensions;
        private bool selectByExtension;

        #endregion

        #region - Constructors -

        /// <summary>
        /// Creates a structure that represents a folder with subfolders and files. If extensions are specified only finds certain files.
        /// </summary>
        /// <param name="fullPath">Full path to the folder.</param>
        /// <param name="parent">Parent folder.</param>
        /// <param name="extensions">Extensions for the files of the folder. Only files with certain extensions will be found.</param>
        public Folder(string fullPath, Folder parent = null, IList<string> extensions = null)
        {
            this.FullPath = fullPath;
            this.Parent = parent;
            this.extensions = extensions;

            if (extensions != null)
            {
                selectByExtension = true;
            }
        }

        #endregion

        #region - Properties -

        /// <summary>
        /// Gets or sets the full path of the folder.
        /// </summary>
        public string FullPath
        {
            get { return this.folder?.FullName; }
            set
            {
                if (Directory.Exists(value))
                {
                    this.folder = new DirectoryInfo(value);
                }
                else
                {
                    // TODO Determine what to do if the directory doesn't exist
                    // In situation where all partitions need a root, a "virtual Folder" is needed
                }
            }
        }

        /// <summary>
        /// Gets just the name part of the folder's FullPath.
        /// </summary>
        public string Name
        {
            get { return this.folder?.Name; }
        }

        /// <summary>
        /// Gets or sets the parent folder.
        /// </summary>
        public Folder Parent { get; set; }

        /// <summary>
        /// Gets a collection of subfolders.
        /// </summary>
        public ObservableCollection<Folder> SubFolders
        {
            get
            {
                if (subFolders == null)
                {
                    subFolders = new ObservableCollection<Folder>();
                    PopulateSubFolders();
                }
                return subFolders;
            }
        }

        /// <summary>
        /// Gets a collection of files.
        /// </summary>
        public ObservableCollection<FileInfo> Files
        {
            get
            {
                if (files == null)
                {
                    files = new ObservableCollection<FileInfo>();
                    PopulateFiles();
                }
                return files;
            }
        }

        #endregion

        #region - Private methods -
            
        /// <summary>
        /// Populates/fills the sub folders of the folder.
        /// </summary>
        private void PopulateSubFolders()
        {
            if (this.folder == null)
            {
                // This folder is a virtual folder and has no DirectoryInfo. Sub folders are added manually
                return;
            }

            try
            {
                IEnumerable<DirectoryInfo> directories = this.folder.GetDirectories(.
                    .Where(x => !(x.Attributes.HasFlag(FileAttributes.Hidden) || x.Attributes.HasFlag(FileAttributes.System))); // TODO Check which folders to ignore

                foreach (DirectoryInfo directory in directories)
                {
                    Folder subFolder = new Folder(directory.FullName, this, this.extensions);
                    this.subFolders.Add(subFolder);
                }
            }
            catch (UnauthorizedAccessException)
            {
                // Unauthorized folder accessed -> skip it and go to the next one                            
            }
            catch (System.Security.SecurityException)
            {
                // Unauthorized folder accessed -> skip it and go to the next one
            }
        }

        /// <summary>
        /// Populates/fills the files of the folder. If specified selects files by extension.
        /// </summary>
        private void PopulateFiles()
        {
            if (this.folder == null)
            {
                // This folder is a virtual folder and has no files by default
                return;
            }

            try
            {
                IEnumerable<FileInfo> fileInfos;

                if (this.selectByExtension)
                {
                    // this.Files will return only files with the correct extension                     
                    fileInfos = from file in this.folder.EnumerateFiles()
                                where this.extensions.Contains(file.Extension, StringComparer.CurrentCultureIgnoreCase)
                                select file;
                }
                else
                {
                    // this.Files will return all files
                    fileInfos = this.folder.GetFiles();
                }

                foreach (FileInfo file in fileInfos)
                {                    
                        this.files.Add(file);
                }
            }
            catch (UnauthorizedAccessException)
            {
                // Unauthorized file accessed -> skip it and go to the next one
            }
            catch (System.Security.SecurityException)
            {
                // Unauthorized file accessed -> skip it and go to the next one
            }
        }

        #endregion
    }
}
