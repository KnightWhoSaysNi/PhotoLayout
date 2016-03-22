using PhotoLayout.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace PhotoLayout.ViewModels
{
    public class MainViewModel : BindableBase
    {
        #region - Fields -



        #endregion

        #region - Contructors - 

        public MainViewModel()
        {
            TestVM = new TestViewModel();
        }

        #endregion

        #region - Properties -

        public TestViewModel TestVM { get; private set; }

        #endregion
    }
}
