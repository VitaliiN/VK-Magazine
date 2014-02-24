using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using VKMagazine.ViewModels;
using System.Collections.ObjectModel;
using VKMagazine.Helpers;
using VKMagazine.DAO;
using System.Windows.Input;
using Microsoft.Devices;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using VKMagazine.Response;
using System.Diagnostics;
using Microsoft.Phone.Net.NetworkInformation;
using Microsoft.Phone.Reactive;


namespace VKMagazine
{
    public partial class GroupListPage : PhoneApplicationPage
    {
        private const int _offsetKnob = 7;
        private const int GROUPS_LOAD_COUNT = 20;
        private const string ADD_GROUP = "Добавить";
        private const string DELETE_GROUP = "Удалить";
        private const string JOIN_GROUP = "Вступить";
        private bool isNetworkAvailable;

        private Category collectionGroups;
        private List<Group> selectedGroups;
        private bool isAddButtonClicked;
        private bool isInSearching = false;
        private ProgressIndicator progressIndicator;
        private bool isCheckBoxPushed = false;
        private int _pageNumber = 0;
        private SearchGroupViewModel searchModel = new SearchGroupViewModel();

        //public ObservableCollection<GroupListItemViewModel> groupList { get; set; }
        public GroupListPage()
        {

            InitializeComponent();
            collectionGroups = DbSingleton.Instance.Categories.FirstOrDefault(x => x.isCollection);
            this.Loaded += new RoutedEventHandler(GroupListPage_Loaded);
            ApplicationBar = ApplicationBarBuilder.BuildAppBar(ApplicationBar);
            var appBarButtonSearch =
                 new ApplicationBarIconButton(new Uri("/Icons/feature.search.png", UriKind.Relative))
                 {
                     Text = "Найти группу"
                 };
            appBarButtonSearch.Click += AppBarButtonSearch_Click;
            ApplicationBar.Buttons.Insert(0, appBarButtonSearch);
            SetProgressIndicator();
            // groupList = new ObservableCollection<GroupListItemViewModel>();
        }

        private void AppBarButtonSearch_Click(object sender, EventArgs e)
        {
            if (!isNetworkAvailable)
            {
                MessageBox.Show("нет подключения к интернету");
                return;
            }
            if (VkHelper.IsUserAuthenticated())
            {
                isInSearching = true;
                TitleTextBlock.Text = "Поиск групп";
                ApplicationBar.IsVisible = false;
                SearchTextBox.Visibility = Visibility.Visible;
                SearchTextBox.Focus();
            }
            else
            {
                MessageBox.Show("Для поиска групп авторизируйтесь через вконтакте");
            }
        }

        private void GroupListPage_Loaded(object sender, RoutedEventArgs e)
        {

            DeviceNetworkInformation.NetworkAvailabilityChanged += new EventHandler<NetworkNotificationEventArgs>(ChangeNetworkIdDetected);
        }

        private async Task<bool> IsNetworkAvailable()
        {
            return NetworkInterface.GetIsNetworkAvailable() && NetworkInterface.NetworkInterfaceType != NetworkInterfaceType.None;
        }
        protected override async void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            if (isInSearching)
            {
                //DbSingleton.Instance.Groups.InsertAllOnSubmit(collectionGroups);
                SearchTextBox.Visibility = Visibility.Collapsed;
                DbSingleton.Instance.SubmitChanges();
                this.Focus();
                isInSearching = false;
                ApplicationBar.IsVisible = true;
                SearchLongListSelector.Visibility = Visibility.Collapsed;
                LongListSelector.Visibility = Visibility.Visible;
                e.Cancel = true;
                LongListSelector.ItemsSource = await Helpers.CategoryHelper.GetCategories();
            }
            else
            {
                if (selectedGroups.Count != DbSingleton.Instance.Groups.Count(x => x.isSelected == true))
                {
                    NewsRefreshHelper.isNeedToRefresh = true;
                    return;
                }
                foreach (var group in DbSingleton.Instance.Groups.Where(x => x.isSelected == true))
                {
                    if (!selectedGroups.Any(x => x.GroupId == group.GroupId))
                    {
                        NewsRefreshHelper.isNeedToRefresh = true;
                        break;
                    }
                }
                NavigationService.GoBack();
                //NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
            }
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

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            selectedGroups = DbSingleton.Instance.Groups.Where(x => x.isSelected == true).ToList();
            isNetworkAvailable = await IsNetworkAvailable();
            LongListSelector.ItemsSource = await Helpers.CategoryHelper.GetCategories();

        }

        private async void SubmitChangesAsync()
        {
            DbSingleton.Instance.SubmitChanges();
        }

