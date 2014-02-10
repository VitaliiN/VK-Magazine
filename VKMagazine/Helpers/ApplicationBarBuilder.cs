using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Controls;
using System.Windows;
using VKMagazine.DAO;
using Microsoft.Phone.Net.NetworkInformation;

namespace VKMagazine.Helpers
{
    public  class ApplicationBarBuilder
    {
        public static IApplicationBar BuildAppBar(IApplicationBar applicationBar)
        {
            applicationBar = new ApplicationBar();

            //var appBarButtonSearch =
            //    new ApplicationBarIconButton(new Uri("/Icons/feature.search.png", UriKind.Relative))
            //        {
            //            Text = "Найти группу"
            //        };
            //appBarButtonSearch.Click += AppBarButtonSearch_Click;
            //applicationBar.Buttons.Add(appBarButtonSearch);
            var appBarButtonWatchNews =
                new ApplicationBarIconButton(new Uri("/Icons/next.png", UriKind.Relative))
                    {
                        Text = "Просмотр новостей"
                    };
            appBarButtonWatchNews.Click += AppBarButtonWatchNews_Click;
            applicationBar.Buttons.Add(appBarButtonWatchNews);
            return applicationBar;
        }

        private static async void AppBarButtonWatchNews_Click(object sender, EventArgs e)
        {
            List<Group> selectedGroups = DbSingleton.Instance.Groups.Where(x => x.isSelected == true).ToList();
            if (selectedGroups.Count == 0)
                MessageBox.Show("Выбирите группы");
            else
            {
                if (await IsNetworkAvailable())
                {
                    ((PhoneApplicationFrame)Application.Current.RootVisual).Navigate(new Uri(string.Format("/MainPage.xaml?pivotPage={0}", (int)Helpers.PivotPages.NewsPivot), UriKind.Relative));
                }
                else
                {
                    MessageBox.Show("нет подключения к интернету");
                    return;
                }

            }
        }

        private static async Task<bool> IsNetworkAvailable()
        {
            return NetworkInterface.GetIsNetworkAvailable() && NetworkInterface.NetworkInterfaceType != NetworkInterfaceType.None;
        }

        private static void AppBarButtonSearch_Click(object sender, EventArgs e)
        {
            
        }
    }
}
