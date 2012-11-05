using System;
using System.ComponentModel;
using System.Diagnostics;
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
using System.IO;

namespace MyTime
{
    public class PreviousVisitViewModel : INotifyPropertyChanged
    {

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


        private DateTime _lastVisitDate;

        public DateTime LastVisitDate
        {
            get { return _lastVisitDate; }
            set
            {
                if (value != _lastVisitDate) {
                    _lastVisitDate = value;
                    NotifyPropertyChanged("LastVisitDate");
                }
            }
        }

        private string _placements;

        public string Placements
        {
            get { return _placements; }
            set
            {
                if (value != _placements) {
                    _placements = value;
                    NotifyPropertyChanged("Placements");
                }
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
}
