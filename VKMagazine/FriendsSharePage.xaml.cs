using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using VKMagazine.Response;
using VKMagazine.Helpers;
using VKMagazine.DAO;
using VKMagazine.ViewModels;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using Microsoft.Phone.Net.NetworkInformation;
using System.Diagnostics;

namespace VKMagazine
{
    public partial class FriendsSharePage : PhoneApplicationPage
    {
        private ObservableCollection<GroupListItemViewModel> friendsViewModel;
        private ProgressIndicator progressIndicator;
        private bool isCheck = false;
        private bool isUncheck = false;
        private const int FRIENDS_LOAD_COUNT = 40;
        private const int _offsetKnob=10;
        private bool isNetworkAvailable;
        private int _pageNumber=0;
        public FriendsSharePage()
        {
            InitializeComponent();
            friendsViewModel = new ObservableCollection<GroupListItemViewModel>();
            Loaded += FriendsSharePage_Loaded;
        }

        void FriendsSharePage_Loaded(object sender, RoutedEventArgs e)
        {
            DeviceNetworkInformation.NetworkAvailabilityChanged += new EventHandler<NetworkNotificationEventArgs>(ChangeNetworkIdDetected);
        }

        private async Task<bool> IsNetworkAvailable()
        {
            return NetworkInterface.GetIsNetworkAvailable() && NetworkInterface.NetworkInterfaceType != NetworkInterfaceType.None;
        }
        private void ChangeNetworkIdDetected(object sender, NetworkNotificationEventArgs e)
        {

            Debug.WriteLine(e.NotificationType.ToString() + " " + e.NetworkInterface.InterfaceSubtype);
            switch (e.NotificationType)
            {
                case NetworkNotificationType.InterfaceConnected:
                    if (e.NetworkInterface.InterfaceSubtype != NetworkInterfaceSubType.Unknown)
                        isNetworkAvailable = true;
                    //NoInternetMessage.Visibility = Visibility.Collapsed;
                    break;
                case NetworkNotificationType.InterfaceDisconnected:
                    isNetworkAvailable = false;
                    // NoInternetMessage.Visibility = Visibility.Visible;
                    break;
            }
        }
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            SetProgressIndicator();
            isNetworkAvailable = await IsNetworkAvailable();
            if (VkHelper.IsUserAuthenticated() && isNetworkAvailable)
            {
                try
                {
                    await LoadFriendsViewModel();
                }
                catch
                {
                    MessageBox.Show("Ошибка загрузки данных друзей");
                    NavigationService.GoBack();
                }
               
                CreateApplicationBar();


            }
            else
            {
                MessageBox.Show("Авторизируйтесь, что бы приглашать друзей");
                NavigationService.GoBack();
            }
        }

        private void CreateApplicationBar()
        {

            ApplicationBar = new ApplicationBar();

            var appBarButtonSelectAll =
                new ApplicationBarIconButton(new Uri("/Icons/check.png", UriKind.Relative))
                    {
                        Text = "Все"
                    };

            appBarButtonSelectAll.Click += AppBarButtonSelectAll_Click;
            ApplicationBar.Buttons.Add(appBarButtonSelectAll);
            var appBarButtonShare =
                new ApplicationBarIconButton(new Uri("/Icons/appbar.social.sharethis.png", UriKind.Relative))
                    {
                        Text = "Отправить"
                    };

            appBarButtonShare.Click += AppBarButtonShare_Click;
            ApplicationBar.Buttons.Add(appBarButtonShare);

        }

        private void AppBarButtonShare_Click(object sender, EventArgs e)
        {

        }

        private void AppBarButtonSelectAll_Click(object sender, EventArgs e)
        {
            if (friendsViewModel.Where(x => x.IsSelected == true).Count() == friendsViewModel.Count)
            {
                foreach (var friend in friendsViewModel)
                    friend.IsSelected = false;
            }
            else
            {
                foreach (var friend in friendsViewModel)
                    friend.IsSelected = true;
            }
        }

