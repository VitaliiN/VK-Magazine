using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Media;
using VKMagazine.Helpers;

namespace VKMagazine
{
    public partial class SplashScreenPage : PhoneApplicationPage
    {
        private bool isNavgatedFromVk;
        private Uri prevPage;
        public SplashScreenPage()
        {
            InitializeComponent();
            Loaded += SplashScreenPage_Loaded;
            prevPage = new Uri("/MainPage.xaml", UriKind.Relative);
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            int count = NavigationService.BackStack.Count();
            for (int i = 0; i < count; i++)
                NavigationService.RemoveBackEntry();
        }

        void SplashScreenPage_Loaded(object sender, RoutedEventArgs e)
        {
            
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            SystemTray.IsVisible = false;
            if (prevPage.ToString().Contains("VkAuthPage.xaml"))
            {
                SettignsHelper.IsNeedToShowSplashScreen = true;
                return;
            }
            if(!SettignsHelper.IsNeedToShowSplashScreen)
                NavigationService.Navigate(new Uri("/MainPage.xaml",UriKind.Relative));
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            prevPage = e.Uri;
            SystemTray.IsVisible = true;
        }

       
      

        private void LoginWithVk_Click(object sender, RoutedEventArgs e)
        {
            SettignsHelper.IsNeedToShowSplashScreen = false;
            NavigationService.Navigate(new Uri("/VkAuthPage.xaml", UriKind.Relative));
        }

        private void SimpleLoginButton_Click(object sender, RoutedEventArgs e)
        {
            SettignsHelper.IsNeedToShowSplashScreen = false;
            NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
        }
    }
}