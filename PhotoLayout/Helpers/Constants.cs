using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoLayout.Helpers
{
    public static class Constants
    {
        // Maximum value that a preview image can have is 4 000 x 3 000 pixels -> 45MB for 32b format
        public const int MaximumPixelsForPreview = 12000000;
    }
}
