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
        #region Fields

        private ImagesHandler _imagesHandler; 

        #endregion

        #region Properties

        public ObservableCollection<Thumbnail> Images { get; set; }

        private ICommand _openViewerCmd;
        public ICommand OpenViewerCmd
        {
            get
            {
                return _openViewerCmd;
            }
        }

        private ICommand _showTooltipCmd;
        public ICommand ShowTooltipCmd
        {
            get
            {
                return _showTooltipCmd;
            }
        }

        private string _tooltip;
        public string Tooltip
        {
            get
            {
                return _tooltip;
            }

            set
            {
                _tooltip = value;
                RaisePropertyChanged(() => Tooltip);
            }
        }

        #endregion

        #region Constructor

        public ThumbnailContainerViewModel()
        {
            _openViewerCmd = new RelayCommand(OpenViewer);
            _showTooltipCmd = new RelayCommand(ShowTooltip);

            Images = new ObservableCollection<Thumbnail>();

            Messenger.Default.Register<UpdateImagesMessage>(this, ProcessImagesUpdateMessage);
        }

        #endregion

        #region CommandHandlers

        private void OpenViewer(object file)
        {
            Messenger.Default
                     .Send(
                            new OpenImageViewMessage()
                            {
                                CurrentFilePath = file?.ToString(),
                                Files = new List<Thumbnail>(Images)
                            });
        }

        private void ShowTooltip(object filePath)
        {
            if (filePath == null)
            {
                return;
            }

            Tooltip = _imagesHandler.GetImageTooltipInfo(filePath.ToString());
        }

        #endregion

        #region MessageHelpers

        private void ProcessImagesUpdateMessage(UpdateImagesMessage message)
        {
            Images.Clear();
            Images.AddRange(message.Images);

            _imagesHandler = message.ImagesHandler;
        }

        #endregion
    }
}
