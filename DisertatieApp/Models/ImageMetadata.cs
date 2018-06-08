namespace DisertatieApp.Models
{
    public class ImageMetadata
    {
        public string CreationDate { get; set; }
        public string ModifiedDate { get; set; }
        public string Size { get; set; }
        public string Dimensions { get; set; }

        public override string ToString()
        {
            return string.Format("Creation date: {0}\n" + 
                                 "Last modified date: {1}\n" +
                                 "Size (bytes): {2}\n" +
                                 "Dimensions (W x H): {3}",
                                 CreationDate,
                                 ModifiedDate,
                                 Size,
                                 Dimensions);
        }
    }
}
