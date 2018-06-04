using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace DisertatieApp.Utilities
{
    public static class UtilitiesMethods
    {
        public static void AddRange<T>(this ObservableCollection<T> collection, IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                collection.Add(item);
            }
        }

        public static ImageSource SetImageSource(this string filePath)
        {
            BitmapImage image = new BitmapImage();

            try
            {
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                image.UriSource = new Uri(filePath, UriKind.Absolute);
                image.EndInit();
            }
            catch
            {
                return null;
            }

            return image;
        }
    }
}
