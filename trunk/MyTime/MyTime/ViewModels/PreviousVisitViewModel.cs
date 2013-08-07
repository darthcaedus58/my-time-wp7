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
		private RvPreviousVisitData _previousVisitData = new RvPreviousVisitData {
					                                                                           Books = 0,
					                                                                           Brochures = 0,
					                                                                           Date = DateTime.Today,
					                                                                           RvItemId = -1,
					                                                                           Magazines = 0,
					                                                                           Notes = string.Empty
				                                                                           };

		private ReturnVisitViewModel _parentRv;
		private bool _saving;
		public event PropertyChangedEventHandler PropertyChanged;

		public ReturnVisitViewModel ParentRv
		{
			get { if(_parentRv == null) _parentRv = new ReturnVisitViewModel() {ItemId = ReturnVisitItemId}; 
				return _parentRv;
			} 
			set
			{
				if (_parentRv == value) return;
				_parentRv = value;
				OnPropertyChanged("ParentRv");
			}
		}

		public RvPreviousVisitData PreviousVisitData
		{
			get
			{
				return _previousVisitData;
			} 
			set
			{
				if (_previousVisitData == value) return;
				_previousVisitData = value;
				OnPropertyChanged("PreviousVisitData");
				OnPropertyChanged("Books");
				OnPropertyChanged("Brochures");
				OnPropertyChanged("Magazines");
				OnPropertyChanged("Date");
				OnPropertyChanged("DaysSinceVisit");
				OnPropertyChanged("Notes");
			}
		}

		public int Books
		{
			get { return _previousVisitData.Books; }
			set
			{
				if (_previousVisitData.Books == value) return;
				_previousVisitData.Books = value;
				OnPropertyChanged("Books");
			}
		}

		public int Brochures
		{
			get { return _previousVisitData.Brochures; }
			set
			{
				if (_previousVisitData.Brochures == value) return;
				_previousVisitData.Brochures = value;
				OnPropertyChanged("Brochures");
			}
		}

		public int Magazines
		{
			get { return _previousVisitData.Magazines; }
			set
			{
				if (_previousVisitData.Magazines == value) return;
				_previousVisitData.Magazines = value;
				OnPropertyChanged("Magazines");
			}
		}

		public DateTime Date
		{
			get { return _previousVisitData.Date; }
			set
			{
				if (_previousVisitData.Date == value) return;
				_previousVisitData.Date = value;
				OnPropertyChanged("Date");
				OnPropertyChanged("DaysSinceVisit");
			}
		}

		public string Notes
		{
			get { return _previousVisitData.Notes; }
			set
			{
				if (_previousVisitData.Notes == value) return;
				_previousVisitData.Notes = value;
				OnPropertyChanged("Notes");
			}
		}

		public int DaysSinceVisit { get {return  DateTime.Today.Subtract(_previousVisitData.Date).Days; } }

		public int PreviousVisitItemId
		{
			get { return _previousVisitData == null ? -1 : _previousVisitData.ItemId; }
			set
			{
				_previousVisitData = RvPreviousVisitsDataInterface.GetCall(value);
				OnPropertyChanged("PreviousVisitData");
				OnPropertyChanged("Notes");
				OnPropertyChanged("Date");
				OnPropertyChanged("Magazines");
				OnPropertyChanged("Brochures");
				OnPropertyChanged("Books");
				OnPropertyChanged("DaysSinceVisit");
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

		public bool AddOrUpdateItem()
		{
			return RvPreviousVisitsDataInterface.AddOrUpdateCall(ref _previousVisitData);
		}
		public bool DeleteCall()
		{
			if (_previousVisitData.ItemId < 0) return false;
			return RvPreviousVisitsDataInterface.DeleteCall(_previousVisitData.ItemId);
		}
	}
}
