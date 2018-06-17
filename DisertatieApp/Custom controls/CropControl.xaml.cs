using DisertatieApp.Messages;
using GalaSoft.MvvmLight.Messaging;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media.Imaging;
using System.IO;
using System.Windows.Media;
using System.Windows;
using DisertatieApp.Utilities;
using System;

namespace DisertatieApp.Custom_controls
{
    /// <summary>
    /// Interaction logic for CropControl.xaml
    /// </summary>
    public partial class CropControl : UserControl
    {
        public int ImageWidth
        {
            get { return (int)GetValue(ImageWidthProperty); }
            set { SetValue(ImageWidthProperty, value); }
        }

        public static readonly DependencyProperty ImageWidthProperty =
            DependencyProperty.Register("ImageWidth", typeof(int), typeof(UserControl));

        public int ImageHeight
        {
            get { return (int)GetValue(ImageHeightProperty); }
            set { SetValue(ImageHeightProperty, value); }
        }

        public static readonly DependencyProperty ImageHeightProperty =
            DependencyProperty.Register("ImageHeight", typeof(int), typeof(UserControl));


        public ImageSource ImgSource
        {
            get { return (ImageSource)GetValue(ImgSourceProperty); }
            set { SetValue(ImgSourceProperty, value); }
        }

        public static readonly DependencyProperty ImgSourceProperty =
            DependencyProperty.Register("ImgSource", typeof(ImageSource), typeof(CropControl));


        private CroppingAdorner _clp;
        private FrameworkElement _felCur = null;
        private Brush _brOriginal;

        public CropControl()
        {
            InitializeComponent();

            Messenger.Default.Register<InitializeCropAdornerMessage>(this, OnInitializeCropAdorner);
            Messenger.Default.Register<SaveCroppedImageMessage>(this, OnSaveCropImage);
            Messenger.Default.Register<DestroyCropAdornerMessage>(this, OnDestroyCropAdorner);
       }


        private void OnDestroyCropAdorner(DestroyCropAdornerMessage obj)
        {
            RemoveCropFromCur();
        }

        private void RemoveCropFromCur()
        {
            if (_clp == null)
            {
                return;
            }

            AdornerLayer aly = AdornerLayer.GetAdornerLayer(_felCur);
            aly.Remove(_clp);
        }

        private void OnSaveCropImage(SaveCroppedImageMessage message)
        {
            BitmapSource croppedBitmapFrame = _clp.BpsCrop();

            BitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(croppedBitmapFrame));

            string tempFile = Path.GetTempFileName();
            using (FileStream imageFile =
                    new FileStream(tempFile, FileMode.Create, FileAccess.Write))
            {
                encoder.Save(imageFile);
                imageFile.Flush();
                imageFile.Close();
            }

            Messenger.Default
                     .Send(new CroppedImageSavedMessage()
                     {
                         ImagePath = tempFile
                     });
        }

        private void OnInitializeCropAdorner(InitializeCropAdornerMessage obj)
        {
            if (_clp != null)
            {
                return;
            }

            AddCropToElement(currentImage);
            RefreshCropImage();
        }

        private void AddCropToElement(FrameworkElement fel)
        {
            if (_felCur != null)
            {
                RemoveCropFromCur();
            }
            Rect rcInterior = new Rect(
                fel.ActualWidth * 0.2,
                fel.ActualHeight * 0.2,
                fel.ActualWidth * 0.6,
                fel.ActualHeight * 0.6);
            AdornerLayer aly = AdornerLayer.GetAdornerLayer(fel);
            _clp = new CroppingAdorner(fel, rcInterior);
            aly.Add(_clp);

            //imgCrop.Source = _clp.BpsCrop();

            _clp.CropChanged += CropChanged;
            _felCur = fel;
        }

        private void RefreshCropImage()
        {
            if (_clp != null)
            {
                //Rect rc = _clp.ClippingRectangle;

                //tblkClippingRectangle.Text = string.Format(
                //    "Clipping Rectangle: ({0:N1}, {1:N1}, {2:N1}, {3:N1})",
                //    rc.Left,
                //    rc.Top,
                //    rc.Right,
                //    rc.Bottom);
                //imgCrop.Source = _clp.BpsCrop();
            }
        }

        private void CropChanged(Object sender, RoutedEventArgs rea)
        {
            RefreshCropImage();
        }
    }
}
