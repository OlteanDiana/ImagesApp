using DisertatieApp.Utilities;
using System.Collections.ObjectModel;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using DisertatieApp.Messages;
using DisertatieApp.Models;
using System.Collections.Generic;

namespace DisertatieApp.ViewModels
{
    public class ThumbnailContainerViewModel : ViewModelBase
    {
        #region Properties

        public ObservableCollection<ThumbnailFile> Images { get; set; }

        private ICommand _openViewerCmd;
        public ICommand OpenViewerCmd
        {
            get
            {
                return _openViewerCmd;
            }
        }

        #endregion

        #region Constructor

        public ThumbnailContainerViewModel()
        {
            _openViewerCmd = new RelayCommand(OpenViewer);
            Images = new ObservableCollection<ThumbnailFile>();
            Messenger.Default.Register<UpdateImagesMessage>(this, ProcessImagesUpdateMessage);
        }

        #endregion

        #region CommandHandlers

        private void OpenViewer(object file)
        {
            Messenger.Default
                     .Send(
                            new OpenImageViewerMessage()
                            {
                                CurrentFilePath = file?.ToString(),
                                Files = new List<ThumbnailFile>(Images)
                            });
        }

        #endregion

        #region MessageHelpers

        private void ProcessImagesUpdateMessage(UpdateImagesMessage message)
        {
            Images.Clear();
            Images.AddRange(message.Images);
        }

        #endregion
    }
}
