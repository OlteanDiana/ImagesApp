using DisertatieApp.Models;
using DisertatieApp.Utilities;
using GalaSoft.MvvmLight;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Threading;
using System;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Drawing;
using System.Windows;
using System.Windows.Interop;
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
        private bool _isMovieRunning;

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
                ImgSource = Images.Count > _index ? Images[_index].FilePath.SetImageSource()
                                                  : null;
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

        #endregion

        #region Constructor

        public MovieViewModel()
        {
            _dispatcherTimer = new DispatcherTimer();

            _saveAsGifCmd = new RelayCommand(SaveMovieAsGif);
            _modifyTimeFrameCmd = new RelayCommand(ModifyTimeFrame);
            _closeCmd = new RelayCommand(CloseScreen);

            Messenger.Default.Register<ModalWindowResultMessage>(this, UpdateTimeFrame);
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
                CreateGIF(Images.ToImageList(500, 500, tempFilePath), fileDialog.FileName);
                System.Windows.MessageBox.Show("Gif created!");
                DeleteTempImages(tempFilePath);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
        }

        private void ModifyTimeFrame(object obj)
        {
            if (_isMovieRunning)
            {
                ResetTimer();
            }

            Messenger.Default
                     .Send(new OpenModalWindowMessage());
        }

        private void CloseScreen(object obj)
        {
            Messenger.Default
                     .Send(new CloseMovieWindowMessage());
        }

        #endregion

        #region PrivateMethods

        private void StartDisplayingMovie()
        {
            _index = 0;
            _isMovieRunning = true;

            _dispatcherTimer.Tick += dispatcherTimer_Tick;
            _dispatcherTimer.Interval = new TimeSpan(0, 0, TimeFrame);
            _dispatcherTimer.Start();
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            _index++;
            if (Images.Count <= _index)
            {
                ResetTimer();
                return;
            }

            ImgSource = Images[_index].FilePath.SetImageSource();
        }

        private void ResetTimer()
        {
            _dispatcherTimer.Stop();
            _isMovieRunning = false;
            _index = 0;
        }

        private void DeleteTempImages(string tempFilePath)
        {
            foreach (string path in Images.Select(i => i.FilePath).ToList())
            {
                File.Delete(Path.Combine(tempFilePath, Path.GetFileName(path)));
            }
        }

        public void CreateGIF(List<Image> images, string path)
        {
            try
            {
                GifBitmapEncoder gifEncoder = new GifBitmapEncoder();
                foreach (Bitmap bmpImage in images)
                {
                    var gifSource = Imaging.CreateBitmapSourceFromHBitmap(
                                        bmpImage.GetHbitmap(),
                                        IntPtr.Zero,
                                        Int32Rect.Empty,
                                        BitmapSizeOptions.FromEmptyOptions());

                    gifEncoder.Frames.Add(BitmapFrame.Create(gifSource));
                }

                gifEncoder.Save(new FileStream(path, FileMode.Create));
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }
        }

        #endregion
    }
}
