using DisertatieApp.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DisertatieApp.Utilities
{
    public class ImagesHandler
    {
        #region Fields

        private string _folderPath;
        private string[] _filters;
        private int _minutesSpan;
        private MetadataHandler _metadataHandler;

        #endregion

        #region Properties 

        private List<Thumbnail> _images;
        public List<Thumbnail> Images
        {
            get
            {
                return _images;
            }
        }

        private Dictionary<DateTime, List<Thumbnail>> _similarImages;
        public Dictionary<DateTime, List<Thumbnail>> SimilarImages
        {
            get
            {
                return _similarImages;
            }
        }

        private Dictionary<string, ImageMetadata> _imagesMetadata;

        #endregion

        #region Constructor
        
        public ImagesHandler(string folderPath, int minutesSpan)
        {
            _folderPath = folderPath;
            _minutesSpan = minutesSpan;

            _filters = Enum.GetNames(typeof(FileTypes));
            _metadataHandler = new MetadataHandler();

            SetImages();
            SetImagesMetadata();
            SetSimilarImages();
        }

        #endregion

        #region PublicMethods

        public List<Thumbnail> GetSimilarImagesList(int index)
        {
            if (_similarImages.Count - 1 >= index)
            {
                return _similarImages.ElementAt(index).Value;
            }

            return new List<Thumbnail>();
        }

        public string GetImageTooltipInfo(string filePath)
        {
            string fileName = string.Format("File name: {0} \n",
                                            Path.GetFileName(filePath));

            if (!_imagesMetadata.ContainsKey(filePath)
               || _imagesMetadata[filePath] == null)
            {
                return fileName;
            }

            ImageMetadata metadata = _imagesMetadata[filePath];
            return string.Format("{0}{1}", 
                                 fileName, 
                                 metadata.ToString()); 
        }

        #endregion

        #region PropertiesSetters

        private void SetImages()
        {
            _images = new List<Thumbnail>();

            foreach (string filter in _filters)
            {
                string[] images = Directory.GetFiles(_folderPath,
                                                     string.Format("*.{0}", filter),
                                                     SearchOption.AllDirectories);
                foreach (string image in images)
                {
                    _images.Add(new Thumbnail()
                    {
                        FilePath = image
                    });
                }
            }
        }

        private void SetImagesMetadata()
        {
            _imagesMetadata = new Dictionary<string, ImageMetadata>();

            foreach (Thumbnail image in _images)
            {
                ImageMetadata metadata = _metadataHandler.GetImageMetadata(image.FilePath);
                _imagesMetadata.Add(image.FilePath, metadata);
            }
        }

        private void SetSimilarImages()
        {
            _similarImages = new Dictionary<DateTime, List<Thumbnail>>();

            foreach (Thumbnail image in Images)
            {
                string dateString = _imagesMetadata[image.FilePath]?.CreationDate;

                if (dateString.IsNullOrEmpty())
                {
                    continue;
                }

                DateTime? date = dateString.ParseToDate(' ', '.', ':');
                if (date == null)
                {
                    continue;
                }

                DateTime? referenceDate = GetClosestDate((DateTime)date, _minutesSpan);
                if ( referenceDate == null)
                {
                    _similarImages.Add((DateTime)date, new List<Thumbnail>() { image });
                    continue;
                }

                _similarImages[(DateTime)referenceDate].Add(image);
            }

            RemoveKeysWithSingleValue();
        }

        #endregion

        #region Helpers

        private void RemoveKeysWithSingleValue()
        {
            foreach (var item in _similarImages.Where(k => k.Value.Count == 1).ToList())
            {
                _similarImages.Remove(item.Key);
            }
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

        #endregion
    }
}
