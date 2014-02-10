using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using VKMagazine.Helpers;
using System.IO.IsolatedStorage;

namespace VKMagazine
{
    public partial class VkAuthPage : PhoneApplicationPage
    {
        private ProgressIndicator progressIndicator;
        private const string App_id = "4006032";
        private const string Redirect_uri = "https://oauth.vk.com/blank.html";

        public VkAuthPage()
        {
            InitializeComponent();
            browser.LoadCompleted += browser_LoadCompleted;
            browser.Navigated += browser_Navigated;
            browser.Navigating += browser_Navigating;
            SetProgressIndicator();
            
        }

        void browser_Navigating(object sender, NavigatingEventArgs e)
        {
            progressIndicator.IsIndeterminate = true;
            progressIndicator.IsVisible = true;
        }

        void browser_Navigated(object sender, NavigationEventArgs e)
        {
            
            try
            {
                if (e.Uri.ToString().Contains(VkHelper.AccessToken))
                {
                    string[] response = e.Uri.ToString().Split(new char[] { '=', '&' });
                    VkHelper.CurrentAccess_token = response[1];
                    VkHelper.CurrentUserId = response[5];
                    var settings = IsolatedStorageSettings.ApplicationSettings;
                    if (settings.Contains(VkHelper.AccessToken))
                    {
                        settings[VkHelper.AccessToken] = response[1];
                    }
                    else
                    {
                        settings.Add(VkHelper.AccessToken, response[1]);
                    }
                    if (settings.Contains(VkHelper.VkUserId))
                    {
                        settings[VkHelper.VkUserId] = response[5];
                    }
                    else
                    {
                        settings.Add(VkHelper.VkUserId, response[5]);
                    }
                    settings.Save();
                    NavigationService.GoBack();
                    //NavigationService.Navigate(new Uri("/GroupListPage.xaml", UriKind.Relative));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при авторизации через вк");
            }
            
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            await browser.ClearCookiesAsync();
            string url = String.Format("https://oauth.vk.com/authorize?client_id={0}&scope=groups,offline,friends,wall&redirect_uri={1}&display=mobile&v=5.7&response_type=token", App_id, Redirect_uri);
            browser.Navigate(new Uri(url)); 
        }

        void browser_LoadCompleted(object sender, NavigationEventArgs e)
        {
            progressIndicator.IsIndeterminate = false; ;
            progressIndicator.IsVisible = false;
        }

        void SetProgressIndicator()
        {
            // progressIndicator = SystemTray.ProgressIndicator;
            //if (progressIndicator != null)
            //{
            //    return;
            //}
            progressIndicator = new ProgressIndicator();
            SystemTray.SetProgressIndicator(this, progressIndicator);
            progressIndicator.Text = "Загрузка";
        }
    }
}