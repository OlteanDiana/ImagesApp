using DisertatieApp.Messages;
using DisertatieApp.Utilities;
using GalaSoft.MvvmLight.Messaging;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System;

namespace DisertatieApp.Custom_controls
{
    /// <summary>
    /// Interaction logic for CustomDialog.xaml
    /// </summary>
    public partial class CustomDialog : UserControl
    {
        #region Properties

        public List<int> CmbItems
        {
            get { return (List<int>)GetValue(CmbItemsProperty); }
            set { SetValue(CmbItemsProperty, value); }
        }

        public static readonly DependencyProperty CmbItemsProperty =
            DependencyProperty.Register("CmbItems", typeof(List<int>), typeof(CustomDialog));

        #endregion

        #region Constructor

        public CustomDialog(int timeframe)
        {
            InitializeComponent();
            InitializeComboBox(timeframe);
        }

        #endregion

        #region Events

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

        #endregion

        #region Helpers

        private void InitializeComboBox(int timeFrame)
        {
            CmbItems = UtilitiesMethods.Range(100, 1000, 100).ToList();
            cmbTime.SelectedIndex = CmbItems.IndexOf(timeFrame);
        } 

        #endregion
    }
}
