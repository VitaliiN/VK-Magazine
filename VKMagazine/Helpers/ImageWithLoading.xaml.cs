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
using System.Windows.Media.Imaging;

namespace VKMagazine.Helpers
{
    public partial class ImageWithLoading : UserControl
    {
        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("Source", typeof(ImageSource), typeof(ImageWithLoading), new PropertyMetadata(default(ImageSource)));

        public ImageSource Source
        {
            get { return (ImageSource)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }



        public string PostUrlShare
        {
            get { return (string)GetValue(PostUrlShareProperty); }
            set { SetValue(PostUrlShareProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PostUrlShare.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PostUrlShareProperty =
            DependencyProperty.Register("PostUrlShare", typeof(string), typeof(ImageWithLoading), new PropertyMetadata(""));

        

        public string Url
        {
            get { return (string)GetValue(UrlProperty); }
            set { SetValue(UrlProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Url.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UrlProperty =
            DependencyProperty.Register("Url", typeof(string), typeof(ImageWithLoading), new PropertyMetadata("WWW"));

        
        
        public ImageWithLoading()
        {
            InitializeComponent();
            object o =  MainImage.Source;
            
        }

        private void RemoteImage_OnLoaded(object sender, RoutedEventArgs e)
        {
            temporaryImage.Visibility = Visibility.Collapsed;
        }

        private void MainImage_Loaded(object sender, RoutedEventArgs e)
        {
            if(Url!=null && Url.Contains("isostore"))
                
            //if(Source!=null && ((BitmapImage) Source).UriSource.ToString().Length==0)
              temporaryImage.Visibility = Visibility.Collapsed;
        }
    }
}
