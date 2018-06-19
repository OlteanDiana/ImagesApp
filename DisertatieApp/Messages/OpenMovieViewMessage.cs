using DisertatieApp.Models;
using System.Collections.Generic;
using System.Windows.Media;

namespace DisertatieApp.Messages
{
    public class OpenMovieViewMessage
    {
        public List<Thumbnail> Images { get; set; }
        public int TimeFrame { get; set; }
        public List<ImageSource> ImagesSource { get; set; }
        public ImageSource ImgSource { get; set; }
    }
}
