using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using VKMagazine.DAO;
using VKMagazine.Helpers;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Diagnostics;
using VKMagazine.ViewModels;
using VKMagazine.Response;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Windows.Media.Imaging;
using System.Windows.Data;
using System.IO.IsolatedStorage;
using System.IO;
using System.Windows.Resources;
using Microsoft.Phone.Tasks;
using Microsoft.Phone.Net.NetworkInformation;

namespace VKMagazine
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        //private NewsViewModel ViewModel;
        private bool isFavouriteTapped = false;
        public const string FavouriteFolder = "/Shared/ShellContent";
        private int _offsetKnob = 5;
        private int currentOffset = 0;
        private string currentUrl = String.Empty;
        private bool isDataLoadedFromSelectedGroups = false;
        private bool isInFavourite = false;
        private bool isFavouriteImageTaped = false;
        private bool isMailShareTapped = false;
        private bool isExpandeButtonClick;
        private bool isNetworkAvailable;
        private string sharePostUrl;

        ProgressIndicator progressIndicator;
        private const string More = "Далее";
        private const string Collapse = "Скрыть";
        public MainPage()
        {
            InitializeComponent();
            NewsLongListSelector.SelectionChanged += NewsLongListSelector_SelectionChanged;
            BitmapImage test = new BitmapImage();
            Loaded += MainPage_Loaded;
        }

        /// <summary>
        /// Handler for maim page loaded event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            SetProgressIndicator();
            DeviceNetworkInformation.NetworkAvailabilityChanged += new EventHandler<NetworkNotificationEventArgs>(ChangeNetworkIdDetected);
            var userStoreForApplication = IsolatedStorageFile.GetUserStoreForApplication();
            if (!userStoreForApplication.DirectoryExists(FavouriteFolder))
                userStoreForApplication.CreateDirectory(FavouriteFolder);

        }

        /// <summary>
        /// Handles network change status
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChangeNetworkIdDetected(object sender, NetworkNotificationEventArgs e)
        {

            Debug.WriteLine(e.NotificationType.ToString() + " " + e.NetworkInterface.InterfaceSubtype);
            switch (e.NotificationType)
            {
                case NetworkNotificationType.InterfaceConnected:
                    if (e.NetworkInterface.InterfaceSubtype != NetworkInterfaceSubType.Unknown)
                        isNetworkAvailable = true;
                    NoInternetMessage.Visibility = Visibility.Collapsed;
                    break;
                case NetworkNotificationType.InterfaceDisconnected:
                    isNetworkAvailable = false;
                    //NoInternetMessage.Visibility = Visibility.Visible;
                    break;
            }
        }

        /// <summary>
        /// Return current network avalable status
        /// </summary>
        /// <returns></returns>
        private async Task<bool> IsNetworkAvailable()
        {
            return NetworkInterface.GetIsNetworkAvailable() && NetworkInterface.NetworkInterfaceType != NetworkInterfaceType.None;
        }

        /// <summary>
        /// Create application bar for news pivot page
        /// </summary>
        private void CreateApplicationBar()
        {
            if (ApplicationBar == null)
            {
                ApplicationBar = new ApplicationBar();

                var appBarButtonGotTop =
                    new ApplicationBarIconButton(new Uri("/Icons/top.png", UriKind.Relative))
                    {
                        Text = "В начало"
                    };
                appBarButtonGotTop.Click += AppBarButtonToTop;
                ApplicationBar.Buttons.Add(appBarButtonGotTop);
                var appBarButtonRefresh =
                    new ApplicationBarIconButton(new Uri("/Icons/refresh.png", UriKind.Relative))
                    {
                        Text = "Обновить"
                    };
                appBarButtonRefresh.Click += AppBarButtonRefresh;
                ApplicationBar.Buttons.Add(appBarButtonRefresh);
                if (MainPivot.SelectedItem == MenuPivotPage)
                    ApplicationBar.IsVisible = false;
            }


        }

        /// <summary>
        /// Handler for application bar button refresh click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void AppBarButtonRefresh(object sender, EventArgs e)
        {
            if (isNetworkAvailable)
            {
                if (DbSingleton.Instance.Groups.Any(x => x.isSelected))
                {
                    App.viewModel.News.Clear();
                    await App.viewModel.LoadPage(false);
                }
                else
                {
                    MessageBox.Show("Выбирите группы для показа");
                }
            }
        }

         /// <summary>
        /// Handler for application bar button go top click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AppBarButtonToTop(object sender, EventArgs e)
        {
            try
            {
                if (NewsLongListSelector.ItemsSource != null)
                    NewsLongListSelector.ScrollTo(NewsLongListSelector.ItemsSource[0]);
            }
            catch
            {
                return;
            }
        }

        /// <summary>
        /// Handler for selected item change event for news long list selector
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void NewsLongListSelector_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (NewsLongListSelector.SelectedItem != null)
            {
                if (isFavouriteImageTaped)
                {
                    //NewsLongListSelector
                }
                if (isMailShareTapped)
                {
                    isMailShareTapped = false;
                    EmailComposeTask mailTask = new EmailComposeTask();
                    mailTask.Body = (NewsLongListSelector.SelectedItem as PostDetailsViewModel).PostUrl;
                    mailTask.Show();
                    NewsLongListSelector.SelectedItem = null;
                    return;
                }
                if (isFavouriteTapped)
                {
                    PostDetailsViewModel selectedModel = NewsLongListSelector.SelectedItem as PostDetailsViewModel;
                    if (selectedModel.FavouriteIcon == Helpers.GroupsHelper.NoFavouritedIcon)
                    {
                        selectedModel.FavouriteIcon = Helpers.GroupsHelper.FavouritedIcon;
                        FavouritedPost favPost = new FavouritedPost()
                        {
                            Created = selectedModel.Date,
                            GroupIcon = "isostore:" + SaveIconToLocalStorage(selectedModel.GroupImage, selectedModel.Group_id.ToString()),
                            GroupName = selectedModel.GroupName,
                            Photos = String.Join("~", selectedModel.Src_big_All.Select(x => "isostore:" + SaveIconToLocalStorage(x.OnlineUri, x.OnlineUri.Substring(x.OnlineUri.LastIndexOf('/') + 1, 11)))),
                            VkPostId = selectedModel.Id,
                            Text = selectedModel.Text,
                            VKGroupId = selectedModel.Group_id

                        };
                        DbSingleton.Instance.FavouritedPosts.InsertOnSubmit(favPost);
                        DbSingleton.Instance.SubmitChanges();
                    }
                    else
                    {
                        selectedModel.FavouriteIcon = Helpers.GroupsHelper.NoFavouritedIcon;
                        if (App.favouritesViewModel != null)
                        {
                            App.favouritesViewModel.News.Remove(selectedModel);
                            if (isInFavourite)
                                NewsLongListSelector.ItemsSource = App.favouritesViewModel.News;
                            if (App.viewModel != null)
                            {
                                PostDetailsViewModel post = App.viewModel.News.FirstOrDefault(x => x.Id == selectedModel.Id);
                                if (post != null)
                                    post.FavouriteIcon = Helpers.GroupsHelper.NoFavouritedIcon;
                            }
                        }

                        FavouritedPost postTodelete = DbSingleton.Instance.FavouritedPosts.FirstOrDefault(x => x.VkPostId == selectedModel.Id);
                        DbSingleton.Instance.FavouritedPosts.DeleteOnSubmit(postTodelete);
                        DbSingleton.Instance.SubmitChanges();
                    }
                    isFavouriteTapped = false;
                }
                NewsLongListSelector.SelectedItem = null;
            }
            //throw new NotImplementedException();
        }

        /// <summary>
        /// ovverided handler for back key pressed event
        /// </summary>
        /// <param name="e"></param>
        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            if (MainPivot.SelectedItem != MenuPivotPage)
            {
                e.Cancel = true;
                MainPivot.SelectedItem = MenuPivotPage;
            }
            else if (MessageBoxResult.Cancel == MessageBox.Show("Выйти из приложения?", "Выход", MessageBoxButton.OKCancel))
            {
                e.Cancel = true;
            }

        }

        /// <summary>
        /// Save image to localstorage
        /// </summary>
        /// <param name="ImageUrl">Image url</param>
        /// <param name="id">Unique id for file name</param>
        /// <returns></returns>
        private string SaveIconToLocalStorage(string ImageUrl, string id)
        {
            string fileName = Path.Combine(FavouriteFolder, id + ".jpg");
            var webClient = new WebClient();
            webClient.OpenReadCompleted += WebClientOpenReadCompleted;
            webClient.OpenReadAsync(new Uri(ImageUrl, UriKind.Absolute), fileName);
            return fileName;
        }

        /// <summary>
        /// Hadnler for web client open read completed event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WebClientOpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            try
            {
                string fileName = e.UserState.ToString();
                var streamResourceInfo = new StreamResourceInfo(e.Result, null);
                var userStoreForApplication = IsolatedStorageFile.GetUserStoreForApplication();
                if (userStoreForApplication.FileExists(fileName))
                {
                    return;
                }
                var isolatedStorageFileStream = userStoreForApplication.CreateFile(fileName);
                var bitmapImage = new BitmapImage { CreateOptions = BitmapCreateOptions.None };
                bitmapImage.SetSource(streamResourceInfo.Stream);
                var writeableBitmap = new WriteableBitmap(bitmapImage);
                writeableBitmap.SaveJpeg(isolatedStorageFileStream, writeableBitmap.PixelWidth, writeableBitmap.PixelHeight, 0, 85);
                isolatedStorageFileStream.Close();
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.Message+ " "+ ex.StackTrace);
            }
                
        }

        /// <summary>
        /// Sets progrss indicator for system tray
        /// </summary>
        void SetProgressIndicator()
        {
            //var progressIndicator = SystemTray.ProgressIndicator;
            //if (progressIndicator != null)
            //{
            //    return;
            //}
            progressIndicator = new ProgressIndicator();
            SystemTray.SetProgressIndicator(this, progressIndicator);
            Binding binding = new Binding("IsLoading") { Source = App.viewModel };
            BindingOperations.SetBinding(progressIndicator, ProgressIndicator.IsVisibleProperty, binding);
            binding = new Binding("IsLoading") { Source = App.viewModel };
            BindingOperations.SetBinding(progressIndicator, ProgressIndicator.IsIndeterminateProperty, binding);
            progressIndicator.Text = "Загрузка новостей";
        }

        /// <summary>
        ///  ovverided handler fo for page on navegate to event
        /// </summary>
        /// <param name="e"></param>
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            DataLoaderHelper.LoadDataToDabase();
            CreateApplicationBar();
            SetNormalbackground();


            if (VkHelper.IsUserAuthenticated())
                VkLogOut.Visibility = System.Windows.Visibility.Visible;
            else
                VkLogOut.Visibility = System.Windows.Visibility.Collapsed;
            await SetUserIcon();
            isNetworkAvailable = await IsNetworkAvailable();
            if (!isNetworkAvailable   )
            {
                if(isInFavourite)
                    return;
                
                NoInternetMessage.Visibility = Visibility.Visible;
                MainPivot.SelectedIndex = 1;
                return;
                //NewsLongListSelector.Visibility = Visibility.Collapsed;
            }
            else
                NoInternetMessage.Visibility = Visibility.Collapsed;
            string pivotPage = string.Empty;
            List<Group> selectedGroups = DbSingleton.Instance.Groups.Where(x => x.isSelected == true).ToList();
            if (selectedGroups.Count > 0 && App.viewModel == null)
            {
                MainPivot.SelectedItem = NewsPivotPage;
                await LoadDataToNewsPivotPage();
            }
            if (NavigationContext.QueryString.TryGetValue("pivotPage", out pivotPage))
            {
                try
                {
                    int pivotPageint = Int32.Parse(pivotPage);
                    if ((Helpers.PivotPages)pivotPageint == PivotPages.NewsPivot && isDataLoadedFromSelectedGroups == false)
                    {
                        MainPivot.SelectedItem = NewsPivotPage;
                        await LoadDataToNewsPivotPage();
                        isDataLoadedFromSelectedGroups = true;
                    }
                }
                catch (Exception err)
                {
                    Debug.WriteLine("Loading data from groups in OnNavigatedTo failed \n" + err.Message);
                }
            }
        }

        /// <summary>
        /// Sets user auth icon
        /// </summary>
        /// <returns></returns>
        private async Task SetUserIcon()
        {
            var settings = IsolatedStorageSettings.ApplicationSettings;

            if (VkTextBlock.Text.Contains("Войти вконтакте"))
            {
                if (settings.Contains(VkHelper.AccessToken))
                {
                    if (settings.Contains(VkHelper.VkUserPhotoUrl))
                    {
                        VkTextBlock.Text = IsolatedStorageSettings.ApplicationSettings[VkHelper.VkUserName].ToString();
                        LoginVkImage.Source = new BitmapImage(new Uri(IsolatedStorageSettings.ApplicationSettings[VkHelper.VkUserPhotoUrl].ToString()));
                    }
                    else
                    {
                        try
                        {
                            HttpClient client = new HttpClient();
                            string response = await client.GetStringAsync(String.Format("https://api.vk.com/method/users.get?user_ids={0}&fields=photo_100", settings[VkHelper.VkUserId]));
                            GetuserInfoResponse user = JsonConvert.DeserializeObject<GetuserInfoResponse>(response);
                            LoginVkImage.Source = new BitmapImage(new Uri(user.response[0].photo_100));
                            VkHelper.UserPhoto = user.response[0].photo_100;
                            VkTextBlock.Text = user.response[0].first_name + " " + user.response[0].last_name;
                            VkHelper.UserName = VkTextBlock.Text;
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Ошабка получения данных пользователя из вк");
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Loads data to news and show progress indicator without offset
        /// </summary>
        /// <returns></returns>
        private async Task LoadDataToNewsPivotPage()
        {
            App.viewModel = new NewsViewModel();
            App.viewModel.IsLoading = true;
            await App.viewModel.LoadPage(false);
            NewsLongListSelector.ItemsSource = App.viewModel.News;
            App.viewModel.IsLoading = false;
        }

        /// <summary>
        /// Redirect to Groups list page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GroupList_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {

            NavigationService.Navigate(new Uri("/GroupListPage.xaml", UriKind.Relative));
            //NavigationService.Navigate(new Uri("/ImageViewPage.xaml", UriKind.Relative));
        }

        private void Pivot_Loaded_1(object sender, RoutedEventArgs e)
        {

        }

        /// <summary>
        /// Handler for tap on image from news event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onImageNewsTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            ImageWithLoading image = sender as ImageWithLoading;
            BitmapImage bitmap = image.Source as BitmapImage;
            if (bitmap.UriSource.ToString().Contains("Icons"))
                return;
            NavigationService.Navigate(new Uri("/ImageViewPage.xaml?Url=" + image.Url, UriKind.Relative));
        }

        /// <summary>
        /// Handler for NewsLongListSelector ItemRealized event, need for dynamic data load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void NewsLongListSelector_ItemRealized(object sender, ItemRealizationEventArgs e)
        {
            if (isInFavourite == false && !App.viewModel.IsLoading && NewsLongListSelector.ItemsSource != null && NewsLongListSelector.ItemsSource.Count >= _offsetKnob)
            {
                if (e.ItemKind == LongListSelectorItemKind.Item)
                {
                    if ((e.Container.Content as PostDetailsViewModel).Equals(NewsLongListSelector.ItemsSource[NewsLongListSelector.ItemsSource.Count - _offsetKnob])) //||
                    {
                        Debug.WriteLine("Searching for {0}", ++currentOffset);
                        if (!isNetworkAvailable)
                        {
                            return;
                        }
                        await App.viewModel.LoadPage(true);
                    }
                    else if ((e.Container.Content as PostDetailsViewModel).Equals(NewsLongListSelector.ItemsSource[NewsLongListSelector.ItemsSource.Count - 1]))
                    {
                        Debug.WriteLine("Searching for additional {0}", ++currentOffset);
                        if (!isNetworkAvailable)
                        {
                            return;
                        }
                        await App.viewModel.LoadPage(true);
                    }
                }
            }
        }


        /// <summary>
        /// Handler for tap event on add/remove to favourite icon
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onFavouriteIconTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            isFavouriteTapped = true;
        }

        /// <summary>
        /// Handler for tap on favourite menu item 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Favourites_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        { 
            NewsPivotPage.Header = "Избранное";
            isInFavourite = true;
            NoInternetMessage.Visibility = Visibility.Collapsed;
            App.favouritesViewModel = new NewsViewModel();
            List<FavouritedPost> favDatabase = DbSingleton.Instance.FavouritedPosts.ToList();

            foreach (var post in favDatabase)
            {
                App.favouritesViewModel.News.Add(new PostDetailsViewModel()
                {
                    FavouriteIcon = Helpers.GroupsHelper.FavouritedIcon,
                    Text = post.Text,
                    Date = post.Created,
                    GroupImage = post.GroupIcon,
                    GroupName = post.GroupName,
                    //Src_big = new ObservableCollection<string>(post.Photos.Split('~')),
                    Src_big = new ObservableCollection<SrcBig>(post.Photos.Split('~').Select(x => new SrcBig() { OnlineUri = x })),
                    Id = post.VkPostId,
                    Group_id = post.VKGroupId

                });
            }
            NewsLongListSelector.ItemsSource = App.favouritesViewModel.News.OrderByDescending(x => x.Date).ToList();
            MainPivot.SelectedItem = NewsPivotPage;
        }

        /// <summary>
        /// Pivot selection change event 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainPivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {


            if (MainPivot.SelectedItem == NewsPivotPage && ((string)NewsPivotPage.Header) == "Новости")
            {
                NoInternetMessage.Visibility = isNetworkAvailable ? Visibility.Collapsed : Visibility.Visible;
                isInFavourite = false;
                if (App.viewModel != null)
                {
                    NewsLongListSelector.ItemsSource = App.viewModel.News;
                    if (ApplicationBar != null && isInFavourite == false)
                        ApplicationBar.IsVisible = true;
                    
                }
                else if(isNetworkAvailable==false)
                {
                    
                    NoInternetMessage.Visibility = Visibility.Visible;
                    NewsLongListSelector.ItemsSource = null;
                }
            }
            if (MainPivot.SelectedItem == MenuPivotPage)
            {
                if (ApplicationBar != null)
                    ApplicationBar.IsVisible = false;
                NewsPivotPage.Header = "Новости";
            }
            if (MainPivot.SelectedItem == NewsPivotPage && isInFavourite == false)
                ApplicationBar.IsVisible = true;
            if (MainPivot.SelectedItem == NewsPivotPage && isInFavourite && ApplicationBar != null)
                ApplicationBar.IsVisible = false;
        }

        /// <summary>
        /// Handler for share icon tap
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShareIcon_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            ImageWithLoading img = sender as ImageWithLoading;
            currentUrl = img.Url;
            sharePostUrl = img.PostUrlShare;
            my_popup_xaml.IsOpen = true;
            double op = NewsPivotPage.Opacity;
            Content.Opacity = 0.4;
            Content.IsHitTestVisible = false;
            //offlineIcon.Visibility = System.Windows.Visibility.Visible;
        }


        /// <summary>
        /// Handler for sharing via vk icon tap
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void VkShareIcon_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            //VkShareIcon.Source = new BitmapImage(new Uri("/Icons/vk_pressed.png", UriKind.Relative));
            if (!isNetworkAvailable)
            {
                MessageBox.Show("Отсутствует интернет подключение");
                return;
            }
            if (!VkHelper.IsUserAuthenticated())
            {
                NavigationService.Navigate(new Uri("/VkAuthPage.xaml", UriKind.Relative));
            }
            else
            {
                try
                {
                    HttpClient client = new HttpClient();
                    string request = String.Format("https://api.vk.com/method/wall.repost?object={0}&access_token={1}", sharePostUrl, VkHelper.CurrentAccess_token);
                    string response = await client.GetStringAsync(request);
                    PostShareResponse jsonResponse = await JsonConvert.DeserializeObjectAsync<PostShareResponse>(response);
                    if (jsonResponse.response.success != 1)
                    {
                        MessageBox.Show("Репост не удался, попробуйте перезайти в акаунт");
                    }
                    else
                    {
                        SharedSucces.IsOpen = true;

                    }
                    my_popup_xaml.IsOpen = false;
                }
                catch
                {
                    MessageBox.Show("Повторите попытку");
                }

            }

        }

        /// <summary>
        /// Handler for sharing via email icon tap
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MailShareIcon_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            MailShareIcon.Source = new BitmapImage(new Uri("/Icons/mail_pressed.png", UriKind.Relative));
            //Thread.Sleep(500);
            EmailComposeTask task = new EmailComposeTask();
            task.Body = currentUrl;
            task.Show();
            SetNormalbackground();
            MailShareIcon.Source = new BitmapImage(new Uri("/Icons/mail.png", UriKind.Relative));
        }

        /// <summary>
        /// Set's normal application background and hide share popup window
        /// </summary>
        private void SetNormalbackground()
        {
            my_popup_xaml.IsOpen = false;
            Content.Opacity = 1;
            Content.IsHitTestVisible = true;
        }

        /// <summary>
        /// Handler for share pop up back button click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PopupBackButton_Click(object sender, RoutedEventArgs e)
        {
            SetNormalbackground();
        }

        /// <summary>
        /// Handler for sharing via sms icon tap
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MessageShareIcon_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            SmsComposeTask task = new SmsComposeTask();
            task.Body = currentUrl;
            task.Show();
            SetNormalbackground();
        }

        private void onExpandButtunClick(object sender, RoutedEventArgs e)
        {
            isExpandeButtonClick = true;
        }

        /// <summary>
        /// Expands text in news
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextGrid_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (isExpandeButtonClick)
            {
                Grid grid = sender as Grid;
                Button moreButton = grid.Children[1] as Button;
                if (moreButton.Content.ToString().Equals(More))
                {
                    UrlizerTextBox textBlock = grid.Children[0] as UrlizerTextBox;
                    textBlock.MaxHeight = 9999;
                    moreButton.Content = Collapse;
                }
                else
                {
                    UrlizerTextBox textBlock = grid.Children[0] as UrlizerTextBox;
                    textBlock.MaxHeight = 85;
                    moreButton.Content = More;

                }
                isExpandeButtonClick = false;
            }
        }

        /// <summary>
        /// Handler for vk login button tap in menu 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LoginVk_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (!VkHelper.IsUserAuthenticated())
            {
                if (!isNetworkAvailable)
                {
                    MessageBox.Show("Отсутствует интернет подключение");
                    return;
                }
                NavigationService.Navigate(new Uri("/VkAuthPage.xaml", UriKind.Relative));
            }
        }
        
        /// <summary>
        /// Handler for vk login out button tap in menu 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void VkLogOut_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (MessageBoxResult.OK == MessageBox.Show("Вы действительно хотите выйти?", "Выход", MessageBoxButton.OKCancel))
            {
                VkHelper.LogOut();
                VkLogOut.Visibility = System.Windows.Visibility.Collapsed;
                LoginVkImage.Source = new BitmapImage(new Uri("/Icons/vk-wp.png", UriKind.Relative));
                VkTextBlock.Text = "Войти вконтакте";
                foreach (var grp in DbSingleton.Instance.Groups.Where(x => x.isUserGroups == true || x.isFinded == true))
                    grp.isSelected = false;
                DbSingleton.Instance.SubmitChanges();
                List<Group> selectedgrps = DbSingleton.Instance.Groups.Where(x => x.isSelected == true).ToList();
                NewsLongListSelector.ItemsSource = null;
                App.viewModel.News.Clear();
            }
        }

        private void OnSharedSuccessClick(object sender, RoutedEventArgs e)
        {
            SharedSucces.IsOpen = false;
            SetNormalbackground();
        }

        /// <summary>
        /// Handler for shared with friends button tap in menu 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnShareWithFriendsTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (VkHelper.IsUserAuthenticated())
            {
                if (!isNetworkAvailable)
                {
                    MessageBox.Show("Отсутствует интернет подключение");
                    return;
                }
                NavigationService.Navigate(new Uri("/FriendsSharePage.xaml", UriKind.Relative));
            }
            else
            {
                MessageBox.Show("Залогиньтесь, что бы поделиться с друзьями");
            }
        }

        private void SettingsMenuTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/SettignsPage.xaml", UriKind.Relative));
        }
    }
}