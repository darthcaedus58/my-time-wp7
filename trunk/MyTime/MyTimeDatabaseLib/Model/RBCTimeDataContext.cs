using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Text;

namespace MyTimeDatabaseLib.Model
{
	/// <summary>
	/// Class TimeDataItem
	/// </summary>
	[Table]
	internal class RBCTimeDataItem : INotifyPropertyChanged, INotifyPropertyChanging
	{
		/// <summary>
		/// The _date
		/// </summary>
		private DateTime _date;

		/// <summary>
		/// The _item id
		/// </summary>
		private int _itemId;

		/// <summary>
		/// The _minutes
		/// </summary>
		private int _minutes;

		private string _notes;

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

	class RBCTimeDataContext : DataContext
	{
		// Specify the connection string as a static, used in main page and app.xaml.
		/// <summary>
		/// The DB connection string
		/// </summary>
		public const string DBConnectionString = "Data Source=isostore:/RBCTimeData.sdf;Max Database Size=512";

		// Pass the connection string to the base class.

		// Specify a single table for the items.
		/// <summary>
		/// The time data items
		/// </summary>
		public Table<RBCTimeDataItem> RBCTimeDataItems;

		/// <summary>
		/// Initializes a new instance of the <see cref="TimeDataContext" /> class.
		/// </summary>
		/// <param name="connectionString">The connection string.</param>
		public RBCTimeDataContext(string connectionString = DBConnectionString)
			: base(connectionString) { }
	}
}
