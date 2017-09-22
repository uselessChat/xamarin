using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Xamarin.Forms;
using Java.IO;
using Java.Net;
using Android.Graphics;
using System.IO;

[assembly: Dependency(typeof(DemoImage.Droid.StorageAndroid))]
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