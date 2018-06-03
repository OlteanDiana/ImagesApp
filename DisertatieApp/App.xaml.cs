﻿using GalaSoft.MvvmLight.Threading;
using System.Windows;

namespace DisertatieApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        static App()
        {
            DispatcherHelper.Initialize();
        }
    }
}