        private async void LongListSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (isCheckBoxPushed == true && LongListSelector.SelectedItem != null)
            {
                GroupListItemViewModel selectedModel = (GroupListItemViewModel)LongListSelector.SelectedItem;
                if (selectedModel.Enabled == false)
                {
                    LongListSelector.SelectedItem = null;
                    return;
                }
                Category selectedCategory = DbSingleton.Instance.Categories
                    .SingleOrDefault(x => x.CategoryId == selectedModel.Id);
                selectedCategory.IsSelected = selectedModel.IsSelected;
                if (selectedModel.IsSelected == true)
                {
                    foreach (var group in selectedCategory.Groups)
                        group.isSelected = true;
                    selectedModel.CurrentCount = selectedModel.MaxCount;
                    //selectedModel.CurrentSelectedCount = selectedModel.CurrentCount + "/" + selectedModel.MaxCount;
                }
                else
                {
                    foreach (var group in selectedCategory.Groups)
                        group.isSelected = false;
                    selectedModel.CurrentCount = 0;
                    //selectedModel.CurrentSelectedCount = 0 + "/" + selectedModel.MaxCount;
                }
                Stopwatch st = new Stopwatch();
                st.Start();
                //SubmitChangesAsync();
                Scheduler.Dispatcher.Schedule(SubmitChangesAsync, TimeSpan.FromMilliseconds(100));
                //DbSingleton.Instance.SubmitChanges();
                Debug.WriteLine("SubmitChanges = " + st.ElapsedMilliseconds);
                isCheckBoxPushed = false;
                LongListSelector.SelectedItem = null;
                return;
            }
            if (LongListSelector.SelectedItem != null && (LongListSelector.SelectedItem as GroupListItemViewModel).Enabled)
            {
                try
                {
                    GroupListItemViewModel selectedModel = (GroupListItemViewModel)LongListSelector.SelectedItem;
                    if (!isNetworkAvailable)
                    {
                        if (DbSingleton.Instance.Categories.FirstOrDefault(x => x.CategoryId == selectedModel.Id).Groups.Any(x => x.Name == null))
                        {
                            ShowMessageBoxNoInternet();
                            return;
                        }
                    }

                    NavigationService.Navigate(new Uri("/GroupsSelectPage.xaml?selectedItem=" + selectedModel.Id, UriKind.Relative));
                    LongListSelector.SelectedItem = null;

                }
                catch
                {
                    return;
                }
            }

