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

            SimpleIoc.Default.Register<ImagesViewModel>();
            SimpleIoc.Default.Register<ImagesView>();

            SimpleIoc.Default.Register<MovieViewModel>();
            SimpleIoc.Default.Register<MovieView>();
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

        public ImagesViewModel ImageViewerVM
        {
            get
            {
                return ServiceLocator.Current.GetInstance<ImagesViewModel>();
            }
        }
        public ImagesView ImageViewer
        {
            get
            {
                return ServiceLocator.Current.GetInstance<ImagesView>();
            }
        }

        public MovieViewModel MovieViewerVM
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MovieViewModel>();
            }
        }
        public MovieView MovieViewer
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MovieView>();
            }
        }

        #endregion

        #region Cleanup

        public static void Cleanup()
        {
            SimpleIoc.Default.Unregister<MainViewModel>();
            SimpleIoc.Default.Unregister<MainView>();

            SimpleIoc.Default.Unregister<ThumbnailContainerViewModel>();
            SimpleIoc.Default.Unregister<ThumbnailContainerView>();

            SimpleIoc.Default.Unregister<ImagesViewModel>();
            SimpleIoc.Default.Unregister<ImagesView>();

            SimpleIoc.Default.Unregister<MovieViewModel>();
            SimpleIoc.Default.Unregister<MovieView>();

            SimpleIoc.Default.Unregister<ViewModelLocator>();
        }

        #endregion
    }
}