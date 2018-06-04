using DisertatieApp.Models;
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace DisertatieApp.ViewModels
{
    public class MovieViewModel : ViewModelBase
    {
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
                StartDisplayingMovie();
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

        public MovieViewModel()
        {
            
        }

        private void StartDisplayingMovie()
        {
           
        }
    }
}
