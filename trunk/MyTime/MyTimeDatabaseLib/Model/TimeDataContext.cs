// ***********************************************************************
// Assembly         : MyTimeDatabaseLib
// Author           : trevo_000
// Created          : 11-03-2012
//
// Last Modified By : trevo_000
// Last Modified On : 11-07-2012
// ***********************************************************************
// <copyright file="TimeDataContext.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace MyTimeDatabaseLib.Model
{
    /// <summary>
    /// Class TimeDataItem
    /// </summary>
    [Table]
    internal class TimeDataItem : INotifyPropertyChanged, INotifyPropertyChanging
    {
        // Define ID: private field, public property, and database column.
        /// <summary>
        /// The _books
        /// </summary>
        private int _books;
        /// <summary>
        /// The _brochures
        /// </summary>
        private int _brochures;
        /// <summary>
        /// The _BS
        /// </summary>
        private int _bs;
        /// <summary>
        /// The _date
        /// </summary>
        private DateTime _date;
        /// <summary>
        /// The _item id
        /// </summary>
        private int _itemId;
        /// <summary>
        /// The _mags
        /// </summary>
        private int _mags;
        /// <summary>
        /// The _minutes
        /// </summary>
        private int _minutes;
        /// <summary>
        /// The _notes
        /// </summary>
        private string _notes;
        /// <summary>
        /// The _RVS
        /// </summary>
        private int _rvs;
        /// <summary>
        /// The _version
        /// </summary>
        [Column(IsVersion = true)] private Binary _version;

        /// <summary>
        /// Gets or sets the item id.
        /// </summary>
        /// <value>The item id.</value>
        [Column(IsPrimaryKey = true, IsDbGenerated = true, DbType = "INT NOT NULL Identity", CanBeNull = false, AutoSync = AutoSync.OnInsert)]
        public int ItemId
        {
            get { return _itemId; }
            set
            {
                if (_itemId != value) {
                    NotifyPropertyChanging("ItemId");
                    _itemId = value;
                    NotifyPropertyChanged("ItemId");
                }
            }
        }

        // Define item name: private field, public property, and database column.

        /// <summary>
        /// Gets or sets the date.
        /// </summary>
        /// <value>The date.</value>
        [Column]
        public DateTime Date
        {
            get { return _date; }
            set
            {
                if (_date != value) {
                    NotifyPropertyChanging("Date");
                    _date = value;
                    NotifyPropertyChanged("Date");
                }
            }
        }

        // Define completion value: private field, public property, and database column.

        /// <summary>
        /// Gets or sets the minutes.
        /// </summary>
        /// <value>The minutes.</value>
        [Column]
        public int Minutes
        {
            get { return _minutes; }
            set
            {
                if (_minutes != value) {
                    NotifyPropertyChanging("Minutes");
                    _minutes = value;
                    NotifyPropertyChanged("Minutes");
                }
            }
        }

        /// <summary>
        /// Gets or sets the magazines.
        /// </summary>
        /// <value>The magazines.</value>
        [Column]
        public int Magazines
        {
            get { return _mags; }

            set
            {
                if (_mags != value) {
                    NotifyPropertyChanging("Magazines");
                    _mags = value;
                    NotifyPropertyChanged("Magazines");
                }
            }
        }

        /// <summary>
        /// Gets or sets the books.
        /// </summary>
        /// <value>The books.</value>
        [Column]
        public int Books
        {
            get { return _books; }

            set
            {
                if (_books != value) {
                    NotifyPropertyChanging("Books");
                    _books = value;
                    NotifyPropertyChanged("Books");
                }
            }
        }

        /// <summary>
        /// Gets or sets the brochures.
        /// </summary>
        /// <value>The brochures.</value>
        [Column]
        public int Brochures
        {
            get { return _brochures; }

            set
            {
                if (_brochures != value) {
                    NotifyPropertyChanging("Brochures");
                    _brochures = value;
                    NotifyPropertyChanged("Brochures");
                }
            }
        }

        /// <summary>
        /// Gets or sets the return visits.
        /// </summary>
        /// <value>The return visits.</value>
        [Column]
        public int ReturnVisits
        {
            get { return _rvs; }

            set
            {
                if (_rvs != value) {
                    NotifyPropertyChanging("ReturnVisits");
                    _rvs = value;
                    NotifyPropertyChanged("ReturnVisits");
                }
            }
        }

        /// <summary>
        /// Gets or sets the bible studies.
        /// </summary>
        /// <value>The bible studies.</value>
        [Column]
        public int BibleStudies
        {
            get { return _bs; }

            set
            {
                if (_bs != value) {
                    NotifyPropertyChanging("BibleStudies");
                    _bs = value;
                    NotifyPropertyChanged("BibleStudies");
                }
            }
        }

        /// <summary>
        /// Gets or sets the notes.
        /// </summary>
        /// <value>The notes.</value>
        [Column]
        public string Notes
        {
            get { return _notes; }
            set
            {
                if (_notes != value) {
                    NotifyPropertyChanging("Notes");
                    _notes = value;
                    NotifyPropertyChanged("Notes");
                }
            }
        }


        // Version column aids update performance.

        #region INotifyPropertyChanged Members

        /// <summary>
        /// Occurs when [property changed].
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        // Used to notify that a property changed

        #region INotifyPropertyChanging Members

        /// <summary>
        /// Occurs when [property changing].
        /// </summary>
        public event PropertyChangingEventHandler PropertyChanging;

        #endregion

        /// <summary>
        /// Notifies the property changed.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null) {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        // Used to notify that a property is about to change
        /// <summary>
        /// Notifies the property changing.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        private void NotifyPropertyChanging(string propertyName)
        {
            if (PropertyChanging != null) {
                PropertyChanging(this, new PropertyChangingEventArgs(propertyName));
            }
        }
    }

    /// <summary>
    /// Class TimeDataContext
    /// </summary>
    internal class TimeDataContext : DataContext
    {
        // Specify the connection string as a static, used in main page and app.xaml.
        /// <summary>
        /// The DB connection string
        /// </summary>
        public static string DBConnectionString = "Data Source=isostore:/TimeData.sdf;Max Database Size=512";

        // Pass the connection string to the base class.

        // Specify a single table for the items.
        /// <summary>
        /// The time data items
        /// </summary>
        public Table<TimeDataItem> TimeDataItems;

        /// <summary>
        /// Initializes a new instance of the <see cref="TimeDataContext" /> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        public TimeDataContext(string connectionString)
            : base(connectionString) { }
    }
}