using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DisertatieApp.Custom_controls
{
    /// <summary>
    /// Interaction logic for NavigationButton.xaml
    /// </summary>
    public partial class ImageButton : UserControl
    {
        #region Properties

        public string ContentImagePath
        {
            get { return (string)GetValue(ContentImagePathProperty); }
            set { SetValue(ContentImagePathProperty, value); }
        }

        public static readonly DependencyProperty ContentImagePathProperty =
            DependencyProperty.Register("ContentImagePath", typeof(string), typeof(ImageButton));


        public ICommand ButtonCommand
        {
            get { return (ICommand)GetValue(ButtonCommandProperty); }
            set { SetValue(ButtonCommandProperty, value); }
        }

        public static readonly DependencyProperty ButtonCommandProperty =
            DependencyProperty.Register("ButtonCommand", typeof(ICommand), typeof(ImageButton));

        #endregion

        #region Constructor

        public ImageButton()
        {
            InitializeComponent();
        } 

        #endregion
    }
}
