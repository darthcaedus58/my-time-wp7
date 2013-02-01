using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using FieldService.Model;
using MyTimeDatabaseLib;

namespace FieldService.ViewModels
{
	public class EditReturnVisitViewModel : INotifyPropertyChanged
	{
		private ReturnVisitData _returnVisitData;

		public EditReturnVisitViewModel() { lbRvPreviousItems = new ObservableCollection<PreviousVisitModel>(); }

		/// <summary>
		/// Gets a value indicating whether this instance is previous visits loaded.
		/// </summary>
		/// <value><c>true</c> if this instance is previous visits loaded; otherwise, <c>false</c>.</value>
		public bool IsPreviousVisitsLoaded { get; private set; }

		/// <summary>
		/// Gets the lb rv previous items.
		/// </summary>
		/// <value>The lb rv previous items.</value>
		public ObservableCollection<PreviousVisitModel> lbRvPreviousItems { get; private set; }

		public ReturnVisitData ReturnVisitData
		{
			get
			{
				return _returnVisitData ?? (_returnVisitData = new ReturnVisitData {
					                                                                   AddressOne = string.Empty,
					                                                                   AddressTwo = string.Empty,
					                                                                   Age = string.Empty,
					                                                                   City = string.Empty,
					                                                                   Country = string.Empty,
					                                                                   DateCreated = DateTime.Today,
					                                                                   FullName = string.Empty,
					                                                                   Gender = "Male",
					                                                                   ImageSrc = new int[0],
					                                                                   OtherNotes = string.Empty,
					                                                                   PhoneNumber = string.Empty,
					                                                                   PhysicalDescription = string.Empty,
					                                                                   PostalCode = string.Empty,
					                                                                   StateProvince = string.Empty,
				                                                                   });
			}
			set { _returnVisitData = value; }
		}

		public string ReturnVisitDataFullName
		{
			get { return ReturnVisitData.FullName; }
			set
			{
				if (_returnVisitData.FullName == value) return;
				_returnVisitData.FullName = value;
				OnPropertyChanged("ReturnVisitDataFullName");
			}
		}

		public int ReturnVisitItemId
		{
			get { return _returnVisitData == null ? -1 : _returnVisitData.ItemId; }
			set
			{
				if (value < 0) return;
				ReturnVisitData = ReturnVisitsInterface.GetReturnVisit(value);
				OnPropertyChanged("ReturnVisitDataFullName");
				OnPropertyChanged("ReturnVisitDataAge");
				OnPropertyChanged("ReturnVisitDataGender");
				OnPropertyChanged("ReturnVisitDataPhoneNumber");
				OnPropertyChanged("ReturnVisitDataPhysicalDescription");
				OnPropertyChanged("ReturnVisitDataAddressOne");
				OnPropertyChanged("ReturnVisitDataAddressTwo");
				OnPropertyChanged("ReturnVisitDataCity");
				OnPropertyChanged("ReturnVisitDataStateProvince");
				OnPropertyChanged("ReturnVisitDataPostalCode");
				OnPropertyChanged("ReturnVisitDataCountry");
				OnPropertyChanged("ReturnVisitDataOtherNotes");
			}
		}

		public string ReturnVisitDataAge
		{
			get { return ReturnVisitData.Age; }
			set
			{
				if (_returnVisitData.Age == value) return;
				_returnVisitData.Age = value;
				OnPropertyChanged("ReturnVisitDataAge");
			}
		}

		public string ReturnVisitDataGender
		{
			get { return ReturnVisitData.Gender; }
			set
			{
				if (_returnVisitData.Gender == value) return;
				_returnVisitData.Gender = value;
				OnPropertyChanged("ReturnVisitDataGender");
			}
		}

		public string ReturnVisitDataPhoneNumber
		{
			get { return ReturnVisitData.PhoneNumber; }
			set
			{
				if (_returnVisitData.PhoneNumber == value) return;
				_returnVisitData.PhoneNumber = value;
				OnPropertyChanged("ReturnVisitDataPhoneNumber");
			}
		}

