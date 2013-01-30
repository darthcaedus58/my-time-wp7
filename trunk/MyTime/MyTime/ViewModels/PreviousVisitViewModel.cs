using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using MyTimeDatabaseLib;

namespace FieldService.ViewModels
{
	public class PreviousVisitViewModel : INotifyPropertyChanged
	{
		private RvPreviousVisitData _previousVisitData;
		public event PropertyChangedEventHandler PropertyChanged;

		public RvPreviousVisitData PreviousVisitData
		{
			get
			{
				return _previousVisitData ?? (_previousVisitData = new RvPreviousVisitData {
					                                                                           Books = 0,
					                                                                           Brochures = 0,
					                                                                           Date = DateTime.Today,
					                                                                           RvItemId = -1,
					                                                                           Magazines = 0,
					                                                                           Notes = string.Empty
				                                                                           });
			} 
			set
			{
				if (_previousVisitData == value) return;
				_previousVisitData = value;
				OnPropertyChanged("PreviousVisitData");
			}
		}

		public int PreviousVisitItemId
		{
			get { return _previousVisitData == null ? -1 : _previousVisitData.ItemId; }
			set
			{
				_previousVisitData = RvPreviousVisitsDataInterface.GetCall(value);
				OnPropertyChanged("PreviousVisitData");
			}
		}

		public int ReturnVisitItemId
		{
			get { return _previousVisitData.RvItemId; }
			set
			{
				if (_previousVisitData != null && _previousVisitData.RvItemId == value) return;
				_previousVisitData = new RvPreviousVisitData {
					                                             Books = 0,
					                                             Brochures = 0,
					                                             RvItemId = value,
					                                             Date = DateTime.Today,
					                                             Magazines = 0,
					                                             Notes = string.Empty
				                                             };
				OnPropertyChanged("PreviousVisitData");
			}
		}

		protected virtual void OnPropertyChanged(string propertyName)
		{
			PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
		}

		public bool AddOrUpdateItem() { return RvPreviousVisitsDataInterface.AddOrUpdateCall(ref _previousVisitData); }
		public bool DeleteCall()
		{
			if (_previousVisitData.ItemId < 0) return false;
			return RvPreviousVisitsDataInterface.DeleteCall(_previousVisitData.ItemId);
		}
	}
}
