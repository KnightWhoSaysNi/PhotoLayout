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
    public class TestViewModel : BindableBase
    {
        public TestViewModel()
        {
            TestCommand = new RelayCommand(x => MessageBox.Show("It's working"));
        }

        public ICommand TestCommand { get; set; }
    }
}
