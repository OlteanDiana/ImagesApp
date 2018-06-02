using DisertatieApp.Utilities;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows.Forms;
using System.IO;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using DisertatieApp.Messages;
using DisertatieApp.Models;

namespace DisertatieApp.ViewModels
{
    public class ThumbnailContainerViewModel : ViewModelBase
    {
        private ICommand _openFileCmd;
        private string _folderPath;
        private FolderBrowserDialog _folderBrowser;
        private string[] _filters = new string[] { "jpg", "jpeg", "png", "gif", "tiff", "bmp" };
        private ICommand _openViewerCmd;

        public ObservableCollection<ThumbnailFile> Images { get; set; }

        public ICommand OpenFileCmd { get { return _openFileCmd; } }

        public ICommand OpenViewerCmd { get { return _openViewerCmd; } }

        public string FolderPath
        {
            get { return _folderPath; }
            set
            {
                _folderPath = value;
                RaisePropertyChanged(() => FolderPath);
            }
        }

        public ThumbnailContainerViewModel()
        {
            _folderBrowser =  new FolderBrowserDialog();
            _openFileCmd = new RelayCommand(OpenFile);
            _openViewerCmd = new RelayCommand(OpenViewer);
            Images = new ObservableCollection<ThumbnailFile>();
        }

        private void OpenViewer(object obj)
        {
            Messenger.Default
                     .Send(
                            new OpenWindowMessage()
                            {
                                FilePath = obj.ToString()
                            });
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
            Images.Clear();

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
