﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoLayout.Enums
{
    /// <summary>
    /// Type of bitmap based on what it's used for with values for decoding pixel width.
    /// </summary>
    public enum BitmapType 
    {
        // Bitmap is decoded with original pixel width. 
        // Used for saving the photo to hdd and/or sending it to a specified location
        OriginalBitmap = 0,

        // Bitmap is decoded with specified pixel width
        // Used for showing thumbnails of the original
        Thumbnail = 300,

        // Bitmap is decoded with specified pixel width
        // Used for previewing/manipulating photos in LayoutGrid
        PreviewBitmap = 1920
    }
}
