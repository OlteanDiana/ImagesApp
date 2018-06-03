using DisertatieApp.Models;
using System.Collections.Generic;

namespace DisertatieApp.Messages
{
    public class UpdateImagesMessage
    {
        public List<ThumbnailFile> Images { get; set; }
    }
}
