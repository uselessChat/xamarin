using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;
using Xamarin.Forms;

[assembly: Dependency(typeof(StorageiOS))]
namespace DemoImage.iOS
{
    public class StorageiOS : Storage.IImages
    {
        public string Location(string fileName)
        {
            throw new NotImplementedException();
        }

        public string MergeByUrl(string urlFront, string urlBack)
        {
            throw new NotImplementedException();
        }
    }
}