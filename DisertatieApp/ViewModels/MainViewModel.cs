using DisertatieApp.Utilities;
using GalaSoft.MvvmLight;
using System.Windows;

namespace DisertatieApp.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private const string LOCATOR = "Locator";
        private MessageMediator _messageMediator;

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            _messageMediator = new MessageMediator(Application.Current.TryFindResource(LOCATOR) as ViewModelLocator);
        }
    }
}