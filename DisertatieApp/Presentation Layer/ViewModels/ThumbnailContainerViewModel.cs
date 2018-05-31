using DisertatieApp.Base_Classes;
using DisertatieApp.Presentation_Layer.Models;
using DisertatieApp.Utilities;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows.Forms;
using System.IO;

namespace DisertatieApp.Presentation_Layer.ViewModels
{
    public class ThumbnailContainerViewModel : ViewModelsBase
    {
        private ICommand _openFileCmd;
        private string _folderPath;
        private FolderBrowserDialog _folderBrowser;
        private string[] _filters = new string[] { "jpg", "jpeg", "png", "gif", "tiff", "bmp" };

        public ObservableCollection<ThumbnailFile> Images { get; set; }

        public ICommand OpenFileCmd { get { return _openFileCmd; } }

        public string FolderPath
        {
            get { return _folderPath; }
            set
            {
                _folderPath = value;
                OnPropertyChanged("FolderPath");
            }
        }

        public ThumbnailContainerViewModel()
        {
            _folderBrowser =  new FolderBrowserDialog();
            _openFileCmd = new RelayCommand(OpenFile);
            Images = new ObservableCollection<ThumbnailFile>();
        }

        private void OpenFile(object obj)
        {
            DialogResult result = _folderBrowser.ShowDialog();

            if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(_folderBrowser.SelectedPath))
            {
                _folderPath = _folderBrowser.SelectedPath;
                GetListOfImages();
                return;
            }

            MessageBox.Show("No folder selected.");
        }

        private void GetListOfImages()
        {
            foreach (string filter in _filters)
            {
                string[] images = Directory.GetFiles(_folderPath, string.Format("*.{0}", filter), SearchOption.AllDirectories);
                foreach(string image in images)
                { 
                    Images.Add(new ThumbnailFile() { FileName = Path.GetFileName(image), FilePath = image });
                }
            }
        }
    }
}
