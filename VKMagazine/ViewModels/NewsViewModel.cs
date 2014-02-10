using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using VKMagazine.DAO;
using VKMagazine.Response;
using VKMagazine.Helpers;

namespace VKMagazine.ViewModels
{
    public class NewsViewModel : INotifyPropertyChanged
    {
        private const int MAX_GROUP_COUNT_FOR_AUTH_USER = 15;
        private const int MAX_GROUP_COUNT_FOR_SIMPLE_USER = 25;
        //private static readonly Regex UrlRegex = new Regex(@"(?#Protocol)(?:(?:ht|f)tp(?:s?)\:\/\/|~/|/)?(?#Username:Password)(?:\w+:\w+@)?(?#Subdomains)(?:(?:[-\w]+\.)+(?#TopLevel Domains)(?:com|org|net|gov|mil|biz|info|mobi|name|aero|jobs|museum|travel|[a-z]{2}))(?#Port)(?::[\d]{1,5})?(?#Directories)(?:(?:(?:/(?:[-\w~!$+|.,=]|%[a-f\d]{2})+)+|/)+|\?|#)?(?#Query)(?:(?:\?(?:[-\w~!$+|.,*:]|%[a-f\d{2}])+=(?:[-\w~!$+|.,*:=]|%[a-f\d]{2})*)(?:&(?:[-\w~!$+|.,*:]|%[a-f\d{2}])+=(?:[-\w~!$+|.,*:=]|%[a-f\d]{2})*)*)*(?#Anchor)(?:#(?:[-\w~!$+|.,*:=]|%[a-f\d]{2})*)?");
        private int AddedPages = 0;
       // public static int offset=0;
        private static int diff=0;
        private ObservableCollection<PostDetailsViewModel> _news;
        private bool _isLoading;
        private int count;
        private int totalOffset = 0;
        private int currentGroupNumber = 0;
        //private int currentOffset = 0;
        public NewsViewModel()
        {
            _news = new ObservableCollection<PostDetailsViewModel>();
        }
        public bool IsLoading
        {
            get
            {
                return _isLoading;
            }
            set
            {
                _isLoading = value;
                NotifyPropertyChanged();

            }
        }
        public ObservableCollection<PostDetailsViewModel> News
        {
            get
            {
                return _news;
            }
            set
            {
                if (_news != value)
                {
                    _news = value;
                    NotifyPropertyChanged();
                }
            }
        }




