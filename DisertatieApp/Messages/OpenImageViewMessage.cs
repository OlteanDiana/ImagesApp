using DisertatieApp.Models;
using System.Collections.Generic;

namespace DisertatieApp.Messages
{
    public class OpenImageViewMessage
    {
        public string CurrentFilePath { get; set; }
        public List<Thumbnail> Files { get; set; }
    }
}
