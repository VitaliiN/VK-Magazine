using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using VKMagazine.Response;
using VKMagazine.DAO;
using VKMagazine.Helpers;
using RestSharp;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using VKMagazine.RequestWrapper;
using VKMagazine.ViewModels;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Phone.Net.NetworkInformation;

namespace VKMagazine
{
    public partial class GroupsSelectPage : PhoneApplicationPage
    {

        bool isCheck = false;
        bool isUncheck = false;
        private List<Group> selectedGroups;
        private int catId;
        private bool isNetworkAvailable;

        public GroupsSelectPage()
        {
            InitializeComponent();
            ApplicationBar = ApplicationBarBuilder.BuildAppBar(ApplicationBar);
            ApplicationBar.IsVisible = false;
            Loaded += GroupsSelectPage_Loaded;
            App.GroupSelectPageViewModel = new ObservableCollection<GroupListItemViewModel>();
        }

        void GroupsSelectPage_Loaded(object sender, RoutedEventArgs e)
        {
            ApplicationBar.IsVisible = true;
        }

        private async Task<bool> IsNetworkAvailable()
        {
            return NetworkInterface.GetIsNetworkAvailable() && NetworkInterface.NetworkInterfaceType != NetworkInterfaceType.None;
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            string categoryId = string.Empty;
            isNetworkAvailable = await IsNetworkAvailable();
            try
            {
                if (NavigationContext.QueryString.TryGetValue("selectedItem", out categoryId))
                {
                    selectedGroups = DbSingleton.Instance.Groups.Where(x => x.isSelected == true).ToList();
                    catId = int.Parse(categoryId);
                    Category selectedCategory = DbSingleton.Instance.Categories.SingleOrDefault(x => x.CategoryId == catId);
                    if (selectedCategory == null)
                        return;
                    CategoryName.Text = selectedCategory.CategoryName;
                    if (selectedCategory.isCollection && selectedCategory.Groups.Count > 0)
                    {
                        var deleteAppBarButton = new ApplicationBarIconButton(new Uri("/Icons/delete.png", UriKind.Relative))
                            {
                                Text = "Удалить группы"
                            };
                        deleteAppBarButton.Click += deleteAppBarButton_Click;
                        ApplicationBar.Buttons.Add(deleteAppBarButton);
                    }

                    if (App.GroupSelectPageViewModel.Count > 0)
                        return;
                    List<Group> slectedCategoruGroups = selectedCategory.Groups.ToList();
                    if (isNetworkAvailable && slectedCategoruGroups.Any(x=>x.Name==null))
                    {
                        List<string> ids = new List<string>();
                        foreach (var grp in slectedCategoruGroups)
                        {
                            ids.Add(grp.VkId.ToString());
                        }
                        string groups_ids = String.Join(",", ids.ToArray());
                        Dictionary<string, string> parameters = new Dictionary<string, string>();
                        parameters.Add("group_ids", groups_ids);
                        ShowProgressBar();
                        VkRequestWrapper.DoRequest<GetGroupsById>("groups.getById", parameters, Callback);
                    }
                    else
                    {
                        foreach (var grp in slectedCategoruGroups)
                        {
                            App.GroupSelectPageViewModel.Add(new GroupListItemViewModel()
                            {
                                Enabled = false,
                                ImageSrc = new Uri(grp.Image),
                                IsSelected = grp.isSelected,
                                Id = grp.VkId,
                                ListGroupName = grp.Name,
                                Visible = "Visible",
                            });
                        }
                        LongListSelector.ItemsSource = App.GroupSelectPageViewModel;
                    }
                    //GroupsHelper.CreateGroupsViewModel(catId);
                }
            }
            catch
            {
                MessageBox.Show("Ошибка получения данных о группах");
            }
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
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
        }

        void deleteAppBarButton_Click(object sender, EventArgs e)
        {
            List<GroupListItemViewModel> selectedGroups = App.GroupSelectPageViewModel.Where(x => x.IsSelected == true).ToList();
            if (selectedGroups.Count > 0)
            {
                try
                {
                    foreach (var group in selectedGroups)
                    {
                        var groupForDelete = DbSingleton.Instance.Groups.FirstOrDefault(x => x.vkUserId == VkHelper.CurrentUserId && x.isFinded == true && x.VkId == group.Id);
                        if (groupForDelete == null)
                        {
                            MessageBox.Show("Ошибка при удалении группы " + group.ListGroupName);
                            return;
                        }
                        DbSingleton.Instance.Groups.DeleteOnSubmit(groupForDelete);
                        App.GroupSelectPageViewModel.Remove(group);
                    }
                    DbSingleton.Instance.SubmitChanges();
                }
                catch
                {
                    MessageBox.Show("Ошибка при удалении группы ");
                    return;
                }
            }
            else
                MessageBox.Show("Выбирите группы для удаления");

        }

        private void ShowProgressBar()
        {
            Dispatcher.BeginInvoke(() =>
            {
                globalProgressBar.IsIndeterminate = true;
                globalProgressBar.Visibility = System.Windows.Visibility.Visible;
            });
        }
        private void HideProgressBar()
        {
            Dispatcher.BeginInvoke(() =>
            {
                globalProgressBar.Visibility = System.Windows.Visibility.Collapsed;
                globalProgressBar.IsIndeterminate = false;
            });
        }



        public void Callback(IRestResponse<GetGroupsById> response, RestRequestAsyncHandle arg2)
        {
            //GetGroupsById response = JsonConvert.DeserializeObject<GetGroupsById>(arg1.Content);
            FillAndShowGroups(response.Data.response);
            
            //throw new NotImplementedException();
        }

        private void FillAndShowGroups(List<GroupResponse> groups)
        {
            try
            {
                foreach (var grp in groups)
                {

                    Group groupFromDb = DbSingleton.Instance.Groups.FirstOrDefault(x => x.VkId == grp.gid);
                    App.GroupSelectPageViewModel.Add(new GroupListItemViewModel()
                    {
                        Enabled = false,
                        ImageSrc = new Uri(grp.photo),
                        IsSelected = groupFromDb.isSelected,
                        Id = grp.gid,
                        ListGroupName = grp.name,
                        Visible = "Visible",
                    });
                    groupFromDb.Name = grp.name;
                    groupFromDb.Image = grp.photo;
                    DbSingleton.Instance.SubmitChanges();
                }
                HideProgressBar();
                LongListSelector.ItemsSource = App.GroupSelectPageViewModel;
            }
            catch
            {
                MessageBox.Show("Ошибка получения данных о группах");
                HideProgressBar();
            }
        }

        private void LongListSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LongListSelector.SelectedItem != null)
            {
                GroupListItemViewModel selectedModel = (GroupListItemViewModel)LongListSelector.SelectedItem;
                if (isCheck == false && isUncheck == false)
                    selectedModel.IsSelected = !selectedModel.IsSelected;
                isCheck = isUncheck = false;
                Group group = DbSingleton.Instance.Groups.SingleOrDefault(x => x.VkId == selectedModel.Id);
                if (group != null)
                    group.isSelected = selectedModel.IsSelected;
                DbSingleton.Instance.SubmitChanges();
                LongListSelector.SelectedItem = null;
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

    }
}