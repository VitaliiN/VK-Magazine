﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using VKMagazine.Helpers;

namespace VKMagazine
{
    public partial class SettignsPage : PhoneApplicationPage
    {
        private const string ON = "Включено";
        private const string OFF = "Выключено";
        private bool firstIsneedTohidePostsWithlinks;
        private bool firstIsNeedToHidePostsWithGroups;
        public SettignsPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            
            LinksToggle.IsChecked = firstIsneedTohidePostsWithlinks = SettignsHelper.IsNeedToHidePostsWithLinks;
            LinksToggle.Content = LinksToggle.IsChecked.Value ? ON : OFF;
            GroupsToggle.IsChecked = firstIsNeedToHidePostsWithGroups = SettignsHelper.IsNeedToHidePostsToGroups;
            GroupsToggle.Content = GroupsToggle.IsChecked.Value ? ON : OFF;
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            if (firstIsNeedToHidePostsWithGroups != SettignsHelper.IsNeedToHidePostsToGroups || firstIsneedTohidePostsWithlinks != SettignsHelper.IsNeedToHidePostsWithLinks)
                NewsRefreshHelper.isNeedToRefresh = true;
        }

        private void LinksToggle_Checked(object sender, RoutedEventArgs e)
        {
            SettignsHelper.IsNeedToHidePostsWithLinks = true;
            LinksToggle.Content = ON;
        }

        private void LinksToggle_Unchecked(object sender, RoutedEventArgs e)
        {
            SettignsHelper.IsNeedToHidePostsWithLinks = false;
            LinksToggle.Content = OFF;
        }

        private void GroupsToggle_Unchecked(object sender, RoutedEventArgs e)
        {
            SettignsHelper.IsNeedToHidePostsToGroups = false;
            GroupsToggle.Content = OFF;
        }

        private void GroupsToggle_Checked(object sender, RoutedEventArgs e)
        {
            SettignsHelper.IsNeedToHidePostsToGroups = true;
            GroupsToggle.Content = ON;
        }
    }
}