            //LongListSelector.SelectedItem = null;
        }

        private static void ShowMessageBoxNoInternet()
        {
            MessageBox.Show("нет подключения к интернету");
            return;
        }

        private void CheckBox_Checked_1(object sender, RoutedEventArgs e)
        {
            isCheckBoxPushed = true;
        }

        private void CheckBox_Unchecked_1(object sender, RoutedEventArgs e)
        {
            isCheckBoxPushed = true;
        }

        private void Image_Tap_1(object sender, System.Windows.Input.GestureEventArgs e)
        {
            isCheckBoxPushed = false;
        }

        private void OnListItemTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            isCheckBoxPushed = false;
        }

        private void SearchTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TitleTextBlock.Text = "Список групп";
           // SearchTextBox.Visibility = Visibility.Collapsed;
            if (SearchLongListSelector.Visibility == Visibility.Collapsed)
            {
                isInSearching = false;
                ApplicationBar.IsVisible = true;
            }
        }

        private async void SearchTextBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {

                string _searchTerm = SearchTextBox.Text.Trim();
                searchModel = new SearchGroupViewModel();
                _pageNumber = 0;
                if (String.IsNullOrEmpty(_searchTerm))
                {
                    VibrateController.Default.Start(TimeSpan.FromMilliseconds(200));
                    return;
                }
                LongListSelector.Visibility = Visibility.Collapsed;
                SearchLongListSelector.Visibility = Visibility.Visible;
                progressIndicator.IsIndeterminate = true;
                progressIndicator.IsVisible = true;
                await SearchGroups(_searchTerm);
                progressIndicator.IsIndeterminate = false;
                progressIndicator.IsVisible = false;
                this.Focus();
            }
        }

        private async Task SearchGroups(string _searchTerm, int ofsset = 0)
        {
            HttpClient client = new HttpClient();
            string request = String.Format("https://api.vk.com/method/groups.search?access_token={0}&q={1}&count={2}&offset={3}", VkHelper.CurrentAccess_token, _searchTerm, GROUPS_LOAD_COUNT, GROUPS_LOAD_COUNT * ofsset);
            string response = await client.GetStringAsync(request);
            List<GroupResponse> FindedGroups = new List<GroupResponse>();
            JObject objResp = await JsonConvert.DeserializeObjectAsync<JObject>(response);
            JArray jsonPosts;
            try
            {
                jsonPosts = (objResp["response"] as JArray);
                if (jsonPosts == null)
                    return;
                jsonPosts.RemoveAt(0);
            }
            catch (NullReferenceException)
            {
                return;
            }
            foreach (JObject grpInfo in jsonPosts)
            {
                FindedGroups.Add(grpInfo.ToObject<GroupResponse>());
            }
            foreach (var grp in FindedGroups)
            {
                // string btnTxt = DbSingleton.Instance.Groups.FirstOrDefault(x=> x.VkId==grp.
                bool isGroupInCollection = DbSingleton.Instance.Groups.Any(x => x.vkUserId == VkHelper.CurrentUserId && x.VkId == grp.gid && x.isFinded == true);
                if (grp.is_member)
                    continue;
                searchModel.SearchedGroups.Add(new GroupListItemViewModel()
                {
                    Enabled = false,
                    ImageSrc = new Uri(grp.photo),
                    IsSelected = false,//DbSingleton.Instance.Groups.FirstOrDefault(x => x.VkId == grp.gid).isSelected,
                    Id = grp.gid,
                    ListGroupName = HttpUtility.HtmlDecode(grp.name),
                    Visible = "Visible",
                    IsClosed = grp.is_closed,
                    ButtomColor = isGroupInCollection ? "Red" : grp.is_closed ? "Blue" : "Green",
                    ButtonText = isGroupInCollection ? DELETE_GROUP : grp.is_closed ? JOIN_GROUP : ADD_GROUP
                });
            }
            SearchLongListSelector.ItemsSource = searchModel.SearchedGroups;
        }

        private async void SearchLongListSelector_ItemRealized(object sender, ItemRealizationEventArgs e)
        {
            if (!searchModel.IsLoading && SearchLongListSelector.ItemsSource != null && SearchLongListSelector.ItemsSource.Count >= _offsetKnob)
            {
                if (e.ItemKind == LongListSelectorItemKind.Item)
                {
                    if ((e.Container.Content as GroupListItemViewModel).Equals(SearchLongListSelector.ItemsSource[SearchLongListSelector.ItemsSource.Count - _offsetKnob]))
                    {
                        if (isNetworkAvailable)
                        {
                            Debug.WriteLine("Searching for {0}", _pageNumber);
                            progressIndicator.IsIndeterminate = true;
                            progressIndicator.IsVisible = true;
                            await SearchGroups(SearchTextBox.Text.Trim(), ++_pageNumber);
                            progressIndicator.IsIndeterminate = false;
                            progressIndicator.IsVisible = false;
                            //searchModel.LoadPage(_pageNumber++);
                        }
                    }
                }
            }
        }

        private void SearchTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            SearchTextBox.SelectAll();
        }

        private void SetProgressIndicator()
        {
            progressIndicator = new ProgressIndicator();
            SystemTray.SetProgressIndicator(this, progressIndicator);
            progressIndicator.Text = "Поиск групп";
        }

        private async void SearchLongListSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SearchLongListSelector.SelectedItem != null && isAddButtonClicked == true)
            {
                GroupListItemViewModel selectedModel = SearchLongListSelector.SelectedItem as GroupListItemViewModel;
                Category userGroupsCollection = DbSingleton.Instance.Categories.FirstOrDefault(x => x.isCollection == true);
                if (userGroupsCollection == null)
                    return;
                switch (selectedModel.ButtonText)
                {
                    case ADD_GROUP:
                        //userGroupsCollection.Groups.Add(new Group()
                        collectionGroups.Groups.Add(new Group()
                        {
                            Image = selectedModel.ImageSrc.ToString(),
                            isFinded = true,
                            Name = selectedModel.ListGroupName,
                            VkId = selectedModel.Id,
                            vkUserId = VkHelper.CurrentUserId,
                        });
                        selectedModel.ButtonText = DELETE_GROUP;
                        selectedModel.ButtomColor = "Red";
                        //DbSingleton.Instance.SubmitChanges();
                        break;
                    case DELETE_GROUP:
                        Group groupToDelete = collectionGroups.Groups.FirstOrDefault(x => x.isFinded && x.VkId == selectedModel.Id); //DbSingleton.Instance.Groups.FirstOrDefault(x => x.isFinded == true && x.VkId == selectedModel.Id);
                        if (groupToDelete == null)
                        {
                            MessageBox.Show("Ошибка при удалении группы");
                            SearchLongListSelector.SelectedItem = null;
                            return;
                        }
                        collectionGroups.Groups.Remove(groupToDelete);
                        //DbSingleton.Instance.Groups.DeleteOnSubmit(groupToDelete);
                        //DbSingleton.Instance.SubmitChanges();
                        selectedModel.ButtonText = ADD_GROUP;
                        selectedModel.ButtomColor = "Green";
                        break;
                    case JOIN_GROUP:
                        int result = await JoinToGroup(selectedModel.Id);
                        if (result != 1)
                        {
                            if (result == -1)
                                return;
                            MessageBox.Show("Ошибка при вступлении в группу");
                            return;
                        }
                        searchModel.SearchedGroups.Remove(selectedModel);

                        break;
                    default:
                        break;
                }

            }
            SearchLongListSelector.SelectedItem = null;
        }

        private async Task<int> JoinToGroup(int vkGroupId)
        {
            if (isNetworkAvailable)
            {
                HttpClient client = new HttpClient();
                string request = String.Format("https://api.vk.com/method/groups.join?group_id={0}&access_token={1}", vkGroupId.ToString(), VkHelper.CurrentAccess_token);
                string response = await client.GetStringAsync(request);
                JoinGroupResponse jGR = await JsonConvert.DeserializeObjectAsync<JoinGroupResponse>(response);
                if (jGR != null)
                    return jGR.response;
                return -1;
            }
            else
            {
                ShowMessageBoxNoInternet();
                return -1;
            }

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            isAddButtonClicked = true;
        }
    }
}