using DisertatieApp.Models;
using System.Collections.Generic;

namespace DisertatieApp.Messages
{
    public class OpenMovieViewMessage
    {
        public List<ThumbnailFile> Images { get; set; }
        public int TimeFrame { get; set; }
    }
}
