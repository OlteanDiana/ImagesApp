using DisertatieApp.Models;
using System.Collections.Generic;

namespace DisertatieApp.Messages
{
    public class OpenMovieViewMessage
    {
        public Dictionary<string, List<ThumbnailFile>> Images { get; set; }
        public int TimeFrame { get; set; }
    }
}
