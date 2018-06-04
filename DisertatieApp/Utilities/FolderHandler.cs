using DisertatieApp.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;

namespace DisertatieApp.Utilities
{
    public class FolderHandler
    {
        #region Constants

        private const string DATE_KEY = "EXtdate:create\0"; 

        #endregion

        #region Fields

        private string _folderPath;
        private string[] _filters;
        private int _minutesSpan;

        #endregion

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

        #region Constructor

        public FolderHandler(string folderPath, int minutesSpan)
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

        private DateTime? GetDate(string filePath)
        {
            try
            {
                byte[] imageBytes = File.ReadAllBytes(filePath);
                var asString = Encoding.UTF8.GetString(imageBytes);
                var start = asString.IndexOf(DATE_KEY);
                var end = asString.IndexOf(DATE_KEY) + 19;

                if (start == -1 || end == -1)
                {
                    return null;
                }

                var creationDate = asString.Substring(start + DATE_KEY.Length, 19);
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
                string[] dateTimeParts = creationDate.Split('T');

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
                MessageBox.Show("Error while parsing date.");
                return null;
            }
        }

        #endregion
    }
}
