// ***********************************************************************
// Assembly         : MyTimeDatabaseLib
// Author           : trevo_000
// Created          : 11-03-2012
//
// Last Modified By : trevo_000
// Last Modified On : 11-03-2012
// ***********************************************************************
// <copyright file="ReturnVisitDataContext.cs" company="">
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
    /// Class ReturnVisitDataItem
    /// </summary>
    [Table]
    internal class ReturnVisitDataItem : INotifyPropertyChanged, INotifyPropertyChanging
    {
        // Define ID: private field, public property, and database column.
        /// <summary>
        /// The _ date created
        /// </summary>
        private DateTime _dateCreated;
        /// <summary>
        /// The _address one
        /// </summary>
        private string _addressOne;
        /// <summary>
        /// The _address two
        /// </summary>
        private string _addressTwo;
        /// <summary>
        /// The _age
        /// </summary>
        private string _age;
        /// <summary>
        /// The _category id
        /// </summary>
        [Column] 
        internal int CategoryId;
        /// <summary>
        /// The _city
        /// </summary>
        private string _city;
        /// <summary>
        /// The _country
        /// </summary>
        private string _country;
        /// <summary>
        /// The _full name
        /// </summary>
        private string _fullName;
        /// <summary>
        /// The _gender
        /// </summary>
        private string _gender;
        /// <summary>
        /// The _image
        /// </summary>
        private byte[] _image;
        /// <summary>
        /// The _item id
        /// </summary>
        private int _itemId;
        /// <summary>
        /// The _other notes
        /// </summary>
        private string _otherNotes;
        /// <summary>
        /// The _PH num
        /// </summary>
        private string _phNum;
        /// <summary>
        /// The _physical description
        /// </summary>
        private string _physicalDescription;
        /// <summary>
        /// The _postal code
        /// </summary>
        private string _postalCode;
        /// <summary>
        /// The _state province
        /// </summary>
        private string _stateProvince;
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

        /// <summary>
        /// Gets or sets the date created.
        /// </summary>
        /// <value>The date created.</value>
        [Column]
        public DateTime DateCreated
        {
            get { return _dateCreated; }
            set
            {
                if (_dateCreated != value) {
                    NotifyPropertyChanging("DateCreated");
                    _dateCreated = value;
                    NotifyPropertyChanged("DateCreated");
                }
            }
        }

        /// <summary>
        /// Gets or sets the phone number.
        /// </summary>
        /// <value>The phone number.</value>
        [Column]
        public string PhoneNumber
        {
            get { return _phNum; }
            set
            {
                if (_phNum != value) {
                    NotifyPropertyChanging("PhoneNumber");
                    _phNum = value;
                    NotifyPropertyChanged("PhoneNumber");
                }
            }
        }


        /// <summary>
        /// Gets or sets the full name.
        /// </summary>
        /// <value>The full name.</value>
        [Column]
        public string FullName
        {
            get { return _fullName; }
            set
            {
                if (_fullName != value) {
                    NotifyPropertyChanging("FullName");
                    _fullName = value;
                    NotifyPropertyChanged("FullName");
                }
            }
        }

        /// <summary>
        /// Gets or sets the gender.
        /// </summary>
        /// <value>The gender.</value>
        [Column]
        public string Gender
        {
            get { return _gender; }
            set
            {
                if (_gender != value) {
                    NotifyPropertyChanging("Gender");
                    _gender = value;
                    NotifyPropertyChanged("Gender");
                }
            }
        }

        /// <summary>
        /// Gets or sets the physical description.
        /// </summary>
        /// <value>The physical description.</value>
        [Column]
        public string PhysicalDescription
        {
            get { return _physicalDescription; }
            set
            {
                if (_physicalDescription != value) {
                    NotifyPropertyChanging("PhysicalDescription");
                    _physicalDescription = value;
                    NotifyPropertyChanged("PhysicalDescription");
                }
            }
        }

        /// <summary>
        /// Gets or sets the age.
        /// </summary>
        /// <value>The age.</value>
        [Column]
        public string Age
        {
            get { return _age; }
            set
            {
                if (_age != value) {
                    NotifyPropertyChanging("Age");
                    _age = value;
                    NotifyPropertyChanged("Age");
                }
            }
        }

        /// <summary>
        /// Gets or sets the address one.
        /// </summary>
        /// <value>The address one.</value>
        [Column]
        public string AddressOne
        {
            get { return _addressOne; }
            set
            {
                if (_addressOne != value) {
                    NotifyPropertyChanging("AddressOne");
                    _addressOne = value;
                    NotifyPropertyChanged("AddressOne");
                }
            }
        }

        /// <summary>
        /// Gets or sets the address two.
        /// </summary>
        /// <value>The address two.</value>
        [Column]
        public string AddressTwo
        {
            get { return _addressTwo; }
            set
            {
                if (_addressTwo != value) {
                    NotifyPropertyChanging("AddressTwo");
                    _addressTwo = value;
                    NotifyPropertyChanged("AddressTwo");
                }
            }
        }

        /// <summary>
        /// Gets or sets the city.
        /// </summary>
        /// <value>The city.</value>
        [Column]
        public string City
        {
            get { return _city; }
            set
            {
                if (_city != value) {
                    NotifyPropertyChanging("City");
                    _city = value;
                    NotifyPropertyChanged("City");
                }
            }
        }

        /// <summary>
        /// Gets or sets the state province.
        /// </summary>
        /// <value>The state province.</value>
        [Column]
        public string StateProvince
        {
            get { return _stateProvince; }
            set
            {
                if (_stateProvince != value) {
                    NotifyPropertyChanging("StateProvince");
                    _stateProvince = value;
                    NotifyPropertyChanged("StateProvince");
                }
            }
        }

        /// <summary>
        /// Gets or sets the country.
        /// </summary>
        /// <value>The country.</value>
        [Column]
        public string Country
        {
            get { return _country; }
            set
            {
                if (_country != value) {
                    NotifyPropertyChanging("Country");
                    _country = value;
                    NotifyPropertyChanged("Country");
                }
            }
        }

        /// <summary>
        /// Gets or sets the postal code.
        /// </summary>
        /// <value>The postal code.</value>
        [Column]
        public string PostalCode
        {
            get { return _postalCode; }
            set
            {
                if (_postalCode != value) {
                    NotifyPropertyChanging("PostalCode");
                    _postalCode = value;
                    NotifyPropertyChanged("PostalCode");
                }
            }
        }

        /// <summary>
        /// Gets or sets the other notes.
        /// </summary>
        /// <value>The other notes.</value>
        [Column]
        public string OtherNotes
        {
            get { return _otherNotes; }
            set
            {
                if (_otherNotes != value) {
                    NotifyPropertyChanging("OtherNotes");
                    _otherNotes = value;
                    NotifyPropertyChanged("OtherNotes");
                }
            }
        }

        /// <summary>
        /// Gets or sets the saved image.
        /// </summary>
        /// <value>The saved image.</value>
        [Column(DbType = "IMAGE")]
        public byte[] SavedImage
        {
            get { return _image; }
            set
            {
                if (_image != value) {
                    NotifyPropertyChanging("SavedImage");
                    _image = value;
                    NotifyPropertyChanged("SavedImage");
                }
            }
        }

        /// <summary>
        /// Gets or sets the image SRC.
        /// </summary>
        /// <value>The image SRC.</value>
        public int[] ImageSrc
        {
            get
            {
                var result2 = new int[_image.Length/sizeof (int)];
                Buffer.BlockCopy(_image, 0, result2, 0, _image.Length);
                return result2;
            }
            set
            {
                var result = new byte[value.Length*sizeof (int)];
                Buffer.BlockCopy(value, 0, result, 0, result.Length);
                if (_image != result) {
                    NotifyPropertyChanging("SavedImage");
                    _image = result;
                    NotifyPropertyChanged("SavedImage");
                }
            }
        }

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
    /// Class ReturnVisitDataContext
    /// </summary>
    internal class ReturnVisitDataContext : DataContext
    {
        // Specify the connection string as a static, used in main page and app.xaml.
        /// <summary>
        /// The DB connection string
        /// </summary>
        public static string DBConnectionString = "Data Source=isostore:/ReturnVisitData.sdf";

        // Pass the connection string to the base class.

        // Specify a single table for the  items.
        /// <summary>
        /// The return visit items
        /// </summary>
        public Table<ReturnVisitDataItem> ReturnVisitItems;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReturnVisitDataContext" /> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        public ReturnVisitDataContext(string connectionString)
            : base(connectionString) { }
    }
}