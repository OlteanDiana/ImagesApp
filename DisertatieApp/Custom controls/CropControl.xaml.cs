using CroppingImageLibrary;
using DisertatieApp.Messages;
using GalaSoft.MvvmLight.Messaging;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System;
using System.Windows.Media.Imaging;
using System.IO;
using System.Windows.Media;
using System.Windows;

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




        public CroppingAdorner CroppingAdorner;

        public CropControl()
        {
            InitializeComponent();

            Messenger.Default.Register<InitializeCropAdornerMessage>(this, OnInitializeCropAdorner);
            Messenger.Default.Register<SaveCroppedImageMessage>(this, OnSaveCropImage);
            Messenger.Default.Register<DestroyCropAdornerMessage>(this, OnDestroyCropAdorner);

            CanvasPanel.MouseLeftButtonDown += CanvasPanel_MouseLeftButtonDown;
        }

        private void OnDestroyCropAdorner(DestroyCropAdornerMessage obj)
        {
            if (CroppingAdorner == null)
            {
                return;
            }

            AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(CanvasPanel);
            adornerLayer.Remove(CroppingAdorner);
            CroppingAdorner = null;
        }

        private void OnSaveCropImage(SaveCroppedImageMessage message)
        {
            BitmapFrame croppedBitmapFrame = CroppingAdorner.GetCroppedBitmapFrame();

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

        private void CanvasPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (CroppingAdorner == null)
            {
                return;
            }

            CroppingAdorner.CaptureMouse();
            CroppingAdorner.MouseLeftButtonDownEventHandler(sender, e);
        }

        private void OnInitializeCropAdorner(InitializeCropAdornerMessage obj)
        {
            if (CroppingAdorner != null)
            {
                return;
            }

            AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(CanvasPanel);
            CroppingAdorner = new CroppingAdorner(CanvasPanel);
            adornerLayer.Margin = CurrentImage.Margin;
            adornerLayer.Add(CroppingAdorner);
        }
    }
}
