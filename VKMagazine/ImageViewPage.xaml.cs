using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Media.Imaging;
using System.Windows.Input;
using System.Windows.Resources;
using System.Windows.Controls.Primitives;
using System.IO;
using System.IO.IsolatedStorage;
using System.Collections.ObjectModel;
using VKMagazine.DAO;
using VKMagazine.ViewModels;

namespace VKMagazine
{
    public partial class ImageViewPage : BasePage
    {
        BitmapImage _image = new BitmapImage();
        private ObservableCollection<SrcBig> itemss;
        
        public ImageViewPage()
        {
            InitializeComponent();
             this.Loaded += MainPage_Loaded;
             this.OrientationChanged += MainPage_OrientationChanged;
             itemss = new ObservableCollection<SrcBig>();

            //List<ViewModel> items = new List<ViewModel>();

            //for (int i = 1; i <= 8; i++)
            //{
            //    MyViewModel model = new MyViewModel()
            //    {
            //        Title = "Item " + i,
            //        ImagePath = "Icons/acura.jpg",
            //        Description = "Description of " + "Item " + i
            //    };

            //    items.Add(model);
            //}

            //this.DataContext = items;
        }
        void MainPage_OrientationChanged(object sender, OrientationChangedEventArgs e)
        {
            foreach (var t in itemss)
                t.IsNeedToUpdate = true;

        }


        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {

         
        }
        
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            string imageUrl;
            try
            {
                if (NavigationContext.QueryString.TryGetValue("Url", out imageUrl))
                {
                   // ObservableCollection<SrcBig> itemss;// = App.viewModel.News.FirstOrDefault(x => x.Src_big_All.Any(y => y.OnlineUri == imageUrl)).Src_big_All;

                    if (imageUrl.Contains("isostore"))
                    {
                        itemss = App.favouritesViewModel.News.FirstOrDefault(x => x.Src_big_All.Any(y => y.OnlineUri == imageUrl)).Src_big_All;
                    }
                    else
                    {
                        itemss = App.viewModel.News.FirstOrDefault(x => x.Src_big_All.Any(y => y.OnlineUri == imageUrl)).Src_big_All;

                    }
                    HorizontalFlipView.ItemsSource = itemss;
                    HorizontalFlipView.SelectedItem = itemss.FirstOrDefault(x => x.OnlineUri == imageUrl);
                }
            }
            catch
            {
                NavigationService.GoBack();
            }
        }



        private void HorizontalFlipView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //myStoryboard.Begin();
            //_image=new System.Windows.Media.Imaging.BitmapImage(new Uri(((FlipView)sender).SelectedItem.ToString(),UriKind.Relative));
           // imageDocument.Source=_image;
           // var it = HorizontalFlipView.Items.ToList();

        }
    }

    //public class MyViewModel : ViewModel
    //{
    //    public string Title { get; set; }
    //    public string ImagePath { get; set; }
    //    public string Description { get; set; }
    //}
}