using DisertatieApp.Messages;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using System.Windows;

namespace DisertatieApp.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private ViewModelLocator _locator;

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            _locator = Application.Current.TryFindResource("Locator") as ViewModelLocator;
            Messenger.Default.Register<OpenWindowMessage>(this, ProcessOpenWindowMessage);
        }

        private void ProcessOpenWindowMessage(OpenWindowMessage message)
        {
            if (_locator == null)
            {
                return;
            }

            var windowVM = _locator.ViewerVM;
            windowVM.CurrentFilePath = message.CurrentFilePath;
            windowVM.Files = message.Files;

            var modalWindow = _locator.Viewer;
            modalWindow.DataContext = windowVM;

            modalWindow.Show();
        }
    }
}