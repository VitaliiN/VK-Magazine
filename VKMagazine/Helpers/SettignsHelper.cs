using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.IsolatedStorage;

namespace VKMagazine.Helpers
{
    public class SettignsHelper
    {
        private const string NEED_SHOW_POSTS_WITH_GROUPS = "posts_to_groups";
        private const string NEED_SHOW_POSTS_WITH_LINKS = "posts_tolinks";
        private const string NEED_SHOW_SPASH_SCREEN = "need_show_spash_screen";

        public static bool IsNeedToShowSplashScreen
        {
            get
            {
                if (IsolatedStorageSettings.ApplicationSettings.Contains(NEED_SHOW_SPASH_SCREEN))
                    return (bool)IsolatedStorageSettings.ApplicationSettings[NEED_SHOW_SPASH_SCREEN];
                else
                    return true;
            }
            set
            {
                if (IsolatedStorageSettings.ApplicationSettings.Contains(NEED_SHOW_SPASH_SCREEN))
                    IsolatedStorageSettings.ApplicationSettings[NEED_SHOW_SPASH_SCREEN] = value;
                else
                    IsolatedStorageSettings.ApplicationSettings.Add(NEED_SHOW_SPASH_SCREEN, value);
            }
        }

        public static bool IsNeedToHidePostsToGroups
        {
            get
            {
                if (IsolatedStorageSettings.ApplicationSettings.Contains(NEED_SHOW_POSTS_WITH_GROUPS))
                    return (bool)IsolatedStorageSettings.ApplicationSettings[NEED_SHOW_POSTS_WITH_GROUPS];
                else
                    return false;
            }
            set
            {
                if (IsolatedStorageSettings.ApplicationSettings.Contains(NEED_SHOW_POSTS_WITH_GROUPS))
                    IsolatedStorageSettings.ApplicationSettings[NEED_SHOW_POSTS_WITH_GROUPS] = value;
                else
                    IsolatedStorageSettings.ApplicationSettings.Add(NEED_SHOW_POSTS_WITH_GROUPS, value);
            }
        }

        public static bool IsNeedToHidePostsWithLinks
        {
            get
            {
                if (IsolatedStorageSettings.ApplicationSettings.Contains(NEED_SHOW_POSTS_WITH_LINKS))
                    return (bool)IsolatedStorageSettings.ApplicationSettings[NEED_SHOW_POSTS_WITH_LINKS];
                else
                    return false;
            }
            set
            {
                if (IsolatedStorageSettings.ApplicationSettings.Contains(NEED_SHOW_POSTS_WITH_LINKS))
                    IsolatedStorageSettings.ApplicationSettings[NEED_SHOW_POSTS_WITH_LINKS] = value;
                else
                    IsolatedStorageSettings.ApplicationSettings.Add(NEED_SHOW_POSTS_WITH_LINKS, value);
            }
        }
    }
}
