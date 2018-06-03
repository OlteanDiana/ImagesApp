using DisertatieApp.Utilities;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows.Forms;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using DisertatieApp.Messages;
using DisertatieApp.Models;
using System.Collections.Generic;

namespace DisertatieApp.ViewModels
{
    public class ThumbnailContainerViewModel : ViewModelBase
    {
        #region Fields

        private FolderBrowserDialog _folderBrowser;
        private FolderHandler _folderHandler;

        #endregion

        #region Properties

        public ObservableCollection<ThumbnailFile> Images { get; set; }

        private ICommand _openFileCmd;
        public ICommand OpenFileCmd
        {
            get
            {
                return _openFileCmd;
            }
        }

        private ICommand _openViewerCmd;
        public ICommand OpenViewerCmd
        {
            get
            {
                return _openViewerCmd;
            }
        }

        private string _folderPath;
        public string FolderPath
        {
            get
            {
                return _folderPath;
            }

            set
            {
                _folderPath = value;
                RaisePropertyChanged(() => FolderPath);
            }
        }

        #endregion

        #region Constructor

        public ThumbnailContainerViewModel()
        {
            _folderBrowser = new FolderBrowserDialog();
            _openFileCmd = new RelayCommand(OpenFile);
            _openViewerCmd = new RelayCommand(OpenViewer);

            Images = new ObservableCollection<ThumbnailFile>();
        }

        #endregion

        #region CommandHandlers

        private void OpenFile(object obj)
        {
            DialogResult result = _folderBrowser.ShowDialog();

            if (result != DialogResult.OK)
            {
                //show custom message box
                return;
            }

            if (string.IsNullOrWhiteSpace(_folderBrowser.SelectedPath))
            {
                //show custom message box
                return;
            }

            _folderPath = _folderBrowser.SelectedPath;

            Images.Clear();
            _folderHandler = new FolderHandler(_folderPath);
            Images.AddRange(_folderHandler.GetListOfImages());
        }

        private void OpenViewer(object obj)
        {
            Messenger.Default
                     .Send(
                            new OpenImageViewerMessage()
                            {
                                CurrentFilePath = obj?.ToString(),
                                Files = new List<ThumbnailFile>(Images)
                            });
        } 

        #endregion
    }
}
