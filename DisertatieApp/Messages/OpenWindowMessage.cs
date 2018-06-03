﻿using DisertatieApp.Models;
using System.Collections.Generic;

namespace DisertatieApp.Messages
{
    public class OpenWindowMessage
    {
        public string CurrentFilePath { get; set; }
        public List<ThumbnailFile> Files { get; set; }
    }
}
