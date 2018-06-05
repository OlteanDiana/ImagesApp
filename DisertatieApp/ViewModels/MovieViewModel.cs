using DisertatieApp.Models;
using DisertatieApp.Utilities;
using GalaSoft.MvvmLight;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Threading;
using System;

namespace DisertatieApp.ViewModels
{
    public class MovieViewModel : ViewModelBase
    {
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

        private List<ThumbnailFile> _images;
        public List<ThumbnailFile> Images
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

        #endregion

        #region Constructor

        public MovieViewModel()
        {
            _dispatcherTimer = new DispatcherTimer();
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

        #endregion
    }
}
