using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace VKMagazine
{
    public partial class SplashScreenPage : PhoneApplicationPage
    {
        public SplashScreenPage()
        {
            InitializeComponent();
            Loaded += SplashScreenPage_Loaded;
            
        }

        void SplashScreenPage_Loaded(object sender, RoutedEventArgs e)
        {
            SystemTray.IsVisible = false;
        }
    }
}