        private async Task LoadFriendsViewModel(int offset=0)
        {
            HttpClient client = new HttpClient();
            progressIndicator.IsIndeterminate = true;
            progressIndicator.IsVisible = true;
            try
            {
                string request = String.Format("https://api.vk.com/method/friends.get?user_id={0}", VkHelper.CurrentUserId);
                string response = await client.GetStringAsync(request);
                GetUserGroupsResponse friendsIds = await JsonConvert.DeserializeObjectAsync<GetUserGroupsResponse>(response);
                string friendsIdsForRequest = String.Join(",", friendsIds.response.ToArray().Skip(offset*FRIENDS_LOAD_COUNT).Take(FRIENDS_LOAD_COUNT));
                request = String.Format("https://api.vk.com/method/users.get?fields=photo_100&user_ids={0}", friendsIdsForRequest);
                response = await client.GetStringAsync(request);
                GetuserInfoResponse friendsReposne = await JsonConvert.DeserializeObjectAsync<GetuserInfoResponse>(response);
                //friendsReposne.response = friendsReposne.response.OrderBy(x => x.last_name).ToList();
                foreach (var frnd in friendsReposne.response)
                {
                    friendsViewModel.Add(new GroupListItemViewModel()
                    {
                        Id = (int)frnd.uid,
                        ImageSrc = new Uri(frnd.photo_100),
                        ListGroupName = frnd.last_name + " " + frnd.first_name,
                        Visible = "Visible"
                    });
                }
                FriendsLongListSelector.ItemsSource = friendsViewModel;
                progressIndicator.IsIndeterminate = false;
                progressIndicator.IsVisible = false;
            }
            catch
            {
                MessageBox.Show("Ошабка при получении списка друзей");
                progressIndicator.IsIndeterminate = false;
                progressIndicator.IsVisible = false;
                return;
            }

        }

        private async void FriendsLongListSelector_ItemRealized(object sender, ItemRealizationEventArgs e)
        {
            if (  FriendsLongListSelector.ItemsSource != null && FriendsLongListSelector.ItemsSource.Count >= _offsetKnob)
            {
                if (e.ItemKind == LongListSelectorItemKind.Item)
                {
                    if ((e.Container.Content as GroupListItemViewModel).Equals(FriendsLongListSelector.ItemsSource[FriendsLongListSelector.ItemsSource.Count - _offsetKnob]))
                    {
                        if (isNetworkAvailable)
                        {
                            Debug.WriteLine("Searching for {0}", _pageNumber);
                            progressIndicator.IsIndeterminate = true;
                            progressIndicator.IsVisible = true;
                            await LoadFriendsViewModel(++_pageNumber);
                            progressIndicator.IsIndeterminate = false;
                            progressIndicator.IsVisible = false;
                            //searchModel.LoadPage(_pageNumber++);
                        }
                    }
                }
            }
        }

        private void CheckBox_Unchecked_1(object sender, RoutedEventArgs e)
        {
            isUncheck = true;
        }

        private void CheckBox_Checked_1(object sender, RoutedEventArgs e)
        {
            isCheck = true;
        }

        private void LongListSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FriendsLongListSelector.SelectedItem != null)
            {
                GroupListItemViewModel selectedModel = (GroupListItemViewModel)FriendsLongListSelector.SelectedItem;
                if (isCheck == false && isUncheck == false)
                    selectedModel.IsSelected = !selectedModel.IsSelected;
                isCheck = isUncheck = false;
                //Group group = DbSingleton.Instance.Groups.SingleOrDefault(x => x.VkId == selectedModel.Id);
                //if (group != null)
                //    group.isSelected = selectedModel.IsSelected;
                //DbSingleton.Instance.SubmitChanges();
                FriendsLongListSelector.SelectedItem = null;
            }
        }

        void SetProgressIndicator()
        {
            progressIndicator = new ProgressIndicator();
            SystemTray.SetProgressIndicator(this, progressIndicator);
            progressIndicator.Text = "Загрузка";
        }
    }
}