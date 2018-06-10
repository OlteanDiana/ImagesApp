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

namespace DisertatieApp.ViewModels
{
    public class MovieViewModel : ViewModelBase
    {
        private const string TEMP_PATH = @"C:\Users\Oltean\Desktop\master";

        #region Fields

        private int _index = 0;
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

        #endregion

        #region Constructor

        public MovieViewModel()
        {
            _dispatcherTimer = new DispatcherTimer();
            _saveAsGifCmd = new RelayCommand(SaveMovieAsGif);
        }

        #endregion

        #region PrivateMethods

        private void StartDisplayingMovie()
        {
            _dispatcherTimer.Tick += dispatcherTimer_Tick;
            _dispatcherTimer.Interval = new TimeSpan(0, 0, TimeFrame);
            _dispatcherTimer.Start();
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            _index++;
            if (Images.Count <= _index)
            {
                _dispatcherTimer.Stop();
                _index = 0;
                return;
            }

            ImgSource = Images[_index].FilePath.SetImageSource();
        }

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

                CreateGIF(Images.ToImageList(400, 400, TEMP_PATH), fileDialog.FileName);
                System.Windows.Forms.MessageBox.Show("Gif created!");
                DeleteTempImages();
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }
        }

        private void DeleteTempImages()
        {
            foreach (string path in Images.Select(i => i.FilePath).ToList())
            {
                File.Delete(Path.Combine(TEMP_PATH, Path.GetFileName(path)));
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
