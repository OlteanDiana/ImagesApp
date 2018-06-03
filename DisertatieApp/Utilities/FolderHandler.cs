using DisertatieApp.Models;
using System;
using System.Collections.Generic;
using System.IO;

namespace DisertatieApp.Utilities
{
    public class FolderHandler
    {
        #region Fields

        private string _folderPath;

        #endregion

        #region Constructor

        public FolderHandler(string folderPath)
        {
            _folderPath = folderPath;
        }

        #endregion

        #region Methods

        public List<ThumbnailFile> GetListOfImages()
        {
            List<ThumbnailFile> result = new List<ThumbnailFile>();
            string[] filters = Enum.GetNames(typeof(FileTypes));

            foreach (string filter in filters)
            {
                string[] images = Directory.GetFiles(_folderPath,
                                                     string.Format("*.{0}", filter),
                                                     SearchOption.AllDirectories);
                foreach (string image in images)
                {
                    result.Add(new ThumbnailFile()
                    {
                        FileName = Path.GetFileName(image),
                        FilePath = image
                    });
                }
            }

            return result;
        } 

        #endregion
    }
}
