// ***********************************************************************
// Assembly         : MyTime
// Author           : trevo_000
// Created          : 11-06-2012
//
// Last Modified By : trevo_000
// Last Modified On : 11-06-2012
// ***********************************************************************
// <copyright file="TimeReportSummaryViewModel.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.ComponentModel;

namespace MyTime
{
    /// <summary>
    /// Class TimeReportSummaryViewModel
    /// </summary>
    public class TimeReportSummaryViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// The _BKS
        /// </summary>
        private int _bks;
        /// <summary>
        /// The _bros
        /// </summary>
        private int _bros;
        /// <summary>
        /// The _BSS
        /// </summary>
        private int _bss;
        /// <summary>
        /// The _days
        /// </summary>
        private int _days;
        /// <summary>
        /// The _mags
        /// </summary>
        private int _mags;
        /// <summary>
        /// The _min
        /// </summary>
        private int _min;
        /// <summary>
        /// The _month
        /// </summary>
        private string _month;
        /// <summary>
        /// The _RVS
        /// </summary>
        private int _rvs;

        /// <summary>
        /// The _time
        /// </summary>
        private string _time;

        /// <summary>
        /// Gets or sets the month.
        /// </summary>
        /// <value>The month.</value>
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

        /// <summary>
        /// Gets or sets the time.
        /// </summary>
        /// <value>The time.</value>
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

        /// <summary>
        /// Gets or sets the minutes.
        /// </summary>
        /// <value>The minutes.</value>
        public int Minutes
        {
            get { return _min; }

            set
            {
                if (_min != value) {
                    _min = value;
                    NotifyPropertyChanged("Minutes");
                }
            }
        }

        /// <summary>
        /// Gets or sets the days.
        /// </summary>
        /// <value>The days.</value>
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

        /// <summary>
        /// Gets or sets the magazines.
        /// </summary>
        /// <value>The magazines.</value>
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

        /// <summary>
        /// Gets or sets the books.
        /// </summary>
        /// <value>The books.</value>
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

        /// <summary>
        /// Gets or sets the brochures.
        /// </summary>
        /// <value>The brochures.</value>
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

        /// <summary>
        /// Gets or sets the return visits.
        /// </summary>
        /// <value>The return visits.</value>
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

        /// <summary>
        /// Gets or sets the bible studies.
        /// </summary>
        /// <value>The bible studies.</value>
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

        #region INotifyPropertyChanged Members

        /// <summary>
        /// Occurs when [property changed].
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        /// <summary>
        /// Notifies the property changed.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler) {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }

    /// <summary>
    /// Class TimeReportEntryViewModel
    /// </summary>
    public class TimeReportEntryViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// The _date
        /// </summary>
        private string _date;
        /// <summary>
        /// The _hours
        /// </summary>
        private string _hours;
        /// <summary>
        /// The _id
        /// </summary>
        private int _id;

        /// <summary>
        /// The _min
        /// </summary>
        private int _min;

        /// <summary>
        /// Gets or sets the item id.
        /// </summary>
        /// <value>The item id.</value>
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

        /// <summary>
        /// Gets or sets the minutes.
        /// </summary>
        /// <value>The minutes.</value>
        public int Minutes
        {
            get { return _min; }

            set
            {
                if (_min != value) {
                    _min = value;
                    NotifyPropertyChanged("Minutes");
                }
            }
        }


        /// <summary>
        /// Gets or sets the date.
        /// </summary>
        /// <value>The date.</value>
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

        /// <summary>
        /// Gets or sets the hours.
        /// </summary>
        /// <value>The hours.</value>
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

        #region INotifyPropertyChanged Members

        /// <summary>
        /// Occurs when [property changed].
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        /// <summary>
        /// Notifies the property changed.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler) {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}