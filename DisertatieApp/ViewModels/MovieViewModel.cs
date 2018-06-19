using DisertatieApp.Models;
using DisertatieApp.Utilities;
using GalaSoft.MvvmLight;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Threading;
using System;
using System.Windows.Input;
using System.IO;
using System.Windows.Forms;
using System.Linq;
using GalaSoft.MvvmLight.Messaging;
using DisertatieApp.Messages;

namespace DisertatieApp.ViewModels
{
    public class MovieViewModel : ViewModelBase
    {
        #region Fields

        private int _index;
        private DispatcherTimer _dispatcherTimer;

        #endregion

        #region Properties

        private int _timeFrame;
        public int TimeFrame
        {
            get
            {
                return _timeFrame;
            }

            set
            {
                _timeFrame = value;
                StartDisplayingMovie();
                RaisePropertyChanged(() => TimeFrame);
            }
        }

        private List<Thumbnail> _images;
        public List<Thumbnail> Images
        {
            get
            {
                return _images;
            }

            set
            {
                _images = value;
                RaisePropertyChanged(() => Images);
            }
        }

        private List<ImageSource> _imagesSource;
        public List<ImageSource> ImagesSource
        {
            get
            {
                return _imagesSource;
            }

            set
            {
                _imagesSource = value;
                RaisePropertyChanged(() => Images);
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

        private ICommand _saveAsGifCmd;
        public ICommand SaveAsGifCmd
        {
            get
            {
                return _saveAsGifCmd;
            }
        }

        private ICommand _modifyTimeFrameCmd;
        public ICommand ModifyTimeFrameCmd
        {
            get
            {
                return _modifyTimeFrameCmd;
            }
        }

        private ICommand _closeCmd;
        public ICommand CloseCmd
        {
            get
            {
                return _closeCmd;
            }
        }

        private ICommand _playCmd;
        public ICommand PlayCmd
        {
            get
            {
                return _playCmd;
            }
        }

        private bool _isPlayEnabled;
        public bool IsPlayEnabled
        {
            get
            {
                return _isPlayEnabled;
            }

            set
            {
                _isPlayEnabled = value;
                RaisePropertyChanged(() => IsPlayEnabled);
            }
        }

        private ICommand _replayCmd;
        public ICommand ReplayCmd
        {
            get
            {
                return _replayCmd;
            }
        }

        private bool _isReplayEnabled;
        public bool IsReplayEnabled
        {
            get
            {
                return _isReplayEnabled;
            }

            set
            {
                _isReplayEnabled = value;
                RaisePropertyChanged(() => IsReplayEnabled);
            }
        }

        private ICommand _stopCmd;
        public ICommand StopCmd
        {
            get
            {
                return _stopCmd;
            }
        }

        private bool _isStopEnabled;
        public bool IsStopEnabled
        {
            get
            {
                return _isStopEnabled;
            }

            set
            {
                _isStopEnabled = value;
                RaisePropertyChanged(() => IsStopEnabled);
            }
        }

        #endregion

        #region Constructor

        public MovieViewModel()
        {

            _saveAsGifCmd = new RelayCommand(SaveMovieAsGif);
            _modifyTimeFrameCmd = new RelayCommand(ModifyTimeFrame);
            _closeCmd = new RelayCommand(CloseScreen);
            _playCmd = new RelayCommand(Play);
            _replayCmd = new RelayCommand(Replay);
            _stopCmd = new RelayCommand(Stop);

            IsStopEnabled = true;

            Messenger.Default.Register<ModalWindowResultMessage>(this, UpdateTimeFrame);
        }

        private void Stop(object obj)
        {
            ResetTimer(false);
        }

        private void Replay(object obj)
        {
            StartDisplayingMovie();
        }

        private void Play(object obj)
        {
            StartDisplayingMovie();
        }

        #endregion

        #region MessagesHandler

        private void UpdateTimeFrame(ModalWindowResultMessage message)
        {
            Messenger.Default
                     .Send(new CloseModalWindowMessage());

            int timeFrame = message.TimeFrame;
            if (timeFrame == 0)
            {
                return;
            }

            TimeFrame = timeFrame;
        } 

        #endregion

        #region CommandHandlers

        private void SaveMovieAsGif(object obj)
        {
            try
            {
                SaveFileDialog fileDialog = new SaveFileDialog();
                fileDialog.Title = "Select destination file";
                fileDialog.Filter = "Gif Image|*.gif";

                DialogResult dialogResult = fileDialog.ShowDialog();
                if (dialogResult != DialogResult.OK)
                {
                    return;
                }

                string tempFilePath = Path.GetTempPath();
                Images.ToImageList(500, 500, tempFilePath)
                      .SaveAnimatedGifImage(fileDialog.FileName, 
                                            TimeSpan.FromMilliseconds(TimeFrame));
                MessageBox.Show("Gif created!");

                DeleteTempImages(tempFilePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ModifyTimeFrame(object obj)
        {
            if (IsStopEnabled)
            {
                ResetTimer(true);
            }

            Messenger.Default
                     .Send(new OpenModalWindowMessage());
        }

        private void CloseScreen(object obj)
        {
            ResetTimer(true);
            Messenger.Default
                     .Send(new CloseMovieWindowMessage());
        }

        #endregion

        #region PrivateMethods

        private void StartDisplayingMovie()
        {
            IsStopEnabled = true;
            IsReplayEnabled = false;
            IsPlayEnabled = false;

            _dispatcherTimer = new DispatcherTimer();
            _dispatcherTimer.Tick += dispatcherTimer_Tick;
            _dispatcherTimer.Interval = TimeSpan.FromMilliseconds(TimeFrame);
            _dispatcherTimer.Start();
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            _index++;
            if (ImagesSource.Count <= _index)
            {
                ResetTimer(true);
                return;
            }

            ImgSource = ImagesSource[_index];
        }

        private void ResetTimer(bool resetIndex)
        {
            if (resetIndex)
            {
                _index = 0;
            }

            _dispatcherTimer?.Stop();
            _dispatcherTimer = null;

            IsReplayEnabled = _index == 0;
            IsStopEnabled = false;
            IsPlayEnabled = _index != 0;
        }

        private void DeleteTempImages(string tempFilePath)
        {
            foreach (string path in Images.Select(i => i.FilePath).ToList())
            {
                File.Delete(Path.Combine(tempFilePath, Path.GetFileName(path)));
            }
        }

        #endregion
    }
}
