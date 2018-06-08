using DisertatieApp.Models;
using DisertatieApp.Utilities;
using System.Collections.Generic;

namespace DisertatieApp.Messages
{
    public class UpdateImagesMessage
    {
        public List<Thumbnail> Images { get; set; }
        public ImagesHandler ImagesHandler { get; set; }
    }
}
