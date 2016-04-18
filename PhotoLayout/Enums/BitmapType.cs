using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoLayout.Enums
{
    /// <summary>
    /// Type of bitmap with decode pixel width value, based what the bitmap is used for.
    /// </summary>
    public enum BitmapType 
    {        
        /// <summary>
        /// Bitmap is decoded with original pixel width. 
        /// Used for saving the photo to hdd and/or sending it to a specified location.
        /// </summary>
        OriginalBitmap = 0,
        
        /// <summary>
        /// Bitmap is decoded with specified pixel width.
        /// Used for showing thumbnails of the original.
        /// </summary>
        Thumbnail = 300,

        /// <summary>
        /// Bitmap is decoded with specified pixel width.
        /// Used for previewing/manipulating photos in LayoutGrid.
        /// </summary>        
        PreviewBitmap = 1920
    }
}
