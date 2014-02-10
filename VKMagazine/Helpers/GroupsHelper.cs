using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VKMagazine.ViewModels;
using VKMagazine.DAO;
using System.Net;
using RestSharp;
using VKMagazine.Response;

namespace VKMagazine.Helpers
{
    public static class GroupsHelper
    {
        public const string FavouritedIcon = "/Icons/icon_sl_star.png";
        public const string NoFavouritedIcon = "/Icons/icon_sl_black_star.png";
        public static Dictionary<int, List<int>> GroupsByCategory { get; set; }
        public static ObservableCollection<GroupListItemViewModel> CreateGroupsViewModel(int categoryId)
        {
            ObservableCollection<GroupListItemViewModel> viewModel = new ObservableCollection<GroupListItemViewModel>();
            List<Group> groups = DbSingleton.Instance.Groups.Where(x => x.CategoryId == categoryId).ToList();
            createGroupViewModelItem(groups);
            foreach (var grp in groups)
            {
                //viewModel.Add(createGroupViewModelItem(grp));
            }
            return null;
        }

        private static GroupListItemViewModel createGroupViewModelItem(List<Group> groups)
        {
            List<string> ids = new List<string>();
             
            foreach (var grp in groups)
            {
                ids.Add(grp.VkId.ToString());
            }
            string groups_ids = String.Join(",", ids.ToArray());
            RestClient client = new RestClient("https://api.vk.com/method/groups.getById/");
            RestRequest request = new RestRequest();
            request.AddParameter("group_ids", groups_ids);
           // client.ExecuteAsync<GetGroupsById>(request, GroupsSelectPage.Callback);
            
            return null;

        }

        
        
    }
}
