using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VKMagazine.DAO;

namespace VKMagazine.Helpers
{
    /// <summary>
    /// class is responsible for the authorization through vkontakte
    /// </summary>
    public static class VkHelper
    {
        public const string AccessToken = "access_token";
        public const string VkUserId = "user_id";
        public const string VkUserPhotoUrl = "vk_user_photo_url";
        public const string VkUserName = "vk_user_name";
        public static string CurrentAccess_token
        {
            get
            {
                if (IsolatedStorageSettings.ApplicationSettings.Contains(AccessToken))
                    return IsolatedStorageSettings.ApplicationSettings[AccessToken].ToString();
                else
                    return string.Empty;
            }
            set
            {
                if (IsolatedStorageSettings.ApplicationSettings.Contains(AccessToken))
                    IsolatedStorageSettings.ApplicationSettings[AccessToken] = value;
                else
                    IsolatedStorageSettings.ApplicationSettings.Add(AccessToken, value);
            }
        }
        public static string CurrentUserId
        {
            get
            {
                if (IsolatedStorageSettings.ApplicationSettings.Contains(VkUserId))
                    return IsolatedStorageSettings.ApplicationSettings[VkUserId].ToString();
                else
                    return string.Empty;
            }
            set
            {
                if (IsolatedStorageSettings.ApplicationSettings.Contains(VkUserId))
                    IsolatedStorageSettings.ApplicationSettings[VkUserId] = value;
                else
                    IsolatedStorageSettings.ApplicationSettings.Add(VkUserId, value);
            }
        }
        public static string UserPhoto
        {
            get
            {
                if (IsolatedStorageSettings.ApplicationSettings.Contains(VkUserPhotoUrl))
                    return IsolatedStorageSettings.ApplicationSettings[VkUserPhotoUrl].ToString();
                else
                    return string.Empty;
            }
            set
            {
                if (IsolatedStorageSettings.ApplicationSettings.Contains(VkUserPhotoUrl))
                    IsolatedStorageSettings.ApplicationSettings[VkUserPhotoUrl] = value;
                else
                    IsolatedStorageSettings.ApplicationSettings.Add(VkUserPhotoUrl, value);
            }
        }
        public static string UserName
        {
            get
            {
                if (IsolatedStorageSettings.ApplicationSettings.Contains(VkUserName))
                    return IsolatedStorageSettings.ApplicationSettings[VkUserName].ToString();
                else
                    return string.Empty;
            }
            set
            {
                if (IsolatedStorageSettings.ApplicationSettings.Contains(VkUserName))
                    IsolatedStorageSettings.ApplicationSettings[VkUserName] = value;
                else
                    IsolatedStorageSettings.ApplicationSettings.Add(VkUserName, value);
            }
        }

        static VkHelper()
        {
            if (IsUserAuthenticated())
            {
                var settings = IsolatedStorageSettings.ApplicationSettings;
                CurrentAccess_token = settings[AccessToken].ToString();
                CurrentUserId = settings[VkUserId].ToString();
                if (settings.Contains(VkUserPhotoUrl))
                    UserPhoto = settings[VkUserPhotoUrl].ToString();
                if (settings.Contains(VkUserName))
                    UserName = settings[VkUserName].ToString();
            }
        }

        public static bool IsUserAuthenticated()
        {
            return IsolatedStorageSettings.ApplicationSettings.Contains(AccessToken) ;
        }

        public static void LogOut()
        {
            if (IsUserAuthenticated())
            {
                Category userGroupsCategory = DbSingleton.Instance.Categories.SingleOrDefault(x=> x.VkUserId==CurrentUserId && x.isUserGroups==true);
                if (userGroupsCategory != null)
                {
                    userGroupsCategory.IsSelected = false;
                    foreach (var grp in userGroupsCategory.Groups)
                    {
                        grp.isSelected = false;
                    }
                    DbSingleton.Instance.SubmitChanges();
                }
                var settings = IsolatedStorageSettings.ApplicationSettings;
                settings.Remove(AccessToken);
                settings.Remove(VkUserId);
                if (settings.Contains(VkUserPhotoUrl))
                    settings.Remove(VkUserPhotoUrl);
                if (settings.Contains(VkUserName))
                    settings.Remove(VkUserName);
                settings.Save();
            }
        }

    }
}
