using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace PhotoLayout.Controls
{
    /* TODO Create a UI and Data virtualizing wrap panel which works in the following way:
        ! Numbers in the following text need revision !
        On loading of photos the wrap panel should load 300 thumbnail photos, even though only a small number may be visibile at any one time.
        After the user scrolls down to 200th photo the next 100 should be loaded.
        When 250th is reached the first 100 should be removed (Source of the image controls holding those photos should be set to null) and GC.Collect should be called (probably)
        If the user goes back to 200th, the first 100 should be added again.
        When 150th is reached the last 100 should be removed

        Image controls should be recycled and Photo.Thumbnail should be set to null on each Photo not being used (for Data virtualization)
    */
    public class VirtualizingWrapPanel : WrapPanel
    {
    }
}
