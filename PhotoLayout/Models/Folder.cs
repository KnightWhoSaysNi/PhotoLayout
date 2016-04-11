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

        public Folder(string rootPath, Folder parent = null, IList<string> extensions = null)
        {
            this.FullPath = rootPath;
            this.parent = parent;
            this.extensions = extensions;

            if (extensions != null)
            {
                selectByExtension = true;
            }
        }

        #endregion

        #region - Properties -

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

        public string Name
        {
            get { return this.folder?.Name; }
        }

        public Folder Parent
        {
            get { return parent; }
            set
            {
                if (this.parent == null)
                {
                    parent = value;
                }
                else
                {
                    throw new ArgumentException($"{Name} already has a parent: {parent.Name}");
                }
            }
        }

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
            
        private void PopulateSubFolders()
        {
            if (this.folder == null)
            {
                return;
            }

            try
            {
                var dirInfos = this.folder.GetDirectories().Where(x => !(x.Attributes.HasFlag(FileAttributes.Hidden) || x.Attributes.HasFlag(FileAttributes.System))); // TODO Do not show certain folders
                foreach (DirectoryInfo directory in dirInfos)
                {
                    Folder subFolder = new Folder(directory.FullName, this, extensions);
                    subFolders.Add(subFolder);
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

        private void PopulateFiles()
        {
            if (this.folder == null)
            {
                return;
            }

            try
            {
                IEnumerable<FileInfo> fileInfos;

                if (selectByExtension)
                {
                    // this.Files will return only files with the correct extension
                    fileInfos = from file in this.folder.EnumerateFiles()
                                where this.extensions.Contains(file.Extension)
                                select file;
                }
                else
                {
                    // this.Files will return all files
                    fileInfos = this.folder.GetFiles();
                }

                foreach (FileInfo file in fileInfos)
                {                    
                        files.Add(file);
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
