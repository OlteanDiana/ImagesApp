using GalaSoft.MvvmLight;

namespace DisertatieApp.ViewModels
{
    public class ImagesViewerViewModel : ViewModelBase
    {
        private string _filePath;

        public string FilePath
        {
            get
            {
                return _filePath;
            }

            set
            {
                _filePath = value;
                RaisePropertyChanged(() => FilePath);
            }
        }
    }
}
