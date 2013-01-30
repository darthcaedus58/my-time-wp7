using System;
using System.ComponentModel;
using MyTimeDatabaseLib;

namespace FieldService.ViewModels
{
	public class AddModifyRBCTimeViewModel : INotifyPropertyChanged
	{
		private RBCTimeData _rbcTimeData;

		public RBCTimeData RBCTimeData
		{
			get
			{
				return _rbcTimeData ?? (_rbcTimeData = new RBCTimeData {
					                                                       Date = DateTime.Today,
					                                                       Minutes = 0,
					                                                       Notes = string.Empty,
				                                                       });
			}
			set
			{
				if (_rbcTimeData == value) return;
				_rbcTimeData = value;
				OnPropertyChanged("RBCTimeData");
			}
		}

		public int RBCTimeDataItemId
		{
			get { return _rbcTimeData == null ? -1 : _rbcTimeData.ItemID; }
			set
			{
				if (value < 0) return;
				_rbcTimeData = RBCTimeDataInterface.GetRBCTimeData(value);
				OnPropertyChanged("RBCTimeData");
			}
		}

		#region INotifyPropertyChanged Members

		public event PropertyChangedEventHandler PropertyChanged;

		#endregion

		protected virtual void OnPropertyChanged(string propertyName)
		{
			PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
		}

		public bool AddOrUpdateTime() { return RBCTimeDataInterface.AddOrUpdateTime(ref _rbcTimeData); }

		public bool ConvertToRegularTime()
		{
			var td = new TimeData {
				                      Date = _rbcTimeData.Date,
				                      Minutes = _rbcTimeData.Minutes,
				                      Notes = _rbcTimeData.Notes
			                      };
			return DeleteTime() && TimeDataInterface.AddTime(ref td);
		}

		public bool DeleteTime()
		{
			bool v = _rbcTimeData.ItemID >= 0 && RBCTimeDataInterface.DeleteTime(RBCTimeData);
			if (v) {
				_rbcTimeData = null;
				OnPropertyChanged("RBCTimeData");
			}
			return v;
		}
	}
}