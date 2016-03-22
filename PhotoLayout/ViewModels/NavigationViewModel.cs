using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PhotoLayout.ViewModels
{
    public class NavigationViewModel : BindableBase
    {
        #region - Fields -



        #endregion

        #region - Contructor -

        public NavigationViewModel()
        {

        }

        #endregion

        #region - Properties - 

        #region = Commands =

        public ICommand Discard { get; private set; }
        public ICommand New { get; private set; }
        public ICommand Rename { get; private set; }
        public ICommand Save { get; private set; }
        public ICommand Undo { get; private set; }
        public ICommand Redo { get; private set; }
        public ICommand PreviousLayout { get; private set; }
        public ICommand NextLayout { get; private set; }

        #endregion





        #endregion

        #region - Public methods - 



        #endregion

        #region - Private methods -

        private void InitializeCommands()
        {

        }

        private void OnCancel()
        {
                        
        }

        #endregion
    }
}
