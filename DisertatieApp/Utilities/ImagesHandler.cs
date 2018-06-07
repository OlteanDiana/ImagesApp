using DisertatieApp.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;

namespace DisertatieApp.Utilities
{
    public class ImagesHandler
    {
        #region Constants

        private const string DATE_CREATE_KEY = "EXtdate:create\0";
        private const string DATE_MODIFY_KEY = "EXtdate:modify\0";

        #endregion

        #region Fields

        private string _folderPath;
        private string[] _filters;
        private int _minutesSpan;

        #endregion

        #region Properties 
        private List<ThumbnailFile> _images;
        public List<ThumbnailFile> Images
        {
            get
            {
                return _images;
            }
        }

        private Dictionary<DateTime, List<ThumbnailFile>> _similarImages;
        public Dictionary<DateTime, List<ThumbnailFile>> SimilarImages
        {
            get
            {
                return _similarImages;
            }
        }

        #endregion

        #region Constructor

        public ImagesHandler()
        {
        }

        public ImagesHandler(string folderPath, int minutesSpan)
        {
            _folderPath = folderPath;
            _minutesSpan = minutesSpan;
            _filters = Enum.GetNames(typeof(FileTypes));
            SetImages();
            SetSimilarImages();
        }

        #endregion

        #region PublicMethods

        public List<ThumbnailFile> GetSimilarImagesList(int index)
        {
            if (_similarImages.Count - 1 >= index)
            {
                return _similarImages.ElementAt(index).Value;
            }

            return new List<ThumbnailFile>();
        }

        public string GetImageTooltipInfo(string filePath)
        {
            byte[] imageBytes = File.ReadAllBytes(filePath);
            var asString = Encoding.UTF8.GetString(imageBytes);

            FileMetadata metadata = new FileMetadata()
            {
                FileName = Path.GetFileName(filePath),
                CreationDate = ExtractMetadataInfo(asString, DATE_MODIFY_KEY, 19),
                ModifiedDate = ExtractMetadataInfo(asString, DATE_MODIFY_KEY, 19)
            };

            return metadata.GetTooltipString();
        }

        #endregion

        #region PrivateMethods

        private void SetImages()
        {
            _images = new List<ThumbnailFile>();

            foreach (string filter in _filters)
            {
                string[] images = Directory.GetFiles(_folderPath,
                                                     string.Format("*.{0}", filter),
                                                     SearchOption.AllDirectories);
                foreach (string image in images)
                {
                    _images.Add(new ThumbnailFile()
                    {
                        FileName = Path.GetFileName(image),
                        FilePath = image
                    });
                }
            }
        }

        private void SetSimilarImages()
        {
            _similarImages = new Dictionary<DateTime, List<ThumbnailFile>>();

            foreach (ThumbnailFile image in Images)
            {
                DateTime? date = GetDate(image.FilePath);

                if (date == null || date.Equals(string.Empty))
                {
                    continue;
                }

                DateTime? referenceDate = GetClosestDate((DateTime)date, _minutesSpan);
                if ( referenceDate == null)
                {
                    _similarImages.Add((DateTime)date, new List<ThumbnailFile>() { image });
                    continue;
                }

                _similarImages[(DateTime)referenceDate].Add(image);
            }

            RemoveKeysWithSingleValue();
        }

        private DateTime? GetClosestDate(DateTime date, int minutesSpan)
        {
            foreach (DateTime referenceDate in _similarImages.Keys)
            {
                if (referenceDate.Date.Equals(date.Date)
                   && ((referenceDate - date).TotalMinutes <= minutesSpan
                        || (date - referenceDate).TotalMinutes <= minutesSpan))
                {
                    return referenceDate;
                }
            }

            return null;
        }

        private void RemoveKeysWithSingleValue()
        {
            foreach (var item in _similarImages.Where(k => k.Value.Count == 1).ToList())
            {
                _similarImages.Remove(item.Key);
            }
        }

        private string ExtractMetadataInfo(string imageAsString, string keyword, int size)
        {
            try
            {
                var start = imageAsString.IndexOf(keyword);
                var end = imageAsString.IndexOf(keyword) + size;

                if (start == -1 || end == -1)
                {
                    return null;
                }

                if (keyword.Equals(DATE_CREATE_KEY) 
                    || keyword.Equals(DATE_MODIFY_KEY))
                {
                    return imageAsString.Substring(start + keyword.Length, size).Replace("T", " ");
                }

                return imageAsString.Substring(start + keyword.Length, size);
            }
            catch (Exception)
            {
                MessageBox.Show("Error while reading metadata.");
                return null;
            }
        }

        private DateTime? GetDate(string filePath)
        {
            try
            {
                byte[] imageBytes = File.ReadAllBytes(filePath);
                var asString = Encoding.UTF8.GetString(imageBytes);

                var creationDate = ExtractMetadataInfo(asString, DATE_CREATE_KEY, 19);
                return ParseDate(creationDate);
            }
            catch (Exception)
            {
                MessageBox.Show("Error while reading metadata.");
                return null;
            }
        }

        private DateTime? ParseDate(string creationDate)
        {
            try
            {
                string[] dateTimeParts = creationDate.Split(' ');

                string[] dateParts = dateTimeParts[0].Split('-');
                int year = int.Parse(dateParts[0].ToString());
                int month = int.Parse(dateParts[1].ToString());
                int day = int.Parse(dateParts[2].ToString());

                string[] timeParts = dateTimeParts[1].Split(':');
                int hour = int.Parse(timeParts[0].ToString());
                int minute = int.Parse(timeParts[1].ToString());
                int second = int.Parse(timeParts[2].ToString());

                return new DateTime(year, month, day, hour, minute, second);
            }
            catch (Exception)
            {
                //MessageBox.Show("Error while parsing date.");
                return null;
            }
        }

        #endregion
    }
}
