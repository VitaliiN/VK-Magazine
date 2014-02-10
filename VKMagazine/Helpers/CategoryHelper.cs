using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VKMagazine.ViewModels;
using VKMagazine.DAO;
using System.IO.IsolatedStorage;
using System.Net.Http;
using Newtonsoft.Json;
using VKMagazine.Response;

namespace VKMagazine.Helpers
{
    public class CategoryHelper
    {
        public const string userToken = "userToken";
        // private static ObservableCollection<GroupListItemViewModel> Categories;
        public CategoryHelper()
        {
           
            //Categories = new ObservableCollection<GroupListItemViewModel>();
            //foreach (var cat in DbSingleton.Instance.Categories)
            //{

            //    Categories.Add(new GroupListItemViewModel()
            //    {
            //        Id = cat.CategoryId,
            //        Enabled = true,
            //        Visible = "Visible",
            //        ImageSrc = new Uri(cat.IconPath, UriKind.Relative),
            //        CurrentCount = cat.Groups.Where(x => x.isSelected == true).Count(),
            //        MaxCount = cat.Groups.Count,
            //        IsSelected = cat.IsSelected,
            //        ListGroupName = cat.CategoryName
            //    });
            //    return Categories;

        }
        public async static Task<ObservableCollection<GroupListItemViewModel>> GetCategories()
        {
            ObservableCollection<GroupListItemViewModel> Categories = new ObservableCollection<GroupListItemViewModel>();
            bool isNeedToDoSubmit = false;
            var settings = IsolatedStorageSettings.ApplicationSettings;
            Category userCollectionCategory = DbSingleton.Instance.Categories.FirstOrDefault(x => x.isCollection == true); //&& x.VkUserId == VkHelper.CurrentUserId );
            if (userCollectionCategory == null)
            {
                userCollectionCategory = new Category()
                {
                    VkUserId = VkHelper.CurrentUserId,
                    CategoryName = "Мои коллекции",
                    IconPath = "/Icons/list_icon.png",
                    IsSelected = false,
                    isCollection = true
                };
                DbSingleton.Instance.Categories.InsertOnSubmit(userCollectionCategory);
                DbSingleton.Instance.SubmitChanges();
            }
            bool isSelected = userCollectionCategory.Groups.Count == userCollectionCategory.Groups.Where(x => x.isSelected).Count();
            bool collectionEnabledClick = userCollectionCategory.Groups.Count > 0;//DbSingleton.Instance.Groups.Any(x => x.vkUserId == VkHelper.CurrentUserId);
            string collectionVisibleCount = collectionEnabledClick ? "Visible" : "Collapsed";  //DbSingleton.Instance.Groups.Any(x => x.vkUserId == VkHelper.CurrentUserId && x.isFinded) ? "Visible" : "Collapsed"; 
            Categories.Add(new GroupListItemViewModel()
            {
                CurrentCount = userCollectionCategory.Groups.Where(x => x.isSelected).Count(),   // DbSingleton.Instance.Groups.Where(x => x.vkUserId == VkHelper.CurrentUserId && x.isSelected == true && x.isFinded == true).Count(),
                Enabled = collectionEnabledClick,
                Visible = collectionVisibleCount,
                MaxCount = userCollectionCategory.Groups.Count,//DbSingleton.Instance.Groups.Where(x=> x.vkUserId==VkHelper.CurrentUserId && x.isFinded==true).Count(),
                ImageSrc = new Uri(userCollectionCategory.IconPath, UriKind.Relative),
                Id = userCollectionCategory.CategoryId,
                IsSelected = isSelected,
                ListGroupName = userCollectionCategory.CategoryName,
                //IsSelected
            });
            if (!VkHelper.IsUserAuthenticated())
            {
                //Categories.Add(new GroupListItemViewModel()
                //{
                //    Visible = "Collapsed",
                //    ListGroupName = "Мои коллекции",
                //    Enabled= false,
                //    ImageSrc = new Uri("/Icons/list_icon.png", UriKind.Relative)
                //});
                Categories.Add(new GroupListItemViewModel()
                {
                    Visible = "Collapsed",
                    Enabled=false,
                    ListGroupName = "Войдите, чтобы читать свои группы",
                    ImageSrc = new Uri("/Icons/sub_avatar.png", UriKind.Relative)
                });
            }
            else
            {
                
                Category userGroupsCategory = DbSingleton.Instance.Categories.FirstOrDefault(x => x.VkUserId == VkHelper.CurrentUserId && x.isUserGroups == true);
                if( userGroupsCategory==null)
                {
                    userGroupsCategory = new Category()
                    {
                        VkUserId = VkHelper.CurrentUserId,
                        CategoryName = "Мои группы",
                        IconPath = VkHelper.UserPhoto,
                        IsSelected = false,
                        isUserGroups = true
                    };
                    DbSingleton.Instance.Categories.InsertOnSubmit(userGroupsCategory);
                    DbSingleton.Instance.SubmitChanges();
                    await FillGroupsForUser(userGroupsCategory); 
                }
                Categories.Add(new GroupListItemViewModel()
                {
                    CurrentCount=userGroupsCategory.Groups.Where(x=>x.isSelected).Count(),
                    Enabled=true,
                    Visible="Visible",
                    MaxCount=userGroupsCategory.Groups.Count,
                    ImageSrc = new Uri(userGroupsCategory.IconPath),
                    Id=userGroupsCategory.CategoryId,
                    IsSelected = userGroupsCategory.IsSelected,
                    ListGroupName = userGroupsCategory.CategoryName
                });
                                
            }
            
            foreach (var cat in DbSingleton.Instance.Categories.Where(x=>x.isCollection==false && x.isUserGroups==false))
            {
                bool curSelected = cat.Groups.Where(x => x.isSelected == true).Count() == cat.Groups.Count;
                if (curSelected != cat.IsSelected)
                {
                    cat.IsSelected = curSelected;
                    isNeedToDoSubmit = true;
                }
                Categories.Add(new GroupListItemViewModel()
                {
                    Id = cat.CategoryId,
                    Enabled = true,
                    Visible = "Visible",
                    ImageSrc = new Uri(cat.IconPath, UriKind.Relative),
                    CurrentCount = cat.Groups.Where(x => x.isSelected == true).Count(),
                    MaxCount = cat.Groups.Count,
                    IsSelected = cat.IsSelected,
                    ListGroupName = cat.CategoryName
                });
                if (isNeedToDoSubmit) DbSingleton.Instance.SubmitChanges();
            }
            return Categories;

        }

