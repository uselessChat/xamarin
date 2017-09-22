# Images Merge

## Explanation  
Main screen with 3 sections:
1. 2 Images where their source is an url.
2. Button with command/action merge.
3. Image with source by image generated on merge action.  

Merged file is saved on file system.

Dependency Service
* android

Files
* Storage.IImages.cs
* DemoImageViewModel.cs
* MainPage.xaml
* StorageAndroid.cs

Not implemented
* transparency
* image scaling
* android dynamic permission request
 * tested on android < API 21 (granted per default)

## Interface  
```csharp
namespace Storage
{
   public interface IImages
    {
        string Location(String fileName);
        string MergeByUrl(string urlFront, string urlBack);
    }
}
```  

## ViewModel
```csharp
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
```

## XAML
```xml
<Grid>
	<Grid.BindingContext>
		<local:DemoImageViewModel/>
	</Grid.BindingContext>
	<Grid.ColumnDefinitions>
		<ColumnDefinition Width="1*" />
		<ColumnDefinition Width="1*" />
	</Grid.ColumnDefinitions>
	<Grid.RowDefinitions>
		<RowDefinition Height="2*" />
		<RowDefinition Height="1*" />
		<RowDefinition Height="3*" />
	</Grid.RowDefinitions>
	<Image x:Name="imgXamarin" Source="{Binding ImgXamarin}"/>
	<Image x:Name="imgVisualStudio" Grid.Column="1" Grid.Row="0"
		   Source="{Binding ImgVisualStudio}"/>
	<Button Text="Merge" Grid.Row="1" Grid.ColumnSpan="2" Command="{Binding MergeImagesCommand}"/>
	<Image x:Name="imgMerged" Grid.Row="2" Grid.ColumnSpan="2" Source="{Binding ImgMerged}" />
</Grid>
```  

## Platform Dependency Service

### Android
Assembly  
```csharp
[assembly: Dependency(typeof(DemoImage.Droid.StorageAndroid))]
```  

Implementation  
```csharp
namespace DemoImage.Droid
{
    public class StorageAndroid : Storage.IImages
    {
        public string Location(string fileName)
        {
            string path = System.IO.Path.Combine(
                Android.OS.Environment.ExternalStorageDirectory.AbsolutePath,
                Android.OS.Environment.DirectoryDownloads, fileName);
            return path;
        }

        public string MergeByUrl(string urlFront, string urlBack)
        {
            StrictMode.ThreadPolicy policy = new StrictMode.ThreadPolicy.Builder().PermitAll().Build();
            StrictMode.SetThreadPolicy(policy);

            Bitmap bitMapFront = GetImageByUrl(urlFront);
            Bitmap bitMapBack = GetImageByUrl(urlBack);
            if (bitMapBack == null || bitMapFront == null) return null;
            Bitmap merged = Merge(bitMapFront, bitMapBack);
            string fileName = "DemoImage.jpg";
            string location = Location(fileName);
            Save(location, merged);

            return location;
        }

        /** private */
        private Bitmap GetImageByUrl(string url)
        {
            try
            {
                HttpURLConnection connection = (HttpURLConnection)(new URL(url).OpenConnection());
                connection.DoInput = true;
                connection.Connect();
                return BitmapFactory.DecodeStream(connection.InputStream);
            }
            catch (Exception e)
            {
                // Log exception
                return null;
            }
        }

        private Bitmap Merge(Bitmap front, Bitmap back)
        {
            Bitmap overlay = Bitmap.CreateBitmap(front.Width, front.Height, Bitmap.Config.Argb8888);
            Canvas canvas = new Canvas(overlay);
            // canvas.DrawColor(Android.Graphics.Color.Transparent, PorterDuff.Mode.Clear);
            canvas.DrawBitmap(front, new Matrix(), null);
            canvas.DrawBitmap(back, new Matrix(), null);
            return overlay;
        }

        private void Save(string path, Bitmap bitmap)
        {
            using (var fileStream = new FileStream(path, FileMode.Create))
            {
                bitmap.Compress(Bitmap.CompressFormat.Jpeg, 100, fileStream);
            }
        }
    }
}
```
