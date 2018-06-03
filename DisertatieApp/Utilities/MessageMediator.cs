using DisertatieApp.Messages;
using DisertatieApp.ViewModels;
using GalaSoft.MvvmLight.Messaging;

namespace DisertatieApp.Utilities
{
    public class MessageMediator
    {
        private ViewModelLocator _locator;


        public MessageMediator(ViewModelLocator vmLocator)
        {
            _locator = vmLocator;
            RegisterMessages();
        }

        private void RegisterMessages()
        {
            Messenger.Default.Register<OpenImageViewerMessage>(this, ProcessOpenImageViewerMessage);
        }

        private void ProcessOpenImageViewerMessage(OpenImageViewerMessage message)
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
