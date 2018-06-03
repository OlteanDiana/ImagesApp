using DisertatieApp.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Text;

namespace DisertatieApp.Utilities
{
    public class FolderHandler
    {
        private const string DATE_KEY = "EXtdate:create\0";

        #region Fields

        private string _folderPath;
        private string[] _filters;

        #endregion

        private List<ThumbnailFile> _images;
        public List<ThumbnailFile> Images
        {
            get
            {
                return _images;
            }
        }

        private Dictionary<string, List<ThumbnailFile>> _similarImages;
        public Dictionary<string, List<ThumbnailFile>> SimilarImages
        {
            get
            {
                return _similarImages;
            }
        }

        #region Constructor

        public FolderHandler(string folderPath)
        {
            _folderPath = folderPath;
            _filters = Enum.GetNames(typeof(FileTypes));
            SetImages();
            SetSimilarImages();
        }

        #endregion

        #region PrivateMethods

        private void SetImages()
        {
            _images = new List<ThumbnailFile>();

            foreach (string filter in _filters)
            {
                string[] images = System.IO.Directory.GetFiles(_folderPath,
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
            _similarImages = new Dictionary<string, List<ThumbnailFile>>();

            foreach (ThumbnailFile image in Images)
            {
                string date = GetDate(image.FilePath);

                if (date.Equals(string.Empty))
                {
                    continue;
                }

                if (_similarImages.ContainsKey(date))
                {
                    _similarImages[date].Add(image);
                    continue;
                }

                _similarImages.Add(date, new List<ThumbnailFile>() { image });
            }

            RemoveKeysWithSingleValue();
        }

        private void RemoveKeysWithSingleValue()
        {
            foreach (var item in _similarImages.Where(k => k.Value.Count == 1).ToList())
            {
                _similarImages.Remove(item.Key);
            }
        }

        private string GetDate(string filePath)
        {
            byte[] imageBytes = File.ReadAllBytes(filePath);
            var asString = Encoding.UTF8.GetString(imageBytes);
            var start = asString.IndexOf(DATE_KEY);
            var end = asString.IndexOf(DATE_KEY) + 19;

            if (start == -1 || end == -1)
            {
                return string.Empty;
            }

            return asString.Substring(start + DATE_KEY.Length, 19);
        }

        #endregion
    }
}
