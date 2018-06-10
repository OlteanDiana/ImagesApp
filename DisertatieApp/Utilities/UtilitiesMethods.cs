using DisertatieApp.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Linq;
using System.IO;
using System.Drawing.Imaging;

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

        public static DateTime? ParseToDate(this string creationDate, char partsSplitter, char dateSplitter, char timeSplitter)
        {
            try
            {
                string[] dateTimeParts = creationDate.Split(partsSplitter);

                string[] dateParts = dateTimeParts[0].Split(dateSplitter);
                int day = int.Parse(dateParts[0].ToString());
                int month = int.Parse(dateParts[1].ToString());
                int year = int.Parse(dateParts[2].ToString());

                string[] timeParts = dateTimeParts[1].Split(timeSplitter);
                int hour = int.Parse(timeParts[0].ToString());
                int minute = int.Parse(timeParts[1].ToString());
                int second = int.Parse(timeParts[2].ToString());

                return new DateTime(year, month, day, hour, minute, second);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static bool IsNullOrEmpty(this string value)
        {
            return value == null || value.Equals(string.Empty);
        }

        public static List<Image> ToImageList(this List<Thumbnail> thumbnails, int width = 0, int height = 0, string newPath = null)
        {
            List<Image> images = new List<Image>();

            foreach (string path in thumbnails.Select(i => i.FilePath).ToList())
            {
                Image image = Image.FromFile(path);

                if (newPath == null)
                {
                    images.Add(image);
                    continue;
                }

                Image resizedImage = image.GetThumbnailImage(width, height, null, IntPtr.Zero);
                resizedImage.Save(Path.Combine(newPath, Path.GetFileName(path)), ImageFormat.Png);
                images.Add(resizedImage);
            }

            return images;
        }

    }
}
