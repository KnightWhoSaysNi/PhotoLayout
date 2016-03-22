using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace PhotoLayout.Models
{
    public class Photo : INotifyPropertyChanged
    {
        #region - Fields -

        private bool isSaved;

        #endregion

        #region - Constructors -

        public Photo()
        {

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

        #region - Properties -

        // TODO !!! Check if Drawing image is a better solution than an Image control with BitmapSource !!!
        // Might need refactoring later on
        public BitmapSource Source { get; set; }
        public BitmapSource ThumbnailSource { get; set; }

        public string Name { get; set; }
        
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

        #endregion
    }
}
