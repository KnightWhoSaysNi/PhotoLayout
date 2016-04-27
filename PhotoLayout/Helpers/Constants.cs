using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoLayout.Helpers
{
    public static class Constants
    {
        public static readonly string[] PhotoExtensions = { ".jpg", ".jpeg", ".png", ".bmp", ".tiff" };    

        // Number of photos that can be selected from the collection and added to the LayoutGrid   
        // ***** If this value changes the Layout matrices below also need to be updated *******
        public const int MaxSelectedPhotos = 8;

        /// <summary>
        /// Represents a layout that consists only of rows.
        /// </summary>
        public static readonly byte[] JustRowsLayout = new byte[] { MaxSelectedPhotos, 0 };

        /// <summary>
        /// Represents a layout that consists only of columns.
        /// </summary>
        public static readonly byte[] JustColumnsLayout = new byte[] { 0, MaxSelectedPhotos };

        /// <summary>
        /// Represents a layout that can have 3 rows and 3 columns (max 9 photos).
        /// </summary>
        public static readonly byte[] ThreeByThreeLayout = new byte[] { 3, 3 };

        /// <summary>
        /// Represents a layout that can have 5 rows and 2 columns (max 10 photos).
        /// </summary>
        public static readonly byte[] FiveByTwoLayout = new byte[] { 5, 2 };

        /// <summary>
        /// Represents a layout that can have 4 rows and 3 columns (max 12 photos).
        /// </summary>
        public static readonly byte[] FourByThreeLayout = new byte[] { 4, 3 };

        /// <summary>
        /// Represents a layotu that can have 3 rows and 4 columns (max 12 photos).
        /// </summary>
        public static readonly byte[] ThreeByFourLayout = new byte[] { 3, 4 };

    }
}
