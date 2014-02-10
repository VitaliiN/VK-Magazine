using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Kawagoe.Storage;

namespace VKMagazine.ViewModels
{
    public class SrcBig
    {
        public string OnlineUri { get; set; }
        public string IsoStoreUri { get; set; }
        public object OnlineUriCached
        {
            get
            {
                if (OnlineUri.Contains("isostore"))
                    return OnlineUri;
                else
                    return ImageCache.Default.Get(OnlineUri);
            }
        }
    }
    public class PostDetailsViewModel : INotifyPropertyChanged
    {
        private string _text;
        //private ObservableCollection<string> _src_big;
        private ObservableCollection<SrcBig> _src_big;
        private ObservableCollection<string> _src_small;
        private DateTime _date;
        private String _groupImage;
        private string _groupName;
        private string _favouriteIcon;


        public PostDetailsViewModel()
        {
            //_src_big = new ObservableCollection<string>();
            _src_big = new ObservableCollection<SrcBig>();
            _src_small = new ObservableCollection<string>();
        }

        public long Id { get; set; }

        public long Group_id { get; set; }

        public string IsNeedToShowMoreButton
        {
            get
            {
                return ((_text != null && _text.Length > 140) || _text.Count(x=> x=='\n') >3 )? "Visible" : "Collapsed";
            }
        }

        public string PostUrl
        {
            get
            {
                return String.Format("http://vk.com/public{0}?w=wall{1}_{2}", Math.Abs(Group_id),Group_id, Id);
            }
        }

        public string PostShare
        {
            get
            {
                return String.Format("wall{0}_{1}", Group_id, Id);
            }
        }

        public string Ago
        {
            get
            {
                TimeSpan timeZoneSpan = TimeZoneInfo.Local.BaseUtcOffset;
                
                TimeSpan mySpan = DateTime.Now.Subtract(_date).Subtract(timeZoneSpan).Add(new TimeSpan(0,0,40));
                string myDenom =
                  mySpan.Days > 0 ? "дн." :
                  mySpan.Hours > 0 ? "ч." :
                  mySpan.Minutes > 0 ? "мин." : "сек.";
                int myNumeral;
                switch (myDenom)
                {
                    case "сек.":
                        myNumeral = mySpan.Seconds;
                        break;
                    case "мин.":
                        myNumeral = mySpan.Minutes;
                        break;
                    case "дн.":
                        myNumeral = mySpan.Days;
                        break;
                    default:
                        myNumeral = mySpan.Hours;
                        break;
                }

                return String.Format("{0} {1} назад", myNumeral, myDenom);
            }
        }

        public string FavouriteIcon
        {
            get
            {
                return _favouriteIcon;
            }
            set
            {
                if (_favouriteIcon != value)
                {
                    _favouriteIcon = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string Text
        {
            get
            {
                return _text;
            }
            set
            {
                if (_text != value)
                {
                    _text = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string GroupImage
        {
            get
            {
                return _groupImage;
            }
            set
            {
                if (_groupImage != value)
                {
                    _groupImage = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string GroupName
        {
            get
            {
                return _groupName;
            }
            set
            {
                if (_groupName != value)
                {
                    _groupName = value;
                    NotifyPropertyChanged();
                }
            }
        }

        //public ObservableCollection<string> Src_big
        //{
        //    get
        //    {
        //        if (_src_big.Count % 2 == 1)
        //            return new ObservableCollection<string>(_src_big.Skip(1));
        //        else
        //            return _src_big;
        //    }
        //    set
        //    {
        //        if (_src_big != value)
        //        {
        //            _src_big = value;
        //            NotifyPropertyChanged();
        //        }
        //    }
        //}

        public ObservableCollection<SrcBig> Src_big
        {
            get
            {
                if (_src_big.Count % 2 == 1)
                    return new ObservableCollection<SrcBig>(_src_big.Skip(1));
                else
                    return _src_big;
            }
            set
            {
                if (_src_big != value)
                {
                    _src_big = value;
                    NotifyPropertyChanged();
                }
            }
        }

        //public ObservableCollection<string> Src_big_All
        //{
        //    get
        //    {
        //        return _src_big;
        //    }
        //    set
        //    {

        //        _src_big = value;
        //        NotifyPropertyChanged();
        //    }
        //}

        public ObservableCollection<SrcBig> Src_big_All
        {
            get
            {
                return _src_big;
            }
            set
            {

                _src_big = value;
                NotifyPropertyChanged();
            }
        }

        public ObservableCollection<string> Src_small
        {
            get
            {
                return _src_small;
            }
            set
            {
                if (_src_small != value)
                {
                    _src_small = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public DateTime Date
        {
            get
            {
                return _date;
            }
            set
            {
                if (_date != value)
                {
                    _date = value;
                    NotifyPropertyChanged();
                }
            }
        }

        //public string First
        //{
        //    get
        //    {
        //        if ((_src_big != null && _src_big.Count > 0 && _src_big.Count % 2 == 1))
        //            return _src_big[0];
        //        return null;
        //    }
        //    private set
        //    {
        //    }
        //}

        public string First
        {
            get
            {
                if ((_src_big != null && _src_big.Count > 0 && _src_big.Count % 2 == 1))
                {
                    return _src_big[0].OnlineUri;
                }
                return null;
            }
            private set
            {
            }
        }

        public object FirstCached
        {
            get
            {
                if ((_src_big != null && _src_big.Count > 0 && _src_big.Count % 2 == 1))
                {
                    if (_src_big[0].OnlineUri.Contains("isostore"))
                        return _src_big[0].OnlineUri;
                    else
                        return ImageCache.Default.Get(_src_big[0].OnlineUri);
                }
                return null;
            }
        }


        public string isNeedShowBigPicture
        {
            get
            {
                if (_src_big.Count % 2 == 1 || _src_big.Count == 1)
                    return "Visible";
                else
                    return "Collapsed";
            }
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
    }
}
