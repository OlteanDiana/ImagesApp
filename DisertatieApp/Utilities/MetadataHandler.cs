using DisertatieApp.Models;
using Microsoft.WindowsAPICodePack.Shell;
using Microsoft.WindowsAPICodePack.Shell.PropertySystem;

namespace DisertatieApp.Utilities
{
    public class MetadataHandler
    {
        #region Constructor

        public MetadataHandler()
        {
        }

        #endregion

        #region PublicMethods

        public ImageMetadata GetImageMetadata(string imagePath)
        {
            ShellObject image = ShellObject.FromParsingName(imagePath);
            if (image == null)
            {
                return null;
            }

            return new ImageMetadata()
            {
                CreationDate = GetPropertyValue(image, SystemProperties.System.DateCreated),
                ModifiedDate = GetPropertyValue(image, SystemProperties.System.DateModified),
                Size = GetPropertyValue(image, SystemProperties.System.Size),
                Dimensions = GetPropertyValue(image, SystemProperties.System.Image.Dimensions)
            };
        }

        #endregion

        #region PrivateMethods

        private string GetPropertyValue(ShellObject image, PropertyKey key)
        {
            IShellProperty property = image.Properties.GetProperty(key);
            return GetValue(property);
        }

        private string GetValue(IShellProperty value)
        {
            if (value == null || value.ValueAsObject == null)
            {
                return string.Empty;
            }

            return value.ValueAsObject.ToString();
        } 

        #endregion
    }
}
