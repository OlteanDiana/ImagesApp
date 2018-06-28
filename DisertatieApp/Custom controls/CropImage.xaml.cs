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
    public partial class CropImage : UserControl
    {
        #region Fields

        private CroppingAdorner _croppingAdorner;
        private FrameworkElement _currentElement = null;

        #endregion

        #region Properties

        public ImageSource ImgSource
        {
            get { return (ImageSource)GetValue(ImgSourceProperty); }
            set { SetValue(ImgSourceProperty, value); }
        }

        public static readonly DependencyProperty ImgSourceProperty =
            DependencyProperty.Register("ImgSource", typeof(ImageSource), typeof(CropImage));

        #endregion

        #region Constructor

        public CropImage()
        {
            InitializeComponent();

            Messenger.Default.Register<InitializeCropAdornerMessage>(this, OnInitializeCropAdorner);
            Messenger.Default.Register<SaveCroppedImageMessage>(this, OnSaveCropImage);
            Messenger.Default.Register<DestroyCropAdornerMessage>(this, OnDestroyCropAdorner);
        }

        #endregion

        #region MessagesHandlers

        private void OnInitializeCropAdorner(InitializeCropAdornerMessage message)
        {
            if (_croppingAdorner != null)
            {
                return;
            }

            AddCropToElement(currentImage);
        }

        private void OnSaveCropImage(SaveCroppedImageMessage message)
        {
            BitmapSource croppedBitmapFrame = _croppingAdorner.BpsCrop();

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

        private void OnDestroyCropAdorner(DestroyCropAdornerMessage message)
        {
            RemoveCrop();
        }

        #endregion
        
        #region Helpers

        private void AddCropToElement(FrameworkElement element)
        {
            if (_currentElement != null)
            {
                RemoveCrop();
            }

            Rect rectangle = new Rect(
                element.ActualWidth * 0.2,
                element.ActualHeight * 0.2,
                element.ActualWidth * 0.6,
                element.ActualHeight * 0.6);

            AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(element);
            _croppingAdorner = new CroppingAdorner(element, rectangle);
            adornerLayer.Add(_croppingAdorner);

            _currentElement = element;
        }

        private void RemoveCrop()
        {
            if (_croppingAdorner == null)
            {
                return;
            }

            AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(_currentElement);
            adornerLayer.Remove(_croppingAdorner);
            _croppingAdorner = null;
        } 

        #endregion

    }
}
