using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MyTime.ViewModels
{
    public class ReturnVisitLLItemModel : INotifyPropertyChanged
    {
        private string _text;
        private string _address1;
        private string _address2;
        private BitmapImage _image; 
        private int _itemID;

        public int ItemId
        {
            get { return _itemID; }
            set
            {
                if (value != _itemID) {
                    _itemID = value;
                    NotifyPropertyChanged("ItemId");
                }
            }
        }
        /// <summary>
        /// Sample ViewModel property; this property is used in the view to display its value using a Binding.
        /// </summary>
        /// <returns></returns>
        public BitmapImage ImageSource
        {
            get
            {
                return _image;
            }

            set
            {
                if (value != _image) {
                    _image = value;
                    NotifyPropertyChanged("ImageSource");
                }
            }
        }


        public string Text
        {
            get { return _text; }
            set
            {
                if (_text == value) return;
                _text = value;
                NotifyPropertyChanged("Text");
            }
        }

        public string Address1
        {
            get { return _address1; }

            set
            {
                if (_address1 == value) return;
                _address1 = value;
                NotifyPropertyChanged("Address1");
            }
        }

        public string Address2
        {
            get { return _address2; }
            set
            {
                if (_address2 == value) return;
                _address2 = value;
                NotifyPropertyChanged("Address2");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler) {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }

    public class ReturnVistLLCategory : IEnumerable
    {
        public ReturnVistLLCategory() { Items = new ObservableCollection<ReturnVisitLLItemModel>(); }
        private string _name;

        public string Name
        {
            get {return _name;}
            set
            {
                if(_name == value) return;
                _name = value;
                NotifyPropertyChanged("Name");
            }
        }

        private ObservableCollection<ReturnVisitLLItemModel> _rvItems;

        public ObservableCollection<ReturnVisitLLItemModel> Items { get { return _rvItems; } 
            set
            {
                if(_rvItems == value) return;
                _rvItems = value;
                NotifyPropertyChanged("llReturnVisitFullListItems");
            } }

        public IEnumerator GetEnumerator() { return _rvItems.GetEnumerator(); }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler) {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