		public string ReturnVisitDataPhysicalDescription
		{
			get { return ReturnVisitData.PhysicalDescription; }
			set
			{
				if (_returnVisitData.PhysicalDescription == value) return;
				_returnVisitData.PhysicalDescription = value;
				OnPropertyChanged("ReturnVisitDataPhysicalDescription");
			}
		}

		public string ReturnVisitDataAddressOne
		{
			get { return ReturnVisitData.AddressOne; }
			set
			{
				if (_returnVisitData.AddressOne == value) return;
				_returnVisitData.AddressOne = value;
				OnPropertyChanged("ReturnVisitDataAddressOne");
			}
		}

		public string ReturnVisitDataAddressTwo
		{
			get { return ReturnVisitData.AddressTwo; }
			set
			{
				if (_returnVisitData.AddressTwo == value) return;
				_returnVisitData.AddressTwo = value;
				OnPropertyChanged("ReturnVisitDataAddressTwo");
			}
		}

		public string ReturnVisitDataCity
		{
			get { return ReturnVisitData.City; }
			set
			{
				if (_returnVisitData.City == value) return;
				_returnVisitData.City = value;
				OnPropertyChanged("ReturnVisitDataCity");
			}
		}

		public string ReturnVisitDataStateProvince
		{
			get { return ReturnVisitData.StateProvince; }
			set
			{
				if (_returnVisitData.StateProvince == value) return;
				_returnVisitData.StateProvince = value;
				OnPropertyChanged("ReturnVisitDataStateProvince");
			}
		}

		public string ReturnVisitDataPostalCode
		{
			get { return ReturnVisitData.PostalCode; }
			set
			{
				if (_returnVisitData.PostalCode == value) return;
				_returnVisitData.PostalCode = value;
				OnPropertyChanged("ReturnVisitDataPostalCode");
			}
		}

		public string ReturnVisitDataCountry
		{
			get { return ReturnVisitData.Country; }
			set
			{
				if (_returnVisitData.Country == value) return;
				_returnVisitData.Country = value;
				OnPropertyChanged("ReturnVisitDataCountry");
			}
		}

		public string ReturnVisitDataOtherNotes
		{
			get { return ReturnVisitData.OtherNotes; }
			set
			{
				if (_returnVisitData.OtherNotes == value) return;
				_returnVisitData.OtherNotes = value;
				OnPropertyChanged("ReturnVisitDataOtherNotes");
			}
		}

		#region INotifyPropertyChanged Members

		public event PropertyChangedEventHandler PropertyChanged;

		#endregion

		/// <summary>
		/// Updates the previous visits.
		/// </summary>
		public void UpdatePreviousVisits()
		{
			if (ReturnVisitData.ItemId < 0) return;
			RvPreviousVisitData[] visits = RvPreviousVisitsDataInterface.GetPreviousVisits(_returnVisitData.ItemId, SortOrder.DateNewestToOldest);
			if (IsPreviousVisitsLoaded) lbRvPreviousItems.Clear();
			IsPreviousVisitsLoaded = false;
			foreach (RvPreviousVisitData v in visits) {
				lbRvPreviousItems.Add(new PreviousVisitModel {
					                                             LastVisitDate = v.Date.ToShortDateString(),
					                                             ItemId = v.ItemId,
					                                             Placements = string.Format("{0} Mg's, {1} Bk's, {2} Bro's", v.Magazines, v.Books, v.Brochures),
					                                             Description = v.Notes
				                                             }
					);
			}
			IsPreviousVisitsLoaded = true;
		}


		protected virtual void OnPropertyChanged(string propertyName)
		{
			PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
		}

		public bool Delete()
		{
			if (_returnVisitData.ItemId < 0) return false;
			return ReturnVisitsInterface.DeleteReturnVisit(_returnVisitData.ItemId);
		}

		public bool AddOrUpdate() { return ReturnVisitsInterface.AddOrUpdateRV(ref _returnVisitData); }
	}
}