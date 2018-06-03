using CommonServiceLocator;
using DisertatieApp.Views;
using GalaSoft.MvvmLight.Ioc;

namespace DisertatieApp.ViewModels
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<MainView>();

            SimpleIoc.Default.Register<ThumbnailContainerViewModel>();
            SimpleIoc.Default.Register<ThumbnailContainerView>();

            SimpleIoc.Default.Register<ImagesViewerViewModel>();
            SimpleIoc.Default.Register<ImagesViewerView>();
        }

        #endregion

        #region Properties

        public MainViewModel MainVM
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MainViewModel>();
            }
        }
        public MainView Main
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MainView>();
            }
        }

        public ThumbnailContainerViewModel ContainerVM
        {
            get
            {
                return ServiceLocator.Current.GetInstance<ThumbnailContainerViewModel>();
            }
        }
        public ThumbnailContainerView Container
        {
            get
            {
                return ServiceLocator.Current.GetInstance<ThumbnailContainerView>();
            }
        }

        public ImagesViewerViewModel ViewerVM
        {
            get
            {
                return ServiceLocator.Current.GetInstance<ImagesViewerViewModel>();
            }
        }
        public ImagesViewerView Viewer
        {
            get
            {
                return ServiceLocator.Current.GetInstance<ImagesViewerView>();
            }
        }

        #endregion

        #region Cleanup

        public static void Cleanup()
        {
        } 

        #endregion
    }
}