using DisertatieApp.Messages;
using DisertatieApp.Utilities;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using System.Windows.Input;
using System.Windows.Forms;

namespace DisertatieApp.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        #region Constants

        private const string LOCATOR = "Locator";

        #endregion

        #region Fields

        private ViewsMediator _mediator;
        private FolderBrowserDialog _folderBrowser;
        private ImagesHandler _imagesHandler;
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

        private ICommand _windowClosingCommand;
        public ICommand WindowClosingCommand
        {
            get
            {
                return _windowClosingCommand;
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
            _windowClosingCommand = new RelayCommand(OnWindowClosing);

            _mediator = new ViewsMediator(System.Windows.Application.Current.TryFindResource(LOCATOR) as ViewModelLocator);
        }

        #endregion

        #region CommandHelpers

        private void OpenFolderBrowser(object obj)
        {
            DialogResult result = _folderBrowser.ShowDialog();

            if (result != DialogResult.OK
                || string.IsNullOrWhiteSpace(_folderBrowser.SelectedPath))
            {
                System.Windows.MessageBox.Show("No folder selected.");
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
                         Images = _imagesHandler.GetSimilarImagesList(0),
                         TimeFrame = 1
                     });
        }

        private void OnWindowClosing(object obj)
        {
            Messenger.Default
                     .Send(new CleanUpViewsMessage()
                     {
                         DeleteFiles = true
                     });
            System.Windows.Application.Current.Shutdown();
        }

        #endregion

        #region PrivateMethods

        private void LoadImagesFromFolder()
        {
            _folderPath = _folderBrowser.SelectedPath;

            _imagesHandler = new ImagesHandler(_folderPath, _minutesSpan);

            Messenger.Default
                     .Send(new UpdateImagesMessage()
                                {
                                    Images = _imagesHandler.Images,
                                    ImagesHandler = _imagesHandler
                                });
        }

        private void HandleEnableDisableForMovieButton()
        {
            IsMovieButtonEnabled = _imagesHandler?.SimilarImages.Count > 0;
        }

        #endregion
    }
}