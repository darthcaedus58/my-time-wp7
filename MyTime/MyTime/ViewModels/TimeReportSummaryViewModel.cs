using System;
using System.ComponentModel;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace MyTime
{
    public class TimeReportSummaryViewModel : INotifyPropertyChanged
    {
        private string _month;

        public string Month
        {
            get { return _month; }

            set
            {
                if (_month != value) {
                    _month = value;
                    NotifyPropertyChanged("Month");
                }
            }
        }
        private string _time;

        public string Time
        {
            get { return _time; }

            set
            {
                if (_time != value) {
                    _time = value;
                    NotifyPropertyChanged("Time");
                }
            }
        }

        private int _days;

        public int Days
        {
            get { return _days; }

            set
            {
                if (_days != value) {
                    _days = value;
                    NotifyPropertyChanged("Days");
                }
            }
        }

        private int _mags;

        public int Magazines
        {
            get { return _mags; }

            set
            {
                if (_mags != value) {
                    _mags = value;
                    NotifyPropertyChanged("Magazines");
                }
            }
        }

        private int _bks;

        public int Books
        {
            get { return _bks; }

            set
            {
                if (_bks != value) {
                    _bks = value;
                    NotifyPropertyChanged("Books");
                }
            }
        }

        private int _bros;

        public int Brochures
        {
            get { return _bros; }

            set
            {
                if (_bros != value) {
                    _bros = value;
                    NotifyPropertyChanged("Brochures");
                }
            }
        }

        private int _rvs;

        public int ReturnVisits
        {
            get { return _rvs; }

            set
            {
                if (_rvs != value) {
                    _rvs = value;
                    NotifyPropertyChanged("ReturnVisits");
                }
            }
        }

        private int _bss;
        public int BibleStudies
        {
            get { return _bss; }

            set
            {
                if (_bss != value) {
                    _bss = value;
                    NotifyPropertyChanged("BibleStudies");
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

    public class TimeReportEntryViewModel : INotifyPropertyChanged
    {
        private int _id;

        public int ItemId
        {
            get { return _id; }

            set
            {
                if (_id != value) {
                    _id = value;
                    NotifyPropertyChanged("ItemId");
                }
            }
        }

        private string _date;

        public string Date
        {
            get { return _date; }

            set
            {
                if (_date != value) {
                    _date = value;
                    NotifyPropertyChanged("Date");
                }
            }
        }

        private string _hours;

        public string Hours
        {
            get { return _hours; }

            set
            {
                if (_hours != value) {
                    _hours = value;
                    NotifyPropertyChanged("Hours");
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
