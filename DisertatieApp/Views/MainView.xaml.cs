using DisertatieApp.Messages;
using GalaSoft.MvvmLight.Messaging;
using System.Windows;
using CommonServiceLocator;
using DisertatieApp.ViewModels;

namespace DisertatieApp.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainView : Window
    {
        public MainView()
        {
            InitializeComponent();
            Messenger.Default.Register<OpenWindowMessage>(this, ProcessOpenWindowMessage);
        }

        private void ProcessOpenWindowMessage(OpenWindowMessage message)
        {
            var windowVM = ServiceLocator.Current.GetInstance<ImagesViewerViewModel>();
            windowVM.FilePath = message.FilePath;

            var modalWindow = new ImagesViewerView()
            {
                DataContext = windowVM
            };

            modalWindow.Show();
        }
    }
}