        public async Task LoadPage(bool isNeedLoadNextPage)
        {
           
            IsLoading = true;
            await GetPostsfFromGroups(isNeedLoadNextPage);
            IsLoading = false;
            //if (pageNumber == 1) this.TwitterCollection.Clear();

            //IsLoading = true;
            //HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(new Uri(String.Format(SEARCH_URI, searchTerm, pageNumber)));
            //request.BeginGetResponse(new AsyncCallback(ReadCallback), request);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public async Task GetPostsfFromGroups(bool isNeedLoadNextPage = false)
        {
            int MAX_GROUP_COUNT=0;
            if (Helpers.VkHelper.IsUserAuthenticated())
                MAX_GROUP_COUNT = MAX_GROUP_COUNT_FOR_AUTH_USER;
            else
                MAX_GROUP_COUNT = MAX_GROUP_COUNT_FOR_SIMPLE_USER;
            List<Group> selectedGroups = DbSingleton.Instance.Groups.Where(x => x.isSelected == true).ToList();
            Task fillGroupsTask = FillGroupsNameAndImages(selectedGroups);
            List<PostDetails> postsResponse = new List<PostDetails>();
            if (isNeedLoadNextPage == false) currentGroupNumber = 0;
            int count = selectedGroups.Count;
            if (count < 3)
            {
                count = 8;
            }
            else if (selectedGroups.Count < 5)
            {
                count = 5;
            }
            else if (selectedGroups.Count < 15)
            {
                count = 2;
            }
            else
            {
                count = 1;
            }
            if (isNeedLoadNextPage ==true && selectedGroups.Count<MAX_GROUP_COUNT)
                totalOffset += count;
            else if(selectedGroups.Count<MAX_GROUP_COUNT)
                totalOffset = 0;
            System.Net.Http.HttpClient httpClient = new System.Net.Http.HttpClient();
            List<Task<string>> responseListTask = new List<Task<string>>();
            List<string> responseList = new List<string>();
            Stopwatch st = new Stopwatch();
            st.Start();
            try
            {
                if (selectedGroups.Count < MAX_GROUP_COUNT)
                    currentGroupNumber = 0;
                for (; currentGroupNumber < selectedGroups.Count; currentGroupNumber++)
                {
                    
                    string req = String.Format("https://api.vk.com/method/wall.get?count={0}&filter=owner&owner_id=-{1}&offset={2}", count, selectedGroups[currentGroupNumber].VkId, totalOffset);
                    if (Helpers.VkHelper.IsUserAuthenticated())
                        req += "&access_token=" + Helpers.VkHelper.CurrentAccess_token;
                    //responseList.Add(await httpClient.GetStringAsync(req));
                    if (Helpers.VkHelper.IsUserAuthenticated())
                    {
                        try
                        {
                            //string response = await httpClient.GetStringAsync(req);
                            //responseList.Add(response);
                            responseListTask.Add(httpClient.GetStringAsync(req));
                        }
                        catch
                        {
                            continue;
                        }
                    }
                    else
                        responseListTask.Add(httpClient.GetStringAsync(req));

                    if ((currentGroupNumber + 1) % MAX_GROUP_COUNT == 0)
                    {
                        currentGroupNumber++;
                        break;
                    }
                }
                if (currentGroupNumber >= selectedGroups.Count-1 && selectedGroups.Count>= MAX_GROUP_COUNT)
                {
                    currentGroupNumber = 0;

                    totalOffset++;
                }
                //if (!Helpers.VkHelper.IsUserAuthenticated())
                {
                    await Task.WhenAll(responseListTask.ToArray());
                    foreach (var tsk in responseListTask)
                        responseList.Add(tsk.Result);
                }
                //foreach (var grp in selectedGroups)
                //{

                //    string req = String.Format("https://api.vk.com/method/wall.get?count={0}&filter=owner&owner_id=-{1}&offset={2}", count, grp.VkId, totalOffset);
                //    if(Helpers.VkHelper.IsUserAuthenticated())
                //        req+="&access_token=" + Helpers.VkHelper.CurrentAccess_token;
                //    //responseList.Add(await httpClient.GetStringAsync(req));
                //        responseListTask.Add(httpClient.GetStringAsync(req));
                //}
                    
                
               
                Debug.WriteLine("Get response for groups = " + st.ElapsedMilliseconds);
                st.Restart();
                //foreach (Task<string> task in responseListTask)
                List<Task<JObject>> jsonPostsArray = new List<Task<JObject>>();
                foreach (string responseJson in responseList)
                {
                    jsonPostsArray.Add(JsonConvert.DeserializeObjectAsync<JObject>(responseJson));
                }
                await Task.WhenAll(jsonPostsArray.ToArray());
                foreach (var tsk in jsonPostsArray)
                {
                    //JObject response = await JsonConvert.DeserializeObjectAsync<JObject>(responseJson);
                    JArray jsonPosts;
                    try
                    {
                        jsonPosts = (tsk.Result["response"] as JArray);
                        if (jsonPosts == null)
                        {
                            if (tsk.Result.First.First.First.First.Value<Int32>() == 5)
                            {
                                MessageBox.Show("Сеанс истек, перезайдите в аккаунт вк");
                                return;
                            }
                            continue;
                        }
                        jsonPosts.RemoveAt(0);
                    }
                    catch (NullReferenceException)
                    {
                        continue;
                    }
                    
                    foreach (JObject postDetails in jsonPosts)
                    {
                        PostDetails model = postDetails.ToObject<PostDetails>();
                        if (model.attachments != null)
                        {
                            if (model.attachments.Any(x => x.type == "photo"))
                            {
                                if (!SettignsHelper.IsNeedToHidePostsToGroups)
                                {
                                    if (model.text.Contains("[club") || model.text.Contains("vk.com"))
                                        continue;
                                }
                                if (!SettignsHelper.IsNeedToHidePostsWithLinks)
                                {
                                    if (model.text.Contains("http://"))
                                        continue;
                                }
                                postsResponse.Add(model);
                            }
                        }
                    }
                }
                Debug.WriteLine("Transoforms response to objects = " + st.ElapsedMilliseconds);
                postsResponse = postsResponse.OrderByDescending(x => x.date).ToList();
                List<FavouritedPost> favouritePosts = DbSingleton.Instance.FavouritedPosts.ToList();
                await fillGroupsTask;
                foreach (PostDetails post in postsResponse)
                {

                    string favIcon = favouritePosts.Any(x => x.VkPostId == post.id) ? Helpers.GroupsHelper.FavouritedIcon : Helpers.GroupsHelper.NoFavouritedIcon;
                    Group postGroup = selectedGroups.FirstOrDefault(x => x.VkId == Math.Abs(post.from_id));
                    if (postGroup == null)
                        continue;
                    if (News.Any(x => x.Id == post.id) == false)
                    {
                        News.Add(new PostDetailsViewModel()
                        {
                            Date = TransformUnixDateToSimpleDateTime(post.date),
                            Text = WebUtility.HtmlDecode(post.text).Replace("<br>", Environment.NewLine),
                            GroupImage = postGroup.Image,
                            GroupName = postGroup.Name.Length > 14 ? postGroup.Name.Substring(0, 14) + "..." : postGroup.Name,
                            FavouriteIcon = favIcon,//Helpers.GroupsHelper.NoFavouritedIcon,
                            //Src_big_image = new ObservableCollection<BitmapImage>(post.attachments.Where(x => x.type == "photo").Select(x => new BitmapImage(new Uri(x.photo.src_big)))),
                            //Src_big = new ObservableCollection<string>(post.attachments.Where(x => x.type == "photo").Select(x => x.photo.src_big)),
                            Src_big = new ObservableCollection<SrcBig>(post.attachments.Where(x => x.type == "photo").Select(x => new SrcBig() { OnlineUri = x.photo.src_big })),
                            Src_small = new ObservableCollection<string>(post.attachments.Where(x => x.type == "photo").Select(x => x.photo.src_small)),
                            Id = post.id,
                            Group_id = post.from_id,

                        });
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message + "\n " + e.StackTrace);
            }
            if (postsResponse.Count < 5)
            {
                // if(diff<0)
                AddedPages += postsResponse.Count;
                if (AddedPages > 7)
                {
                    AddedPages = 0;
                    return;
                }
                await LoadPage(true);
            }
        }
        /// <summary>
        /// fill image and name fileds for selected groups
        /// </summary>
        /// <param name="selectedGroups"> selected for viewing groups</param>
        /// <returns></returns>
        private async Task FillGroupsNameAndImages(List<Group> selectedGroups)
        {
            Stopwatch st = new Stopwatch();
            st.Start();
            System.Net.Http.HttpClient httpClinet = new System.Net.Http.HttpClient();
            List<string> ids = new List<string>();
            foreach (var grp in selectedGroups)
            {
                ids.Add(grp.VkId.ToString());
            }
            string groups_ids = String.Join(",", ids.ToArray());
            //string request = 
            try
            {
                string response = await httpClinet.GetStringAsync(String.Format("https://api.vk.com/method/groups.getById?group_ids={0}", groups_ids));
                GetGroupsById responseGroups = await JsonConvert.DeserializeObjectAsync<GetGroupsById>(response);
                for (int i = 0; i < responseGroups.response.Count; i++)
                {
                    selectedGroups[i].Image = responseGroups.response[i].photo;
                    selectedGroups[i].Name = responseGroups.response[i].name;
                }
            }
            catch (Exception e)
            {
                IsLoading = false;
                Debug.WriteLine(" FillGroupsNameAndImages \n" + e.Message);
            }
            Debug.WriteLine("FillGroupsNameAndImages elapsed = " + st.ElapsedMilliseconds);
        }
        /// <summary>
        /// transform date from c# presentation to unix 
        /// </summary>
        /// <param name="date">date for transforming</param>
        /// <returns></returns>
        public static long TransoformDateToUnix(DateTime date)
        {
            DateTime ePoch = new DateTime(1970, 1, 1, 0, 0, 0);
            TimeSpan timeSpan = (date.ToUniversalTime() - ePoch);
            long unixTimeStamp = (long)Math.Floor(timeSpan.TotalSeconds);
            return unixTimeStamp;
        }

        /// <summary>
        /// transform date from unix presentation to c# 
        /// </summary>
        /// <param name="unixTimeStamp">unix datetime</param>
        /// <returns></returns>
        private DateTime TransformUnixDateToSimpleDateTime(long unixTimeStamp)
        {
            DateTime ePoch = new DateTime(1970, 1, 1, 0, 0, 0);
            DateTime dateTimeConvertedBack = ePoch.AddSeconds(unixTimeStamp);
            return dateTimeConvertedBack;
        }
    }
}