        public static async Task FillGroupsForUser(Category userGroupsCategory)
        {
           
                HttpClient client = new HttpClient();
                string request = String.Format("https://api.vk.com/method/groups.get?user_id={0}&access_token={1}&v=3.0", userGroupsCategory.VkUserId, VkHelper.CurrentAccess_token);
                string response = await client.GetStringAsync(request);
                GetUserGroupsResponse groupIds = await JsonConvert.DeserializeObjectAsync<GetUserGroupsResponse>(response);
                foreach (int grpId in groupIds.response)
                {
                    Group g = DbSingleton.Instance.Groups.SingleOrDefault(x => x.VkId == grpId);
                    if (g == null)
                        userGroupsCategory.Groups.Add(new Group() { VkId = grpId, Category = userGroupsCategory });

                }
                DbSingleton.Instance.SubmitChanges();

            
        }
        //Categories.Add(new GroupListItemViewModel()
        //{
        //    ImageSrc = new Uri("Icons/smi.png", UriKind.Relative),
        //    IsSelected = false,
        //    ListGroupName = "СМИ",
        //    MaxCount = 14,
        //    Visible = "Visible",
        //    Id=1
        //});
        //Categories.Add(new GroupListItemViewModel()
        //{
        //    ImageSrc = new Uri("/Icons/tech.png", UriKind.Relative),
        //    IsSelected = false,
        //    ListGroupName = "Hi-Tech",
        //    MaxCount = 11,
        //    Visible="Visible",
        //    Id=2
        //});
        //Categories.Add(new GroupListItemViewModel()
        //{
        //    ImageSrc = new Uri("/Icons/tech.png", UriKind.Relative),
        //    IsSelected = false,
        //    ListGroupName = "Образ Жизни",
        //    MaxCount = 10,
        //    Visible = "Visible",
        //    Id = 2
        //});



    }


}
