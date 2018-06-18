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
using System.Windows.Forms;
using System.Windows.Media.Imaging;

namespace DisertatieApp.ViewModels
{
    public class ImagesViewModel : ViewModelBase
    {
        #region Fields

        private int _currentFileIndex;
        private List<string> _currentImageTempPaths;
        private List<string> _undoTempPaths;
        private List<string> _tempPaths;

        #endregion

        #region Properties

        private string _lastTempFilePath
        {
            get
            {
                return _currentImageTempPaths?.ElementAtOrDefault(_currentImageTempPaths.Count - 1);
            }
        }

        public string UndoImage
        {
            get
            {
                return "\\Resources\\undo.png";
            }
        }

        public string SaveImage
        {
            get
            {
                return "\\Resources\\save.png";
            }
        }

        public string RotateLeftImage
        {
            get
            {
                return "\\Resources\\rotateLeft.png";
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

        public string RotateRightImage
        {
            get
            {
                return "\\Resources\\rotateRight.png";
            }
        }

        public string CropImage
        {
            get
            {
                return "\\Resources\\crop.png";
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

        private bool _isUndoEnabled;
        public bool IsUndoEnabled
        {
            get
            {
                return _isUndoEnabled;
            }

            set
            {
                _isUndoEnabled = value;
                RaisePropertyChanged(() => IsUndoEnabled);
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

        private ICommand _saveCmd;
        public ICommand SaveCmd
        {
            get
            {
                return _saveCmd;
            }
        }

        private ICommand _undoCmd;
        public ICommand UndoCmd
        {
            get
            {
                return _undoCmd;
            }
        }

        #endregion

        #region Constructor

        public ImagesViewModel()
        {
            Messenger.Default.Register<CleanUpViewsMessage>(this, OnCleanUp);
            Messenger.Default.Register<CroppedImageSavedMessage>(this, OnCroppedImageSaved);

            _nextImageCmd = new RelayCommand(GoToNext);
            _previousImageCmd = new RelayCommand(GoToPrevious);
            _rotateLeftImageCmd = new RelayCommand(RotateLeft);
            _rotateRightImageCmd = new RelayCommand(RotateRight);
            _selectRegionCmd = new RelayCommand(SelectRegion);
            _cropImageCmd = new RelayCommand(Crop);
            _saveCmd = new RelayCommand(Save);
            _undoCmd = new RelayCommand(Undo);

            _currentImageTempPaths = new List<string>();
            _tempPaths = new List<string>();
            _undoTempPaths = new List<string>();

            IsSelectEnabled = true;
        }

        #endregion

        #region MessagesEvents

        private void OnCroppedImageSaved(CroppedImageSavedMessage message)
        {
            _undoTempPaths.Add(CurrentFilePath);

            CurrentFilePath = message.ImagePath;
            _currentImageTempPaths.Add(message.ImagePath);

            IsSaveEnabled = IsUndoEnabled = true;
            ResetCrop();
        }

        private void OnCleanUp(CleanUpViewsMessage message)
        {
            if (message.DeleteFiles)
            {
                _tempPaths.DeleteFiles();

                _currentImageTempPaths.Clear();
                _undoTempPaths.Clear();
                _tempPaths.Clear();

                ResetCrop();
                return;
            }

            HandleCleanup();
        }

        #endregion

        #region CommandHandlers

        private void GoToPrevious(object obj)
        {
            _currentFileIndex--;
            HandleCleanup();

            CurrentFilePath = Files.ElementAt(_currentFileIndex)?.FilePath;
            HandleEnableDisableButtons();
        }

        private void GoToNext(object obj)
        {
            _currentFileIndex++;
            HandleCleanup();

            CurrentFilePath = Files.ElementAt(_currentFileIndex)?.FilePath;
            HandleEnableDisableButtons();
        }

        private void RotateLeft(object obj)
        {
            RotateImage(false);
            IsSaveEnabled = IsUndoEnabled = true;
            ResetCrop();
        }

        private void RotateRight(object obj)
        {
            RotateImage(true);
            IsSaveEnabled = IsUndoEnabled = true;
            ResetCrop();
        }

        private void SelectRegion(object obj)
        {
            IsSelectEnabled = false;
            Messenger.Default
                     .Send(new InitializeCropAdornerMessage());
            IsCropEnabled = true;
        }

        private void Crop(object obj)
        {
            Messenger.Default
                     .Send(new SaveCroppedImageMessage());
        }

        private void Save(object obj)
        {
            SaveFileDialog fileDialog = new SaveFileDialog();
            fileDialog.Title = "Select destination file";
            fileDialog.Filter = "Bitmap Image (.bmp)|*.bmp|JPEG Image (.jpeg)|*.jpeg|JPG Image (.jpg)|*.jpg|Png Image (.png)|*.png";

            DialogResult dialogResult = fileDialog.ShowDialog();
            if (dialogResult != DialogResult.OK)
            {
                return;
            }

            string fileName = fileDialog.FileName;
            BitmapEncoder encoder = Path.GetExtension(fileName)?.Substring(1).GetEncoder();

            File.WriteAllBytes(fileName, ImgSource.ImageSourceToBytes(encoder));
            IsSaveEnabled = IsUndoEnabled = false;
        }

        private void Undo(object obj)
        {
            CurrentFilePath = _undoTempPaths.ElementAt(_undoTempPaths.Count - 1);
            _undoTempPaths.RemoveAt(_undoTempPaths.Count - 1);
            IsUndoEnabled = _undoTempPaths.Count > 0;
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

        private void RotateImage(bool positiveRotation)
        {
            string tempFilePath = Path.GetTempFileName();

            Image image = !_lastTempFilePath.IsNullOrEmpty() ? Image.FromFile(_lastTempFilePath)
                                                             : Image.FromFile(CurrentFilePath);
            image.RotateImage(positiveRotation ? 90 : -90, tempFilePath);
            _undoTempPaths.Add(CurrentFilePath);

            CurrentFilePath = tempFilePath;
            _currentImageTempPaths.Add(tempFilePath);
            
            IsSaveEnabled = true;
        }

        private void HandleCleanup()
        {
            _tempPaths.AddRange(_currentImageTempPaths);

            _currentImageTempPaths.Clear();
            _undoTempPaths.Clear();

            IsSaveEnabled = IsUndoEnabled = false;
            ResetCrop();
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
