using DisertatieApp.Messages;
using DisertatieApp.ViewModels;
using GalaSoft.MvvmLight.Messaging;
using System.ComponentModel;
using System.Windows;
using System;
using DisertatieApp.Custom_controls;

namespace DisertatieApp.Utilities
{
    public class ViewsMediator
    {
        #region Fields

        private ViewModelLocator _locator;
        private Window _modal;

        #endregion

        #region Constructor

        public ViewsMediator(ViewModelLocator vmLocator)
        {
            _locator = vmLocator;
            RegisterMessages();
        }

        #endregion

        #region RegisterMessages

        private void RegisterMessages()
        {
            Messenger.Default.Register<OpenImageViewMessage>(this, OpenImageView);
            Messenger.Default.Register<OpenMovieViewMessage>(this, OpenMovieView);
            Messenger.Default.Register<OpenModalWindowMessage>(this, OpenModalWindow);
            Messenger.Default.Register<CloseModalWindowMessage>(this, CloseModalWindow);
            Messenger.Default.Register<CloseMovieWindowMessage>(this, CloseMovieView);
        }

        #endregion

        #region MessagesHandler

        private void OpenImageView(OpenImageViewMessage message)
        {
            if (_locator == null)
            {
                return;
            }

            var windowVM = _locator.ImageViewerVM;
            windowVM.CurrentFilePath = message.CurrentFilePath;
            windowVM.Files = message.Files;

            var modalWindow = _locator.ImageViewer;
            modalWindow.DataContext = windowVM;
            modalWindow.Closing += Window_Closing;

            modalWindow.Show();
        }

        private void OpenMovieView(OpenMovieViewMessage message)
        {
            if (_locator == null)
            {
                return;
            }

            var windowVM = _locator.MovieViewerVM;
            windowVM.Images = message.Images;
            windowVM.TimeFrame = message.TimeFrame;
            windowVM.ImagesSource = message.ImagesSource;
            windowVM.ImgSource = message.ImgSource;

            var modalWindow = _locator.MovieViewer;
            modalWindow.DataContext = windowVM;
            modalWindow.Closing += Window_Closing;

            modalWindow.Show();
        }

        private void OpenModalWindow(OpenModalWindowMessage obj)
        {
            _modal = new Window
            {
                Title = "Select time frame",
                Content = new CustomDialog(),
                Width = 250,
                Height = 150,
                ResizeMode = ResizeMode.NoResize,
            };

            _modal.ShowDialog();
        }

        private void CloseModalWindow(CloseModalWindowMessage obj)
        {
            _modal.Close();
            _modal = null;
        }

        private void CloseMovieView(CloseMovieWindowMessage obj)
        {
            if (_locator == null)
            {
                return;
            }

            var window = _locator.MovieViewer;
            window.Visibility = Visibility.Collapsed;
        }

        #endregion

        #region PrivateMethods

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            var currentWindow = sender as Window;
            if (currentWindow == null)
            {
                return;
            }

            Messenger.Default
                     .Send(new CleanUpViewsMessage()
                     {
                         DeleteFiles = false
                     });

            currentWindow.Visibility = Visibility.Collapsed;
            e.Cancel = true;
        } 

        #endregion
    }
}
