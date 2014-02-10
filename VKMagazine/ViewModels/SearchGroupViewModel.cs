using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace VKMagazine.ViewModels
{
     public class SearchGroupViewModel  : INotifyPropertyChanged
    {
         public ObservableCollection<GroupListItemViewModel> SearchedGroups { get; set; }
         private bool _isLoading;
         public SearchGroupViewModel()
         {
             SearchedGroups = new ObservableCollection<GroupListItemViewModel>();
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

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        internal void LoadPage(int p)
        {
            throw new NotImplementedException();
        }
    }
}
