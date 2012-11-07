using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Linq;
using Microsoft.Phone.Data.Linq;
using Microsoft.Phone.Data.Linq.Mapping;
using System.Xml.Linq;

namespace MyTimeDatabaseLib.Model
{
    [Table]
    internal class RvPreviousVisitItem : INotifyPropertyChanged, INotifyPropertyChanging
    {

        // Define ID: private field, public property, and database column.
        private int _itemId;

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
        private DateTime _date;

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
        private int _rvItemId;

        [Column]
        public int RvItemId
        {
            get { return _rvItemId; }
            set
            {
                if (_rvItemId != value) {
                    NotifyPropertyChanging("RvItemId");
                    _rvItemId = value;
                    NotifyPropertyChanged("RvItemId");
                }
            }
        }

        private int _mags;

        [Column]
        public int Magazines
        {
            get
            {
                return _mags;
            }

            set
            {
                if (_mags != value) {
                    NotifyPropertyChanging("Magazines");
                    _mags = value;
                    NotifyPropertyChanged("Magazines");
                }
            }
        }

        private int _books;

        [Column]
        public int Books
        {
            get
            {
                return _books;
            }

            set
            {
                if (_books != value) {
                    NotifyPropertyChanging("Books");
                    _books = value;
                    NotifyPropertyChanged("Books");
                }
            }
        }

        private int _brochures;

        [Column]
        public int Brochures
        {
            get
            {
                return _brochures;
            }

            set
            {
                if (_brochures != value) {
                    NotifyPropertyChanging("Brochures");
                    _brochures = value;
                    NotifyPropertyChanged("Brochures");
                }
            }
        }

        private int _rvs;

        // Define completion value: private field, public property, and database column.
        private string _notes;

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
        [Column(IsVersion = true)]
        private Binary _version;

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        // Used to notify that a property changed
        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null) {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        #region INotifyPropertyChanging Members

        public event PropertyChangingEventHandler PropertyChanging;

        // Used to notify that a property is about to change
        private void NotifyPropertyChanging(string propertyName)
        {
            if (PropertyChanging != null) {
                PropertyChanging(this, new PropertyChangingEventArgs(propertyName));
            }
        }

        #endregion
    }


    internal class RvPreviousVisitsContext : DataContext
    {
        // Specify the connection string as a static, used in main page and app.xaml.
        public static string DBConnectionString = "Data Source=isostore:/RvPreviousVisits.sdf";

        // Pass the connection string to the base class.
        public RvPreviousVisitsContext(string connectionString) : base(connectionString) { }

        // Specify a single table for the items.
        public Table<RvPreviousVisitItem> RvPreviousVisitItems;
    }
}
