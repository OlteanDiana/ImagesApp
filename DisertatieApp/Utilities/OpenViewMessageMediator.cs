using System;
using DisertatieApp.Messages;
using DisertatieApp.ViewModels;
using GalaSoft.MvvmLight.Messaging;
using System.ComponentModel;
using System.Windows;

namespace DisertatieApp.Utilities
{
    public class OpenViewMessageMediator
    {
        #region Fields

        private ViewModelLocator _locator;

        #endregion

        #region Constructor

        public OpenViewMessageMediator(ViewModelLocator vmLocator)
        {
            _locator = vmLocator;
            RegisterMessages();
        }

        #endregion

        #region RegisterMessages

        private void RegisterMessages()
        {
            Messenger.Default.Register<OpenImageViewMessage>(this, ProcessOpenImageViewMessage);
            Messenger.Default.Register<OpenMovieViewMessage>(this, ProcessOpenMovieViewMessage);
        }

        #endregion

        #region MessagesHandler

        private void ProcessOpenImageViewMessage(OpenImageViewMessage message)
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

        private void ProcessOpenMovieViewMessage(OpenMovieViewMessage message)
        {
            if (_locator == null)
            {
                return;
            }

            var windowVM = _locator.MovieViewerVM;
            windowVM.Images = message.Images;
            windowVM.TimeFrame = message.TimeFrame;

            var modalWindow = _locator.MovieViewer;
            modalWindow.DataContext = windowVM;
            modalWindow.Closing += Window_Closing;

            modalWindow.Show();
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

            currentWindow.Visibility = Visibility.Collapsed;
            e.Cancel = true;
        } 

        #endregion
    }
}
