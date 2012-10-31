using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Linq;
using Microsoft.Phone.Data.Linq.Mapping;
using Microsoft.Phone.Data.Linq;

namespace MyTimeDatabaseLib.Model
{
	[Table]
	internal class ReturnVisitDataItem : INotifyPropertyChanged, INotifyPropertyChanging
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

		private DateTime _DateCreated;

		[Column]
		public DateTime DateCreated
		{
			get { return _DateCreated; }
			set
			{
				if (_DateCreated != value) {
					NotifyPropertyChanging("DateCreated");
					_DateCreated = value;
					NotifyPropertyChanged("DateCreated");
				}
			}
		}

		private string _fullName;

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

		private string _gender;

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

		private string _physicalDescription;

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

		private string _age;

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

		private string _addressOne;

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

		private string _addressTwo;

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

		private string _city;

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

		private string _stateProvince;

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

		private string _country;

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

		private string _postalCode;

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

		private string _otherNotes;

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

		private byte[] _image;

		[Column(DbType = "IMAGE")]
		public byte[] SavedImage
		{
			get
			{
				return _image;
			}
			set
			{
				if (_image != value) {
					NotifyPropertyChanging("SavedImage");
					_image = value;
					NotifyPropertyChanged("SavedImage");
				}
			}
		}

		public int[] ImageSrc
		{
			get {
				int[] result2 = new int[_image.Length / sizeof(int)];
				Buffer.BlockCopy(_image, 0, result2, 0, _image.Length);
				return result2;
			}
			set
			{
				byte[] result = new byte[value.Length * sizeof(int)];
				Buffer.BlockCopy(value, 0, result, 0, result.Length);
				if (_image != result) {
					NotifyPropertyChanging("SavedImage");
					_image = result;
					NotifyPropertyChanged("SavedImage");
				}
			}
		}

		[Column]
		internal int _categoryId;

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

	internal class ReturnVisitDataContext : DataContext
	{
		// Specify the connection string as a static, used in main page and app.xaml.
		public static string DBConnectionString = "Data Source=isostore:/ReturnVisitData.sdf";

		// Pass the connection string to the base class.
		public ReturnVisitDataContext(string connectionString)
			: base(connectionString)
		{ }

		// Specify a single table for the  items.
		public Table<ReturnVisitDataItem> ReturnVisitItems;
	}
}
