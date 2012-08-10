using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace MyTime.Model
{
	[Table]
	public class TimeData : INotifyPropertyChanged, INotifyPropertyChanging
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
		private int _minutes;

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

	
}