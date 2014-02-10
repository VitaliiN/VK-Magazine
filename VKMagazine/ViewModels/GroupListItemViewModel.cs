using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace VKMagazine.ViewModels
{
    public class GroupListItemViewModel : INotifyPropertyChanged 
    {
        private bool _isSelected;
        private int _currentCount;
        private string _buttonText;
        private string _buttonColor;
        public Uri ImageSrc { get; set; }
        public String ListGroupName { get; set; }
        public bool Enabled { get; set; }
        public int MaxCount { get; set; }
        public string ButtonText
        {
            get
            {
                return _buttonText;
            }
            set
            {
                _buttonText = value;
                OnPropertyChanged();
            }
        }
        public string ButtomColor
        {
            get
            {
                return _buttonColor;
            }
            set
            {
                _buttonColor = value;
                OnPropertyChanged();
            }
        }

        public string CurrentSelectedCount { get { return CurrentCount + "/" + MaxCount; } }
        public string Visible { get; set; }
        public int Id { get; set; }
        public bool IsClosed { get; set; }


        public bool IsSelected { get { return _isSelected; }
            set { _isSelected = value; OnPropertyChanged("IsSelected"); } }

        public int CurrentCount { get { return _currentCount; } 
            set { _currentCount = value; OnPropertyChanged("CurrentSelectedCount"); } }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName]string propertyName="")
        {
            var evt = PropertyChanged;
            if (evt != null) 
            {
                evt(this, new PropertyChangedEventArgs(propertyName)); 
            } 
        } 


    }
}
