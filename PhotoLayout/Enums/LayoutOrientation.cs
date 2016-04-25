using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoLayout.Enums
{
    public enum LayoutOrientation
    {
        /// <summary>
        /// On new photo new row is added until a limit is reached and only at that point is the new column added.
        /// </summary>
        RowFirst,

        /// <summary>
        /// On new photo new column is added until a limit is reached and only at that point is the new row added.
        /// </summary>
        ColumnFirst
    }
}
