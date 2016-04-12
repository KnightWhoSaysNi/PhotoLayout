using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.IO;
using System.Drawing;

namespace PhotoLayout.Models
{
    public class Photo : INotifyPropertyChanged
    {
        #region - Fields -        

        private string name;
        private string extension;
        private bool isSaved;
        private bool isNewPhoto;
        private BitmapSource originalBitmap;
        private BitmapSource thumbnail;
        private BitmapSource previewBitmap;

        #endregion

        #region - Constructors -
        
        // On Save a name is given (extension as well, most likely) and an image is saved to temp folder with a specific uri.
        // That uri is specified here, with the name and extension
        public Photo(Uri uri, string name, string extension, bool isNewPhoto = false)
        {
            // If it's an existing photo no need for explanation
            // If it's a complex photo then it's actually an already created image and technically there is no difference (YET!)
            this.PhotoUri = uri;
            this.isNewPhoto = isNewPhoto;
            this.name = name;
            this.extension = extension;
        }

        #endregion

        #region - Events -

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        #region - Enums -

        // TODO Check if these are optimal values
        enum DecodePixelWidth
        {
            OriginalPixelWidth = 0,
            ThumbnailPixelWidth = 300,
            PreviewPixelWidth = 1920
        }

        #endregion

        #region - Properties -

        //public Image ComplexImage { get; set; } // This should be the screenshot of the mixed images. It might need to be WriteableBitmap instead of Drawing.Image

        public Uri PhotoUri { get; private set; }
        public BitmapSource OriginalBitmap
        {
            get
            {
                return originalBitmap;
            }
            set
            {
                originalBitmap = value;
                OnPropertyChanged(nameof(OriginalBitmap));
            }        
        }

        public BitmapSource Thumbnail
        {
            get
            {
                return thumbnail;
            }
            set
            {
                thumbnail = value;
                OnPropertyChanged(nameof(Thumbnail));
            }
        }     
                   
        public BitmapSource PreviewBitmap
        {
            get
            {
                return previewBitmap;
            }
            set
            {
                previewBitmap = value;
                OnPropertyChanged(nameof(PreviewBitmap));
            }         
        } 
        // public BitmapSource PrintBitmap { get; private set; } // TODO Determine if this property is also needed

        public string Name
        {
            get { return name; }
            set
            {
                if (name != value)
                {
                    name = value;
                    OnPropertyChanged(nameof(Name));

                    if (isNewPhoto)
                    {
                        SetPhotoUri();
                    }
                }
            }
        }
        
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
        public void RefreshBitmapSources()
        {
            if (originalBitmap == null)
                OriginalBitmap = GetBitmapSource(DecodePixelWidth.OriginalPixelWidth);

            if (thumbnail == null)
                Thumbnail = GetBitmapSource(DecodePixelWidth.ThumbnailPixelWidth);

            if (previewBitmap == null)
                PreviewBitmap = GetBitmapSource(DecodePixelWidth.PreviewPixelWidth);
        }

        #endregion

        #region - Private methods -

        private BitmapSource GetBitmapSource(DecodePixelWidth decodeWidth)
        {
            try
            {
                BitmapImage source = new BitmapImage();
                source.BeginInit();
                source.CacheOption = BitmapCacheOption.None; // TODO Check if this should be OnLoad
                //source.DecodeFailed
                if (decodeWidth != DecodePixelWidth.OriginalPixelWidth)
                {
                    source.DecodePixelWidth = (int)decodeWidth;
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
                switch (decodeWidth)
                {
                    case DecodePixelWidth.OriginalPixelWidth:
                        property = "an Original BitmapSource";
                        break;
                    case DecodePixelWidth.ThumbnailPixelWidth:
                        property = "a Thumbnail BitmapSource";
                        break;
                    case DecodePixelWidth.PreviewPixelWidth:
                        property = "a Preview BitmapSource";
                        break;                    
                }
                System.Diagnostics.Trace.WriteLine($"\t- Could not create {property} from '{PhotoUri}'");
                return null;
            }
            
        }

        /// <summary>
        /// Sets uri for the photo. Used only for the newly created photo.
        /// </summary>
        private void SetPhotoUri()
        {
            // TODO Resolve different extensions already in name
            // TODO Check if 1 folder up is the place to put the temp 
            string tempPhotoUri = $"../TempPhoto/{name}{extension}"; 
            PhotoUri = new Uri(tempPhotoUri, UriKind.Relative); 
        }

        #endregion
    }
}
