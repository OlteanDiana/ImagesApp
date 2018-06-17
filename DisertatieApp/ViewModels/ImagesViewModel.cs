using DisertatieApp.Models;
using DisertatieApp.Utilities;
using GalaSoft.MvvmLight;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;
using System.Drawing;
using System.IO;
using GalaSoft.MvvmLight.Messaging;
using DisertatieApp.Messages;
using System;

namespace DisertatieApp.ViewModels
{
    public class ImagesViewModel : ViewModelBase
    {
        #region Fields

        private int _currentFileIndex;
        private string _tempFilePath;
        private string _lastTempFilePath;
        private List<string> _rotateLeftTempPaths;
        private List<string> _rotateRightTempPaths;
        private List<string> _cropTempPaths;
        private List<string> _tempPaths;

        #endregion

        #region Properties

        public string RotateLeftImage
        {
            get
            {
                return "\\Resources\\rotateLeft.png";
            }
        }

        public string RotateRightImage
        {
            get
            {
                return "\\Resources\\rotateRight.png";
            }
        }

        public string LeftArrowImage
        {
            get
            {
                return "\\Resources\\leftArrow.png";
            }
        }

        public string RightArrowImage
        {
            get
            {
                return "\\Resources\\rightArrow.png";
            }
        }

        public string CropImage
        {
            get
            {
                return "\\Resources\\crop.png";
            }
        }

        public string SaveImage
        {
            get
            {
                return "\\Resources\\save.png";
            }
        }

        public string SelectImage
        {
            get
            {
                return "\\Resources\\select.png";
            }
        }

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
                CurrentImageHeight = ImgSource.Height;
                CurrentImageWidth = ImgSource.Width;
                RaisePropertyChanged(() => CurrentFilePath);
            }
        }

        private double _currentImageWidth;
        public double CurrentImageWidth
        {
            get
            {
                return _currentImageWidth;
            }

            set
            {
                _currentImageWidth = value;
                RaisePropertyChanged(() => CurrentImageWidth);
            }
        }

        private double _currentImageHeight;
        public double CurrentImageHeight
        {
            get
            {
                return _currentImageHeight;
            }

            set
            {
                _currentImageHeight = value;
                RaisePropertyChanged(() => CurrentImageHeight);
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

        private bool _isSaveEnabled;
        public bool IsSaveEnabled
        {
            get
            {
                return _isSaveEnabled;
            }

            set
            {
                _isSaveEnabled = value;
                RaisePropertyChanged(() => IsSaveEnabled);
            }
        }
        
        private bool _isSelectEnabled;
        public bool IsSelectEnabled
        {
            get
            {
                return _isSelectEnabled;
            }

            set
            {
                _isSelectEnabled = value;
                RaisePropertyChanged(() => IsSelectEnabled);
            }
        }

        private bool _isCropEnabled;
        public bool IsCropEnabled
        {
            get
            {
                return _isCropEnabled;
            }

            set
            {
                _isCropEnabled = value;
                RaisePropertyChanged(() => IsCropEnabled);
            }
        }

        private List<Thumbnail> _files;
        public List<Thumbnail> Files
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

        private ICommand _rotateLeftImageCmd;
        public ICommand RotateLeftImageCmd
        {
            get
            {
                return _rotateLeftImageCmd;
            }
        }

        private ICommand _rotateRightImageCmd;
        public ICommand RotateRightImageCmd
        {
            get
            {
                return _rotateRightImageCmd;
            }
        }

        private ICommand _selectRegionCmd;
        public ICommand SelectRegionCmd
        {
            get
            {
                return _selectRegionCmd;
            }
        }

        private ICommand _cropImageCmd;
        public ICommand CropImageCmd
        {
            get
            {
                return _cropImageCmd;
            }
        }

        #endregion

        #region Constructor

        public ImagesViewModel()
        {
            Messenger.Default.Register<CleanUpViewsMessage>(this, OnCleanUp);
            Messenger.Default.Register<CroppedImageSavedMessage>(this, OnCroppedImageSaved);

            _nextImageCmd = new RelayCommand(GoToNextImage);
            _previousImageCmd = new RelayCommand(GoToPreviousImage);
            _rotateLeftImageCmd = new RelayCommand(OnRotateLeftImage);
            _rotateRightImageCmd = new RelayCommand(OnRotateRightImage);
            _selectRegionCmd = new RelayCommand(OnSelectRegion);
            _cropImageCmd = new RelayCommand(OnCropImage);

            _rotateLeftTempPaths = new List<string>();
            _rotateRightTempPaths = new List<string>();
            _cropTempPaths = new List<string>();
            _tempPaths = new List<string>();

            IsSelectEnabled = true;
        }

        #endregion

        #region MessagesEvents

        private void OnCroppedImageSaved(CroppedImageSavedMessage message)
        {
            _tempFilePath = _lastTempFilePath = message.ImagePath;

            ImgSource = _tempFilePath.SetImageSource();
            _cropTempPaths.Add(_tempFilePath);
            IsSaveEnabled = true;
            ResetCrop();
        }

        private void OnSelectRegion(object obj)
        {
            IsSelectEnabled = false;
            Messenger.Default
                     .Send(new InitializeCropAdornerMessage());
            IsCropEnabled = true;
        }

        private void OnCropImage(object obj)
        {
            Messenger.Default
                     .Send(new SaveCroppedImageMessage());
        }

        private void OnCleanUp(CleanUpViewsMessage message)
        {
            if (message.DeleteFiles)
            {
                _tempPaths.DeleteFiles();
                _tempPaths.Clear();
                return;
            }

            HandlePathCleanup();
        }

        #endregion

        #region CommandHandlers

        private void GoToPreviousImage(object obj)
        {
            _currentFileIndex--;
            HandlePathCleanup();

            CurrentFilePath = Files.ElementAt(_currentFileIndex)?.FilePath;
            HandleEnableDisableButtons();

            ResetCrop();
        }

        private void GoToNextImage(object obj)
        {
            _currentFileIndex++;
            HandlePathCleanup();

            CurrentFilePath = Files.ElementAt(_currentFileIndex)?.FilePath;
            HandleEnableDisableButtons();

            ResetCrop();
        }

        private void OnRotateLeftImage(object obj)
        {
            RotateImage(false, _rotateLeftTempPaths);

            ResetCrop();
        }

        private void OnRotateRightImage(object obj)
        {
            RotateImage(true, _rotateRightTempPaths);

            ResetCrop();
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

        private void RotateImage(bool positiveRotation, List<string> pathsList)
        {
            _tempFilePath = Path.GetTempFileName();

            Image image = !_lastTempFilePath.IsNullOrEmpty() ? Image.FromFile(_lastTempFilePath)
                                                             : Image.FromFile(CurrentFilePath);
            image.RotateImage(positiveRotation ? 90 : -90, _tempFilePath);
            ImgSource = _tempFilePath.SetImageSource();

            pathsList.Add(_tempFilePath);
            _lastTempFilePath = _tempFilePath;
            IsSaveEnabled = true;
        }

        private void HandlePathCleanup()
        {
            _lastTempFilePath = string.Empty;

            _tempPaths.AddRange(_rotateLeftTempPaths);
            _tempPaths.AddRange(_rotateRightTempPaths);
            _tempPaths.AddRange(_cropTempPaths);

            _rotateLeftTempPaths.Clear();
            _rotateRightTempPaths.Clear();
            _cropTempPaths.Clear();
        }

        private void ResetCrop()
        {
            Messenger.Default
                     .Send(new DestroyCropAdornerMessage());
            IsSelectEnabled = true;
            IsCropEnabled = false;
        }

        #endregion
    }
}
