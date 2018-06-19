using DisertatieApp.Messages;
using DisertatieApp.Utilities;
using GalaSoft.MvvmLight.Messaging;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace DisertatieApp.Custom_controls
{
    /// <summary>
    /// Interaction logic for CustomDialog.xaml
    /// </summary>
    public partial class CustomDialog : UserControl
    {
        public List<int> CmbItems
        {
            get { return (List<int>)GetValue(CmbItemsProperty); }
            set { SetValue(CmbItemsProperty, value); }
        }

        public static readonly DependencyProperty CmbItemsProperty =
            DependencyProperty.Register("CmbItems", typeof(List<int>), typeof(CustomDialog));

        public CustomDialog()
        {
            InitializeComponent();
            CmbItems = UtilitiesMethods.Range(100, 1000, 100).ToList();
            cmbTime.SelectedIndex = 0;
        }

        private void buttonSelect_Click(object sender, RoutedEventArgs e)
        {
            Messenger.Default
                     .Send(new ModalWindowResultMessage()
                     {
                         TimeFrame = (int)cmbTime.SelectedItem
                     });

        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            Messenger.Default
                     .Send(new ModalWindowResultMessage()
                     {
                         TimeFrame = 0
                     });
        }
    }
}
