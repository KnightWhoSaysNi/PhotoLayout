using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.IO;
using System.Drawing;
using PhotoLayout.Enums;
using System.Runtime.CompilerServices;

namespace PhotoLayout.Models
{
    public class Photo : INotifyPropertyChanged
    {
        #region - Fields -        

        private string name;
        private string extension;
        private bool isSaved;
        private BitmapSource originalBitmap;
        private BitmapSource thumbnail;
        private BitmapSource previewBitmap;

        #endregion

        #region - Constructors -
        
        // On Save a name is given (extension as well, most likely) and an image is saved to temp folder with a specific uri.
        // Part of that uri is specified here, with the name and extension
        /// <summary>
        /// Creates a photo object; a representation of an image/photograph.
        /// </summary>
        /// <param name="uri">Uri of the photo.</param>
        /// <param name="name">Name of the photo (file).</param>
        /// <param name="extension">File extension of the photo: .jpg or .png or others.</param>
        public Photo(Uri uri, string name, string extension)
        {
            this.PhotoUri = uri;
            this.name = name;
            this.extension = extension;
        }

        #endregion

        #region - Events -

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName]string propertyName="")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        #region - Properties -

        /// <summary>
        /// Gets or privately sets the uri of the photo.
        /// </summary>
        public Uri PhotoUri { get; private set; }

        /// <summary>
        /// Gets or sets the bitmap source of the original file that the photo has been made of, i.e. without decoded pixels.
        /// </summary>
        public BitmapSource OriginalBitmap
        {
            get
            {
                return originalBitmap;
            }
            set
            {
                originalBitmap = value;
                OnPropertyChanged();
            }        
        }

        /// <summary>
        /// Gets or sets the bitmap source for the thumbnail of the photo, i.e. with decoded pixel width of a certain value.
        /// </summary>
        public BitmapSource Thumbnail
        {
            get
            {
                return thumbnail;
            }
            set
            {
                thumbnail = value;
                OnPropertyChanged();
            }
        }     
                           
        /// <summary>
        /// Gets or sets the bitmap source for the preview version of the photo, with decoded pixel width of a certain value.
        /// </summary>
        public BitmapSource PreviewBitmap
        {
            get
            {
                return previewBitmap;
            }
            set
            {
                previewBitmap = value;
                OnPropertyChanged();
            }         
        } 

        /// <summary>
        /// Gets or sets the name of the photo.
        /// </summary>
        public string Name
        {
            get { return name; }
            set
            {
                if (name != value)
                {
                    name = value;
                    OnPropertyChanged();
                    SetPhotoUri();
                }
            }
        }
        
        // TODO Determine if/when this is/will be necessary
        public bool IsSaved
        {
            get { return isSaved; }
            set
            {
                if (isSaved != value)
                {
                    isSaved = value;
                    OnPropertyChanged(nameof(IsSaved)); 
                }
            }
        }
        
        #endregion

        #region - Public methods -

        public override string ToString()
        {
            return this.Name;
        }

        // TODO Remove this and refactor properties if VirtualizingWrapPanel doesn't need to use this method
        /// <summary>
        /// Refreshes the photo's bitmap source of the specified type. Used when bitmap source is needed, but the current value is null.
        /// </summary>
        /// <param name="bitmapType">Type of photo bitmap source being refreshed.</param>
        public void RefreshBitmapSource(BitmapType bitmapType)
        {
            switch (bitmapType)
            {
                case BitmapType.OriginalBitmap:
                    OriginalBitmap = GetBitmapSource(bitmapType);
                    break;
                case BitmapType.Thumbnail:
                    Thumbnail = GetBitmapSource(bitmapType);
                    break;                
                case BitmapType.PreviewBitmap:
                    PreviewBitmap = GetBitmapSource(bitmapType);
                    break;
            }
        }

        #endregion

        #region - Private methods -

        /// <summary>
        /// Gets the bitmap source for the specified bitmap type, i.e. with the specified decode pixel width value.
        /// </summary>
        /// <param name="bitmapType">Type for the bitmap source, with the value for the pixel width decoding.</param>
        /// <returns></returns>
        private BitmapSource GetBitmapSource(BitmapType bitmapType)
        {
            try
            {
                BitmapImage source = new BitmapImage();
                source.BeginInit();
                source.CacheOption = BitmapCacheOption.OnLoad; // TODO Check if this should be OnLoad
                //source.DecodeFailed
                if (bitmapType != BitmapType.OriginalBitmap)
                {
                    source.DecodePixelWidth = (int)bitmapType;
                }                
                source.UriSource = PhotoUri;
                source.EndInit();
                source.Freeze();
                     
                    
                return source;
            }
            catch (Exception e)
            {
                //System.Diagnostics.Trace.WriteLine(e.StackTrace);
                //System.Diagnostics.Trace.WriteLine(e.Source);
                //System.Diagnostics.Trace.WriteLine(e.Message);

                string property = string.Empty;
                switch (bitmapType)
                {
                    case BitmapType.OriginalBitmap:
                        property = "an Original BitmapSource";
                        break;
                    case BitmapType.Thumbnail:
                        property = "a Thumbnail BitmapSource";
                        break;
                    case BitmapType.PreviewBitmap:
                        property = "a Preview BitmapSource";
                        break;                    
                }
                System.Diagnostics.Trace.WriteLine($"\t- Could not create {property} from '{PhotoUri}'");
                return null; // TODO Determine if null should be returned
            }            
        }

        /// <summary>
        /// Sets uri for the photo. Used only for the newly created photo.
        /// </summary>
        private void SetPhotoUri()
        {
            // TODO Resolve different extensions already in name
            // TODO Determine where to put the TempPhoto folder
            string tempPhotoUri = $"../TempPhoto/{name}{extension}"; 
            PhotoUri = new Uri(tempPhotoUri, UriKind.Relative); 
        }

        #endregion
    }
}
