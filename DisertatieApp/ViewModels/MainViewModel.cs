using DisertatieApp.Messages;
using DisertatieApp.Utilities;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using System.Windows.Forms;
using System.Windows.Input;

namespace DisertatieApp.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        #region Constants

        private const string LOCATOR = "Locator";

        #endregion

        #region Fields

        private OpenViewMessageMediator _mediator;
        private FolderBrowserDialog _folderBrowser;
        private FolderHandler _folderHandler;
        private int _minutesSpan = 5;

        #endregion

        #region Properties

        private ICommand _openFolderBrowserCmd;
        public ICommand OpenFolderBrowserCmd
        {
            get
            {
                return _openFolderBrowserCmd;
            }
        }
        
        private string _folderPath;
        public string FolderPath
        {
            get
            {
                return _folderPath;
            }

            set
            {
                _folderPath = value;
                RaisePropertyChanged(() => FolderPath);
            }
        }

        private ICommand _openMovieViewCmd;
        public ICommand OpenMovieViewCmd
        {
            get
            {
                return _openMovieViewCmd;
            }
        }

        private bool _isMovieButtonEnabled;
        public bool IsMovieButtonEnabled
        {
            get
            {
                return _isMovieButtonEnabled;
            }

            set
            {
                _isMovieButtonEnabled = value;
                RaisePropertyChanged(() => IsMovieButtonEnabled);
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            _folderBrowser = new FolderBrowserDialog();
            _openFolderBrowserCmd = new RelayCommand(OpenFolderBrowser);
            _openMovieViewCmd = new RelayCommand(OpenMovieView);
            _mediator = new OpenViewMessageMediator(System.Windows.Application.Current.TryFindResource(LOCATOR) as ViewModelLocator);
        }

        #endregion

        #region CommandHelpers

        private void OpenFolderBrowser(object obj)
        {
            DialogResult result = _folderBrowser.ShowDialog();

            if (result != DialogResult.OK
                || string.IsNullOrWhiteSpace(_folderBrowser.SelectedPath))
            {
                MessageBox.Show("No folder selected.");
                return;
            }

            LoadImagesFromFolder();
            HandleEnableDisableForMovieButton();
        }

        private void OpenMovieView(object obj)
        {
            Messenger.Default
                     .Send(new OpenMovieViewMessage()
                     {
                         Images = _folderHandler.GetSimilarImagesList(0),
                         TimeFrame = 5
                     });
        }

        #endregion

        #region PrivateMethods

        private void LoadImagesFromFolder()
        {
            _folderPath = _folderBrowser.SelectedPath;
            _folderHandler = new FolderHandler(_folderPath, _minutesSpan);

            Messenger.Default
                     .Send(new UpdateImagesMessage()
                                {
                                    Images = _folderHandler.Images
                                });
        }

        private void HandleEnableDisableForMovieButton()
        {
            IsMovieButtonEnabled = _folderHandler?.SimilarImages.Count > 0;
        }

        #endregion
    }
}