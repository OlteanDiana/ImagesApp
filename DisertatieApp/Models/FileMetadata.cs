namespace DisertatieApp.Models
{
    public class FileMetadata
    {
        public string FileName { get; set; }
        public string CreationDate { get; set; }
        public string ModifiedDate { get; set; }

        public string GetTooltipString()
        {
            return string.Format("File name: {0}\n" + 
                                 "Creation date: {1}\n" + 
                                 "Last modified date: {2}\n", 
                                 FileName,
                                 CreationDate,
                                 ModifiedDate);
        }
    }
}
