using System.ComponentModel;
using System.Drawing;
using System.Windows.Input;
using Xamarin.Forms;

namespace DemoImage
{
    class DemoImageViewModel : INotifyPropertyChanged
    {
        /** Binding */
        public string ImgXamarin { protected set; get; }
        public string ImgVisualStudio { protected set; get; }
        public string ImgMerged { protected set; get; }
        public ICommand MergeImagesCommand { protected set; get; }
        /** end Binding */

        // Contructor
        public DemoImageViewModel()
        {   
            // Commandings
            this.MergeImagesCommand = new Command(() => {
                var platform = DependencyService.Get<Storage.IImages>();
                if (platform == null) return;
                var newSource = this.ImgMerged != null ? null : platform.MergeByUrl(this.ImgXamarin, this.ImgVisualStudio);
                this.ImgMerged = newSource;
                this.OnPropertyChanged("ImgMerged");
            });

            // Image Sources
            this.ImgXamarin = "https://maxcdn.icons8.com/Share/icon/color/Logos/xamarin1600.png";
            this.ImgVisualStudio = "http://icons.iconarchive.com/icons/dakirby309/simply-styled/128/Microsoft-Visual-Studio-icon.png";

            // Notify Changes
            this.OnPropertyChanged("ImgXamarin");
            this.OnPropertyChanged("ImgVisualStudio");
        }
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
