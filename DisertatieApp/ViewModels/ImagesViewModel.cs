using DisertatieApp.Models;
using DisertatieApp.Utilities;
using GalaSoft.MvvmLight;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using System;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace DisertatieApp.ViewModels
{
    public class ImagesViewModel : ViewModelBase
    {
        #region Fields

        private int _currentFileIndex;

        #endregion

        #region Properties

        private ImageSource _imgSource;
        public ImageSource ImgSource
        {
            get
            {
                return _imgSource;
            }

            set
            {
                _imgSource = value;
                RaisePropertyChanged(() => ImgSource);
            }
        }

        private string _currentFilePath;
        public string CurrentFilePath
        {
            get
            {
                return _currentFilePath;
            }

            set
            {
                _currentFilePath = value;
                ImgSource = CurrentFilePath.SetImageSource();
                RaisePropertyChanged(() => CurrentFilePath);
            }
        }

        private bool _isPreviousEnabled;
        public bool IsPreviousEnabled
        {
            get
            {
                return _isPreviousEnabled;
            }

            set
            {
                _isPreviousEnabled = value;
                RaisePropertyChanged(() => IsPreviousEnabled);
            }
        }

        private bool _isNextEnabled;
        public bool IsNextEnabled
        {
            get
            {
                return _isNextEnabled;
            }

            set
            {
                _isNextEnabled = value;
                RaisePropertyChanged(() => IsNextEnabled);
            }
        }

        private List<ThumbnailFile> _files;
        public List<ThumbnailFile> Files
        {
            get
            {
                return _files;
            }

            set
            {
                _files = value;
                SetCurrentFileIndex();
                HandleEnableDisableButtons();
                RaisePropertyChanged(() => Files);
            }
        }

        private ICommand _nextImageCmd;
        public ICommand NextImageCmd
        {
            get
            {
                return _nextImageCmd;
            }
        }

        private ICommand _previousImageCmd;
        public ICommand PreviousImageCmd
        {
            get
            {
                return _previousImageCmd;
            }
        }

        #endregion

        #region Constructor

        public ImagesViewModel()
        {
            _nextImageCmd = new RelayCommand(GoToNextImage);
            _previousImageCmd = new RelayCommand(GoToPreviousImage);
        }

        #endregion

        #region CommandHandlers

        private void GoToPreviousImage(object obj)
        {
            _currentFileIndex--;
            CurrentFilePath = Files.ElementAt(_currentFileIndex)?.FilePath;
            HandleEnableDisableButtons();
        }

        private void GoToNextImage(object obj)
        {
            _currentFileIndex++;
            CurrentFilePath = Files.ElementAt(_currentFileIndex)?.FilePath;
            HandleEnableDisableButtons();
        }

        #endregion

        #region PrivateMethods

        private void SetCurrentFileIndex()
        {
            if (Files == null)
            {
                return;
            }

            _currentFileIndex = Files.IndexOf(Files.Where(f => f.FilePath.Equals(CurrentFilePath)).FirstOrDefault());
        }

        private void HandleEnableDisableButtons()
        {
            if (_currentFileIndex == -1)
            {
                return;
            }

            IsPreviousEnabled = _currentFileIndex != 0;
            IsNextEnabled = _currentFileIndex != Files.Count - 1;
        }

        #endregion
    }
}
